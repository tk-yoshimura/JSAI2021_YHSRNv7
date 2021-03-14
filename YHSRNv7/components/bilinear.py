# Definition BiLinear
# T.Yoshimura
# tensorflow 2.1.0

import tensorflow as tf

from components.padding import edge_padding
from components.halfresize import halfresize

def bilinear2d(x, data_format='NHWC'):
    if x.shape.ndims != 4:
        raise ValueError('invalid ndim "x".')
    if not data_format in ['NHWC', 'NCHW']:
        raise ValueError('invalid data_format.')

    shape = x.shape.as_list()

    @tf.custom_gradient
    def core(x):
        if data_format == 'NHWC':
            x_pad = edge_padding(x, paddings=[[0, 0], [1, 1], [1, 1], [0, 0]])

            x_lt = tf.slice(x_pad, [0, 0, 0, 0], shape)
            x_t  = tf.slice(x_pad, [0, 0, 1, 0], shape)
            x_rt = tf.slice(x_pad, [0, 0, 2, 0], shape)
            x_l  = tf.slice(x_pad, [0, 1, 0, 0], shape)
            x_c  = x
            x_r  = tf.slice(x_pad, [0, 1, 2, 0], shape)
            x_lb = tf.slice(x_pad, [0, 2, 0, 0], shape)
            x_b  = tf.slice(x_pad, [0, 2, 1, 0], shape)
            x_rb = tf.slice(x_pad, [0, 2, 2, 0], shape)
        elif data_format == 'NCHW':
            x_pad = edge_padding(x, paddings=[[0, 0], [0, 0], [1, 1], [1, 1]])

            x_lt = tf.slice(x_pad, [0, 0, 0, 0], shape)
            x_t  = tf.slice(x_pad, [0, 0, 0, 1], shape)
            x_rt = tf.slice(x_pad, [0, 0, 0, 2], shape)
            x_l  = tf.slice(x_pad, [0, 0, 1, 0], shape)
            x_c  = x
            x_r  = tf.slice(x_pad, [0, 0, 1, 2], shape)
            x_lb = tf.slice(x_pad, [0, 0, 2, 0], shape)
            x_b  = tf.slice(x_pad, [0, 0, 2, 1], shape)
            x_rb = tf.slice(x_pad, [0, 0, 2, 2], shape)
                        
        y_lt = (x_c * 2 + (x_l + x_t)) * 2 + x_lt
        y_rt = (x_c * 2 + (x_r + x_t)) * 2 + x_rt
        y_lb = (x_c * 2 + (x_l + x_b)) * 2 + x_lb
        y_rb = (x_c * 2 + (x_r + x_b)) * 2 + x_rb

        channel_axis = 3 if data_format == 'NHWC' else 1
        y = tf.concat([y_lt, y_rt, y_lb, y_rb], axis=channel_axis)
        y = tf.nn.depth_to_space(y, block_size = 2, data_format=data_format) / 9

        def grad(dy):
            dx = halfresize(dy, data_format)

            return dx

        return y, grad

    with tf.name_scope('bilinear2d'):
        y = core(x)

    return y