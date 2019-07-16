using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;
using LibNeeo;
using LibNeeo.Voip;

namespace mcr_data_copier
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
           
            var _conStr = ConfigurationManager.ConnectionStrings["neeoConnectionString"].ConnectionString;
            var _con = new SqlConnection(_conStr);
            try
            {

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select username from neUserExtension where devicePlatform = 1";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = _con;

                System.Data.DataTable dtContactsInfo = new System.Data.DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dtContactsInfo);

                if (dtContactsInfo.Rows.Count > 0)
                {
                    Response.Write("Count == > "+dtContactsInfo.Rows.Count.ToString()+"<br/>");
                    for (int i = 0; i < dtContactsInfo.Rows.Count; i++)
                    {
                        try
                        {
                            var mcr = NeeoVoipApi.GetMcrCount(dtContactsInfo.Rows[i]["username"].ToString());

                            Response.Write(i +" ==> " + dtContactsInfo.Rows[i]["username"].ToString() + " ==> " + mcr + "<br/>");
                            var _conStrr = ConfigurationManager.ConnectionStrings["neeoConnectionString"].ConnectionString;
                            var _conn = new SqlConnection(_conStrr);
                            SqlCommand cmdd = new SqlCommand();
                            cmdd.CommandText = "UPDATE [dbo].[neOfflineUserMessageCount] SET [mcrCount] = " + mcr + "  WHERE username  = '" + dtContactsInfo.Rows[i]["username"].ToString() + "'";
                            cmdd.CommandType = CommandType.Text;
                            cmdd.Connection = _conn;

                            using (_conn)
                            {
                                _conn.Open();
                                cmdd.ExecuteNonQuery();
                                _conn.Close();
                                Response.Write(i + " ==> " + dtContactsInfo.Rows[i]["username"].ToString() + " ==>  updated" + "<br/>");
                            }

                           
                           
                        }
                        catch (SqlException sqlEx)
                        {
                            Response.Write(sqlEx.Message + "<br/>");
                        }
                        catch (ApplicationException applicationException)
                        {
                            Response.Write(applicationException.Message + "<br/>");
                        }
                        catch (Exception exception)
                        {
                            Response.Write(exception.Message + "<br/>");
                        }
                    }
                    

                }
            }
            catch (SqlException sqlEx)
            {
                Response.Write(sqlEx.Message + "<br/>");
            }
            catch (Exception Ex)
            {
                Response.Write(Ex.Message + "<br/>");
            }
            finally
            {
                if (_con.State != ConnectionState.Closed)
                {
                    _con.Close();
                }
            }
        }
    }
}