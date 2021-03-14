# Generation Direction Filter
# T.Yoshimura

import math
import numpy as np

def cossindir(dirs:int) -> (np.ndarray, np.ndarray):
    if dirs < 4 or dirs > 256 or (dirs & (dirs - 1)) != 0:
        raise ValueError('dirs')

    c, s = np.array([1, 0, -1]), np.array([0, 1, 0])
    n = 3
    d = dirs

    while d > 2:
        new_n = n * 2 - 1
        new_c, new_s = np.zeros((new_n,)), np.zeros((new_n,))

        new_c[::2] = c
        new_s[::2] = s

        for i in range(1, n):
            ca, cb = c[i - 1], c[i]
            sa, sb = s[i - 1], s[i]
            c2 = ca * cb - sa * sb;

            new_c[i * 2 - 1] = (1 if (i * 2 < n) else -1) * math.sqrt((1 + c2) / 2)
            new_s[i * 2 - 1] = math.sqrt((1 - c2) / 2)

        n, c, s = new_n, new_c, new_s
        d /= 2

    c, s = c[:-1], s[:-1]

    return (c, s)

def dirfilter(cosdir : float, sindir : float, ksize : float, smoothness : float) -> np.array:
    if ksize % 2 != 1:
        raise ValueError('ksize')

    def g(x : np.array, y : np.array) -> np.array:
        return np.power(2, -smoothness * x * x - 4 * y * y)
    
    def h(x : np.array, y : np.array) -> np.array:
        return g(x, y - 1) - g(x, y + 1)

    i = np.arange(-(ksize//2), (ksize//2)+1)
    x = np.broadcast_to(i[np.newaxis, :], (ksize, ksize))
    y = np.broadcast_to(i[:, np.newaxis], (ksize, ksize))

    u = x * cosdir + y * sindir
    v = y * cosdir - x * sindir

    f = h(u, v)
    scale = np.sum(np.maximum(0, f))

    f /= scale

    return f

def dirfilters(dirs, ksize = 5, smoothness = 0.5):
    cosdirs, sindirs = cossindir(dirs)

    w = np.empty((ksize, ksize, dirs))
    
    for i in range(dirs):
        w[:, :, i] = dirfilter(cosdirs[i], sindirs[i], ksize, smoothness)

    return w

#for d in [4, 8, 16, 32]:
#    ca, sa = cossindir(d)
#    ce, se = np.cos(np.linspace(0, 1, d+1)[:-1] * math.pi), np.sin(np.linspace(0, 1, d+1)[:-1] * math.pi)
#
#    #print(ca)
#    #print(ce)
#    #print(sa)
#    #print(se)
#
#    print(np.sum(np.abs(ca - ce)))
#    print(np.sum(np.abs(sa - se)))

