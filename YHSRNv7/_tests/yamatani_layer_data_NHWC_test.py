import env

import numpy as np
from components.yamatani import *

import tensorflow as tf

n, h, w, inc, outc = 2, 5, 6, 4, 7 
ksize = 3

x = np.arange(0, n * h * w * inc).reshape(n, h, w, inc).astype(np.float32)
x = tf.constant(x)

layer = YamataniConvolution2D(inc, outc, ksize, data_format='NHWC', kernel_initializer=tf.initializers.Constant(), name='hoge')

y = layer(x)

print(y.shape)

print(layer.weights)