namespace GraphTest {
    partial class FrmOHLCTable {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.dgvOHLCTable = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOHLCTable)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvOHLCTable
            // 
            this.dgvOHLCTable.AllowUserToAddRows = false;
            this.dgvOHLCTable.AllowUserToDeleteRows = false;
            this.dgvOHLCTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOHLCTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOHLCTable.Location = new System.Drawing.Point(0, 0);
            this.dgvOHLCTable.Name = "dgvOHLCTable";
            this.dgvOHLCTable.ReadOnly = true;
            this.dgvOHLCTable.RowTemplate.Height = 21;
            this.dgvOHLCTable.Size = new System.Drawing.Size(495, 396);
            this.dgvOHLCTable.TabIndex = 0;
            // 
            // FrmOHLCTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 396);
            this.Controls.Add(this.dgvOHLCTable);
            this.Name = "FrmOHLCTable";
            this.Text = "FrmOHLCTable";
            this.Load += new System.EventHandler(this.FrmOHLCTable_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOHLCTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvOHLCTable;
    }
}