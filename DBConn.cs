using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Volume_Break_Webpage.CLsFolder
{
    public class DBConn
    {
        MySqlConnection mySQLConn = null;
        MySqlCommand cmd = null;
        MySqlDataAdapter adp = null;
        DataSet ds = new DataSet();

        public int ExecuteQueryMySql(string Query)
        {
            int i = 0;
            mySQLConn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringMySql"].ConnectionString);
            try
            {

                if (mySQLConn.State != ConnectionState.Open)
                {
                    mySQLConn.Open();
                }
                cmd = new MySqlCommand(Query, mySQLConn);
                cmd.CommandText = Query;
                i = cmd.ExecuteNonQuery();
                cmd.Dispose();
                return i;
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (mySQLConn.State == ConnectionState.Open)
                {

                    mySQLConn.Close();
                }
            }
        }

        public DataSet ReturnDataSet(string Query)
        {
            try
            {

                mySQLConn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringMySql"].ConnectionString);
                mySQLConn.Open();
                cmd = new MySqlCommand(Query, mySQLConn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                cmd.Dispose();
                adp.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                string xyz = ex.Message;
                return null;
                //throw (ex.Message);
            }
            finally
            {
                mySQLConn.Close();
            }
        }


        public DataSet ReturnDataSet(string Query, string strConnection)
        {
            try
            {

                mySQLConn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[strConnection].ConnectionString);
                mySQLConn.Open();
                cmd = new MySqlCommand(Query, mySQLConn);
                adp = new MySqlDataAdapter(cmd);
                adp.Fill(ds);
                cmd.Dispose();
                adp.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                string xyz = ex.Message;
                return null;
                //throw (ex.Message);
            }
            finally
            {
                mySQLConn.Close();
            }
        }
    }
}