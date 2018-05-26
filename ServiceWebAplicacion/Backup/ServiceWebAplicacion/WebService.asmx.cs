using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Configuration;

namespace ServiceWebAplicacion
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        Conexion con = new Conexion();
        byte[] a;
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public String Login(string user, String password)
        {
            string msg = "";
            msg = con.Session(user, password);

            return msg;
        }
        [WebMethod]
        public String InsertContent(string Description, string Image, int UserID, string torol)
        {
            string msg = string.Empty;
            msg = con.InsertContent(Description, Image, UserID,torol);
            return msg;
        }
        [WebMethod]
        public String InsertVote(string Description, int UserID)
        {
            string msg = string.Empty;
            msg = con.InsertVote(Description, UserID);
            return msg;
        }

        [WebMethod]
        public DataSet GetData()
        {
            string connStr = ConfigurationManager.ConnectionStrings["dsn"].ConnectionString;
            // First Create a New Connection
            SqlConnection conn = new SqlConnection(connStr);
            // Now Pass a Connection String To the Connection
            // Now the Select statement you want to run
            string select = @"select Content.CID,
	                                Users.UserName, 
	                                Content.Description, 
	                                Content.Image,
	                                Convert(varchar(19),Content.Date) 
	                                from Content 
	                                left join Users on Users.ID = Content.ID where torol=N'Үндсэн мэдээ'
	                                ORDER BY Content.Date DESC";
            // Create an Adapter
            SqlDataAdapter da = new SqlDataAdapter(select, conn);
            // Create a New DataSet
            DataSet ds = new DataSet();
            // Fill The DataSet With the Contents of the Stock Table
            da.Fill(ds, "content");
            // Now Return ds which is a DataSet
            return (ds);
        }
        [WebMethod]
        public DataSet GetZar()
        {
            string connStr = ConfigurationManager.ConnectionStrings["dsn"].ConnectionString;
            // First Create a New Connection
            SqlConnection conn = new SqlConnection(connStr);
            // Now Pass a Connection String To the Connection
            // Now the Select statement you want to run
            string select = @"select Content.CID,
	                                Users.UserName, 
	                                Content.Description, 
	                                Content.Image,
	                                Convert(varchar(19),Content.Date) 
	                                from Content 
	                                left join Users on Users.ID = Content.ID where torol=N'Зар'
	                                ORDER BY Content.Date DESC";
            // Create an Adapter
            SqlDataAdapter da = new SqlDataAdapter(select, conn);
            // Create a New DataSet
            DataSet ds = new DataSet();
            // Fill The DataSet With the Contents of the Stock Table
            da.Fill(ds, "content");
            // Now Return ds which is a DataSet
            return (ds);
        }
        [WebMethod]
        public DataSet Getzmedee()
        {
            string connStr = ConfigurationManager.ConnectionStrings["dsn"].ConnectionString;
            // First Create a New Connection
            SqlConnection conn = new SqlConnection(connStr);
            // Now Pass a Connection String To the Connection
            // Now the Select statement you want to run
            string select = @"select Content.CID,
	                                Users.UserName, 
	                                Content.Description, 
	                                Content.Image,
	                                Convert(varchar(19),Content.Date) 
	                                from Content 
	                                left join Users on Users.ID = Content.ID where torol=N'Захирлын мэдээ'
	                                ORDER BY Content.Date DESC";
            // Create an Adapter
            SqlDataAdapter da = new SqlDataAdapter(select, conn);
            // Create a New DataSet
            DataSet ds = new DataSet();
            // Fill The DataSet With the Contents of the Stock Table
            da.Fill(ds, "content");
            // Now Return ds which is a DataSet
            return (ds);
        }
        [WebMethod]
        public DataSet GetGadaad()
        {
            string connStr = ConfigurationManager.ConnectionStrings["dsn"].ConnectionString;
            // First Create a New Connection
            SqlConnection conn = new SqlConnection(connStr);
            // Now Pass a Connection String To the Connection
            // Now the Select statement you want to run
            string select = @"select Content.CID,
	                                Users.UserName, 
	                                Content.Description, 
	                                Content.Image,
	                                Convert(varchar(19),Content.Date) 
	                                from Content 
	                                left join Users on Users.ID = Content.ID where torol=N'Гадаад мэдээ'
	                                ORDER BY Content.Date DESC";
            // Create an Adapter
            SqlDataAdapter da = new SqlDataAdapter(select, conn);
            // Create a New DataSet
            DataSet ds = new DataSet();
            // Fill The DataSet With the Contents of the Stock Table
            da.Fill(ds, "content");
            // Now Return ds which is a DataSet
            return (ds);
        }
        [WebMethod]
        public DataSet GetDotood()
        {
            string connStr = ConfigurationManager.ConnectionStrings["dsn"].ConnectionString;
            // First Create a New Connection
            SqlConnection conn = new SqlConnection(connStr);
            // Now Pass a Connection String To the Connection
            // Now the Select statement you want to run
            string select = @"select Content.CID,
	                                Users.UserName, 
	                                Content.Description, 
	                                Content.Image,
	                                Convert(varchar(19),Content.Date) 
	                                from Content 
	                                left join Users on Users.ID = Content.ID where torol=N'Дотоод мэдээ'
	                                ORDER BY Content.Date DESC";
            // Create an Adapter
            SqlDataAdapter da = new SqlDataAdapter(select, conn);
            // Create a New DataSet
            DataSet ds = new DataSet();
            // Fill The DataSet With the Contents of the Stock Table
            da.Fill(ds, "content");
            // Now Return ds which is a DataSet
            return (ds);
        }
        [WebMethod]
        public DataSet GetUser(string id)
        {
            string connStr = ConfigurationManager.ConnectionStrings["dsn"].ConnectionString;
            // First Create a New Connection
            SqlConnection conn = new SqlConnection(connStr);
            // Now Pass a Connection String To the Connection
            // Now the Select statement you want to run
            string select = "SELECT * FROM Users where id!='"+id+"'";
            // Create an Adapter
            SqlDataAdapter da = new SqlDataAdapter(select, conn);
            // Create a New DataSet
            DataSet ds = new DataSet();
            // Fill The DataSet With the Contents of the Stock Table
            da.Fill(ds, "users");
            // Now Return ds which is a DataSet
            return (ds);
        }

        


    }
}
