# Training Yamatani-based Homogeneous Super Resolution Network
# T.Yoshimura
# tensorflow 2.1.0

import env
import os, glob

import cv2

import tensorflow as tf

from YHSRN import YHSRN

from util.imagelister import *
from util.workspace_saver import *
from util.convergence_tester import *
from components.halfresize import halfresize
from components.loss import loss_srcnn
from components.rms_clip import rms_clip

# setting
data_format='NCHW'
snap_interval = 1024
sample_interval = 256
sample_historys = 256
resume = True

# hyperparams
imgsize = 256
batches = 32
packets = 4
momentum = 0.9
rms_limit = 1e-1
lr_init, lr_decay = 1e-3, 0.975
convergence_tester_samples = 500
subiters = batches // packets

assert batches % packets == 0

# modelparams
residual_channels = 64
residuals = 4
cascades = 3

# image loader
lister = ImageLister('Enter training images direcctory path.')

indexer = ImageIndexer(lister, batches, shuffle_seed = 1234)
batchmaker = GrayscaleImageBatchMaker(imgsize, batches, data_format)

# workspace saver
workspace = WorkspaceSaver(dirpath='_results', move_exists = False if resume else True)

# model
tf.random.set_seed(1234)

model = YHSRN(residual_channels, residuals, cascades, data_format=data_format)
optimizer = tf.keras.optimizers.SGD(learning_rate=lr_init, momentum=momentum, nesterov = False, name='MomentumSGD')
convergence_tester = ConvergenceTester(steps=convergence_tester_samples)

model.compile(optimizer=optimizer, run_eagerly=False)

needs_load_resume = False

# resume
last_iter = 0
if resume:
    modelfiles = glob.glob(workspace.snap + 'model_iter*[0-9].npz')
    for modelfile in modelfiles:
        i = int(modelfile[len(workspace.snap + 'model_iter'):-len('.npz')])
        if last_iter < i:
            last_iter = i
    if last_iter > 0:        
        indexer.skip_iteration(last_iter - 1)
        needs_load_resume = True

if not needs_load_resume:
    model.save_npz(workspace.snap + 'model_init.npz')

# train loop
with open(workspace.log + 'log.txt', 'a' if resume else 'w', buffering=1) as log:
    log.write('iter,loss_abs,loss_dir,loss_noise,lr\n')
    print('iter,loss_abs,loss_dir,loss_noise,lr')

    graphtrace = True
    
    while indexer.epoch < 1:
        imgpaths = indexer.next()
        iter = indexer.iteration
        hrs = batchmaker.make(imgpaths)

        grads, losses = None, None
        for p in range(0, batches, packets):
            # forward
            hr = tf.constant(hrs[p:p+packets])

            lr = halfresize(hr, data_format)

            with tf.GradientTape() as tape:
                sr = model(lr)

                loss, loss_dict = loss_srcnn(sr, hr, data_format=data_format)

            # backward
            if grads is None:
                grads = tape.gradient(loss, model.trainable_weights)
            else:
                append_grads = tape.gradient(loss, model.trainable_weights)
                new_grads = [gp + g for (gp, g) in zip(append_grads, grads)]
                grads = new_grads

            if losses is None:
                losses = loss_dict
            else:
                for key in losses.keys():
                    losses[key] += loss_dict[key]

        # update
        grads_cliped = [rms_clip(g, rms_limit) for g in grads]
        optimizer.apply_gradients(zip(grads_cliped, model.trainable_weights))

        # resume 
        if needs_load_resume:
            needs_load_resume = False

            model.load_npz(workspace.snap + 'model_iter%d.npz' % last_iter)
            model.load_optimizer_npz(workspace.snap + 'optimizer_iter%d.npz' % last_iter)
            print('loaded iter%d model.' % last_iter)
            continue

        # summary
        if iter == 1:            
            with open(workspace.log + 'modelsummary.txt', 'w') as ms:
                model.summary(line_length=512, print_fn=lambda str : ms.write(str + '\n'))

        # logging loss
        loss_abs, loss_dir, loss_noise = losses['abs'].numpy(), losses['dir'].numpy(), losses['noise'].numpy()
        loss_abs, loss_dir, loss_noise = loss_abs / subiters, loss_dir / subiters, loss_noise / subiters
        learning_rate = float(optimizer.learning_rate.numpy())

        log.write('%d,%e,%e,%e,%e\n' % (iter, loss_abs, loss_dir, loss_noise, learning_rate))
        print('%d,%e,%e,%e,%e,%e' % (iter, loss_abs, loss_dir, loss_noise, learning_rate, convergence_tester.r))

        # testing convergence
        sum_loss = loss_abs + loss_dir + loss_noise
        convergence_tester.plunge(sum_loss)

        # decay learning_rate
        if convergence_tester.is_convergence:
            optimizer.learning_rate.assign(learning_rate * lr_decay)
            convergence_tester.clear()

        # snapping model
        if iter % snap_interval == 0:
            model.save_npz(workspace.snap + 'model_iter%d.npz' % iter)
            model.save_optimizer_npz(workspace.snap + 'optimizer_iter%d.npz' % iter)

            try:
                model.load_npz(workspace.snap + 'model_iter%d.npz' % iter)
                model.load_optimizer_npz(workspace.snap + 'optimizer_iter%d.npz' % iter)
            except:
                print('Fail save iter=%d' % iter)
            else:
                print('Success save iter=%d' % iter)

        # sammping images
        if iter % sample_interval == 0:
            sample_index = (iter // sample_interval) % sample_historys
            sr = sr.numpy()
            for i in range(packets):
                if data_format == 'NHWC':
                    img = sr[i, :, :, 0]
                elif data_format == 'NCHW':
                    img = sr[i, 0, :, :]

                img *= 255

                cv2.imwrite(workspace.sample + '%d.png' % (i + sample_index * packets), img)

model.save_npz(workspace.snap + 'model.npz')