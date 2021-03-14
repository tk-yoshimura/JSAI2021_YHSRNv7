# Sample Yamatani-based Homogeneous Super Resolution Network
# T.Yoshimura
# tensorflow 2.1.0

import env
import os, glob

import numpy as np
import cv2

import tensorflow as tf

from YHSRN import YHSRN

from components.convert_ycbcr import * 

# setting
data_format='NHWC'
dirpath_lr = 'Enter low res images directory path.'
dirpath_sr = 'Enter output directory path.'

# modelparams
residual_channels = 64
residuals = 4
cascades = 3

# load model
model = YHSRN(residual_channels, residuals, cascades, data_format=data_format)
model.load_npz('_results/snap/model.npz')

# list images
imgpaths_lr = glob.glob(dirpath_lr + '*.png')
print('image listed {}'.format(len(imgpaths_lr)))

os.makedirs(dirpath_sr, exist_ok=True)

# process loop
for imgpath_lr in imgpaths_lr:
    imgname = os.path.basename(imgpath_lr)[:-len('.png')]
    print('Processing: {}'.format(imgname))

    # load image
    img_lr = cv2.imread(imgpath_lr)

    # render image
    img_lr_ycbcr = bgr_to_ycbcr(img_lr)

    with tf.device('/CPU:0'):
        img_sr_ycbcr = model.super_resolution(img_lr_ycbcr)

    img_sr = ycbcr_to_bgr(img_sr_ycbcr)

    # save image
    cv2.imwrite('{}{}_yhsrnv7.png'.format(dirpath_sr, imgname), img_sr)