using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;

namespace GraphTest {
    static class SQLManager {
        //MySQLアクセス用の定数
        private static readonly string username = "root";
        private static readonly string pass = "viviatamesis";
        private static readonly string dbname = "sc_stock";
        private static readonly string host = "localhost";

        public static DataTable SendSQL(String strsql) {
            //プロパティの初期化
            string connstr =
              "userid = " + username +
              "; password = " + pass +
              "; database = " + dbname +
              "; Host = " + host;

            //各種オブジェクトの作成
            MySqlConnection conn = new MySqlConnection(connstr);
            DataTable dt_temp = new DataTable();//一時置きDataTable

            try {
                conn.Open();
                dt_temp.Clear();

                //SQL文と接続情報を指定し、データアダプタを作製・データ取得
                MySqlDataAdapter da = new MySqlDataAdapter(strsql, conn);
                da.Fill(dt_temp);
            }
            catch {
                MessageBox.Show("MySQLサーバーからのデータ取得に失敗しました。");
            }
            finally {
                conn.Close();
            }

            return dt_temp;
        }

        public static void DataTableDump(ref DataTable dt, string fout) {
            try {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(fout)) {

                    for (int i = 0; i < dt.Columns.Count; i++) file.Write(dt.Columns[i].ColumnName + "|");
                    file.WriteLine("");
                    foreach (DataRow dr in dt.Rows) {
                        for (int i = 0; i < dr.ItemArray.Length; i++) file.Write(dr[i].ToString() + "|");
                        file.WriteLine("");
                    }
                    file.WriteLine("");
                }
            }
            catch (IOException e) {
                MessageBox.Show(e.Message);
                return;
            }
            catch (UnauthorizedAccessException e) {
                MessageBox.Show(e.Message);
                return;
            }
        }


    }
}
