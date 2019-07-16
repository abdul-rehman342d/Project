using System;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.WebSockets;
using Common;
using Common.Extension;
using Logger;

namespace LibNeeo.IO
{
    /// <summary>
    /// Provides methods to manipulated different files related operations including creating, deleting of files.
    /// </summary>
    public class File
    {
        /// <summary>
        /// 
        /// </summary>
        public NeeoFileInfo Info { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FileDataStream FileStream { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SourcePath { get; set; }

        #region Static Member Functions

        /// <summary>
        /// Save file to the mentioned directory.
        /// </summary>
        /// <param name="file">An object containing file information and its data.</param>
        /// <returns>true if file is successfully saved; otherwise, false.</returns>
        public static void Save(File file)
        {
            string filePath = null;

            FileStream fileStream = null;

            if (file.Info.FullName != null)
            {
                filePath = Path.Combine(file.Info.FullPath, file.Info.FullName);
            }
            else
            {
                filePath = Path.Combine(file.Info.FullPath, MediaUtility.AddFileExtension(file.Info.Name, file.Info.MediaType));
            }
            
            if (file.Data != null)
            {
                if (System.IO.File.Exists(filePath))
                {
                    LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Deleted avatar while updating it.");
                    System.IO.File.Delete(filePath);
                }
                try
                {
                    byte[] fileDataBinary = null;
                    fileDataBinary = Convert.FromBase64String(file.Data);
                    //fileStream = System.IO.File.OpenWrite(filePath);
                    //fileStream.Write(fileDataBinary, 0, fileDataBinary.Length);
                    using (MemoryStream stream = new MemoryStream(fileDataBinary, 0, fileDataBinary.Length))
                    {
                        System.Drawing.Image.FromStream(stream, true).Save(filePath, ImageFormat.Jpeg);
                        var fileInfo = new FileInfo(filePath);
                        file.Info.CreationTimeUtc = fileInfo.CreationTimeUtc;
                        file.Info.Length = fileInfo.Length;
                    }
                }
                catch (FormatException formatExp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, formatExp.Message, formatExp);
                    throw new ApplicationException(CustomHttpStatusCode.InvalidFileData.ToString("D"));
                }
                catch (System.Runtime.InteropServices.ExternalException extExp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, extExp.Message, extExp);
                    throw new ApplicationException(CustomHttpStatusCode.FileFormatMismatched.ToString("D"));
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }
            else if (file.FileStream != null)
            {
                if (System.IO.File.Exists(filePath))
                {
                    LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Deleted avatar while updating it.");
                    System.IO.File.Delete(filePath);
                }
                try
                {
                    fileStream = System.IO.File.OpenWrite(filePath);
                    if (file.FileStream.Postion > 0)
                    {
                        if (file.FileStream.Postion > fileStream.Length)
                        {
                            fileStream.Position = file.FileStream.Postion - 1;
                        }
                        else
                        {
                            throw new IndexOutOfRangeException("File seek position is invalid.");
                        }
                    }
                    file.FileStream.Stream.CopyTo(fileStream);
                    var info = new FileInfo(filePath);
                    file.Info.CreationTimeUtc = info.CreationTimeUtc;
                }
                catch (Exception exception)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                    throw;
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }
        }
        /// <summary>
        /// Checks the existance of the file.
        /// </summary>
        /// <param name="path">Path of the file to check.</param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }
        /// <summary>
        /// Delete file from the mentioned path.
        /// </summary>
        /// <param name="fileName">Name of the file to delete.</param>
        /// <param name="directoryPath">Directory path of the file.</param>
        /// <returns>true if file is successfully deleted; otherwise, false.</returns>
        public static bool Delete(string fileName, string directoryPath)
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
        /// Delete file from the mentioned path.
        /// </summary>
        /// <param name="filePath">Path of the file to delete.</param>
        /// <returns></returns>
        public static bool Delete(string filePath)
        {
            bool isDeleted = false;
            try
            {
                System.IO.File.Delete(filePath);
                isDeleted = true;
            }
            catch (System.UnauthorizedAccessException unAuthExp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, unAuthExp.Message, unAuthExp);
            }
            catch (IOException ioExp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ioExp.Message, ioExp);
            }
            return isDeleted;
        }
        /// <summary>
        /// Retrieve path of the file.
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
        /// <summary>
        /// Retrieve Avatar byte stream from user's directory,if it exists.
        /// </summary>
        /// <param name="directoryPath">A string containing user's directory path.</param>
        /// <returns>A stream object containing Avatar byte stream.</returns>
        public static Stream GetStream(string directoryPath)
        {
            using (var stream = new System.IO.FileStream(directoryPath,
                FileMode.Open, FileAccess.Read))
            {
                Stream mystream = new BufferedStream(stream);
                return mystream;
            }
        }

        public static byte[] GetBytesArray(string directoryPath)
        {
            return System.IO.File.ReadAllBytes(directoryPath);
        }

        public static byte[] GetBytesArray(string directoryPath,int startIndex, int endIndex)
        {
            var fileStream = System.IO.File.OpenRead(directoryPath);
            byte[] buffer = new byte[(endIndex == fileStream.Length ? endIndex : endIndex + 1) - startIndex];
            fileStream.Position = startIndex;
            fileStream.Read(buffer, 0, (endIndex - startIndex));
            return buffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="filePath"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static File GetFile(string fileID, string filePath)
        {
            if (System.IO.Directory.Exists(filePath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(filePath);
                //FileInfo fileInfo = dirInfo.GetFiles().SingleOrDefault(x => x.Name == MediaUtility.AddFileExtension(fileID, mediaType));
                FileInfo fileInfo =
                    dirInfo.GetFiles().SingleOrDefault(x => Path.GetFileNameWithoutExtension(x.Name) == fileID);
                if (fileInfo != null)
                {
                    File file = new File()
                    {
                        Info = new NeeoFileInfo() 
                        { 
                            Name = fileID, 
                            FullPath = fileInfo.FullName, 
                            CreationTimeUtc = fileInfo.LastWriteTimeUtc, 
                            MimeType = MimeTypeMapping.GetMimeType(fileInfo.Extension), 
                            Length = fileInfo.Length,
                            Extension = fileInfo.Extension,
                            MediaType = MimeTypeMapping.GetMimeTypeDetail(MimeTypeMapping.GetMimeType(fileInfo.Extension).GetDescription()).MediaType
                        }
                    };
                    return file;
                }
            }
            return null;
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //public static File GetFile(string name, string path)
        //{
        //    if (System.IO.Directory.Exists(path))
        //    {
        //        DirectoryInfo dirInfo = new DirectoryInfo(path);
        //        FileInfo fileInfo =
        //            dirInfo.GetFiles().SingleOrDefault(x => Path.GetFileNameWithoutExtension(x.Name) == name);

        //        if (fileInfo != null)
        //        {
        //            File file = new File() { Info = new NeeoFileInfo() { Name = name, FullPath = fileInfo.FullName, CreationTimeUtc = fileInfo.CreationTimeUtc, MimeType = MimeType.ImageJpeg } };// Replace it with a function to map the extension to the MimeType enum when audio and video files involved in sharing.
        //            return file;
        //        }
        //    }
        //    return null;
        //}

        #endregion
    }
}
