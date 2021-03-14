import env

import numpy as np
from components.conv2d import *

import tensorflow as tf

import chainer
import chainer.functions as F
from chainer.variable import Variable, Parameter

n, h, w, inc, outc = 2, 5, 6, 4, 7 
ksize = 3

def tf_test(x, w):
    x = tf.constant(x)
    w = tf.constant(w)

    with tf.GradientTape(persistent=True) as tape:
        tape.watch(x)
        tape.watch(w)

        y = conv2d(x, w, pad_mode='zero', data_format='NHWC')

    gw = tape.gradient(y, w).numpy()
    gx = tape.gradient(y, x).numpy()
    y  = y.numpy()

    print(gw.shape)
    print(gx.shape)
    print(y.shape)

    return y

def chainer_test(x, w):
    x = Variable(np.transpose(x, (0, 3, 1, 2)))
    w = Variable(np.transpose(w, (3, 2, 0, 1)))

    pad = ksize // 2

    x = F.pad(x, [[0, 0], [0, 0], [pad,pad], [pad,pad]], mode='constant')

    y = F.convolution_2d(x, w)

    y = y.data
    y = np.transpose(y, (0, 2, 3, 1))

    return y

x = np.arange(0, n * h * w * inc).reshape(n, h, w, inc).astype(np.float32)
w = np.arange(0, ksize * ksize * inc * outc).reshape(ksize, ksize, inc, outc).astype(np.float32)

tf_y = tf_test(x, w)
chainer_y = chainer_test(x, w)

print(' y sum diff: ' + str(np.sum(np.abs(tf_y - chainer_y))))