using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace ServiceWebAplicacion
{
    public class Conexion
    {
        SqlConnection con;

        public Conexion()
        {
            string connStr = ConfigurationManager.ConnectionStrings["dsn"].ConnectionString;
            if (con == null)
                con = new SqlConnection(connStr);
            //con = new SqlConnection("Server=NombreServidor;DataBase=ejemplo;User Id=ejem;password=ejem");

        }

        public void Abrir()
        {
            if (con.State == ConnectionState.Closed) con.Open();
        }

        public void Cerrar()
        {
            if (con.State == ConnectionState.Open) con.Close();
        }

        // METODOS
        public String Session(String user, String password)
        {
            String msg = "";
            SqlCommand cmd;
            try
            {
                Abrir();
                cmd = new SqlCommand("Session", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@user", user);
                cmd.Parameters.AddWithValue("@pass", password);
                cmd.Parameters.Add("@msg", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                msg = cmd.Parameters["@msg"].Value.ToString();
                Cerrar();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return msg;
        }

        public String InsertContent(string note, string image, int UserID, string torol)
        {
            String msg = "";
            SqlCommand cmd;
            try
            {
                Abrir();
                cmd = new SqlCommand("InsertContent", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Desc", note);
                cmd.Parameters.AddWithValue("@Image", image);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@torol", torol);
                //cmd.Parameters.AddWithValue("@torol", torol);
                cmd.Parameters.Add("@msg", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                msg = cmd.Parameters["@msg"].Value.ToString();
                Cerrar();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return msg;
        }
        public String InsertVote(string desc,int UserID)
        {
            String msg = "";
            SqlCommand cmd;
            try
            {
                Abrir();
                cmd = new SqlCommand("InsertVote", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Desc", desc);
                cmd.Parameters.AddWithValue("@userid", UserID);
                cmd.Parameters.Add("@msg", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                msg = cmd.Parameters["@msg"].Value.ToString();
                Cerrar();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return msg;
        }
    }
}