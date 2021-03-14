# YHSRNv7
 Yamatani-based Homogeneous Super Resolution Network v7 @ JSAI2021

## Licence
[CC BY-NC-ND](https://github.com/tk-yoshimura/JSAI2021_YHSRNv7/blob/main/LICENSE)

## Author

[tk-yoshimura](https://github.com/tk-yoshimura)

## Train Usage

1. Generate "RandomPattern" images.  
InfinityPatterns, InfinityPatternsGenerator(directories:512 images:8192)

2. Make blur from "RandomPattern".  
makeblur

3. Train and Sample SR model "YHSRNv7".  
YHSRNv7

4. Score SR.  
DIV2K MATLAB scoring

## Validate Usage

3. Sample SR model "YHSRNv7".  
YHSRNv7

4. Score SR.  
DIV2K MATLAB scoring