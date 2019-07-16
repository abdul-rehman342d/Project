using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace NeeoServiceOperator
{
    public partial class Login : System.Web.UI.Page
    {
        private string _password;
        private const string Password = "password";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_password))
            {
                _password = ConfigurationManager.AppSettings[Password].ToString();
            }
            if (Convert.ToBoolean(Session["isAuth"]))
            {
                Response.Redirect("~/Default.aspx");
            }

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Text.Trim()) && !string.IsNullOrEmpty(txtUsername.Text.Trim()))
            {
                if (GenerateHashCode(txtPassword.Text.Trim()) == _password)
                {
                    Session["isAuth"] = true;
                    Response.Redirect("~/Default.aspx");
                }
                else
                {
                    lblErroMessage.Text = "Incorrect username or password!";
                    lblErroMessage.Visible = true;
                }
            }
            else
            {
                lblErroMessage.Text = "Incorrect username or password!";
                lblErroMessage.Visible = true;
            }
        }

        private string GenerateHashCode(string inputString)
        {
            const string key = "+-)N3EoP@L~@$";
            string data = inputString + key + key;
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashedData = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
                StringBuilder hashedString = new StringBuilder();
                for (int i = 0; i < hashedData.Length; i++)
                {
                    hashedString.Append(hashedData[i].ToString("x2"));
                }
                return hashedString.ToString();
            }
        }
    }
}