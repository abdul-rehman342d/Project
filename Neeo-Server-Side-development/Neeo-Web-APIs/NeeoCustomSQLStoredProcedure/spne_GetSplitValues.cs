//------------------------------------------------------------------------------
// <copyright file="CSSqlStoredProcedure.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
/// <summary>
/// Extention of the SQL StoredProcedures class.
/// </summary>
public partial class StoredProcedures
{
    /// <summary>
    /// Splites the input value based on delimeter i.e. comma ','.It returns a table set based on the values given in the input with delimeter.
    /// </summary>
    /// <param name="value">A delimeter (i.e. comma ',') seperated string.</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void spne_GetSplitValues(SqlString value)
    {
        char[] delimeter = { ',' };
        SqlDataRecord record = new SqlDataRecord(new SqlMetaData("contact", SqlDbType.VarChar, 64));
        string[] valueArray = value.ToString().Split(delimeter);
        SqlContext.Pipe.SendResultsStart(record);
        for (int i = 0; i < valueArray.Length; i++)
        {
            record.SetString(0, valueArray[i]);
            SqlContext.Pipe.SendResultsRow(record);
        }
        SqlContext.Pipe.SendResultsEnd();
    }
}
