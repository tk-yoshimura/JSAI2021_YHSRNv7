import os
import glob
import random

import cv2
import numpy as np

dirpath = 'Enter training images directory path.'

np.random.seed(1234)

for dirindex in range(32):
    imgpaths = glob.glob(dirpath + '{}/*.png'.format(dirindex))

    for imgpath in imgpaths:
        sigma = random.triangular(1e-5, 2, 2e-5)

        img = cv2.imread(imgpath, cv2.IMREAD_GRAYSCALE)
        img_blur = cv2.GaussianBlur(img, (5, 5), sigma)

        imgname = os.path.basename(imgpath)

        imgname_dst = dirpath + str(dirindex) + '_blured/' + imgname

        dirpath_dst = os.path.dirname(imgname_dst)
        if not os.path.exists(dirpath_dst):
            os.makedirs(dirpath_dst)

        cv2.imwrite(imgname_dst, img_blur)

        print('{},{}'.format(sigma, imgpath))