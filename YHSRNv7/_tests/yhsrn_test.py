import env

import numpy as np

from YHSRN import *

model1 = YHSRN()

weights = model1.parameters

print(weights.keys())

weights['hoge'] = 'A'

weights = model1.parameters

print(weights.keys())

model1.save_npz('model.npz')

model2 = YHSRN()

print(model1.parameters['entry_block/w1'])
print(model2.parameters['entry_block/w1'])

model2.load_npz('model.npz')

print(model1.parameters['entry_block/w1'])
print(model2.parameters['entry_block/w1'])