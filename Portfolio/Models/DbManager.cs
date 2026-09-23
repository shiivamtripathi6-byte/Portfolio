using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Portfolio.Models
{
    public class DbManager
    {
        private readonly string _connectionString;

        public DbManager()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
            /// <summary>
            /// Executes INSERT, UPDATE, or DELETE statements.
            /// </summary>
            public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        con.Open();
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            /// <summary>
            /// Executes queries that return a single value (e.g., COUNT, MAX, or fetching an ID).
            /// </summary>
            public object ExecuteScalar(string query, SqlParameter[] parameters = null)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        con.Open();
                        return cmd.ExecuteScalar();
                    }
                }
            }
            /// <summary>
            /// Executes SELECT queries and returns the results in a DataTable.
            /// </summary>
            public DataTable GetDataTable(string query, SqlParameter[] parameters = null)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            /// <summary>
            /// Gets the current counter ID from the tabcounter table for a specific table name.
            /// </summary>
            public int TabCounter(string tableName)
            {
                string sql = "SELECT counterid FROM tabcounter WHERE tablename = @TableName";

                SqlParameter[] parameters = {
                new SqlParameter("@TableName", tableName)
            };

                object result = ExecuteScalar(sql, parameters);

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }

                return 0;
            }
            /// <summary>
            /// Updates the counter ID in the tabcounter table for a specific table name.
            /// </summary>
            public void UpdateTabcounter(int count, string tableName)
            {
                string sql = "UPDATE tabcounter SET counterid = @Count WHERE tablename = @TableName";

                SqlParameter[] parameters = 
                {
                   new SqlParameter("@Count", count),
                   new SqlParameter("@TableName", tableName)
                };
                ExecuteNonQuery(sql, parameters);
            }
        }
    }
