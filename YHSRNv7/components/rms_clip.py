# RMS Clipping
# T.Yoshimura
# tensorflow 2.1.0

import numpy as np
import tensorflow as tf

def rms_clip(x, limit):
    squa_limit = limit * limit

    with tf.name_scope('rms_clip'):
        norm = tf.math.reduce_mean(tf.math.square(x))
        scale = tf.math.sqrt(squa_limit / tf.math.maximum(squa_limit, norm))

        y = x * scale

    return y