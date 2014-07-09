using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ADOX;
using System.Data.OleDb;
using System.Data;
namespace MClient.App_Code
{
    public class TDAO
    {
        public OleDbConnection ADOConnection1;
        public OleDbCommand ADOQuery1;
        public OleDbCommand ADOQuery2;
        public OleDbDataReader datareader;
        public TDAO(TDM_Client DM_Client)
        {
            string tmpstr1 = new String(DM_Client.MyRecords.UserInfo.LoginName);
            tmpstr1 = tmpstr1.Substring(0, tmpstr1.IndexOf('\0'));
            ADOConnection1 = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=C:\\" + tmpstr1 + "mdb\\Qty.mdb;");
            ADOQuery1 = new OleDbCommand();
            ADOQuery2 = new OleDbCommand();
            ADOQuery1.Connection = ADOConnection1;
            ADOQuery2.Connection = ADOConnection1;
        }
        public void ExecuteQuery(string sql,int QueryFlag = 0)
        {
            switch(QueryFlag)
            {
                case 0:
                    ADOQuery1.CommandText = sql;
                    ADOQuery1.ExecuteNonQuery();
                    break;
                case 1:
                    ADOQuery1.CommandText = sql;
                    datareader = ADOQuery1.ExecuteReader();
                    break;
                case 2:
                    ADOQuery2.CommandText = sql;
                    datareader = ADOQuery2.ExecuteReader();
                    break;
            }
        }
        public void DeleteAllTables()
        {
            List<string> tableNames;
            int i;
            string sql;

            tableNames = new List<string>();
            ADOConnection1.Open();
            //获取库中的所有表信息
            DataTable tables = ADOConnection1.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (DataRow item in tables.Rows)
            {
                tableNames.Add(item["TABLE_NAME"].ToString());
            }
            for (i = 0; i < tableNames.Count; i++)
            {
                sql = "Drop table " + tableNames[i];
                ExecuteQuery(sql);
            }
        }
    }
}