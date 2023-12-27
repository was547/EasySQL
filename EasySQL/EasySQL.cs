using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;

namespace EasySQL
{
    public class SQL : ISQL
    {
        private MySqlConnection _conn;
        private MySqlCommand _cmd;

        //Connection String required vars
        private string _username;
        private string _password;
        private string _database;
        private string _server;
        private string _sslmode;

        private string connectionString = string.Empty;
        private bool readToConnect = false;

        /// <summary>
        /// Constructor with SSL requirement
        /// </summary>
        /// <param name="pUser">MySQL Username</param>
        /// <param name="pPass">MySQL Password</param>
        /// <param name="pDB">MySQL Database</param>
        /// <param name="pServer">MySQL Host</param>
        /// <param name="pSSL">MySQL SSL Connection Type</param>
        public SQL(string pUser = "root", string pPass = "", string pDB = "my_db", string pServer = "localhost", string pSSL = "none")
        {
            this.readToConnect = false;

            _username = pUser;
            _password = pPass;
            _database = pDB;
            _server = pServer;
            _sslmode = pSSL;

            FormulateConnectionString();
        }

        /// <summary>
        /// Constructor without SSL requirement
        /// </summary>
        /// <param name="pUser">MySQL Username</param>
        /// <param name="pPass">MySQL Password</param>
        /// <param name="pDB">MySQL Database</param>
        /// <param name="pServer">MySQL Host</param>
        public SQL(string pUser = "root", string pPass = "", string pDB = "my_db", string pServer = "localhost")
        {
            this.readToConnect = false;

            _username = pUser;
            _password = pPass;
            _database = pDB;
            _server = pServer;
            _sslmode = "none";

            FormulateConnectionString();
        }

        /// <summary>
        /// Formulate string to create connection from constructor
        /// </summary>
        private void FormulateConnectionString()
        {
            string cs = "";

            cs += string.Format("server={0};", _server);
            cs += string.Format("userid={0};", _username);
            cs += string.Format("password={0};", _password);
            cs += string.Format("database={0};", _database);
            cs += string.Format("SslMode={0};", _sslmode);

            connectionString = cs;

            readToConnect = true;

            return;
        }


        /// <summary>
        /// Create new connection
        /// </summary>
        /// <returns>bool with exec result</returns>
        public bool CreateConnection()
        {
            if (!readToConnect)
            {
                return false;
            }

            if (_conn != null && _conn.State == ConnectionState.Open)
            {
                return true;
            }

            try
            {
                _conn = new MySqlConnection(connectionString);

                _conn.Open();

                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error to open connection: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Check what is the current state of the connection
        /// </summary>
        /// <returns>ConnectionState param</returns>
        public ConnectionState getConnectionState()
        {
            return _conn.State;
        }


        /// <summary>
        /// Insert data into your DB
        /// </summary>
        /// <param name="into">Table that will receive new data</param>
        /// <param name="keys">Row keys names</param>
        /// <param name="values">Row keys values</param>
        /// <returns>bool with exec result</returns>
        public bool Insert(string tableName, List<string> keys, List<object> values)
        {
            if (getConnectionState() != ConnectionState.Open || keys.Count != values.Count || string.IsNullOrEmpty(tableName))
            {
                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO {0} (", tableName);

            for (int i = 0; i < keys.Count; i++)
            {
                sb.AppendFormat("{0}", keys[i]);

                if (i < keys.Count - 1)
                {
                    sb.Append(",");
                }
                else
                {
                    sb.Append(") VALUES (");
                }
            }

            for (int i = 0; i < values.Count; i++)
            {
                sb.Append("@param").Append(i);

                if (i < values.Count - 1)
                {
                    sb.Append(",");
                }
                else
                {
                    sb.Append(")");
                }
            }

            try
            {
                _cmd = new MySqlCommand(sb.ToString(), _conn);

                for (int i = 0; i < values.Count; i++)
                {
                    _cmd.Parameters.AddWithValue("@param" + i, values[i]);
                }

                _cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                _cmd.Dispose();
            }

            return true;
        }



        /// <summary>
        /// Update content in DB
        /// </summary>
        /// <param name="into">Table that will receive the updates</param>
        /// <param name="keys">Rows names of the table</param>
        /// <param name="values">Rows values of the table</param>
        /// <param name="where">Condition for update</param>
        /// <returns>bool with exec result</returns>
        public bool Update(string tableName, List<string> keys, List<object> values, string where)
        {
            if (getConnectionState() != ConnectionState.Open || keys.Count != values.Count || string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(where))
            {
                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0} SET ", tableName);

            for (int i = 0; i < keys.Count; i++)
            {
                sb.AppendFormat("{0} = @param{1}", keys[i], i);

                if (i < keys.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.AppendFormat(" WHERE {0}", where);

            try
            {
                _cmd = new MySqlCommand(sb.ToString(), _conn);

                for (int i = 0; i < values.Count; i++)
                {
                    _cmd.Parameters.AddWithValue("@param" + i, values[i]);
                }

                _cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                _cmd.Dispose();
            }

            return true;
        }


        /// <summary>
        /// Delete content from DB
        /// </summary>
        /// <param name="from">Table that will delete content</param>
        /// <param name="where">Condition to delete the contant</param>
        /// <returns>bool with exec result</returns>
        public bool Delete(string tableName, string where)
        {
            if (getConnectionState() != ConnectionState.Open || string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(where))
            {
                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DELETE FROM {0} WHERE {1}", tableName, where);

            try
            {
                _cmd = new MySqlCommand(sb.ToString(), _conn);

                _cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                _cmd.Dispose();
            }

            return true;
        }


        /// <summary>
        /// Get content from your DB using SELECT parameter
        /// </summary>
        /// <param name="rows">Which rows you desire in your list (Default: *)</param>
        /// <param name="from">What table you desire that select make the search?</param>
        /// <param name="where">Define the condition of use.</param>
        /// <returns>List with all data from query</returns>
        public List<object[]> Get(string columns, string tableName, string where)
        {
            List<object[]> result = new List<object[]>();

            if (getConnectionState() != ConnectionState.Open || string.IsNullOrEmpty(columns) || string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(where))
            {
                return result;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT {0} FROM {1} WHERE {2}", columns, tableName, where);

            try
            {
                _cmd = new MySqlCommand(sb.ToString(), _conn);

                using (MySqlDataReader reader = _cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object[] tempRow = new object[reader.FieldCount];
                        reader.GetValues(tempRow);
                        result.Add(tempRow);
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
            finally
            {
                _cmd.Dispose();
            }
        }


        /// <summary>
        /// Dispose connection, use at end of your connection request!
        /// </summary>
        public void Dispose()
        {
            _conn.Dispose();
        }

        public int GetLastInsertIndex()
        {
            if (getConnectionState() != ConnectionState.Open)
            {
                return -1;
            }

            try
            {
                _cmd = new MySqlCommand("SELECT LAST_INSERT_ID()", _conn);
                int lastInsertId = Convert.ToInt32(_cmd.ExecuteScalar());

                return lastInsertId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error to get lastInsertedID: {ex.Message}");
                return -1;
            }
            finally
            {
                _cmd.Dispose();
            }
        }

        public bool UpdateOrInsert(string tableName, List<string> keys, List<object> values, string where)
        {
            if (getConnectionState() != ConnectionState.Open || keys.Count != values.Count || string.IsNullOrEmpty(tableName))
            {
                return false;
            }

            bool recordExists = RecordExists(tableName, where);

            if (recordExists)
            {
                return Update(tableName, keys, values, where);
            }
            else
            {
                return Insert(tableName, keys, values);
            }
        }

        public bool RecordExists(string tableName, string where)
        {
            try
            {
                string selectQuery = $"SELECT COUNT(*) FROM {tableName} WHERE {where}";
                _cmd = new MySqlCommand(selectQuery, _conn);
                int count = Convert.ToInt32(_cmd.ExecuteScalar());

                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in query RecordExists: {ex.Message}");
                return false;
            }
            finally
            {
                _cmd.Dispose();
            }
        }

        public bool CreateTable(string tableName, List<ColumnDefinition> columnDefinitions)
        {
            if (getConnectionState() != ConnectionState.Open || string.IsNullOrEmpty(tableName) || columnDefinitions == null || columnDefinitions.Count == 0)
            {
                return false;
            }

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("CREATE TABLE IF NOT EXISTS {0} (", tableName);

                for (int i = 0; i < columnDefinitions.Count; i++)
                {
                    sb.Append(columnDefinitions[i]);

                    if (i < columnDefinitions.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }

                sb.Append(")");

                _cmd = new MySqlCommand(sb.ToString(), _conn);
                _cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error to create table: {ex.Message}");
                return false;
            }
            finally
            {
                _cmd.Dispose();
            }
        }
    }
}

/*
 * 
 * Example creating a table:
 * 
        List<ColumnDefinition> columnDefinitions = new List<ColumnDefinition>
        {
            new ColumnDefinition("ID", "INT", isPrimaryKey: true, isAutoIncrement: true),
            new ColumnDefinition("FirstName", "VARCHAR(255)", isNotNull: true),
            new ColumnDefinition("LastName", "VARCHAR(255)", isNotNull: true),
            new ColumnDefinition("Age", "INT", defaultValue: "0")
        };

        bool success = CreateTable("People", columnDefinitions);
 *
 *
 */
