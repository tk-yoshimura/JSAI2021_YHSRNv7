# Definition Yamatani Activation and Convolution
# T.Yoshimura
# tensorflow 2.1.0

import tensorflow as tf
from components.padding import * 

def conv2d(x, w, pad_mode='zero', data_format='NHWC', name=None):
    if not pad_mode in ['zero', 'edge', 'none']:
        raise ValueError('invalid pad_mode')
    if not data_format in ['NHWC', 'NCHW']:
        raise ValueError('invalid data_format.')

    kh, kw, inc, outc = w.shape.as_list()
    pad = kh // 2
    
    if kh != kw:
        raise ValueError('invalid filter shape "width != height".')
    if pad == 0 and not pad_mode is 'none':
        raise ValueError('invalid pad_mode')

    with tf.name_scope('conv2d' if name is None else name):
        if pad > 0 and not pad_mode is 'none':
            if data_format == 'NHWC':
                paddings = [[0, 0], [pad, pad], [pad, pad], [0, 0]]
            elif data_format == 'NCHW':
                paddings = [[0, 0], [0, 0], [pad, pad], [pad, pad]]

            if pad_mode == 'zero':
                x = zero_padding(x, paddings)
            else:
                x = edge_padding(x, paddings)
    
        y = tf.nn.conv2d(x, w, strides=1, padding='VALID', data_format=data_format)

    return y

class Convolution2D(tf.keras.layers.Layer):
    def __init__(self, inchannels, outchannels, ksize, pad_mode='zero', 
                 data_format='NHWC', kernel_initializer=None, name=None):

        super(Convolution2D, self).__init__(name=name)

        if not pad_mode in ['zero', 'edge', 'none']:
            raise ValueError('invalid pad_mode')
        if not data_format in ['NHWC', 'NCHW']:
            raise ValueError('invalid data_format.')
        
        self.__inchannels  = inchannels
        self.__outchannels = outchannels
        self.__ksize = ksize
        self.__pad_mode = pad_mode
        self.__data_format = data_format
        self.__kernel_initializer = kernel_initializer

        self.__kernelshape = [self.__ksize, self.__ksize, self.__inchannels, self.__outchannels]
        
        self.__w = self.add_weight("w", shape=self.__kernelshape, dtype=tf.float64, 
                                    initializer=self.__kernel_initializer)

    def build(self, input_shape):
        inc = input_shape[-1] if self.__data_format == 'NHWC' else input_shape[1]
        
        if inc != self.__inchannels:
            raise ValueError('inchannels')

        super().build(input_shape)

    def call(self, x):
        if x.dtype != tf.float64:
            w = tf.cast(self.__w, x.dtype)
        else:
            w = self.__w

        y = conv2d(x, w, self.__pad_mode, self.__data_format)
        return y

    @property
    def w(self):
        return self.__w