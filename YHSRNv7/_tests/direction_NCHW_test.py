import env

import cv2
import numpy as np

from components.direction import *

img_src = cv2.imread('testimg/test.png')
img_src = img_src[:, :, 0] * 0.114 + img_src[:, :, 1] * 0.587 + img_src[:, :, 2] * 0.299

x = img_src[np.newaxis, np.newaxis, :, :].astype(np.float)

x = tf.constant(x)

with tf.GradientTape(persistent=False) as tape:
    tape.watch(x)
    y = direction(x, dirs=32, data_format='NCHW')

gx = tape.gradient(y, x).numpy()
y = y.numpy()

print(x.shape)
print(gx.shape)
print(y.shape)

img_dst = y[0]

for ch in range(32):
    cv2.imwrite('testimg/test_direction_{}_NCHW.png'.format(ch), img_dst[ch, :, :])