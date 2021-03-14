# Definition Yamatani-based Homogeneous Super Resolution Network
# T.Yoshimura
# tensorflow 2.1.0

from collections import OrderedDict

import numpy as np
from components.bilinear import *
from components.diff import *
from components.conv2d import *
from components.yamatani import *

import tensorflow as tf

class TruncatedHeNormal(tf.keras.initializers.Initializer):
    def __init__(self, scale, limitsigma = 4.0):
        self.__scale = scale
        self.__limitsigma = limitsigma

    def __call__(self, shape, dtype=None, **kwargs):
        norm = tf.random.normal(shape, mean=0, stddev=1, dtype=dtype)
        trunc_norm = tf.clip_by_value(norm, -self.__limitsigma, +self.__limitsigma)

        fan_in = np.prod(shape[:-1])
        stddev = np.sqrt(2 / fan_in) * self.__scale

        return trunc_norm * stddev

class YHSRN(tf.keras.Model):
    def __init__(self, residual_channels=64, residuals=4, cascades=3,
                 data_format='NHWC', kernel_initializer=TruncatedHeNormal(scale=0.1)):

        super(YHSRN, self).__init__()

        if not data_format in ['NHWC', 'NCHW']:
            raise ValueError('invalid data_format.')
        if residuals < 1:
            raise ValueError('residuals')
        if cascades < 1:
            raise ValueError('cascades')

        self.__data_format = data_format

        self.__residuals = residuals
        self.__cascades = cascades

        self.__parameters = OrderedDict()

        self.__channel_axis = 3 if data_format == 'NHWC' else 1

        self.__ksize = 2 * (residuals * cascades + 4) + 1 # residual_blocks, entry_blocks 

        self.__terminal_block = Convolution2D(
            inchannels=residual_channels, outchannels=4, ksize=1,             
            pad_mode='none', data_format=data_format, 
            kernel_initializer=kernel_initializer, name='terminal_block')
        self.__parameters['terminal_block/w'] = self.__terminal_block.w

        self.__residual_blocks = {}
        for i in range(residuals):
            for j in range(cascades):
                name = "residual_block_%d_%d" % (i, j)

                block = YamataniConvolution2D(
                    inchannels=residual_channels, outchannels=residual_channels, ksize=3,             
                    pad_mode='zero', data_format=data_format, 
                    kernel_initializer=kernel_initializer, name=name)
                self.__parameters[name + '/w1'] = block.w1
                self.__parameters[name + '/w2'] = block.w2

                self.__residual_blocks[(i, j)] = block

    def call(self, x):

        # entry block
        hd_3x3 = diff3x3(x, self.__data_format)
        hd_5x5 = diff5x5(x, self.__data_format)
        hd_7x7 = diff7x7(x, self.__data_format)
        hd_9x9 = diff9x9(x, self.__data_format)
        he = tf.concat([hd_3x3, hd_5x5, hd_7x7, hd_9x9], axis=self.__channel_axis)

        # residual block
        hi = he
        for i in range(self.__residuals):            
            # recursive block
            hr = hi
            for j in range(self.__cascades):
                block = self.__residual_blocks[(i, j)]
                hr = block(hr) + hi;

            hi = hr

        # terminal block
        ht = self.__terminal_block(hi)

        # generate sr
        y = bilinear2d(x, data_format=self.__data_format) + \
            tf.nn.depth_to_space(ht, block_size = 2, data_format=self.__data_format)

        return y

    def super_resolution(self, img_lr_ycbcr):
        if not type(img_lr_ycbcr) is np.ndarray:
            raise ValueError('type img_lr_ycbcr')
        if not img_lr_ycbcr.dtype in [np.float32, np.float64]:
            raise ValueError('dtype img_lr_ycbcr')
        if img_lr_ycbcr.ndim != 3:
            raise ValueError('ndim img_lr_ycbcr')

        lr = img_lr_ycbcr.transpose((2, 0, 1))
        lr = lr[:, np.newaxis, :, :] \
            if self.__data_format == 'NCHW' \
            else lr[:, :, :, np.newaxis]

        sr = self(lr).numpy()

        sr = sr[:, 0, :, :]\
            if self.__data_format == 'NCHW'\
            else sr[:, :, :, 0]
        img_sr_ycbcr = sr.transpose((1, 2, 0))

        return img_sr_ycbcr

    @property
    def parameters(self):
        return self.__parameters.copy()

    @property
    def kernel_size(self):
        return self.__ksize

    def save_npz(self, filepath):
        dict = {}

        for k, v in self.parameters.items():
            val = np.transpose(v.numpy(), (0, 1, 3, 2))

            dict[k] = val

        np.savez_compressed(filepath, **dict)

    def load_npz(self, filepath):
        data = np.load(filepath)

        for k, v in self.parameters.items():
            val = np.transpose(data[k], (0, 1, 3, 2))

            v.assign(val)

    def save_optimizer_npz(self, filepath):
        optimizer_weights = dict([(str(i), data) for (i, data) in enumerate(self.optimizer.get_weights())])

        learning_rate = self.optimizer.learning_rate.numpy()

        optimizer_weights['learning_rate'] = learning_rate

        np.savez_compressed(filepath, **optimizer_weights)

    def load_optimizer_npz(self, filepath):
        data = np.load(filepath)

        optimizer_weights = []

        for i in range(len(self.optimizer.weights)):
            optimizer_weights.append(data[str(i)])

        self.optimizer.set_weights(optimizer_weights)

        learning_rate = data['learning_rate']

        self.optimizer.learning_rate.assign(learning_rate)
