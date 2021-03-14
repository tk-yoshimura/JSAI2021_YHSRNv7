# Definition Diff Filter
# T.Yoshimura
# tensorflow 2.1.0

import numpy as np
import tensorflow as tf

from components.padding import edge_padding
from components.diff_filter import DiffFilter

diff_filter = DiffFilter()

def diff3x3(x, data_format='NHWC'):
    if x.shape.ndims != 4:
        raise ValueError('invalid ndim "x".')
    if not data_format in ['NHWC', 'NCHW']:
        raise ValueError('invalid data_format.')

    w = np.stack(diff_filter.kernels_3x3, axis=2)[:, :, np.newaxis, :]

    with tf.name_scope('diff3x3'):
        w = tf.constant(w, dtype=x.dtype)

        if data_format == 'NHWC':
            paddings = [[0, 0], [1, 1], [1, 1], [0, 0]]
        elif data_format == 'NCHW':
            paddings = [[0, 0], [0, 0], [1, 1], [1, 1]]

        y = tf.nn.conv2d(edge_padding(x, paddings), w, strides=1, padding='VALID', data_format=data_format)

    return y

def diff5x5(x, data_format='NHWC'):
    if x.shape.ndims != 4:
        raise ValueError('invalid ndim "x".')
    if not data_format in ['NHWC', 'NCHW']:
        raise ValueError('invalid data_format.')

    w = np.stack(diff_filter.kernels_5x5, axis=2)[:, :, np.newaxis, :]

    with tf.name_scope('diff5x5'):
        w = tf.constant(w, dtype=x.dtype)

        if data_format == 'NHWC':
            paddings = [[0, 0], [2, 2], [2, 2], [0, 0]]
        elif data_format == 'NCHW':
            paddings = [[0, 0], [0, 0], [2, 2], [2, 2]]

        y = tf.nn.conv2d(edge_padding(x, paddings), w, strides=1, padding='VALID', data_format=data_format)

    return y

def diff7x7(x, data_format='NHWC'):
    if x.shape.ndims != 4:
        raise ValueError('invalid ndim "x".')
    if not data_format in ['NHWC', 'NCHW']:
        raise ValueError('invalid data_format.')

    w = np.stack(diff_filter.kernels_7x7, axis=2)[:, :, np.newaxis, :]

    with tf.name_scope('diff7x7'):
        w = tf.constant(w, dtype=x.dtype)

        if data_format == 'NHWC':
            paddings = [[0, 0], [3, 3], [3, 3], [0, 0]]
        elif data_format == 'NCHW':
            paddings = [[0, 0], [0, 0], [3, 3], [3, 3]]

        y = tf.nn.conv2d(edge_padding(x, paddings), w, strides=1, padding='VALID', data_format=data_format)

    return y

def diff9x9(x, data_format='NHWC'):
    if x.shape.ndims != 4:
        raise ValueError('invalid ndim "x".')
    if not data_format in ['NHWC', 'NCHW']:
        raise ValueError('invalid data_format.')

    w = np.stack(diff_filter.kernels_9x9, axis=2)[:, :, np.newaxis, :]

    with tf.name_scope('diff9x9'):
        w = tf.constant(w, dtype=x.dtype)

        if data_format == 'NHWC':
            paddings = [[0, 0], [4, 4], [4, 4], [0, 0]]
        elif data_format == 'NCHW':
            paddings = [[0, 0], [0, 0], [4, 4], [4, 4]]

        y = tf.nn.conv2d(edge_padding(x, paddings), w, strides=1, padding='VALID', data_format=data_format)

    return y