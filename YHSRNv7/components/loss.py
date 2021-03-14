# Definition Loss
# T.Yoshimura
# tensorflow 2.1.0

import math
import tensorflow as tf

from components.direction import direction
from components.local_variance import local_variance

def absolute_error(sr, hr):
    with tf.name_scope('absolute_error'):
        loss = tf.math.abs(sr - hr)

    return loss

def direction_error(sr, hr, dirs=32, data_format='NHWC'):
    with tf.name_scope('direction_error'):
        sr_dir = direction(sr, dirs, data_format=data_format)
        hr_dir = direction(hr, dirs, data_format=data_format)

        loss = tf.math.abs(sr_dir - hr_dir)

    return loss

def noisy_loss(sr, hr, data_format='NHWC', eps=1e-5):
    with tf.name_scope('noisy_loss'):
        sr_var = local_variance(sr, data_format=data_format)
        hr_var = local_variance(hr, data_format=data_format)

        loss = tf.nn.relu((tf.math.log(sr_var + eps) - tf.math.log(hr_var + eps)) * (1 / math.log(2)))

    return loss

def loss_srcnn(sr, hr, direction_loss_scale=1e-1, noisy_loss_scale = 1e-2, data_format='NHWC'):
    loss_abs = absolute_error(sr, hr)
    loss_dir = direction_error(sr, hr, dirs=32, data_format=data_format) * direction_loss_scale
    loss_noisy = noisy_loss(sr, hr, data_format=data_format) * noisy_loss_scale

    axis = 3 if data_format == 'NHWC' else 1

    loss_all = tf.concat([loss_abs, loss_dir, loss_noisy], axis)

    avg_loss_abs = tf.math.reduce_mean(loss_abs)
    avg_loss_dir = tf.math.reduce_mean(loss_dir)
    avg_loss_noisy = tf.math.reduce_mean(loss_noisy)

    loss_dict = { 'abs' : avg_loss_abs, 'dir' : avg_loss_dir, 'noise' : avg_loss_noisy }

    return loss_all, loss_dict