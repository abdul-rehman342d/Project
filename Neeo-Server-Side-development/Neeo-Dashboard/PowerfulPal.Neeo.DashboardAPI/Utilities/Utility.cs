using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PhoneNumbers;

namespace PowerfulPal.Neeo.DashboardAPI.Utilities
{
    public static class Utility
    {
        public static bool IsNullOrEmpty(object obj)
        {
            if (obj != null)
            {
                if (obj.ToString().Trim().Equals(""))
                {
                    return true;
                }
                return false;
            }
            return true;
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

        public static string GetCountryNameOnline(string countryCode, string path, Dictionary<string, string> countryCodeDictionary)
        {
            object response = JsonConvert.DeserializeObject<object>(new RestClient("http://restcountries.eu/rest/v1/callingcode/" + countryCode).MakeRequest());
            string countryName = (string)((dynamic)response)[0]["name"].Value;
            countryCodeDictionary.Add(countryCode, countryName);
            string newCountry = JsonConvert.SerializeObject(countryCodeDictionary);
            StreamWriter writer = new StreamWriter(path);
            writer.Write(newCountry);
            writer.Close();
            return countryName;
        }

        public static DateTime ConvertToUnixTimestamp(long milliseconds)
        {
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            return posixTime.AddMilliseconds(milliseconds);
        }

        /// <summary>
        /// Converts date time to unix time(in milliseconds).
        /// </summary>
        /// <param name="dateTime">A datetime specifying the dateTime.</param>
        /// <returns>date in unix timestamp.</returns>
        public static long ConvertToUnixTimestamp(DateTime dateTime)
        {
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.Subtract(UnixEpoch)).TotalMilliseconds;
        }

        public static DateTime ConvertLocalToUtc(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                throw new InvalidOperationException("DateTime is already in Utc time zone");
            }

            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            }
            
            return TimeZoneInfo.ConvertTimeToUtc(dateTime);
        }

        public static int GetCountry(string phoneNumber)
        {
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            try
            {
                PhoneNumber phoneNumberInfo = phoneNumberUtil.Parse(Utility.FormatAsIntlPhoneNumber(phoneNumber),
                    null);

                return phoneNumberInfo.CountryCode;
            }
            catch (Exception exp)
            {
                return 0;
            }
        }

    }
}