# Color Convert YCbCr(ITU-R BT.601)
# T.Yoshimura

import numpy as np

const = type("_constants", (object,), {
    "yr" : 0.299,
    "yg" : 0.587,
    "yb" : 0.114,
    "cb" : 1.772,
    "cr" : 1.402
})

kernel = type("_kernels", (object,), {
    "rgb_to_ycbcr" : np.array([[+const.yr, +const.yg, +const.yb], 
                               [-const.yr / const.cb, -const.yg / const.cb, +0.5],
                               [+0.5, -const.yg / const.cr, -const.yb / const.cr]]),
    "ycbcr_to_rgb" : np.array([[1, 0, +const.cr], 
                               [1, -const.yb * const.cb / const.yg, -const.yr * const.cr / const.yg], 
                               [1, +const.cb, 0]]),
    "bgr_to_ycbcr" : np.array([[+const.yb, +const.yg, +const.yr], 
                               [+0.5, -const.yg / const.cb, -const.yr / const.cb], 
                               [-const.yb / const.cr, -const.yg / const.cr, +0.5]]),
    "ycbcr_to_bgr" : np.array([[1, +const.cb, 0], 
                               [1, -const.yb * const.cb / const.yg, -const.yr * const.cr / const.yg], 
                               [1, 0, +const.cr]])
})

def rgb_to_ycbcr(img_rgb):
    if not img_rgb.dtype in [np.float32, np.float64]:
        img_rgb = img_rgb.astype(np.float32)

    img_ycbcr = np.tensordot(img_rgb, kernel.rgb_to_ycbcr.astype(img_rgb.dtype), (2, 1))

    return img_ycbcr

def bgr_to_ycbcr(img_bgr):
    if not img_bgr.dtype in [np.float32, np.float64]:
        img_bgr = img_bgr.astype(np.float32)
    
    img_ycbcr = np.tensordot(img_bgr, kernel.bgr_to_ycbcr.astype(img_bgr.dtype), (2, 1))

    return img_ycbcr

def ycbcr_to_rgb(img_ycbcr):
    if not img_ycbcr.dtype in [np.float32, np.float64]:
        img_ycbcr = img_ycbcr.astype(np.float32)
    
    img_rgb = np.tensordot(img_ycbcr, kernel.ycbcr_to_rgb.astype(img_ycbcr.dtype), (2, 1))

    return img_rgb

def ycbcr_to_bgr(img_ycbcr):
    if not img_ycbcr.dtype in [np.float32, np.float64]:
        img_ycbcr = img_ycbcr.astype(np.float32)
    
    img_bgr = np.tensordot(img_ycbcr, kernel.ycbcr_to_bgr.astype(img_ycbcr.dtype), (2, 1))

    return img_bgr