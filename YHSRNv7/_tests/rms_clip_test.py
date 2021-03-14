import env

import numpy as np
from components.rms_clip import rms_clip

import tensorflow as tf

for limit in [10, 1, 0.1, 0.01, 0.001]:
    x = np.random.normal(size=256)
    y = rms_clip(x, limit)

    x_rms = np.sqrt(np.average(np.square(x)))
    y_rms = np.sqrt(np.average(np.square(y)))

    print(x_rms)
    print(y_rms)

