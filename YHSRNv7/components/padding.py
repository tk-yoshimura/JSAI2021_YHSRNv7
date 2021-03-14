# Definition Padding
# T.Yoshimura
# tensorflow 2.1.0

import tensorflow as tf

def zero_padding(x, paddings):
    y = tf.pad(x, tf.constant(paddings), mode='CONSTANT')

    return y

def edge_padding(x, paddings):
    if x.shape.ndims != len(paddings):
        raise ValueError('invalid ndim "x".')

    ndims = x.shape.ndims

    @tf.custom_gradient
    def core(x):
        y = x

        for axis in reversed(range(len(paddings))):
            pad_1, pad_2 = paddings[axis]
            begin_1, begin_2 = [0,] * ndims, [0,] * ndims
            size_1, size_2 = y.shape.as_list(), y.shape.as_list()

            begin_2[axis] = y.shape[axis] - 1
            size_1[axis] = size_2[axis] = 1

            if pad_1 != 0 and pad_2 != 0:
                y1 = tf.slice(y, begin_1, size_1)
                y2 = tf.slice(y, begin_2, size_2)
                y = tf.concat(([y1,] * pad_1) + [y,] + ([y2,] * pad_2), axis)
            elif pad_1 != 0:
                y1 = tf.slice(y, begin_1, size_1)
                y = tf.concat(([y1,] * pad_1) + [y,], axis)
            elif pad_2 != 0:
                y2 = tf.slice(y, begin_2, size_2)
                y = tf.concat([y,] + ([y2,] * pad_2), axis)

        def grad(dy):
            begin = [ pad[0] for pad in paddings ]
            size = x.shape.as_list()

            dx = tf.slice(dy, begin, size)

            return dx

        return y, grad

    with tf.name_scope('edge_padding'):
        y = core(x)

    return y