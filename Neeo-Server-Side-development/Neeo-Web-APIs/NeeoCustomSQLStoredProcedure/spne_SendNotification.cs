//------------------------------------------------------------------------------
// <copyright file="CSSqlStoredProcedure.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Threading;
using Microsoft.SqlServer.Server;
using System.Configuration;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Extention of the SQL StoredProcedures class.
/// </summary>
public partial class StoredProcedures
{
    /// <summary>
    /// Sends push notifications with message and badge count to the user device specified with device token by calling a notification service.
    /// </summary>
    /// <param name="reqBody">An string containing the sender id.</param>

    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void spne_SendNotification(SqlString reqBody)
    {

        HttpWebRequest request = null;
        HttpWebResponse response = null;

        byte[] postBytes = new UTF8Encoding().GetBytes(reqBody.ToString());
        try
        {
            //string url = ConfigurationManager.AppSettings[pushNotificationURL].ToString();
            string url = "http://localhost:9004/api/notification/send";
            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";
            Stream postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Flush();
            postStream.Dispose();
            response = (HttpWebResponse)request.GetResponse();
            response.Close();
            //var taskGetResponse = Task.Factory.StartNew(() =>
            //{
            //    response = (HttpWebResponse) request.GetResponse();
            //    response.Close();
            //});
            //Thread.Sleep(1000);
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
