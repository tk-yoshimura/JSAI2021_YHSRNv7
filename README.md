# YHSRNv7
 Yamatani-based Homogeneous Super Resolution Network v7 @ JSAI2021
 
![fish](https://github.com/tk-yoshimura/TensorShader/blob/master/images/fish.svg)  
![fireworks](https://github.com/tk-yoshimura/TensorShader/blob/master/images/fireworks.png)

## Licence
[CC BY-NC-ND](https://github.com/tk-yoshimura/JSAI2021_YHSRNv7/blob/main/LICENSE)

## Author
[tk-yoshimura](https://github.com/tk-yoshimura)

## Report
[J-STAGE](https://www.jstage.jst.go.jp/article/pjsai/JSAI2021/0/JSAI2021_4G2GS2k05/_article/-char/ja/)  
[slideshare](https://www.slideshare.net/TakumaYoshimura2/jsai2021-4g2gs2k05-yamatani-activation)

## Usage
Tensorflow 2.1.0  
CUDA 11.*

## Train Procedure

1. Generate "RandomPattern" images.  
Run InfinityPatterns, InfinityPatternsGenerator(directories:512 images:8192)

2. Make blur from "RandomPattern".  
Run makeblur/makebulr.py

3. Train and Sample SR model "YHSRNv7".  
Run YHSRNv7/train.py

4. Score SR.  
Run DIV2K\_MATLAB\_scoring eval_div2k_yhsrnv7.m

## Validate Procedure

1. Sample SR model "YHSRNv7".  
Run YHSRNv7/sample.py

2. Score SR.  
Run DIV2K\_MATLAB\_scoring eval_div2k_yhsrnv7.m

## Generate SR Image

Run YHSRNv7/sample.py
