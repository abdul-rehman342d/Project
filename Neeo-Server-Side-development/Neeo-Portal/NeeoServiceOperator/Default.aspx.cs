using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RestSharp;
using RestSharp.Serializers;
using System.Data;
using System.Configuration;

namespace NeeoServiceOperator
{
    enum NumberState
    {
        NotExist = 0,
        Deleted = 1
    }
    public partial class Default : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Convert.ToBoolean(Session["isAuth"]))
            {
                Response.Redirect("~/Login.aspx");
            }
            if (!IsPostBack)
            {
                txtPassword.Text = "123";
                GridView1.DataSourceID = "SqlDataSource1";
                GridView1.DataBind();
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            const string userService = "userService";
            string body = "{\"phoneNumber\": \"" + txtPhoneNumber.Text + "\",\"client\": {\"AppClient\": \"neeo\",\"AppVersion\": \"v1.0\",\"DevicePlatform\": 0,\"DeviceVenderID\": \"" + txtPassword.Text + "\",\"ApplicationID\": \"" + txtPassword.Text + "\"}}";
            RestClient restClient = new RestClient();
            RestRequest restRequest = new RestRequest(ConfigurationManager.AppSettings[userService]);
            restRequest.Method = Method.POST;
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.AddParameter("application/json", body, ParameterType.RequestBody);
            var restResponse = restClient.Execute(restRequest);
            lblStatus.Text = "Response : " + restResponse.StatusCode.ToString() + "-" + restResponse.StatusDescription;
            if (restResponse.StatusCode == HttpStatusCode.OK)
            {
                lblStatus.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }

            txtPhoneNumber.Text = string.Empty;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearchPhoneNumber.Text.Trim()))
            {
                GridView1.DataSourceID = "SqlDataSource2";
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtPhoneNumber.Text = string.Empty;
            txtSearchPhoneNumber.Text = string.Empty;
            txtUnblockPhoneNumber.Text = string.Empty;
            lblStatus.Text = string.Empty;
            lblDeletionStatus.Text = string.Empty;
            lblDeletionStatus.ForeColor = System.Drawing.Color.Black;
            lblStatus.ForeColor = System.Drawing.Color.Black;
            txtPassword.Text = "123";
            GridView1.DataSourceID = "SqlDataSource1";
        }

        protected void SqlDataSource2_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.AffectedRows == 1)
            {
                txtPhoneNumber.Text = txtSearchPhoneNumber.Text;
                txtUnblockPhoneNumber.Text = txtSearchPhoneNumber.Text;
            }
            else
            {
                txtPhoneNumber.Text = "";
            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = GridView1.SelectedRow;
            txtPhoneNumber.Text = row.Cells[0].Text;
            txtUnblockPhoneNumber.Text = row.Cells[0].Text;
            
        }

        protected void lnkBtnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }

        protected void btnUnblock_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUnblockPhoneNumber.Text.Trim()))
            {
                const string xmppConnectionString = "XMPPDbConnectionString";
                const string deleteUserBlockedStateByPhoneNumber = "spne_DeleteUserBlockedStateByPhoneNumber";
                string conStr = ConfigurationManager.ConnectionStrings[xmppConnectionString].ConnectionString;
                SqlConnection con = new SqlConnection(conStr);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = deleteUserBlockedStateByPhoneNumber;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                cmd.Parameters.Add("@phoneNumber", SqlDbType.NVarChar, 64).Value = txtUnblockPhoneNumber.Text.Trim();
                int numberState = 0;
                try
                {
                    con.Open();
                    numberState = cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    numberState = -1;
                }
                finally
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }
                }

                if ((NumberState)numberState == NumberState.Deleted)
                {
                    lblDeletionStatus.ForeColor = System.Drawing.Color.Black;
                    lblDeletionStatus.Text = "Info - Phone number successfully  deleted!";
                }
                else if ((NumberState)numberState == NumberState.NotExist)
                {
                    lblDeletionStatus.ForeColor = System.Drawing.Color.Red;
                    lblDeletionStatus.Text = "Error - Phone number does not exist!";
                }
                else
                {
                    lblDeletionStatus.ForeColor = System.Drawing.Color.Red;
                    lblDeletionStatus.Text = "Error - Deletion operation failed!";
                }
            }
            else
            {
                lblDeletionStatus.ForeColor = System.Drawing.Color.Red;
                lblDeletionStatus.Text = "Error - Invalid phone number!";
            }
            txtUnblockPhoneNumber.Text = string.Empty;
        }

        public string VoipUserData()
        {
            return "";
        }
    }
}