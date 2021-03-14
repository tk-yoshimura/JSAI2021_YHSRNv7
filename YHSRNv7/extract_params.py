import os
import numpy as np

dirpath_result = '_results/'
dirpath_snap = dirpath_result + 'snap/'
dirpath_params = dirpath_result + 'params/'

params = np.load(dirpath_snap + 'model.npz')

os.makedirs(dirpath_params, exist_ok=True)

with open(dirpath_params + 'paramlist.txt', 'w') as f:
    for key, param in params.items():
        filepath = dirpath_params + key.replace('/', '_') + '.txt'

        np.savetxt(filepath, param.flatten())

        f.write('{}:{}\n'.format(key, param.shape))