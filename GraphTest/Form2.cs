using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphTest {
    public partial class Form2 : Form {
        private CurrencyManager currencyManager = null;

        public Form2() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {

            CorpListManager.CalcVoratility();
            String[] delclms = new String[]{"33gyousyuCode",
                                    "17gyousyuCode",
                                    "17gyousyuClass",
                                    "kiboCode",
                                    "kiboClass"};
            foreach (var clm in delclms) {
                if (CorpListManager.dtCache.Columns.Contains(clm))
                    CorpListManager.dtCache.Columns.Remove(clm);
            }

            if (!(CorpListManager.dtCache.Columns.Contains("ﾎﾞﾗﾃｨﾘﾃｨ")))
                CorpListManager.dtCache.Columns.Add("ﾎﾞﾗﾃｨﾘﾃｨ", typeof(double));

            foreach (DataRow dr in CorpListManager.dtCache.Rows) {
                dr["ﾎﾞﾗﾃｨﾘﾃｨ"] = CorpListManager.CorpCache.Get(dr.Field<int>("corpCode")).volatility;
            }

            dataGridView1.DataSource = CorpListManager.dtCache;
        }

        private void Form2_Load(object sender, EventArgs e) {
            dataGridView1.DataSource = CorpListManager.dtCache;

        }
    }
}
