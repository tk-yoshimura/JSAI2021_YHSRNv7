import env

import numpy as np
from components.padding import *

import tensorflow as tf

def tf_pad(x, paddings):
    x = tf.constant(x)

    with tf.GradientTape(persistent=False) as tape:
        tape.watch(x)

        y = zero_padding(x, paddings)

    gx = tape.gradient(y, x).numpy()
    y  = y.numpy()

    return y, gx

def np_pad(x, paddings):
    y = np.pad(x, paddings, mode='constant')

    gx = np.ones(x.shape, x.dtype)

    return y, gx


paddings_list = [
    [[0, 0], [0, 0], [0, 0], [0, 0]],
    [[0, 0], [2, 2], [4, 0], [0, 0]],
    [[0, 0], [2, 2], [3, 1], [0, 0]],
    [[0, 0], [2, 2], [2, 2], [0, 0]],
    [[0, 0], [2, 2], [1, 3], [0, 0]],
    [[0, 0], [2, 2], [0, 4], [0, 0]],
    [[2, 2], [4, 0], [0, 0], [0, 0]],
    [[2, 2], [3, 1], [0, 0], [0, 0]],
    [[2, 2], [2, 2], [0, 0], [0, 0]],
    [[2, 2], [1, 3], [0, 0], [0, 0]],
    [[0, 0], [0, 0], [2, 2], [0, 4]],
    [[0, 0], [0, 0], [2, 2], [4, 0]],
    [[0, 0], [0, 0], [2, 2], [3, 1]],
    [[0, 0], [0, 0], [2, 2], [2, 2]],
    [[0, 0], [0, 0], [2, 2], [1, 3]],
    [[0, 0], [0, 0], [2, 2], [0, 4]],
    [[0, 0], [2, 2], [0, 0], [4, 0]],
    [[0, 0], [2, 2], [0, 0], [3, 1]],
    [[0, 0], [2, 2], [0, 0], [2, 2]],
    [[0, 0], [2, 2], [0, 0], [1, 3]],
    [[0, 0], [2, 2], [0, 0], [0, 4]],
    [[4, 0], [0, 0], [0, 0], [0, 0]],
    [[0, 0], [3, 1], [0, 0], [0, 0]],
    [[0, 0], [0, 0], [2, 2], [0, 0]],
    [[0, 0], [0, 0], [0, 0], [1, 3]],
]

x = np.arange(0, 48).reshape((2, 4, 3, 2)).astype(np.float32)

for paddings in paddings_list:
    tf_y, tf_gx = tf_pad(x, paddings)
    np_y, np_gx = np_pad(x, paddings)

    print(tf_y.shape)
    print(np_y.shape)
    print(tf_gx.shape)
    print(np_gx.shape)

    print(paddings)
    print('gx sum diff: ' + str(np.sum(np.abs(tf_gx - np_gx))))
    print(' y sum diff: ' + str(np.sum(np.abs(tf_y  - np_y))))