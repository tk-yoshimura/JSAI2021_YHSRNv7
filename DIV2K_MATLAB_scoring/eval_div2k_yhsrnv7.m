dirpath_hr = 'D:\workspace\datasets\div2k\DIV2K_valid_HR';
dirpath_sr = 'D:\workspace\datasets\div2k\DIV2K_valid_LR_bicubic\X2_yhsrn_v7';

sr_shuffix = 'x2_yhsrnv7';
filetype = '.png';

file = fopen(strcat(sr_shuffix, '_result.csv'), 'w');

fprintf(file, '%s\n', sr_shuffix);
fprintf(file, 'imgname,psnr,ssim\n');

imgpaths = dir(strcat(dirpath_hr, '\*', filetype));

for index = 1 : length(imgpaths)
  imgpath_hr = fullfile(imgpaths(index).folder, imgpaths(index).name);
  [_, imgname, _] = fileparts(imgpath_hr);  
  imgpath_sr = strcat(dirpath_sr, '\', imgname, sr_shuffix, filetype);
  
  disp(strcat('Processing...', imgname, filetype));
  
  psnr = NTIRE_PeakSNR_imgs(imgpath_hr, imgpath_sr);
  disp(strcat('psnr = ', num2str(psnr, 8)));
  
  [mssim, ssim_map] = NTIRE_SSIM_imgs(imgpath_hr, imgpath_sr);
  disp(strcat('ssim = ', num2str(mssim, 8)));
  
  fprintf(file, '%s,%.8f,%.8f\n', imgname, psnr, mssim);
end

fclose(file);