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

namespace GraphTest {


    public partial class Form1 : Form {       
        Form2 form2; //子フォームのインスタンス宣言
        public TimeSpan dataSpan = new TimeSpan(6*30, 0,0,0);//株価取得期間は6か月

        public Form1() {
            InitializeComponent();

        }

        protected override void OnPaint(PaintEventArgs e) {
            //var cc2 = new CandleChart2();
            //this.AddOwnedForm(cc2);
            //cc2.Show();
            //cc2.DrawGraph(e);
            //MessageBox.Show("!!!");
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                int code = int.Parse(textBox1.Text);
                if (CorpListManager.CorpCache.Keys().Contains(code)) {                    
                    var cc2 = new CandleChart2(code) { Size = new Size(780, 350), BackColor = Color.White };
                    this.AddOwnedForm(cc2);
                    cc2.Show();
                }
                //グラフの描画
                // 
                // Application.Run(new CandleChart2(code) { Size = new Size(780, 350), BackColor = Color.White });
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            //★ＤＢからデータ取得処理            
            textBox2.Text += "--------------全株価取得中...--------------" + "\r\n";

            DateTime today = new DateTime();
            today = DateTime.Today-dataSpan;
            StockDataManager.RefreshCache2(today);

            int corpcount = CorpListManager.CorpCache.Count;
            int datacount = 0;
            foreach (OHLCSeries ohlcs in StockDataManager.StockCache.Dic.Values.AsEnumerable() ){
                datacount += ohlcs.data.Count();
            }
            textBox2.Text += "取得銘柄数：" + corpcount + "\r\n";
            textBox2.Text += "取得株価データ数数：" + datacount + "\r\n";

            form2 = new Form2();
            this.AddOwnedForm(form2);
            form2.Show();
        }
    }


    //グラフ描画の余白を設定
    public static class Constant {
        public const int mrgnLEFT = 30;
        public const int mrgnTOP = 20;
        public const int mrgnRIGHT = 70;
        public const int mrgnBOTTOM = 30;
        public const int mrgnCANDtoVOL = 30;

        public static int[] atrspan={5};
        public static int[] maspan = { 5, 25 };

        public static void InitConstant(){
        }
        

    }
}
