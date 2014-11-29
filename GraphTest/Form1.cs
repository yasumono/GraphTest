using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;

namespace GraphTest
{


    public partial class Form1 : Form
    {
        //子フォームのインスタンス宣言
        Form2 form2;


        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Chartオブジェクトを受け取り、実際にローソクを描画
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="code"></param>
        private void DrawChart(Chart chart, int code)
        {
            string legend = code + " " + CorpListManager.CorpCache.Get(code).corpName;

            chart.Series.Clear();
            chart.Series.Add(legend);
            chart.Series[legend].ChartType = SeriesChartType.Candlestick;
            chart.Series[legend].LegendText = legend;

            chart.ChartAreas[0].InnerPlotPosition.Auto = false;
            chart.ChartAreas[0].InnerPlotPosition.Width = 100; // 100%
            chart.ChartAreas[0].InnerPlotPosition.Height = 90;  // 90%(横軸のメモリラベル印字分の余裕を設ける)
            chart.ChartAreas[0].InnerPlotPosition.X = 8;
            chart.ChartAreas[0].InnerPlotPosition.Y = 0;

            try
            {
                StockData st = StockDataManager.StockCache.Get(code);


                //試験的にStartDateを設定
                DateTime startDate = new DateTime(2014, 8, 1);

                OHLC[] dd = (from daydata in st.data.AsEnumerable()
                                let day = daydata.date
                                where day > startDate
                                select daydata).ToArray();

                chart.ChartAreas[0].AxisY.Maximum = (int)st.MaxHigh() * 1.1; //Y軸最大値
                chart.ChartAreas[0].AxisY.Minimum = (int)st.MinLow() * 0.9; //Y軸最小値

                for (int i = 0; i < dd.Length; i++)
                {
                    chart.Series[legend].Points.AddXY((object)dd[i].date,
                        dd[i].high,
                        dd[i].low,
                        dd[i].open,
                        dd[i].close);
                }
            }
            catch { }
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int code = int.Parse(textBox1.Text);
                if (CorpListManager.CorpCache.Keys().Contains(code))
                {
                    DrawChart(chart1, code);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //★ＤＢからデータ取得処理
            StockDataManager.RefreshCache2();


            //Stopwatch sw = Stopwatch.StartNew();
            //StockDataManager.RefreshCache();
            //sw.Stop();
            //MessageBox.Show("RefreshCache()読み込み終わり\n所要時間：" + sw.ElapsedMilliseconds.ToString() + "ms");

            //銘柄リストForm2を表示
            form2 = new Form2();
            this.AddOwnedForm(form2);
            form2.Show();
        }
    }


    //グラフ描画の余白を設定
    public static class Const
    {
        public const int mrgnLEFT = 30;
        public const int mrgnTOP = 20;
        public const int mrgnRIGHT = 70;
        public const int mrgnBOTTOM = 30;
        public const int mrgnCANDtoVOL = 30;

    }
}
