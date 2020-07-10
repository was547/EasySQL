using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;

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
            if (!this.readToConnect)
                return false;

            try
            {
                _conn = new MySqlConnection(this.connectionString);

                _conn.Open();

                return true;
            }
            catch
            {
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
        public bool Insert(string into, List<string> keys, List<object> values)
        {
            if (getConnectionState() != ConnectionState.Open) return false;

            if (keys.Count != values.Count) return false;

            if (string.IsNullOrEmpty(into)) return false;

            string sf = "";

            //Add INSERT HEADER
            sf += string.Format("INSERT INTO {0} (", into);
            int i = 0;

            //Add Keys into string
            foreach (var val in keys)
            {
                sf += string.Format("{0}", val);

                i++;

                if (i < keys.Count)
                {
                    sf += ",";
                }
                else
                {
                    sf += ") VALUES (";
                }
            }

            //Add values to string
            for (int x = 0; x < i; x++)
            {
                if (values[x].GetType() == typeof(string))
                    sf += string.Format("'{0}'", values[x].ToString());
                else
                    sf += string.Format("{0}", values[x].ToString());

                if ((x + 1) == i)
                {
                    sf += ")";
                }
                else
                {
                    sf += ",";
                }
            }

            try
            {
                _cmd = new MySqlCommand(sf, _conn);

                _cmd.ExecuteNonQuery();
            }
            catch
            {
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
        public bool Update(string into, List<string> keys, List<object> values, string where)
        {
            var test = new List<object> { "aaaa", 1234 };

            if (this.getConnectionState() != ConnectionState.Open) return false;
            if (keys.Count != values.Count) return false;
            if (string.IsNullOrEmpty(into)) return false;
            if (string.IsNullOrEmpty(where)) return false;

            string sf = "";

            // Add UPDATE HEADER
            sf += string.Format("UPDATE {0} SET ", into);
            int countValues = 0;

            // Add Keys and values into string
            foreach (var keyValue in keys)
            {
                if (values[countValues].GetType() == typeof(string))
                    sf += string.Format(" '{0}'='{1}'", keyValue, values[countValues].ToString());
                else
                    sf += string.Format(" '{0}'={1}", keyValue, values[countValues].ToString());

                if (countValues <= values.Count)
                {
                    sf += ", ";
                }

                countValues++;
            }

            sf += " WHERE " + where;

            try
            {
                _cmd = new MySqlCommand(sf, _conn);

                _cmd.ExecuteNonQuery();

                _cmd.Dispose();
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Delete content from DB
        /// </summary>
        /// <param name="from">Table that will delete content</param>
        /// <param name="where">Condition to delete the contant</param>
        /// <returns>bool with exec result</returns>
        public bool Delete(string from, string where)
        {
            if (this.getConnectionState() != ConnectionState.Open) return false;
            if (string.IsNullOrEmpty(from)) return false;
            if (string.IsNullOrEmpty(where)) return false;

            string sf = "";

            //Add DELETE into HEADER
            sf += string.Format("DELETE FROM {0} WHERE {1} ", from, where);

            // Execute SQL string
            try
            {
                _cmd = new MySqlCommand(sf, _conn);

                _cmd.ExecuteNonQuery();

                _cmd.Dispose();
            }
            catch
            {
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
        public List<object[]> Get(string rows, string from, string where)
        {
            List<object[]> retn = new List<object[]>();

            if (this.getConnectionState() != ConnectionState.Open) return retn;
            if (string.IsNullOrEmpty(rows)) rows = "*";
            if (string.IsNullOrEmpty(from)) return retn;
            if (string.IsNullOrEmpty(where)) return retn;


            string sf = string.Format("SELECT {0} FROM {1} WHERE {2}", rows, from, where);

            try
            {
                _cmd = new MySqlCommand(sf, _conn);

                using (MySqlDataReader reader = _cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object[] tempRow = new object[reader.FieldCount];

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            tempRow[i] = reader[i];
                        }

                        retn.Add(tempRow);
                    }

                    return retn;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return retn;
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
    }
}
