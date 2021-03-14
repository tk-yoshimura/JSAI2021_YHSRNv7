import env

import numpy as np

from components.halfresize import *

import tensorflow as tf

x = np.arange(0, 48).reshape((1, 6, 8, 1)).astype(np.float32)
x = tf.constant(x)

y = halfresize(x)

y = y.numpy()

print(x[0,:,:,0])
print(y[0,:,:,0])

print(x.shape)
print(y.shape)