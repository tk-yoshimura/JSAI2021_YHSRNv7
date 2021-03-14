# Definition Direction Filter
# T.Yoshimura
# tensorflow 2.1.0

import numpy as np
import tensorflow as tf

from util.dirfilter import dirfilters
from components.padding import edge_padding

def direction(x, dirs, ksize = 5, smoothness = 0.5, data_format='NHWC'):
    if x.shape.ndims != 4:
        raise ValueError('invalid ndim "x".')
    if not data_format in ['NHWC', 'NCHW']:
        raise ValueError('invalid data_format.')

    p = ksize // 2

    w = dirfilters(dirs, ksize, smoothness)[:, :, np.newaxis, :]

    with tf.name_scope('direction_%d' % dirs):
        w = tf.constant(w, dtype=x.dtype)

        if data_format == 'NHWC':
            paddings = [[0, 0], [p, p], [p, p], [0, 0]]
        elif data_format == 'NCHW':
            paddings = [[0, 0], [0, 0], [p, p], [p, p]]

        y = tf.nn.conv2d(edge_padding(x, paddings), w, strides=1, padding='VALID', data_format=data_format)

    return y
