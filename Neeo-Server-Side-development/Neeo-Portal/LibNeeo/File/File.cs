using System;
using System.Web;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Common;
using Logger;

namespace LibNeeo
{
    /// <summary>
    /// Handles user's files stored on the server.
    /// </summary>
    public static class File
    {
        #region Member Functions

        /// <summary>
        /// Save file to user's directory.
        /// </summary>
        /// <param name="fileName">A string containing phone number as user id.</param>
        /// <param name="fileData">A base64 encoded string containing the file data to be stored in file system.</param>
        /// <param name="directoryPath">A string containing user's directory path.</param>
        /// <returns>true if file is successfully saved; otherwise, false.</returns>
        /// 
        public static bool SaveFile(string fileName, string fileData, string directoryPath)
        {
            string filePath = Path.Combine(directoryPath, fileName);
            byte[] fileDataBinary;

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            try
            {
                fileDataBinary = Convert.FromBase64String(fileData);
            }
            catch (FormatException formatExp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, formatExp.Message, formatExp);
                throw new ApplicationException(CustomHttpStatusCode.InvalidFileData.ToString("D"));
            }
            try
            {
                using (MemoryStream stream = new MemoryStream(fileDataBinary, 0, fileDataBinary.Length))
                {
                    Image.FromStream(stream, true).Save(filePath, ImageFormat.Jpeg);
                }
                return true;
            }
            catch (System.Runtime.InteropServices.ExternalException extExp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, extExp.Message, extExp);
                throw new ApplicationException(CustomHttpStatusCode.FileFormatMismatched.ToString("D"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public static string SaveFile(string directoryPath, string fileData)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Delete file from user's directory with specified file id.
        /// </summary>
        /// <param name="fileName">A string containing the file id that has to be deleted.</param>
        /// <param name="directoryPath">A string containing user's directory path.</param>
        /// <returns>true if file is successfully deleted; otherwise, false.</returns>
        public static bool DeleteFile(string fileName, string directoryPath)
        {
            string filePath = Path.Combine(directoryPath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                }
                catch (System.UnauthorizedAccessException unAuthExp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, unAuthExp.Message, unAuthExp);
                    return false;
                }
                catch (IOException ioExp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ioExp.Message, ioExp);
                }
            }
            return true;
        }

        /// <summary>
        /// Retrieve file path from user's directory,if it exists.
        /// </summary>
        /// <param name="fileName">A string containing the file id that has to be retrieved.</param>
        /// <param name="directoryPath">A string containing user's directory path.</param>
        /// <returns>A string containing path of the request file, if it exists;Otherwise empty.</returns>
        public static string GetFilePath(string fileName, string directoryPath)
        {
            string filePath = Path.Combine(directoryPath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                return filePath;
            }
            else
            {
                return "";
            }
        }

        #endregion
    }
}
