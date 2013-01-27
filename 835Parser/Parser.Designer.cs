namespace RemittanceParser
{
    partial class Parser
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Parser));
            this.btn_browse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_browse
            // 
            this.btn_browse.Location = new System.Drawing.Point(12, 12);
            this.btn_browse.Name = "btn_browse";
            this.btn_browse.Size = new System.Drawing.Size(521, 158);
            this.btn_browse.TabIndex = 1;
            this.btn_browse.Text = "Browse to file...";
            this.btn_browse.UseVisualStyleBackColor = true;
            this.btn_browse.Click += new System.EventHandler(this.btn_browse_Click);
            // 
            // Parser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 182);
            this.Controls.Add(this.btn_browse);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Parser";
            this.Text = "835 Remittance Parser";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_browse;
    }
}

