# Definition Yamatani Activation and Convolution
# T.Yoshimura
# tensorflow 2.1.0

import tensorflow as tf
from components.padding import * 

def yamatani(x1, x2, slope=0.):
    if not type(slope) in (int, float):
        ValueError('invalid type "slope".')

    @tf.custom_gradient
    def core(x1, x2):
        px = tf.math.logical_and(tf.math.greater(x1, 0), tf.math.greater(x2, 0))
        nx = tf.math.logical_and(tf.math.less   (x1, 0), tf.math.less   (x2, 0))
    
        y = tf.where(px, tf.minimum(x1, x2), 0.0) + tf.where(nx, tf.maximum(x1, x2), 0.0)
    
        if slope != 0.:
            y += slope * (x1 + x2)

        def grad(dy):
            dx1 = tf.cast(tf.math.logical_or(
                    tf.math.logical_and(px, tf.math.less_equal   (x1, x2)),
                    tf.math.logical_and(nx, tf.math.greater_equal(x1, x2))
                  ), dy.dtype)
            dx2 = tf.cast(tf.math.logical_or(
                    tf.math.logical_and(px, tf.math.greater_equal(x1, x2)),
                    tf.math.logical_and(nx, tf.math.less_equal   (x1, x2))
                  ), dy.dtype)

            if slope != 0.:
                dx1 += slope
                dx2 += slope

            return dx1 * dy, dx2 * dy

        return y, grad

    with tf.name_scope('yamatani'):
        y = core(x1, x2)

    return y

def yamatani_conv2d(x, w1, w2, yamatani_slope=0., pad_mode='zero', data_format='NHWC', name=None):
    if w1.shape != w2.shape:
        raise ValueError('mismatch shape "w1","w2".')
    if not pad_mode in ['zero', 'edge', 'none']:
        raise ValueError('invalid pad_mode')
    if not data_format in ['NHWC', 'NCHW']:
        raise ValueError('invalid data_format.')

    kh, kw, inc, outc = w1.shape.as_list()
    pad = kh // 2
    
    if kh != kw:
        raise ValueError('invalid filter shape "width != height".')
    if pad == 0 and not pad_mode is 'none':
        raise ValueError('invalid pad_mode')

    with tf.name_scope('yamatani_conv2d' if name is None else name):
        if pad > 0 and not pad_mode is 'none':
            if data_format == 'NHWC':
                paddings = [[0, 0], [pad, pad], [pad, pad], [0, 0]]
            elif data_format == 'NCHW':
                paddings = [[0, 0], [0, 0], [pad, pad], [pad, pad]]

            if pad_mode == 'zero':
                x = zero_padding(x, paddings)
            else:
                x = edge_padding(x, paddings)
    
        y1 = tf.nn.conv2d(x, w1, strides=1, padding='VALID', data_format=data_format)
        y2 = tf.nn.conv2d(x, w2, strides=1, padding='VALID', data_format=data_format)

        y = yamatani(y1, y2, yamatani_slope)

    return y

class YamataniConvolution2D(tf.keras.layers.Layer):
    def __init__(self, inchannels, outchannels, ksize, yamatani_slope=0., pad_mode='zero', 
                 data_format='NHWC', kernel_initializer=None, name=None):

        super(YamataniConvolution2D, self).__init__(name=name)

        if not pad_mode in ['zero', 'edge', 'none']:
            raise ValueError('invalid pad_mode')
        if not data_format in ['NHWC', 'NCHW']:
            raise ValueError('invalid data_format.')
        
        self.__inchannels  = inchannels
        self.__outchannels = outchannels
        self.__ksize = ksize
        self.__yamatani_slope = yamatani_slope
        self.__pad_mode = pad_mode
        self.__data_format = data_format
        self.__kernel_initializer = kernel_initializer

        self.__kernelshape = [self.__ksize, self.__ksize, self.__inchannels, self.__outchannels]
        
        self.__w1 = self.add_weight("w1", shape=self.__kernelshape, dtype=tf.float64, 
                                    initializer=self.__kernel_initializer)
        self.__w2 = self.add_weight("w2", shape=self.__kernelshape, dtype=tf.float64, 
                                    initializer=self.__kernel_initializer)

    def build(self, input_shape):
        inc = input_shape[-1] if self.__data_format == 'NHWC' else input_shape[1]
        
        if inc != self.__inchannels:
            raise ValueError('inchannels')

        super().build(input_shape)

    def call(self, x):
        if x.dtype != tf.float64:
            w1, w2 = tf.cast(self.__w1, x.dtype), tf.cast(self.__w2, x.dtype)
        else:
            w1, w2 = self.__w1, self.__w2

        y = yamatani_conv2d(x, w1, w2, self.__yamatani_slope, self.__pad_mode, self.__data_format)
        return y

    @property
    def w1(self):
        return self.__w1

    @property
    def w2(self):
        return self.__w2