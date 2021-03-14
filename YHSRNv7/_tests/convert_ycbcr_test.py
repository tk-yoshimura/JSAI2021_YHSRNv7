import numpy as np
import cv2

from components.convert_ycbcr import * 

# bgr
img_bgr = cv2.imread('testimg/pepper.png')

# rgb
img_rgb = np.flip(img_bgr, axis=2)

# bgr -> ycbcr
img_bgr_ycbcr = bgr_to_ycbcr(img_bgr)

# rgb -> ycbcr
img_rgb_ycbcr = rgb_to_ycbcr(img_rgb)

# bgr -> ycbcr -> bgr
img_bgr_rev = ycbcr_to_bgr(img_bgr_ycbcr)

# rgb -> ycbcr -> rgb
img_rgb_rev = ycbcr_to_rgb(img_rgb_ycbcr)

cv2.imwrite('testimg/pepper_bgr_y.png',  img_bgr_ycbcr[:, :, 0])
cv2.imwrite('testimg/pepper_bgr_cb.png', img_bgr_ycbcr[:, :, 1] + 128)
cv2.imwrite('testimg/pepper_bgr_cr.png', img_bgr_ycbcr[:, :, 2] + 128)

cv2.imwrite('testimg/pepper_rgb_y.png',  img_rgb_ycbcr[:, :, 0])
cv2.imwrite('testimg/pepper_rgb_cb.png', img_rgb_ycbcr[:, :, 1] + 128)
cv2.imwrite('testimg/pepper_rgb_cr.png', img_rgb_ycbcr[:, :, 2] + 128)

cv2.imwrite('testimg/pepper_bgr_rev.png', img_bgr_rev)
cv2.imwrite('testimg/pepper_rgb_rev.png', np.flip(img_rgb_rev, axis=2))