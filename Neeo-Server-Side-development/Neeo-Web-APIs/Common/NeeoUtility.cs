using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing.Imaging;
using System.ServiceModel.Web;
using Common.Extension;
using PhoneNumbers;
using Logger;
using System.Web;
using System.Web.Http;
using System.Net.Http;

namespace Common
{
    /// <summary>
    /// Contains all commonly used utility functions.
    /// </summary>
    public static class NeeoUtility
    {
        #region Data Members

        /// <summary>
        /// A static string containing the server domain.
        /// </summary>
        private static string _domain;

        /// <summary>
        /// A static string containing the file server address (url).
        /// </summary>
        private static string _fileServerUrl;

        /// <summary>
        /// A static string containing the file store application port on the file servers.
        /// </summary>
        private static string _fileStorePort;

        /// <summary>
        /// A static string containing the avatar handler information.
        /// </summary>
        private static string _avatarHandler;

        /// <summary>
        /// A static string containing the image handler information.
        /// </summary>
        private static string _imageHandler;

        /// <summary>
        /// A static string containing the image extension.
        /// </summary>
        /// <remarks>File server stores images in the image format specified with this extension. </remarks>
        private static string _imageExtension;

        /// <summary>
        /// A static string variable containing the key for signature generation.
        /// </summary>
        private static string _signatureKey;

        /// <summary>
        /// A static string variable containing the key for data encryption/decryption.
        /// </summary>
        private static string _encryptionDecryptionKey;

        private const string EncryptionDecryptionKeySuffix = "~5pA1#Kz";

        /// <summary>
        /// A static string variable containing web protocol for the url generation.
        /// </summary>
        private static string _webProtocol;


        #endregion

        #region Member Functions

        /// <summary>
        /// Returns Integer value from given string.
        /// </summary>
        /// <param name="value">input string from user.</param>
        /// <returns>an integer value.</returns>
        public static int GetIntegerValue(string value)
        {

            string number = "";
            foreach (char var in value)
            {
                if ((ushort)var >= 48 && (ushort)var <= 57)
                {
                    number += var;
                }
            }
            int temp;
            return int.TryParse(number, out temp) ? temp : -1;
        }

        /// <summary>
        /// Converts the given username to its jid.
        /// </summary>
        /// <param name="username">A string containing the user's account username.</param>
        /// <returns>A string representing it its jid.</returns>
        public static string ConvertToJid(string username)
        {
            if (_domain == null)
            {
                _domain = ConfigurationManager.AppSettings[NeeoConstants.Domain].ToString();
            }
            return username + "@" + _domain;
        }

        /// <summary>
        /// Checks a string variable for null or empty value.
        /// </summary>
        /// <param name="value">A string containing the value. It could be null as well.</param>
        /// <returns>true if string is either null or empty; otherwise false.</returns>
        public static bool IsNullOrEmpty(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            else
            {
                if (value.Trim().Equals(string.Empty))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Validates the given value belongs to provide enum.
        /// </summary>
        /// <param name="enumType">It contains the enum type.</param>
        /// <param name="value">A string containing the value that has to be checked for its validity in specified enumeration.</param>
        /// <returns></returns>
        public static bool IsValidEnum(Type enumType, string value)
        {
            int number = 0;
            if (int.TryParse(Enum.Parse(enumType, value, true).ToString(), out number))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Generates the time stamp for a give date time.
        /// </summary>
        /// <param name="datetime">An 'datetime' object containing the given date time.</param>
        /// <returns>A unsigned long integer containing the time stamp generated for the given date time.</returns>
        public static ulong GetTimeStamp(DateTime datetime)
        {
            return Convert.ToUInt64(datetime.Year.ToString().Substring(2, 2) + datetime.Second.ToString() + datetime.Day.ToString() + datetime.Millisecond.ToString() +
                   datetime.Hour.ToString() + datetime.Month.ToString() + datetime.Minute.ToString());
        }

        /// <summary>
        /// Gets filename is offline file format.
        /// </summary>
        /// <param name="senderID">A string containing the sender id.</param>
        /// <param name="timeStamp">A string containing the timestamp.</param>
        /// <returns>A string containing the filename in offline file format.</returns>
        public static string GetFileNameInOfflineFileFormat(string senderID, string timeStamp)
        {
            if (_imageExtension == null)
            {
                _imageExtension = ConfigurationManager.AppSettings[NeeoConstants.ImageExtension].ToString();
            }
            return senderID + "-" + timeStamp + "." + _imageExtension;
        }

        /// <summary>
        /// Formats the phone number as an international phone number by appending "+" with it.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number for sending SMS.</param>
        /// <returns>A string containing formatted phone number.</returns>
        public static string FormatAsIntlPhoneNumber(string phoneNumber)
        {
            return "+" + phoneNumber;
        }

        /// <summary>
        /// Validates user's phone number whether it is correct or not.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number.</param>
        /// <returns>true if provide phone number is valid; otherwise false.</returns>
        public static bool ValidatePhoneNumber(string phoneNumber)
        {

            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            try
            {
                PhoneNumber phoneNumberInfo = phoneNumberUtil.Parse(phoneNumber, null);

                bool isPossiblePhoneNumber = phoneNumberUtil.IsPossibleNumber(phoneNumberInfo);
                bool isValidPhoneNumber = phoneNumberUtil.IsValidNumber(phoneNumberInfo);

                if (isPossiblePhoneNumber || isValidPhoneNumber)
                {
                    var countryCode = phoneNumberInfo.CountryCode.ToString();
                    var numberWithoutCountryCode = phoneNumber.Substring(countryCode.Length + 1, phoneNumber.Length - (countryCode.Length + 1));

                    if (numberWithoutCountryCode.Substring(0, 1) == "0")
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().GetType(), exp.Message);
                return false;
            }
        }

        /// <summary>
        /// Check whether the phone number is in international format.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number.</param>
        /// <returns>If the phone number is in international format, it returns true; otherwise, false.</returns>
        public static bool IsPhoneNumberInInternationalFormat(string phoneNumber)
        {
            string[] internationalNumberPrefix = new string[] { "00", "+" };
            if (phoneNumber.Length >= 2)
            {
                if (phoneNumber.Substring(0, 2) != internationalNumberPrefix[0] && phoneNumber.Substring(0, 1) != internationalNumberPrefix[1])
                {
                    return false;
                }
                return true;
            }
            throw new ApplicationException(CustomHttpStatusCode.InvalidNumber.ToString("D"));
        }

        /// <summary>
        /// Generates signature from given data by MD5 hashing technique.
        /// </summary>
        /// <param name="fileName">A string containing the file name.</param>
        /// <returns>A string containing the generated signature.</returns>
        public static string GenerateSignature(string fileName)
        {
            if (_signatureKey == null)
            {
                _signatureKey = ConfigurationManager.AppSettings[NeeoConstants.SignatureKey].ToString();
            }
            string hashableString = fileName + _signatureKey;
            return GenerateMd5Hash(hashableString);
        }

        /// <summary>
        /// Generates MD5 hash.
        /// </summary>
        /// <param name="hashingData">A string containing the file name.</param>
        /// <returns>A string containing the generated signature.</returns>
        public static string GenerateMd5Hash(string hashingData)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashedData = md5.ComputeHash(Encoding.UTF8.GetBytes(hashingData));

                for (int i = 0; i < hashedData.Length; i++)
                {
                    stringBuilder.Append(hashedData[i].ToString("x2"));
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the formated activation message text.
        /// </summary>
        /// <param name="activationCode">A string containing the activation code</param>
        /// <param name="appKey">A string containing the appKey(For Android).</param>
        /// <returns>A string containing the formated activation message.</returns>
        public static string GetActivationMessage(string activationCode, string appKey = null)
        {
            string activationCodeMask = ConfigurationManager.AppSettings[NeeoConstants.ActivationCodeMask].ToString();
            string messageText = ConfigurationManager.AppSettings[NeeoConstants.ActivationSmsText].ToString();
            messageText = messageText.Replace(".", "." + Environment.NewLine);
            messageText = messageText.Replace(activationCodeMask, activationCode);

            if (!string.IsNullOrWhiteSpace(appKey))
            {
                messageText = string.Concat("<#> ", messageText, appKey);
            }

            return messageText;
        }

        /// <summary>
        /// Converts unix time (in milliseconds) to date time.
        /// </summary>
        /// <param name="milliseconds">A long specifying the the number of milliseconds.</param>
        /// <returns>date in date time format.</returns>
        public static DateTime ConvertUnixTime(long milliseconds)
        {
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            return posixTime.AddMilliseconds(milliseconds);
        }

        /// <summary>
        /// Check whether the request from valid user or not.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="hashKeyFromClient">A string containing Hash Key from Client side.</param>
        /// <returns>If the User is valid  it returns true; otherwise, false.</returns>
        public static bool AuthenticateUserRequest(string userID, string hashKeyFromClient)
        {
            if (hashKeyFromClient != null)
            {
                userID += DateTime.UtcNow.Date;
                byte[] tmpsource = ASCIIEncoding.ASCII.GetBytes(userID);
                byte[] tempHash = new MD5CryptoServiceProvider().ComputeHash(tmpsource);

                for (int i = 0; i < 2; i++)
                {
                    tempHash = new MD5CryptoServiceProvider().ComputeHash(tempHash);
                }
                string HashKeyFromServer = ByteArrayToString(tempHash);
                if (hashKeyFromClient.Equals(HashKeyFromServer))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrInput">It is array of bytes</param>
        /// <returns>it returns a string after conversion from byte array</returns>
        private static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();

        }

        #endregion

        #region Encryption and Decryption

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string EncryptData(string data)
        {
            if (_encryptionDecryptionKey == null)
            {
                _encryptionDecryptionKey = ConfigurationManager.AppSettings[NeeoConstants.EncryptionDecryptionKey].ToString() + EncryptionDecryptionKeySuffix;
            }
            byte[] dataBytes = Encoding.Unicode.GetBytes(data);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_encryptionDecryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataBytes, 0, dataBytes.Length);
                        cs.Close();
                    }
                    data = Convert.ToBase64String(ms.ToArray());
                }
            }
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <returns></returns>
        public static string DecryptData(string encryptedData)
        {
            if (_encryptionDecryptionKey == null)
            {
                _encryptionDecryptionKey = ConfigurationManager.AppSettings[NeeoConstants.EncryptionDecryptionKey].ToString() + EncryptionDecryptionKeySuffix;
            }
            encryptedData = encryptedData.Replace(" ", "+");
            byte[] dataBytes = Convert.FromBase64String(encryptedData);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_encryptionDecryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataBytes, 0, dataBytes.Length);
                        cs.Close();
                    }
                    encryptedData = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return encryptedData;
        }

        #endregion

        #region Service Related Methods

        /// <summary>
        /// Seting custome status code in service response header
        /// </summary>
        /// <param name="code">An enumeration representing custome status code.</param>
        public static void SetServiceResponseHeaders(CustomHttpStatusCode code)
        {
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
            response.StatusCode = (System.Net.HttpStatusCode)code;
            string description = code.GetDescription();
            if (description != null)
                response.StatusDescription = description;
        }

        public static MucMessageType ExtractMucMessageType(string text)
        {
            var result = text.Split(new[] { '~' }, 1);

            text = result[1];
            return (MucMessageType) Convert.ToInt32(result[0]);
        }



        #endregion

        #region Swagger Configuration

        public static bool IsSwaggerDisabled()
        {
            if (ConfigurationManager.AppSettings[NeeoConstants.DisableSwagger] == null || ConfigurationManager.AppSettings[NeeoConstants.DisableSwagger] == "True")
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
