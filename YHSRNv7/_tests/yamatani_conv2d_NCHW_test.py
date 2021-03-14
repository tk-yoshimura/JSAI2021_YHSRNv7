import env

import numpy as np
from components.yamatani import *

import tensorflow as tf

import chainer
import chainer.functions as F
from chainer.variable import Variable, Parameter

n, h, w, inc, outc = 2, 5, 6, 4, 7 
ksize = 3

def tf_test(x, w1, w2):
    x  = tf.constant(x)
    w1 = tf.constant(w1)
    w2 = tf.constant(w2)

    with tf.GradientTape(persistent=True) as tape:
        tape.watch(x)
        tape.watch(w1)
        tape.watch(w2)

        y = yamatani_conv2d(x, w1, w2, pad_mode='zero', data_format='NCHW')

    gw1 = tape.gradient(y, w1).numpy()
    gw2 = tape.gradient(y, w2).numpy()
    gx  = tape.gradient(y, x).numpy()
    y   = y.numpy()

    print(gw1.shape)
    print(gw2.shape)
    print(gx.shape)
    print(y.shape)

    return y

def chainer_test(x, w1, w2):
    x  = Variable(x)
    w1 = Variable(np.transpose(w1, (3, 2, 0, 1)))
    w2 = Variable(np.transpose(w2, (3, 2, 0, 1)))

    pad = ksize // 2

    x = F.pad(x, [[0, 0], [0, 0], [pad,pad], [pad,pad]], mode='constant')

    x1 = F.convolution_2d(x, w1)
    x2 = F.convolution_2d(x, w2)

    y = ((x1.data > 0) * (x2.data > 0)) * F.minimum(x1, x2) + \
        ((x1.data < 0) * (x2.data < 0)) * F.maximum(x1, x2)

    y = y.data

    return y

x = np.arange(0, n * h * w * inc).reshape(n, inc, h, w).astype(np.float32)
w1 = np.arange(0, ksize * ksize * inc * outc).reshape(ksize, ksize, inc, outc).astype(np.float32)
w2 = np.arange(0, ksize * ksize * inc * outc)[::-1].reshape(ksize, ksize, inc, outc).astype(np.float32)

tf_y = tf_test(x, w1, w2)
chainer_y = chainer_test(x, w1, w2)

print(' y sum diff: ' + str(np.sum(np.abs(tf_y - chainer_y))))