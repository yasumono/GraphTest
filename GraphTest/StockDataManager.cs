using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Windows.Forms;
using MathNet.Numerics.Statistics;
using System.Diagnostics;


namespace GraphTest {
    struct OHLC //１銘柄１日分のデータ
    {
        public DateTime date;
        public int open;
        public int close;
        public int high;
        public int low;
        public int adj_close;
        public int volume;

        public double logReturn;
    }

    class StockData //１銘柄のデータ
    {
        public int corpCode;
        public OHLC[] data;
        public int Length { get { return data.Length; } }

        //ジェネリック型のために引数なしのコンストラクタを定義
        public StockData() {
            corpCode = 0;
            data = null;
        }

        public StockData(int length) {
            corpCode = 0;
            data = new OHLC[length];
        }

        public void CalcLogReturn() {
            for (int i = 0; i < data.Length - 1; i++) {
                data[i].logReturn = Math.Log(data[i].close) - Math.Log(data[i + 1].close);
            }
        }

        public int MaxHigh() {
            return (from dd in data.AsEnumerable() select dd.high).Max();
        }

        public int MinLow() {
            return (from dd in data.AsEnumerable() select dd.low).Min();
        }
    }



    class StockDataCache : DBCache<StockData> //全銘柄のデータ
    {
        public override int MakeKey(StockData it) {
            return it.corpCode;
        }
    }

    class CorpInfo {
        public int corpCode { get; set; }
        public String corpName { get; set; }
        public int gyousyuCode33 { get; set; }
        public String gyousyuClass33 { get; set; }
        public int gyousyuCode17 { get; set; }
        public String gyousyuClass17 { get; set; }
        public int kiboCode { get; set; }
        public String kiboClass { get; set; }

        public double volatility { get; set; }
    }

    class CorpInfoCache : DBCache<CorpInfo> {
        //全銘柄リストのキャッシュを作成する際のキーを設定
        public override int MakeKey(CorpInfo it) {
            return it.corpCode;
        }
    }

    static class CorpListManager {
        public static CorpInfoCache CorpCache = new CorpInfoCache();
        public static DataTable dtCache;

        //コンストラクタ…t_allcorpから全銘柄のリストを取得
        public static void RefreshCache() {
            dtCache = new DataTable();
            String sql = "select * from t_allcorp where corpCode <10000 order by corpCode;";

            //一時変数

            DataRow dr_temp;

            //dt_listに全銘柄リストを取得
            dtCache = SQLManager.SendSQL(sql);

            //if (CorpListManager.Cache == null) CorpListManager.RefreshCache(); 

            for (int i = 0; i < dtCache.Rows.Count; i++) {
                CorpInfo ci_temp = new CorpInfo();
                dr_temp = dtCache.Rows[i];
                ci_temp.corpCode = dr_temp.Field<int>("corpCode");
                ci_temp.corpName = dr_temp.Field<String>("corpName");
                ci_temp.gyousyuCode33 = dr_temp.Field<int>("33gyousyuCode");
                ci_temp.gyousyuClass33 = dr_temp.Field<String>("33gyousyuClass");
                ci_temp.gyousyuCode17 = dr_temp.Field<int>("17gyousyuCode");
                ci_temp.gyousyuClass17 = dr_temp.Field<String>("17gyousyuClass");
                ci_temp.kiboCode = dr_temp.Field<int>("kiboCode");
                ci_temp.kiboClass = dr_temp.Field<String>("kiboClass");

                CorpCache.SetData(ci_temp);
            }

        }

        public static void CalcVoratility(params DateTime[] span) {
            DateTime start = new DateTime(1990, 01, 01);
            DateTime end = new DateTime();
            end = DateTime.Today;

            if (span.Length != 2 || span[0] == span[1]) { }
            else if (span[0] <= span[1]) { start = span[0]; end = span[1]; }
            else if (span[1] <= span[0]) { start = span[1]; end = span[0]; }

            CorpInfo ci = new CorpInfo();
            try {
                int[] codelist = new int[CorpCache.KeyLength()];
                CorpCache.Keys().CopyTo(codelist, 0);
                for (int i = 0; i < codelist.Count(); i++)
                // var codelist = CorpCache.Keys();
                // foreach(int code in codelist)
                {
                    try {
                        int code = codelist[i];
                        StockDataManager.StockCache.Get(code).CalcLogReturn();

                        var log_ret = (from row in StockDataManager.StockCache.Get(code).data.AsEnumerable()
                                       where (row.date >= start) && (row.date <= end)
                                       select row.logReturn).ToArray(); //目的期間の対数差収益率を取得

                        ci = CorpCache.Get(code);
                        ci.volatility = Statistics.PopulationStandardDeviation(log_ret);//標本平均分散
                        CorpCache.SetData(ci);
                    }
                    catch { }
                }
            }
            catch (InvalidOperationException ioe) {
                MessageBox.Show(ioe.Message + "\n" + ioe.Source + "\n" + ioe.StackTrace);
            }
        }
    }


    static class StockDataManager {
        public static StockDataCache StockCache = new StockDataCache(); //全銘柄の株価データ

        /// <summary>
        /// RefreshCache：全銘柄の株価データをStockCacheへ取得
        /// </summary>
        public static void RefreshCache() {
            /////////////////////////////////
            Stopwatch sw_dic = new Stopwatch();
            ///////////////////////
            sw_dic.Start();

            //各種オブジェクトの作成
            DataTable dt_list = new DataTable();
            String sql = "select * from t_yahoofdata where (date > '2014-01-01') AND (corpCode <10000);";



            if (CorpListManager.CorpCache.KeyLength() == 0) CorpListManager.RefreshCache(); //CorpListへ企業情報を格納する。

            //dt_listに全株価情報を取得
            dt_list = SQLManager.SendSQL(sql);
            ////////////////////////////////
            sw_dic.Stop();

            //////////////////////
            Stopwatch sw_linq = new Stopwatch();
            Stopwatch sw_copy = new Stopwatch();

            //////////////////////

            //CorpListから
            foreach (var code in CorpListManager.CorpCache.Keys()) {
                ////////////////////////
                sw_linq.Start();

                DataRow[] dr_rows = (
                    from row in dt_list.AsEnumerable()
                    where row.Field<int>("corpCode") == code
                    orderby row.Field<DateTime>("date")
                    select row).ToArray();

                ///////////////////////
                sw_linq.Stop();



                StockData sd_temp = new StockData(dr_rows.Length);//一時変数は銘柄ごとに作り直し



                ///////////////////////
                sw_copy.Start();

                for (int i = 0; i < dr_rows.Length; i++) {
                    sd_temp.data[i].date = (DateTime)dr_rows[i]["Date"];
                    sd_temp.data[i].open = Convert.ToInt32(dr_rows[i]["open"]);
                    sd_temp.data[i].close = Convert.ToInt32(dr_rows[i]["close"]);
                    sd_temp.data[i].high = Convert.ToInt32(dr_rows[i]["high"]);
                    sd_temp.data[i].low = Convert.ToInt32(dr_rows[i]["low"]);
                    sd_temp.data[i].adj_close = Convert.ToInt32(dr_rows[i]["adj_close"]);
                    sd_temp.data[i].volume = Convert.ToInt32(dr_rows[i]["volume"]);
                }
                ////////////////////////////////
                sw_copy.Stop();

                sd_temp.corpCode = code;
                StockCache.SetData(sd_temp);
            }
            MessageBox.Show("LINQ : " + sw_linq.ElapsedMilliseconds
                + "ms\nCOPY : " + sw_copy.ElapsedMilliseconds
                + "ms\nDIC : " + sw_dic.ElapsedMilliseconds + "ms");
        }

        public static void RefreshCache2() {
            /////////////////////////////////
            Stopwatch sw_dic = new Stopwatch();
            ///////////////////////
            sw_dic.Start();

            //各種オブジェクトの作成
            DataTable dt_list = new DataTable();
            DataTable dt_counts = new DataTable();

            if (CorpListManager.CorpCache.KeyLength() == 0) CorpListManager.RefreshCache(); //CorpListへ企業情報を格納する。

            //dt_listに全株価情報を取得
            String fltr = "where (date > '2014-09-01') AND (corpCode <10000) ";
            dt_list = SQLManager.SendSQL("select * from t_yahoofdata " + fltr + "GROUP BY corpCode ASC, date DESC;");

            dt_counts = SQLManager.SendSQL(
                "SELECT corpCode, COUNT(date) FROM t_yahoofdata " + fltr + " GROUP BY corpCode;");

            ////////////////////////////////
            sw_dic.Stop();

            //////////////////////
            Stopwatch sw_linq = new Stopwatch();
            Stopwatch sw_copy = new Stopwatch();

            //CorpListから
            int startpoint = 0;
            int count = 0;
            foreach (var code in CorpListManager.CorpCache.Keys()) {
                
                object[] obj_count = (from row in dt_counts.AsEnumerable()
                                      where (int)row[0] == code
                                      select row[1]).ToArray(); //切り取り終了行数を設定
                count = Convert.ToInt32(obj_count[0]);

////////////////////////
                sw_linq.Start();

                //startpointからcountだけデータを切り取る
                DataRow[] dr_rows = dt_list.AsEnumerable().Skip(startpoint).Take(count).ToArray();
                startpoint += count;

                ///////////////////////
                sw_linq.Stop();

                StockData sd_temp = new StockData(dr_rows.Length);//一時変数は銘柄ごとに作り直し

                ///////////////////////
                sw_copy.Start();

                //SQLManager.DataTableDump(ref dt_list, "List.txt");//dumpテスト

                for (int i = 0; i < dr_rows.Length; i++) {
                    if ((int)dr_rows[i]["corpCode"] != code) {
                        sw_copy.Stop();
                        MessageBox.Show("Refresh()で切り取り行数が一致しませんでした。\n corpcode: " + code.ToString());
                        SQLManager.DataTableDump(ref dt_list, "List.txt");
                        SQLManager.DataTableDump(ref dt_counts, "Counts.txt");
                        return;
                    }
                    sd_temp.data[i].date = (DateTime)dr_rows[i]["Date"];
                    sd_temp.data[i].open = Convert.ToInt32(dr_rows[i]["open"]);
                    sd_temp.data[i].close = Convert.ToInt32(dr_rows[i]["close"]);
                    sd_temp.data[i].high = Convert.ToInt32(dr_rows[i]["high"]);
                    sd_temp.data[i].low = Convert.ToInt32(dr_rows[i]["low"]);
                    sd_temp.data[i].adj_close = Convert.ToInt32(dr_rows[i]["adj_close"]);
                    sd_temp.data[i].volume = Convert.ToInt32(dr_rows[i]["volume"]);
                }
                ////////////////////////////////
                sw_copy.Stop();

                sd_temp.corpCode = code;
                StockCache.SetData(sd_temp);
            }
            MessageBox.Show("LINQ : " + sw_linq.ElapsedMilliseconds
                + "ms\nCOPY : " + sw_copy.ElapsedMilliseconds
                + "ms\nDIC : " + sw_dic.ElapsedMilliseconds + "ms");
        }

        public static double CalcVoratility(int code, params DateTime[] span) {
            DateTime start = new DateTime();
            DateTime end = new DateTime();

            if (span.Length != 2 || span[0] == span[1])//span[1]無い場合は最新株価の日付
            {
                start = span[0]; end = StockCache.Get(code).data[0].date;
            }
            else if (span[0] < span[1]) { start = span[0]; end = span[1]; }
            else if (span[1] < span[0]) { start = span[1]; end = span[0]; }

            var log_ret = (from row in StockCache.Get(code).data.AsEnumerable()
                           where (row.date >= start) && (row.date <= end)
                           select row.logReturn).ToArray();

            return Statistics.PopulationStandardDeviation(log_ret);

        }
    }
}