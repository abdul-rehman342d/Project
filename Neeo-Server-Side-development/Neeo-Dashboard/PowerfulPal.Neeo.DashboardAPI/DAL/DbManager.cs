using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PowerfulPal.Neeo.DashboardAPI.DAL
{
    public class DbManager
    {
        #region Data Members

        /// <summary>
        ///     An object connection to the database.
        /// </summary>
        private readonly SqlConnection _con;

        /// <summary>
        ///     A string containing the connection string of the database
        /// </summary>
        private string _conStr;


        /// <summary>
        ///     An object holds the transaction object.
        /// </summary>
        private SqlTransaction _transaction;

        #endregion

        #region Stored Procedures

        //private const string ProcUserLogin = "spne_UserLogin";
        //private const string ProcUpdateLastActivity = "spne_UpdateLastActivity";
        //private const string ProcGetUserName = "spne_GetUserName";
        //private const string ProcGetAllRegisteredUser = "spne_GetAllRegisteredUser";
        //private const string ProcGetAllAndroidRegisteredUser = "spne_GetAllAndroidRegisteredUser";
        //private const string ProcGetAllIosRegisteredUser = "spne_GetAllIosRegisteredUser";
        //private const string ProcUserSignOut = "spne_UserSignOut";

        private const string ProcBlockUser = "spne_BlockUser";
        private const string ProcDeleteHistory = "spne_ClearLoginSummary";
        private const string ProcDeleteUser = "spne_DeleteUser";
        private const string ProcGetAllAndroidRegisteredUser = "spne_GetAllAndroidRegisteredUser";
        private const string ProcGetAllIosRegisteredUser = "spne_GetAllIosRegisteredUser";
        private const string ProcGetAllRegisteredUser = "spne_GetAllRegisteredUser";
        private const string ProcGetAllUser = "spne_GetAllUser";
        private const string ProcGetLastLoginTime = "spne_GetLastLoginTime ";
        private const string ProcGetLastSyncTime = "spne_GetLastSyncTime";
        private const string ProcGetLoginHistory = "spne_GetLoginHistory";
        private const string ProcGetUserName = "spne_GetUserName";
        private const string ProcRegisterUser = "spne_RegisterUser";
        private const string ProcUnBlockUser = "spne_UnBlockUser";
        private const string ProcUpdateLastActivity = "spne_UpdateLastActivity";
        private const string ProcUpdatePassword = "spne_UpdatePassword";
        private const string ProcUpdateUser = "spne_UpdateUser";
        private const string ProcUpdateUserStatus = "spne_UpdateUserStatus";
        private const string ProcUserLogin = "spne_UserLogin";
        private const string ProcUserSignOut = "spne_UserSignOut";

        private const string Proc_UpdateUserSessionByAuthKey = "spne_UpdateUserSessionByAuthKey";
        private const string Proc_GetUserInfoByAuthKey = "spne_GetUserInfoByAuthKey";

        #endregion

        #region Constructors

        public DbManager()
        {
            _conStr = ConfigurationManager.ConnectionStrings["NeeoDashboardConnectionString"].ConnectionString;
            _con = new SqlConnection(_conStr);
        }

        #endregion

        #region Member Functions

        #region Transaction methods

        /// <summary>
        ///     Starts transaction.
        /// </summary>
        /// <returns></returns>
        public bool StartTransaction()
        {
            if (_con.State != ConnectionState.Open)
            {
                _con.Open();
                _transaction = _con.BeginTransaction();
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Commits all the changes to database.
        /// </summary>
        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
                if (_con.State != ConnectionState.Closed)
                {
                    _con.Close();
                }
            }
        }

        /// <summary>
        ///     Rollback all the changes that are made during the transaction.
        /// </summary>
        public void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
                if (_con.State != ConnectionState.Closed)
                {
                    _con.Close();
                }
            }
        }

        #endregion

        #region User related



        public string BlockUser(string userName, string authKey)
        {
            string str;
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = this._con;
                    command.Transaction = this._transaction;
                    command.CommandText = "spne_BlockUser";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@userName", SqlDbType.NVarChar, 30).Value = userName;
                    command.Parameters.Add("@authKey", SqlDbType.NVarChar, 100).Value = authKey;
                    command.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    this._con.Open();
                    command.ExecuteNonQuery();
                    this._con.Close();
                    str = command.Parameters["@retval"].Value.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return str;
        }

        public void ClearLoginData(string userName)
        {

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = this._con;
                command.Transaction = this._transaction;
                command.CommandText = "spne_ClearLoginSummary";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@userName", SqlDbType.NVarChar, 30).Value = userName;
                this._con.Open();
                command.ExecuteNonQuery();
                this._con.Close();
            }

        }
        public string DeleteUser(string userName)
        {
            string str;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = this._con;
                command.Transaction = this._transaction;
                command.CommandText = "spne_DeleteUser";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@userName", SqlDbType.NVarChar, 30).Value = userName;
                //command.Parameters.Add("@authKey", SqlDbType.NVarChar, 100).Value = authKey;
                command.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                this._con.Open();
                command.ExecuteNonQuery();
                this._con.Close();
                str = command.Parameters["@retval"].Value.ToString();
            }
            return str;
        }

        public DataTable GetAllRegisteredUsers()
        {
            SqlCommand selectCommand = new SqlCommand
            {
                Connection = this._con,
                CommandText = "spne_GetAllRegisteredUser",
                CommandType = CommandType.StoredProcedure
            };
            DataTable dataTable = new DataTable();
            new SqlDataAdapter(selectCommand).Fill(dataTable);
            return dataTable;
        }

        public DataTable GetAllUser()
        {
            DataTable table2;
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = this._con;
                    command.Transaction = this._transaction;
                    command.CommandText = "spne_GetAllUser";
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    table2 = dataTable;
                }
            }
            catch (Exception)
            {
                table2 = null;
            }
            return table2;
        }

        public DataTable GetAndroidRegisteredUser()
        {
            SqlCommand selectCommand = new SqlCommand
            {
                Connection = this._con,
                CommandText = "spne_GetAllAndroidRegisteredUser",
                CommandType = CommandType.StoredProcedure
            };
            DataTable dataTable = new DataTable();
            new SqlDataAdapter(selectCommand).Fill(dataTable);
            return dataTable;
        }

        public DataTable GetIosRegisteredUser()
        {
            SqlCommand selectCommand = new SqlCommand
            {
                Connection = this._con,
                CommandText = "spne_GetAllIosRegisteredUser",
                CommandType = CommandType.StoredProcedure
            };
            DataTable dataTable = new DataTable();
            new SqlDataAdapter(selectCommand).Fill(dataTable);
            return dataTable;
        }

        public string GetLastLoginTime(string username)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = this._con;
                command.Transaction = this._transaction;
                command.CommandText = "spne_GetLastLoginTime ";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@username", SqlDbType.NVarChar, 30).Value = username;
                command.Parameters.Add("@loginTime", SqlDbType.NVarChar, 30).Direction = ParameterDirection.Output;
                this._con.Open();
                command.ExecuteNonQuery();
                this._con.Close();
                return command.Parameters["@loginTime"].Value.ToString();
            }
        }

        public string GetLastSyncTime()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = this._con;
                command.Transaction = this._transaction;
                command.CommandText = "spne_GetLastSyncTime";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@lastSyncTime", SqlDbType.NVarChar, 30).Direction = ParameterDirection.Output;
                this._con.Open();
                command.ExecuteNonQuery();
                this._con.Close();
                return command.Parameters["@lastSyncTime"].Value.ToString();
            }
        }

        public DataTable GetLoginHistory(string userName)
        {
            DataTable table2;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = this._con;
                command.Transaction = this._transaction;
                command.CommandText = "spne_GetLoginHistory";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@userName", SqlDbType.NVarChar, 30).Value = userName;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                table2 = dataTable;
            }

            return table2;
        }

        public string GetUserName(string key)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = this._con;
                command.Transaction = this._transaction;
                command.CommandText = "spne_GetUserName";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@AuthKey", SqlDbType.NVarChar, 30).Value = key;
                command.Parameters.Add("@username", SqlDbType.NVarChar, 30).Direction = ParameterDirection.Output;
                this._con.Open();
                command.ExecuteNonQuery();
                this._con.Close();
                return command.Parameters["@username"].Value.ToString();
            }
        }

        public int LogOut(string username, string authKey)
        {
            int num;
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = this._con;
                    command.Transaction = this._transaction;
                    command.CommandText = Proc_UpdateUserSessionByAuthKey;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@username", SqlDbType.NVarChar, 30).Value = username;
                    command.Parameters.Add("@authKey", SqlDbType.NVarChar, 100).Value = authKey;
                    //command.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    this._con.Open();
                    command.ExecuteNonQuery();
                    this._con.Close();
                    num = int.Parse(command.Parameters["@Return"].Value.ToString());
                }
            }
            catch (Exception)
            {
                num = 1;
            }
            return num = 1;
        }

        public string RegisterUser(string userName, string password, string authKey = "")
        {
            string str;
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = this._con;
                    command.Transaction = this._transaction;
                    command.CommandText = "spne_RegisterUser";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@userName", SqlDbType.NVarChar, 30).Value = userName;
                    command.Parameters.Add("@password", SqlDbType.NVarChar, 200).Value = password;
                    command.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    this._con.Open();
                    command.ExecuteNonQuery();
                    this._con.Close();
                    str = command.Parameters["@retval"].Value.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return str;
        }

        public DataTable GetActiveUsersCount(string upperTimeLimit, string lowerTimeLimit)
        {

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = this._con;
                command.CommandText = "spne_GetActiveUsersByTimeSpan";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@upperTimeLimit", SqlDbType.Char, 15).Value = upperTimeLimit;
                command.Parameters.Add("@lowerTimeLimit", SqlDbType.Char, 15).Value = lowerTimeLimit;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }


        }

        public DataTable testproc()
        {
            DataTable table2;
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = this._con;
                    command.Transaction = this._transaction;
                    command.CommandText = "testproc";
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    table2 = dataTable;
                }
            }
            catch (Exception)
            {
                table2 = null;
            }
            return table2;
        }

        public string UnBlockUser(string userName, string authKey)
        {
            string str;
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = this._con;
                    command.Transaction = this._transaction;
                    command.CommandText = "spne_UnBlockUser";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@userName", SqlDbType.NVarChar, 30).Value = userName;
                    command.Parameters.Add("@authKey", SqlDbType.NVarChar, 100).Value = authKey;
                    command.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    this._con.Open();
                    command.ExecuteNonQuery();
                    this._con.Close();
                    str = command.Parameters["@retval"].Value.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return str;
        }

        public int UpdateLastActivity(string key)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = this._con;
                command.Transaction = this._transaction;
                command.CommandText = "spne_UpdateLastActivity";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@AuthKey", SqlDbType.NVarChar, 30).Value = key;
                command.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                this._con.Open();
                command.ExecuteNonQuery();
                this._con.Close();
                return int.Parse(command.Parameters["@Return"].Value.ToString());
            }
        }

        public string UpdatePassword(string userName, string password, string authKey = "")
        {
            string str;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = this._con;
                command.Transaction = this._transaction;
                command.CommandText = "spne_UpdatePassword";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@userName", SqlDbType.NVarChar, 30).Value = userName;
                command.Parameters.Add("@password", SqlDbType.NVarChar, 200).Value = password;
                command.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                this._con.Open();
                command.ExecuteNonQuery();
                this._con.Close();
                str = command.Parameters["@retval"].Value.ToString();
            }


            return str;
        }

        public string UpdateStatus(string userName, bool isActive, string authKey)
        {
            string str;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = this._con;
                command.Transaction = this._transaction;
                command.CommandText = "spne_UpdateUserStatus";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@username", SqlDbType.NVarChar, 30).Value = userName;
                command.Parameters.Add("@status", SqlDbType.Bit).Value = isActive;
                //command.Parameters.Add("@authKey", SqlDbType.NVarChar, 100).Value = authKey;
                //command.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                this._con.Open();
                command.ExecuteNonQuery();
                this._con.Close();
                //str = command.Parameters["@retval"].Value.ToString();
            }

            return "";
        }

        public void UpdateUser(string userName, int isActive)
        {
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = this._con;
                    command.Transaction = this._transaction;
                    command.CommandText = "spne_UpdateUser";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@userName", SqlDbType.NVarChar, 30).Value = userName;
                    command.Parameters.Add("@isActive", SqlDbType.NVarChar, 200).Value = isActive;
                    this._con.Open();
                    command.ExecuteNonQuery();
                    this._con.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string UserLogin(string userName, string password)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = this._con;
                command.Transaction = this._transaction;
                command.CommandText = "spne_UserLogin";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@username", SqlDbType.NVarChar, 30).Value = userName;
                command.Parameters.Add("@password", SqlDbType.NVarChar, 200).Value = password;
                command.Parameters.Add("@authKey", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
                this._con.Open();
                command.ExecuteNonQuery();
                this._con.Close();
                return command.Parameters["@authKey"].Value.ToString();
            }
        }

        public DataTable GetUserInfo(string authKey)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = _con;
                command.CommandText = Proc_GetUserInfoByAuthKey;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@authKey", SqlDbType.NVarChar, 100).Value = authKey;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        #endregion

        #endregion


        ///// <summary>
        /////     Starts transaction.
        ///// </summary>
        ///// <returns></returns>
        //public string UserLogin(string userName, string parameters)
        //{
        //    using (var command = new SqlCommand())
        //    {
        //        command.Connection = _con;
        //        command.Transaction = _transaction;
        //        command.CommandText = ProcUserLogin;
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.Add("@username", SqlDbType.NVarChar, 30).Value = userName;
        //        command.Parameters.Add("@password", SqlDbType.NVarChar, 200).Value = parameters;
        //        command.Parameters.Add("@authKey", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
        //        _con.Open();
        //        command.ExecuteNonQuery();
        //        _con.Close();
        //        return (command.Parameters["@authKey"].Value).ToString();
        //    }
        //}

        //public int UpdateLastActivity(string key)
        //{
        //    using (var command = new SqlCommand())
        //    {
        //        command.Connection = _con;
        //        command.Transaction = _transaction;
        //        command.CommandText = ProcUpdateLastActivity;
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.Add("@AuthKey", SqlDbType.NVarChar, 30).Value = key;
        //        command.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
        //        _con.Open();
        //        command.ExecuteNonQuery();
        //        _con.Close();
        //        return int.Parse(command.Parameters["@Return"].Value.ToString());
        //    }
        //}

        //public string GetUserName(string key)
        //{
        //    using (var command = new SqlCommand())
        //    {
        //        command.Connection = _con;
        //        command.Transaction = _transaction;
        //        command.CommandText = ProcGetUserName;
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.Add("@AuthKey", SqlDbType.NVarChar, 30).Value = key;
        //        command.Parameters.Add("@username", SqlDbType.NVarChar, 30).Direction = ParameterDirection.Output;
        //        _con.Open();
        //        command.ExecuteNonQuery();
        //        _con.Close();
        //        return command.Parameters["@username"].Value.ToString();
        //    }
        //}

        //public int LogOut(string key)
        //{
        //    using (var command = new SqlCommand())
        //    {
        //        command.Connection = _con;
        //        command.Transaction = _transaction;
        //        command.CommandText = ProcUserSignOut;
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.Add("@AuthKey", SqlDbType.NVarChar, 30).Value = key;
        //        command.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
        //        _con.Open();
        //        command.ExecuteNonQuery();
        //        _con.Close();
        //        return int.Parse(command.Parameters["@Return"].Value.ToString());
        //    }
        //}

        //public DataTable GetAllRegisteredUser()
        //{
        //    var command = new SqlCommand();
        //    command.Connection = _con;
        //    command.CommandText = ProcGetAllRegisteredUser;
        //    command.CommandType = CommandType.StoredProcedure;
        //    var table = new DataTable();
        //    var adapter = new SqlDataAdapter(command);
        //    adapter.Fill(table);
        //    return table;
        //}

        //public DataTable GetAndroidRegisteredUser()
        //{
        //    var command = new SqlCommand();
        //    command.Connection = _con;
        //    command.CommandText = ProcGetAllAndroidRegisteredUser;
        //    command.CommandType = CommandType.StoredProcedure;
        //    var table = new DataTable();
        //    var adapter = new SqlDataAdapter(command);
        //    adapter.Fill(table);
        //    return table;
        //}

        //public DataTable GetIosRegisteredUser()
        //{
        //    var command = new SqlCommand();
        //    command.Connection = _con;
        //    command.CommandText = ProcGetAllIosRegisteredUser;
        //    command.CommandType = CommandType.StoredProcedure;
        //    var table = new DataTable();
        //    var adapter = new SqlDataAdapter(command);
        //    adapter.Fill(table);
        //    return table;
        //}
    }
}