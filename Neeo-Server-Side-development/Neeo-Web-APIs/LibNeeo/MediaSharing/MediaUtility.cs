using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace LibNeeo.IO
{
    public static class MediaUtility
    {


        /// <summary>
        /// Appends image extension with given image identifier.
        /// </summary>
        /// <param name="fileName">A string containing the image identifier without image extension.</param>
        /// <returns></returns>
        public static string AddFileExtension(string fileName, MediaType mediaType)
        {
            string resultingString = null;
            switch (mediaType)
            {
                case MediaType.Image:
                    string imageExtension = ConfigurationManager.AppSettings[NeeoConstants.ImageExtension].ToString();
                    resultingString = fileName + "." + imageExtension;
                    break;
                case MediaType.Audio:
                    // add server audio file extension
                    resultingString = null;
                    break;
                case MediaType.Video:
                    // add server audio file extension
                    resultingString = null;
                    break;
            }
            return resultingString;
        }

        /// <summary>
        /// Resizes the given image to specified height and width and returns the byte array of the resized image. 
        /// </summary>
        /// <param name="imagePath">A string containing the image path.</param>
        /// <param name="height">An integer specifying the required height.</param>
        /// <param name="width">An integer specifying the required width.</param>
        /// <returns>Byte array of the resized image.</returns>
        public static byte[] ResizeImage(string imagePath, int height, int width)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath);
            if (height <= 0 || width <= 0 || height > image.Height || width > image.Width)
            {
                image.Dispose();
                return System.IO.File.ReadAllBytes(imagePath);
            }
            else
            {
                System.Drawing.Image newImage = image.GetThumbnailImage(height, width, null, IntPtr.Zero);
                using (MemoryStream stream = new MemoryStream())
                {
                    newImage.Save(stream, ImageFormat.Jpeg);
                    image.Dispose();
                    newImage.Dispose();
                    return stream.ToArray();
                }
            }
        }

        /// <summary>
        /// Resizes the given image to specified height and width and returns the byte array of the resized image. 
        /// </summary>
        /// <param name="imagePath">A string containing the image path.</param>
        /// <param name="height">An integer specifying the required height.</param>
        /// <param name="width">An integer specifying the required width.</param>
        /// <returns>Byte array of the resized image.</returns>
        public static Stream ResizeImage(string imagePath, int height, int width, bool isOverride = true)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath);
            using (Stream stream = new MemoryStream())
            {
                if (height <= 0 || width <= 0 || height > image.Height || width > image.Width)
                {
                    image.Save(stream, ImageFormat.Jpeg);
                    image.Dispose();
                    return stream;
                }
                System.Drawing.Image newImage = image.GetThumbnailImage(height, width, null, IntPtr.Zero);
                newImage.Save(stream, ImageFormat.Jpeg);
                image.Dispose();
                newImage.Dispose();
                return stream;
            }
        }

        /// <summary>
        /// Determines the path to user directory in the application hierarchical directory structure reference with root directory.
        /// </summary>
        /// <param name="name">A string containing the user directory name.</param>
        /// <returns>A path to the user directory in the application hierarchical directory structure.</returns>
        public static string GetHierarchicalPath(string name, int hierarchyLevelLimit)
        {
            string hierarchicalPath = "";
            int hierarchyLevels = name.Length / hierarchyLevelLimit;
            int remainder = name.Length % hierarchyLevelLimit;
            int index = 0;
            for (int i = 0; i < (remainder == 0 ? hierarchyLevels - 1 : hierarchyLevels); i++)
            {
                hierarchicalPath = Path.Combine(hierarchicalPath, name.Substring(index, hierarchyLevelLimit));
                index += hierarchyLevelLimit;
            }
            return hierarchicalPath;
        }
    }
}
