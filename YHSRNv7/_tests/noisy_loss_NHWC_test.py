import env

import cv2
import numpy as np

from components.loss import *

img_src = cv2.imread('testimg/noise.png')
img_src = img_src[:, :, 0] * 0.114 + img_src[:, :, 1] * 0.587 + img_src[:, :, 2] * 0.299

img_src_sr = cv2.imread('testimg/noise_sr.png')
img_src_sr = img_src_sr[:, :, 0] * 0.114 + img_src_sr[:, :, 1] * 0.587 + img_src_sr[:, :, 2] * 0.299

x = img_src[np.newaxis, :, :, np.newaxis].astype(np.float) / 255
x = tf.constant(x)

x_sr = img_src_sr[np.newaxis, :, :, np.newaxis].astype(np.float) / 255
x_sr = tf.constant(x_sr)

with tf.GradientTape(persistent=False) as tape:
    tape.watch(x_sr)
    loss = noisy_loss(x_sr, x) / 8

gx_sr = tape.gradient(loss, x_sr).numpy()
loss = loss.numpy()

print(x.shape)
print(gx_sr.shape)
print(loss.shape)

img_dst = loss[0]

cv2.imwrite('testimg/noise_loss_NHWC.png', img_dst[:, :, 0] * 255)

