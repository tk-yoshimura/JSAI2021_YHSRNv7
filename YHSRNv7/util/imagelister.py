import os, glob
import numpy as np

import cv2

class ImageLister():
    def __init__(self, dirpath : str):
        dirpath += '/'
        dirpath = dirpath.replace('\\', '/').replace('//', '/')

        imgpaths = glob.glob(dirpath + '**/*.png', recursive=True)
        imgpaths.sort()

        imagecounts = len(imgpaths)

        self.__imgpaths = imgpaths
        self.__imagecounts = imagecounts

        print('%d image loaded.' % imagecounts)

    def __len__(self):
        return self.__imagecounts

    def __getitem__(self, index):
        return self.__imgpaths[index]

    @property
    def counts(self):
        return self.__imagecounts

class ImageIndexer():
    def __init__(self, lister : ImageLister, batches : int, shuffle_seed : int):
        if lister.counts < batches:
            raise ValueError('batches')

        np.random.seed(shuffle_seed)

        index = np.arange(0, len(lister))
        np.random.shuffle(index)

        self.__lister = lister
        self.__index = index
        self.__pos = 0
        self.__epoch = 0
        self.__iteration = 0
        self.__batches = batches

    def skip_iteration(self, iter):
        skipcounts = iter * self.__batches

        self.__epoch += skipcounts // self.__lister.counts
        self.__pos += skipcounts % self.__lister.counts
        self.__iteration += iter

        while self.__pos > self.__lister.counts:
            self.__epoch += 1
            self.__pos -= self.__lister.counts

    def next(self):        
        self.__iteration += 1

        if self.__pos + self.__batches <= self.__lister.counts:
            indexes = self.__index[self.__pos : self.__pos + self.__batches]
            self.__pos += self.__batches
        else:
            indexes = np.concatenate([
                self.__index[self.__pos : self.__lister.counts], 
                self.__index[0 : self.__pos + self.__batches - self.__lister.counts]])

            self.__epoch += 1
            self.__pos += self.__batches
            self.__pos -= self.__lister.counts

        imgpaths = [self.__lister[index] for index in indexes]

        return imgpaths

    @property
    def epoch(self):
        return self.__epoch

    @property
    def iteration(self):
        return self.__iteration

    @property
    def batches(self):
        return self.__batches

class GrayscaleImageBatchMaker():
    def __init__(self, imgsize, batches, data_format = 'NHWC'):
        if not data_format in ['NHWC', 'NCHW']:
            raise ValueError('invalid data_format.')

        self.__imgsize = imgsize
        self.__batches = batches
        self.__data_format = data_format
        
    def make(self, imgpaths):
        if len(imgpaths) != self.__batches:
            raise ValueError('imgpaths')

        if self.__data_format == 'NHWC':
            xs = np.empty((self.__batches, self.__imgsize, self.__imgsize, 1), dtype=np.float32)
        elif self.__data_format == 'NCHW':
            xs = np.empty((self.__batches, 1, self.__imgsize, self.__imgsize), dtype=np.float32)

        for i, imgpath in enumerate(imgpaths):
            img = cv2.imread(imgpath)
            xs[i] = self.preprocess_image(img)

        return xs

    def preprocess_image(self, img):
        # target for grayscale image
        if self.__data_format == 'NHWC':
            x = img[:, :, 0][:, :, np.newaxis].astype(np.float32) / 255
        elif self.__data_format == 'NCHW':
            x = img[:, :, 0][np.newaxis, :, :].astype(np.float32) / 255

        return x

