using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;
using System.Web.Configuration;

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
        public static string connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        [WebMethod]
        public string getLoginAccess(string userID, string password)
        {
            SqlConnection conn;
            SqlCommand command;
            DataTable mainTable;

            conn = new SqlConnection(connectionString);
            conn.Open();

            command = new SqlCommand("SELECT USERID, STUDENTID, PASSWORD, ISACTIVE, CREATED, CREATEDBY, UPDATED, UPDATEDBY, IPADDRESS, MACADDRESS FROM TBLUSER WHERE USERID = @USERID", conn);
            //command.BindByName = true;
            command.Parameters.AddWithValue("@USERID", userID);

            SqlDataReader oraReader = command.ExecuteReader();
            mainTable = new DataTable();
            mainTable.Load(oraReader);
            oraReader.Close();
            conn.Close();

            if (mainTable.Rows.Count == 1)
            {
                DataRow mainRow = mainTable.Rows[0];
                if (Convert.ToString(mainRow["PASSWORD"]).Equals(Encrypt(password)))
                {
                    if (Convert.ToString(mainRow["ISACTIVE"]).Equals("N"))
                    {
                        return "Идэвхгүй хэрэглэгч байна.";
                    }
                    else
                    {
                        return "OK";
                    }
                }
                else
                {
                    return "Нууц үг буруу байна.";
                }
            }
            else
            {
                return "Бүртгэлгүй хэрэглэгч байна.";
            }
        }

        [WebMethod]
        public string setChangePassword(string userID, string password, string newPassword)
        {
            SqlConnection conn;
            SqlCommand command;
            DataTable mainTable;

            conn = new SqlConnection(connectionString);
            conn.Open();

            command = new SqlCommand("SELECT USERID, STUDENTID, PASSWORD, ISACTIVE, CREATED, CREATEDBY, UPDATED, UPDATEDBY, IPADDRESS, MACADDRESS FROM TBLUSER WHERE USERID = @USERID AND PASSWORD = @PASSWORD", conn);
            //command.BindByName = true;
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@USERID", userID);
            command.Parameters.AddWithValue("@PASSWORD", Encrypt(password));

            SqlDataReader oraReader = command.ExecuteReader();
            mainTable = new DataTable();
            mainTable.Load(oraReader);
            oraReader.Close();
            conn.Close();

            if (mainTable.Rows.Count == 1)
            {
                conn.Open();
                command = new SqlCommand("UPDATE TBLUSER SET PASSWORD = @PASSWORD WHERE USERID = @USERID", conn);
                //command.BindByName = true;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@USERID", userID);
                command.Parameters.AddWithValue("@PASSWORD", Encrypt(newPassword));
                command.ExecuteNonQuery();
                conn.Close();
                return "OK";
            }
            else
            {
                return "Нууц үг буруу байна.";
            }
        }

        [WebMethod]
        public string setBookOrder(string userID, string bookID, bool isOrder)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand command;
                using (DataTable mainTable = new DataTable())
                {

                    command = new SqlCommand("SELECT ORDERID, STUDENTID, BOOKID, ORDERDATE, GIVEDATE, TAKEDATE, RETURNDATE, RETURNEDDATE, STATUS, REASON, NOTE, CREATED, CREATEDBY, UPDATED, UPDATEDBY, IPADDRESS, MACADDRESS FROM TBLBOOKORDER WHERE STATUS = 0 AND STUDENTID = (select studentid from tbluser where USERID = @USERID) AND BOOKID = @BOOKID", conn);
                    //command.BindByName = true;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@USERID", userID);
                    command.Parameters.AddWithValue("@BOOKID", bookID);

                    SqlDataReader oraReader = command.ExecuteReader();

                    mainTable.Load(oraReader);
                    oraReader.Close();
                    conn.Close();

                    if (mainTable.Rows.Count == 0 && isOrder)
                    {
                        conn.Open();
                        decimal newID = NewID("TBLBOOKORDER", "ORDERID");
                        command = new SqlCommand(@"insert into tblBookOrder
                                                      (ORDERID,
                                                       STUDENTID,
                                                       BOOKID,
                                                       ORDERDATE,
                                                       GIVEDATE,
                                                       TAKEDATE,
                                                       RETURNDATE,
                                                       RETURNEDDATE,
                                                       STATUS,
                                                       REASON,
                                                       NOTE,
                                                       CREATED,
                                                       CREATEDBY,
                                                       UPDATED,
                                                       UPDATEDBY,
                                                       IPADDRESS,
                                                       MACADDRESS)
                                                    values
                                                      (@ORDERID,
                                                       (select studentid from tbluser where userid = @USERID),
                                                       @BOOKID,
                                                       getdate(),
                                                       null,
                                                       null,
                                                       null,
                                                       null,
                                                       0,
                                                       0,
                                                       null,
                                                       getdate(),
                                                       @USERID,
                                                       getdate(),
                                                       @USERID,
                                                       null,
                                                       null)", conn);
                        //command.BindByName = true;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@ORDERID", newID);
                        command.Parameters.AddWithValue("@USERID", userID);
                        command.Parameters.AddWithValue("@BOOKID", bookID);
                        command.ExecuteNonQuery();
                        conn.Close();
                        return "OK";
                    }
                    else if (mainTable.Rows.Count > 0 && !isOrder)
                    {
                        conn.Open();
                        command = new SqlCommand(@"DELETE TBLBOOKORDER WHERE ORDERID = @ORDERID", conn);
                        //command.BindByName = true;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@ORDERID", mainTable.Rows[0]["ORDERID"]);
                        command.ExecuteNonQuery();
                        conn.Close();

                        return "Захиалга цуцалсан.";
                    }
                    return "Not Found";
                }
            }
        }

        public decimal NewID(string tableName, string fieldName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    DataTable mainTable = new DataTable();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = "SELECT USEDINDEX FROM TBLINDEX WHERE UPPER(INDEXCODE) = UPPER('" + tableName + "')";
                    command.CommandType = CommandType.Text;

                    SqlDataReader dr = command.ExecuteReader();
                    mainTable.Clear();
                    mainTable.Load(dr);
                    if (mainTable.Rows.Count > 0)
                    {
                        command.CommandText = " UPDATE TBLINDEX SET USEDINDEX = '" + (Convert.ToDecimal(mainTable.Rows[0]["USEDINDEX"]) + 1) + "' WHERE UPPER(INDEXCODE) = UPPER('" + tableName + "')";
                        command.ExecuteNonQuery();
                        return Convert.ToDecimal(mainTable.Rows[0]["USEDINDEX"]);
                    }
                    else
                    {
                        command.CommandText = " INSERT INTO TBLINDEX (INDEXCODE, USEDINDEX) VALUES ('" + tableName + "', (SELECT ISNULL(MAX(" + fieldName + "), 0) + 1 FROM " + tableName + ")) ";
                        command.ExecuteNonQuery();
                        command.CommandText = "SELECT USEDINDEX FROM TBLINDEX WHERE UPPER(INDEXCODE) = UPPER('" + tableName + "')";
                        dr = command.ExecuteReader();
                        mainTable.Clear();
                        mainTable.Load(dr);
                        if (mainTable.Rows.Count > 0)
                        {
                            command.CommandText = " UPDATE TBLINDEX SET USEDINDEX = '" + (Convert.ToDecimal(mainTable.Rows[0]["USEDINDEX"]) + 1) + "' WHERE UPPER(INDEXCODE) = UPPER('" + tableName + "')";
                            command.ExecuteNonQuery();
                            return Convert.ToDecimal(mainTable.Rows[0]["USEDINDEX"]);
                        }
                    }
                    dr.Dispose();
                    command.Dispose();
                    mainTable.Dispose();
                    conn.Close();
                }
            }
            catch (Exception)
            {

            }
            return 1;
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "Ariunmunkh.e";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x4a, 0x76, 0x65, 0x6a, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76, 0x7a });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        [WebMethod]
        public string getTestConn()
        {
            return "OK";
        }

        [WebMethod]
        public DataSet GetCategory(string lastMaxID, string lastMotifyDate)
        {
            string SQL;
            SqlConnection conn;
            SqlCommand cmd;
            SqlDataAdapter da;
            DataSet ds;
            string whereLastMaxID;
            string whereLastMotifyDate;
            try
            {
                whereLastMaxID = " CategoryID > " + Convert.ToDecimal(lastMaxID);
            }
            catch (Exception)
            {
                whereLastMaxID = " CategoryID > -1";
            }
            try
            {
                whereLastMotifyDate = " OR Updated > Cast('" + Convert.ToDateTime(lastMaxID).ToString("yyyy.MM.dd HH:mm:ss") + "' as datetime)";
            }
            catch (Exception)
            {
                whereLastMotifyDate = string.Empty;
            }
            conn = new SqlConnection(connectionString);
            conn.Open();

            SQL = "SELECT CATEGORYID, CODE, NAME, ISACTIVE, CREATED, CREATEDBY, UPDATED, UPDATEDBY, IPADDRESS, MACADDRESS FROM TBLCATEGORY WHERE" + whereLastMaxID + whereLastMotifyDate;
            cmd = new SqlCommand(SQL, conn);
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);
            conn.Close();

            return ds;
        }

        [WebMethod]
        public DataSet GetBook(string lastMaxID, string lastMotifyDate)
        {
            string SQL;
            SqlConnection conn;
            SqlCommand cmd;
            SqlDataAdapter da;
            DataSet ds;
            string whereLastMaxID;
            string whereLastMotifyDate;
            try
            {
                whereLastMaxID = " tblBook.BookID > " + Convert.ToDecimal(lastMaxID);
            }
            catch (Exception)
            {
                whereLastMaxID = " tblBook.BookID > -1";
            }
            try
            {
                whereLastMotifyDate = " OR tblBook.Updated > Cast('" + Convert.ToDateTime(lastMotifyDate).ToString("yyyy.MM.dd HH:mm:ss") + "' as datetime)";
            }
            catch (Exception)
            {
                whereLastMotifyDate = string.Empty;
            }
            conn = new SqlConnection(connectionString);
            conn.Open();

            SQL = @"SELECT TBLBOOK.BOOKID,
                           CODE,
                           NAME,
                           ISBN,
                           CATEGORYID,
                           ISACTIVE,
                           PRINTEDYEAR,
                           PRINTEDVERSION,
                           VOLUMENUM,
                           TOTALVOLUMENUM,
                           CREATED,
                           CREATEDBY,
                           UPDATED,
                           UPDATEDBY,
                           TBLBOOKPICTURE.PICTUREDATA
                      FROM TBLBOOK
                      LEFT JOIN TBLBOOKPICTURE
                        ON TBLBOOKPICTURE.BOOKID = TBLBOOK.BOOKID
                     WHERE" + whereLastMaxID + whereLastMotifyDate;
            cmd = new SqlCommand(SQL, conn);
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);
            conn.Close();

            return ds;
        }

        [WebMethod]
        public DataSet GetBookOrder(string lastMotifyDate)
        {
            string SQL;
            SqlConnection conn;
            SqlCommand cmd;
            SqlDataAdapter da;
            DataSet ds;
            string whereLastMotifyDate;

            try
            {
                whereLastMotifyDate = " AND tblBookOrder.Updated > Cast('" + Convert.ToDateTime(lastMotifyDate).ToString("yyyy.MM.dd HH:mm:ss") + "' as datetime)";
            }
            catch (Exception)
            {
                whereLastMotifyDate = string.Empty;
            }

            conn = new SqlConnection(connectionString);
            conn.Open();

            SQL = @"SELECT TBLBOOKORDER.ORDERID,
                           TBLBOOKORDER.STUDENTID,
                           TBLBOOKORDER.BOOKID,
                           TBLBOOKORDER.ORDERDATE,
                           TBLBOOKORDER.GIVEDATE,
                           TBLBOOKORDER.TAKEDATE,
                           TBLBOOKORDER.RETURNDATE,
                           TBLBOOKORDER.RETURNEDDATE,
                           TBLBOOKORDER.STATUS,
                           TBLBOOKORDER.REASON,
                           TBLBOOKORDER.NOTE,
                           TBLBOOKORDER.CREATED,
                           TBLBOOKORDER.CREATEDBY,
                           TBLBOOKORDER.UPDATED,
                           TBLBOOKORDER.UPDATEDBY,
                           TBLBOOKORDER.IPADDRESS,
                           TBLBOOKORDER.MACADDRESS FROM TBLBOOKORDER
                     WHERE 1 = 1 " + whereLastMotifyDate;
            cmd = new SqlCommand(SQL, conn);
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);
            conn.Close();

            return ds;
        }

    }
}