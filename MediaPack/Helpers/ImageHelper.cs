using MediaPack.Data;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace MediaPack.Helpers
{
    public static class ImageHelper
    {
        //public static byte[] ImageToByteArray(this BitmapImage imageSource, object param2)
        //{
        //    byte[] buffer = null;

        //    var stream = imageSource.StreamSource;

        //    if (stream != null && stream.Length > 0)
        //    {
        //        using BinaryReader br = new BinaryReader(stream);
        //        buffer = br.ReadBytes((int)stream.Length);
        //    }
        //    else
        //    {
        //        if (imageSource.UriSource != null)
        //        {
        //            stream = new MemoryStream(File.ReadAllBytes(imageSource.UriSource.LocalPath));

        //            if (stream != null && stream.Length > 0)
        //            {
        //                using BinaryReader br = new BinaryReader(stream);
        //                buffer = br.ReadBytes((int)stream.Length);
        //            }
        //        }
        //    }

        //    return buffer;
        //}

        //public static BitmapImage ByteArrayToBitmapImage(this byte[] byteArrayIn, object param2)
        //{
        //    if (byteArrayIn == null) return null;
        //    var ms = new MemoryStream(byteArrayIn);

        //    var image = new BitmapImage();
        //    image.BeginInit();
        //    image.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
        //    image.CacheOption = BitmapCacheOption.OnLoad;
        //    image.StreamSource = ms;
        //    image.EndInit();

        //    return image;
        //}

        //public static Stream StreamToImage(this Stream originalStream, ImageFormat format)
        //{
        //    var image = Image.FromStream(originalStream);

        //    var stream = new MemoryStream();
        //    image.Save(stream, format);
        //    stream.Position = 0;
        //    return stream;
        //}

        //public static BitmapImage PathToBitmapImage(this string path)
        //{
        //    var image = new BitmapImage();
        //    image.BeginInit();
        //    image.CacheOption = BitmapCacheOption.OnLoad;
        //    image.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
        //    image.EndInit();

        //    return image;
        //}

        public static BitmapImage PathToBitmapImage(this string path)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            image.EndInit();

            return image;
        }

        public static byte[] ImageToByteArray(this BitmapImage imageSource, object pram)
        {
            byte[] buffer = null;

            var stream = imageSource.StreamSource;

            if (stream != null && stream.Length > 0)
            {
                using (BinaryReader br = new BinaryReader(stream))
                    buffer = br.ReadBytes((int)stream.Length);
            }
            else
            {
                if (imageSource.UriSource != null)
                {
                    stream = new MemoryStream(File.ReadAllBytes(imageSource.UriSource.LocalPath));

                    if (stream != null && stream.Length > 0)
                    {
                        using (BinaryReader br = new BinaryReader(stream))
                            buffer = br.ReadBytes((int)stream.Length);
                    }
                }
            }

            return buffer;
        }

        public static BitmapImage ByteArrayToBitmapImage(this byte[] byteArray, object pram)
        {
            if (byteArray == null) return null;
            var ms = new MemoryStream(byteArray);

            var image = new BitmapImage();
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }

        // -------------------------------- Image area

        public static string BitmapImageToPath(this BitmapImage bitmapImage)
        {
            var fullPath = bitmapImage.UriSource.OriginalString;

            return fullPath?.Replace(@$"{Settings.CurrentDirectory}\", null);
        }

        public static void SaveImage(this BitmapImage image, string localFilePath)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using var filestream = new FileStream(localFilePath, FileMode.Create);
            encoder.Save(filestream);
        }
    }
}
