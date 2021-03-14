import env

import cv2
import numpy as np

from components.local_variance import *

img_src = cv2.imread('testimg/noise.png')
img_src = img_src[:, :, 0] * 0.114 + img_src[:, :, 1] * 0.587 + img_src[:, :, 2] * 0.299

x = img_src[np.newaxis, np.newaxis, :, :].astype(np.float)

x = tf.constant(x)

with tf.GradientTape(persistent=False) as tape:
    tape.watch(x)
    y = tf.math.sqrt(local_variance(x, data_format='NCHW'))

gx = tape.gradient(y, x).numpy()
y = y.numpy()

print(x.shape)
print(gx.shape)
print(y.shape)

img_dst = y[0]

cv2.imwrite('testimg/noise_std_NCHW.png', img_dst[0])
