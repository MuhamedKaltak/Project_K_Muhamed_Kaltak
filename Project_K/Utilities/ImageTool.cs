using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Utilities
{
    public static class ImageTool
    {
        public static byte[] CompressAndResizeImage(string filepath, int targetWidth, int targetHeight, int quality)
        {
            using(var imageroot = SKImage.FromEncodedData(filepath))
            using (var original = SKBitmap.FromImage(imageroot))
            {
                int newWidth, newHeight;
                CalculateTargetDimensions(original.Width, original.Height, targetWidth, targetHeight, out newWidth, out newHeight);

                using (var resized = original.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High))
                using (var image = SKImage.FromBitmap(resized))
                using (var data = image.Encode(SKEncodedImageFormat.Jpeg, quality))
                {
                    return data.ToArray();
                }
            }
        }



        private static void CalculateTargetDimensions(int originalWidth, int originalHeight, int targetWidth, int targetHeight, out int newWidth, out int newHeight)
        {
            double aspectRatio = (double)originalWidth / originalHeight;

            if (targetWidth > 0 && targetHeight > 0)
            {
                newWidth = targetWidth;
                newHeight = targetHeight;
            }
            else if (targetWidth > 0)
            {
                newWidth = targetWidth;
                newHeight = (int)(targetWidth / aspectRatio);
            }
            else if (targetHeight > 0)
            {
                newWidth = (int)(targetHeight * aspectRatio);
                newHeight = targetHeight;
            }
            else
            {
                newWidth = originalWidth;
                newHeight = originalHeight;
            }
        }


    }
}
