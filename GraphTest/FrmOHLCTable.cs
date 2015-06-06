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
    public partial class FrmOHLCTable : Form {
        int corpCode;

        public FrmOHLCTable(int code) {
            corpCode = code;
            InitializeComponent();
        }

        private void FrmOHLCTable_Load(object sender, EventArgs e) {
            dgvOHLCTable.DataSource = StockDataManager.StockCache.Get(corpCode).AsDataTable();
        }
    }
}
