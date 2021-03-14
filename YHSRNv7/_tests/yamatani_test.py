import env

import numpy as np
from components.yamatani import *

import tensorflow as tf

import chainer
import chainer.functions as F
from chainer.variable import Variable, Parameter

def tf_test(x1, x2, t, slope):
    x1 = tf.constant(x1)
    x2 = tf.constant(x2)
    t  = tf.constant(t)

    with tf.GradientTape(persistent=True) as tape:
        tape.watch(x1)
        tape.watch(x2)
        tape.watch(t)

        y = yamatani(x1, x2, slope)
        err = y - t

    gx1 = tape.gradient(err, x1).numpy()
    gx2 = tape.gradient(err, x2).numpy()
    y   = y.numpy()
    err = err.numpy()

    #print('tensorflow: ')
    #print('gx1 : \n' + str(gx1))
    #print('gx2 : \n' + str(gx2))
    #print('y   : \n' + str(y))
    #print('err : \n' + str(err))

    return gx1, gx2, y, err

def tfnv_test(x1, x2, t, slope):
    x1 = tf.constant(x1)
    x2 = tf.constant(x2)
    t  = tf.constant(t)

    with tf.GradientTape(persistent=True) as tape:
        tape.watch(x1)
        tape.watch(x2)
        tape.watch(t)

        px = tf.cast(tf.math.logical_and(tf.math.greater(x1, 0), tf.math.greater(x2, 0)), x1.dtype)
        nx = tf.cast(tf.math.logical_and(tf.math.less   (x1, 0), tf.math.less   (x2, 0)), x1.dtype)
    
        y = px * tf.minimum(x1, x2) + nx * tf.maximum(x1, x2)
    
        if slope != 0.:
            y += slope * (x1 + x2)

        err = y - t

    gx1 = tape.gradient(err, x1).numpy()
    gx2 = tape.gradient(err, x2).numpy()
    y   = y.numpy()
    err = err.numpy()

    #print('tensorflow native impl: ')
    #print('gx1 : \n' + str(gx1))
    #print('gx2 : \n' + str(gx2))
    #print('y   : \n' + str(y))
    #print('err : \n' + str(err))

    return gx1, gx2, y, err

def chainer_test(x1, x2, t, slope):
    x1 = Parameter(x1)
    x2 = Parameter(x2)
    t  = Variable(t) 

    y = slope * (x1 + x2) + \
        ((x1.data > 0) * (x2.data > 0)) * F.minimum(x1, x2) + \
        ((x1.data < 0) * (x2.data < 0)) * F.maximum(x1, x2)

    err = y - t

    err.grad = err.data

    err.backward()

    gx1 = x1.grad
    gx2 = x2.grad
    y   = y.data
    err = err.data

    #print('chainer: ')
    #print('gx1 \n:' + str(gx1))
    #print('gx2 \n:' + str(gx2))
    #print('y   \n:' + str(y))
    #print('err \n:' + str(err))

    return gx1, gx2, y, err


n = 128

idx = np.arange(0, n)
x1 = np.sin(idx * 0.09) * idx * 0.1
x2 = np.cos(idx * 0.11) * idx * 0.1
t = idx / 24

for slope in [0, 0.1]:
    tf_gx1, tf_gx2, tf_y, tf_err = tf_test(x1, x2, t, slope)
    tfnv_gx1, tfnv_gx2, tfnv_y, tfnv_err = tfnv_test(x1, x2, t, slope)
    ch_gx1, ch_gx2, ch_y, ch_err = chainer_test(x1, x2, t, slope)

    print('gx1 sum diff: ' + str(np.sum(np.abs(tf_gx1 - tfnv_gx1))))
    print('gx2 sum diff: ' + str(np.sum(np.abs(tf_gx2 - tfnv_gx2))))
    print('  y sum diff: ' + str(np.sum(np.abs(tf_y   - ch_y  ))))
    print('err sum diff: ' + str(np.sum(np.abs(tf_err - ch_err))))