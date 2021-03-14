import env

import cv2
import numpy as np

from components.bilinear import *

def tf_bilinear(x):
    x = tf.constant(x)

    with tf.GradientTape(persistent=False) as tape:
        tape.watch(x)
        y = bilinear2d(x)

    gx = tape.gradient(y, x).numpy()
    y = y.numpy()

    print(x.shape)
    print(gx.shape)
    print(y.shape)

    return y

def np_bilinear(x):
    h, w, c = x.shape

    x_pad = np.pad(x, [(1, 1), (1, 1), (0, 0)], mode='edge').astype(np.float32)

    x_c = x_pad[1:-1, 1:-1]

    x_l = x_pad[1:-1, :-2]
    x_r = x_pad[1:-1, 2: ]

    x_u = x_pad[:-2, 1:-1]
    x_d = x_pad[2:,  1:-1]

    x_lu = x_pad[0:-2,  :-2]
    x_ru = x_pad[0:-2, 2:  ]

    x_ld = x_pad[2:,    :-2]
    x_rd = x_pad[2:,   2:  ]

    xs = np.empty((h*2, w*2, c), np.float32)

    xs[0::2, 0::2] = x_c * 4 + x_l * 2 + x_u * 2 + x_lu
    xs[0::2, 1::2] = x_c * 4 + x_r * 2 + x_u * 2 + x_ru
    xs[1::2, 0::2] = x_c * 4 + x_l * 2 + x_d * 2 + x_ld
    xs[1::2, 1::2] = x_c * 4 + x_r * 2 + x_d * 2 + x_rd

    y = xs / 9

    return y

img_src = cv2.imread('testimg/test.png')

tf_x = img_src[np.newaxis, :, :, :].astype(np.float32)
tf_y = tf_bilinear(tf_x)[0]

np_x = img_src.astype(np.float)
np_y = np_bilinear(np_x)

print('diff:%f' % np.sum(np.abs(tf_y - np_y)))

cv2.imwrite('testimg/test_bilinear_NHWC.png', tf_y)