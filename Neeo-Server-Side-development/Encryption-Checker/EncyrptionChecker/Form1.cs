using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Security.Cryptography;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EncyrptionChecker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string hashingData = txtPhoneNumber.Text + txtVenderID.Text + txtAppID.Text;
            StringBuilder sBuilder = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashedData = md5.ComputeHash(Encoding.UTF8.GetBytes(hashingData));

                for (int i = 0; i < hashedData.Length; i++)
                {
                    sBuilder.Append(hashedData[i].ToString("x2"));
                }
                txtEncryptedResult.Text = sBuilder.ToString();
            }
        }


    }
}
