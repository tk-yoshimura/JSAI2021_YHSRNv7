import env

import cv2
import numpy as np

from components.diff import *

img_src = cv2.imread('testimg/test.png')
img_src = img_src[:, :, 0] * 0.114 + img_src[:, :, 1] * 0.587 + img_src[:, :, 2] * 0.299

x = img_src[np.newaxis, :, :, np.newaxis].astype(np.float)

x = tf.constant(x)

with tf.GradientTape(persistent=False) as tape:
    tape.watch(x)
    y_3x3 = diff3x3(x, data_format='NHWC')
    y_5x5 = diff5x5(x, data_format='NHWC')
    y_7x7 = diff7x7(x, data_format='NHWC')
    y_9x9 = diff9x9(x, data_format='NHWC')
    y = tf.concat([y_3x3, y_5x5, y_7x7, y_9x9], axis=3)

gx = tape.gradient(y, x).numpy()
y = y.numpy()

print(x.shape)
print(gx.shape)
print(y.shape)

img_dst = y[0]

for ch in range(64):
    cv2.imwrite('testimg/test_diff_{}_NHWC.png'.format(ch), img_dst[:, :, ch])
