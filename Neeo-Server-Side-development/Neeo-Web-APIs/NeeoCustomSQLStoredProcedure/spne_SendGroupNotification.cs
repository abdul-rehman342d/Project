using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.SqlServer.Server;

public partial class StoredProcedures
{
    /// <summary>
    /// Sends push notifications with message and badge count to the user device specified with device token by calling a notification service.
    /// </summary>
    /// <param name="roomID">An string containing the room id.</param>
    /// <param name="senderID">An string containing the sender id.</param>
    /// <param name="msg">A string containing message content.</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void spne_SendGroupNotification(SqlInt32 roomID, SqlString senderID, SqlString msgBody)
    {

        HttpWebRequest request = null;
        HttpWebResponse response = null;
        const int notificationType = 4;

        string nType = "\"nType\": " + notificationType + ",";
        string rID = "\"rID\": " + roomID + ",";
        string sID = "\"senderID\": \"" + senderID.ToString() + "\",";
        string alert = "\"alert\": \"" + msgBody.ToString() + "\"";

        string body = "{";
        body += nType;
        body += rID;
        body += sID;
        body += alert;
        body += "}";

        byte[] postBytes = new UTF8Encoding().GetBytes(body.ToString());
        try
        {
            //string url = ConfigurationManager.AppSettings[pushNotificationURL].ToString();
            string url = "http://localhost:9004/api/notification/send";
            request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";
            Stream postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Flush();
            postStream.Dispose();
            response = (HttpWebResponse) request.GetResponse();
            response.Close();
        }
        catch (System.Configuration.ConfigurationErrorsException ex)
        {
            SqlContext.Pipe.Send(ex.Message);
        }
        catch (System.Net.WebException ex)
        {
        }
        catch (System.ObjectDisposedException ex)
        {
        }
        catch (System.NotSupportedException ex)
        {
        }
        catch (System.Net.ProtocolViolationException ex)
        {
        }
        catch (System.Security.SecurityException ex)
        {
        }
        catch (System.UriFormatException ex)
        {

        }
    }
}
