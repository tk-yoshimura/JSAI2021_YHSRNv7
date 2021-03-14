# Definition HalfResize
# T.Yoshimura
# tensorflow 2.1.0

import tensorflow as tf

def halfresize(x, data_format='NHWC'):
    if x.shape.ndims != 4:
        raise ValueError('invalid ndim "x".')
    if not data_format in ['NHWC', 'NCHW']:
        raise ValueError('invalid data_format.')

    shape = x.shape.as_list()

    if data_format == 'NHWC':
        n, h, w, c = shape
    elif data_format == 'NCHW':
        n, c, h, w = shape

    if h % 2 != 0 or w % 2 != 0:
        raise ValueError('invalid size "x".')

    with tf.name_scope('halfresize'):
        y = tf.nn.avg_pool2d(x, 2, 2, padding='VALID', data_format=data_format)

    return y