# Definition BiLinear
# T.Yoshimura
# tensorflow 2.1.0

import tensorflow as tf

from components.padding import edge_padding

def local_variance(x, data_format='NHWC'):
    if x.shape.ndims != 4:
        raise ValueError('invalid ndim "x".')
    if not data_format in ['NHWC', 'NCHW']:
        raise ValueError('invalid data_format.')

    shape = x.shape.as_list()

    with tf.name_scope('local_variance'):
        with tf.name_scope('stack'):
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

            x_stack = tf.stack([x_lt, x_t, x_rt, x_l, x_c, x_r, x_lb, x_b, x_rb], axis = 4)

        with tf.name_scope('variance'):
            x_mean = tf.math.reduce_mean(x_stack, axis=4, keepdims=True, name='mean')
            x_diverce = x_stack - tf.broadcast_to(x_mean, x_stack.shape)
            x_var = tf.math.reduce_mean(tf.math.square(x_diverce), axis=4, name='var')

    return x_var