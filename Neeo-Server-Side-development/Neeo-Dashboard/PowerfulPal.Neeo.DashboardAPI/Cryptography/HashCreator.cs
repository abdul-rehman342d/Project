using System.Security.Cryptography;
using System.Text;

namespace PowerfulPal.Neeo.DashboardAPI.Cryptography
{
    public class HashCreator
    {
        public const string Key = "@%^N#3OP@L~!$";

        public static string Create(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashedData = md5.ComputeHash(Encoding.UTF8.GetBytes(value));

                for (int i = 0; i < hashedData.Length; i++)
                {
                    stringBuilder.Append(hashedData[i].ToString("x2"));
                }
            }
            return stringBuilder.ToString();
        }
    }
}