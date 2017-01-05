namespace FFXIVPacketViewer
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.txtInput = new System.Windows.Forms.TextBox();
            this.lblInput = new System.Windows.Forms.Label();
            this.btnLoadText = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lblHexOut = new System.Windows.Forms.Label();
            this.btnNextPacket = new System.Windows.Forms.Button();
            this.btnLastPacket = new System.Windows.Forms.Button();
            this.lblCurentPacket = new System.Windows.Forms.Label();
            this.lblHexTop = new System.Windows.Forms.Label();
            this.lblHexSide = new System.Windows.Forms.Label();
            this.txtReadable = new System.Windows.Forms.TextBox();
            this.btnLoadHex = new System.Windows.Forms.Button();
            this.btnDataReport = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // txtInput
            // 
            this.txtInput.AllowDrop = true;
            this.txtInput.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInput.Location = new System.Drawing.Point(44, 17);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInput.Size = new System.Drawing.Size(857, 170);
            this.txtInput.TabIndex = 0;
            this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(41, 0);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(42, 14);
            this.lblInput.TabIndex = 1;
            this.lblInput.Text = "Input";
            // 
            // btnLoadText
            // 
            this.btnLoadText.Location = new System.Drawing.Point(786, 192);
            this.btnLoadText.Name = "btnLoadText";
            this.btnLoadText.Size = new System.Drawing.Size(115, 25);
            this.btnLoadText.TabIndex = 2;
            this.btnLoadText.Text = "Load Text File";
            this.btnLoadText.UseVisualStyleBackColor = true;
            this.btnLoadText.Click += new System.EventHandler(this.btnLoadText_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lblHexOut
            // 
            this.lblHexOut.BackColor = System.Drawing.Color.White;
            this.lblHexOut.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHexOut.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHexOut.Location = new System.Drawing.Point(44, 220);
            this.lblHexOut.Name = "lblHexOut";
            this.lblHexOut.Size = new System.Drawing.Size(425, 486);
            this.lblHexOut.TabIndex = 3;
            this.lblHexOut.Text = resources.GetString("lblHexOut.Text");
            // 
            // btnNextPacket
            // 
            this.btnNextPacket.Location = new System.Drawing.Point(814, 716);
            this.btnNextPacket.Name = "btnNextPacket";
            this.btnNextPacket.Size = new System.Drawing.Size(87, 25);
            this.btnNextPacket.TabIndex = 5;
            this.btnNextPacket.Text = "Next";
            this.btnNextPacket.UseVisualStyleBackColor = true;
            this.btnNextPacket.Click += new System.EventHandler(this.btnNextPacket_Click);
            // 
            // btnLastPacket
            // 
            this.btnLastPacket.Location = new System.Drawing.Point(720, 716);
            this.btnLastPacket.Name = "btnLastPacket";
            this.btnLastPacket.Size = new System.Drawing.Size(87, 25);
            this.btnLastPacket.TabIndex = 6;
            this.btnLastPacket.Text = "Previous";
            this.btnLastPacket.UseVisualStyleBackColor = true;
            this.btnLastPacket.Click += new System.EventHandler(this.btnLastPacket_Click);
            // 
            // lblCurentPacket
            // 
            this.lblCurentPacket.BackColor = System.Drawing.Color.White;
            this.lblCurentPacket.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCurentPacket.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurentPacket.Location = new System.Drawing.Point(44, 716);
            this.lblCurentPacket.Name = "lblCurentPacket";
            this.lblCurentPacket.Size = new System.Drawing.Size(396, 25);
            this.lblCurentPacket.TabIndex = 7;
            this.lblCurentPacket.Text = "Current Packet: 0";
            // 
            // lblHexTop
            // 
            this.lblHexTop.AutoSize = true;
            this.lblHexTop.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHexTop.Location = new System.Drawing.Point(45, 205);
            this.lblHexTop.Name = "lblHexTop";
            this.lblHexTop.Size = new System.Drawing.Size(336, 14);
            this.lblHexTop.TabIndex = 8;
            this.lblHexTop.Text = "00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F";
            // 
            // lblHexSide
            // 
            this.lblHexSide.AutoSize = true;
            this.lblHexSide.BackColor = System.Drawing.SystemColors.Control;
            this.lblHexSide.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHexSide.Location = new System.Drawing.Point(6, 221);
            this.lblHexSide.Name = "lblHexSide";
            this.lblHexSide.Size = new System.Drawing.Size(35, 490);
            this.lblHexSide.TabIndex = 41;
            this.lblHexSide.Text = resources.GetString("lblHexSide.Text");
            // 
            // txtReadable
            // 
            this.txtReadable.Location = new System.Drawing.Point(474, 220);
            this.txtReadable.Multiline = true;
            this.txtReadable.Name = "txtReadable";
            this.txtReadable.ReadOnly = true;
            this.txtReadable.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReadable.Size = new System.Drawing.Size(426, 486);
            this.txtReadable.TabIndex = 42;
            // 
            // btnLoadHex
            // 
            this.btnLoadHex.Enabled = false;
            this.btnLoadHex.Location = new System.Drawing.Point(665, 192);
            this.btnLoadHex.Name = "btnLoadHex";
            this.btnLoadHex.Size = new System.Drawing.Size(115, 25);
            this.btnLoadHex.TabIndex = 43;
            this.btnLoadHex.Text = "Load Hex File";
            this.btnLoadHex.UseVisualStyleBackColor = true;
            // 
            // btnDataReport
            // 
            this.btnDataReport.Enabled = false;
            this.btnDataReport.Location = new System.Drawing.Point(544, 192);
            this.btnDataReport.Name = "btnDataReport";
            this.btnDataReport.Size = new System.Drawing.Size(115, 25);
            this.btnDataReport.TabIndex = 44;
            this.btnDataReport.Text = "Data Report";
            this.btnDataReport.UseVisualStyleBackColor = true;
            this.btnDataReport.Click += new System.EventHandler(this.btnDataReport_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(48, 194);
            this.progressBar1.Maximum = 65535;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(490, 23);
            this.progressBar1.TabIndex = 45;
            this.progressBar1.Visible = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 751);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnDataReport);
            this.Controls.Add(this.btnLoadHex);
            this.Controls.Add(this.txtReadable);
            this.Controls.Add(this.lblHexOut);
            this.Controls.Add(this.lblHexSide);
            this.Controls.Add(this.lblHexTop);
            this.Controls.Add(this.lblCurentPacket);
            this.Controls.Add(this.btnLastPacket);
            this.Controls.Add(this.btnNextPacket);
            this.Controls.Add(this.btnLoadText);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.txtInput);
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmMain";
            this.Text = "Dude22072\'s FFXIV Classic Packet Interpreter v0.2.5";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.Button btnLoadText;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label lblHexOut;
        private System.Windows.Forms.Button btnNextPacket;
        private System.Windows.Forms.Button btnLastPacket;
        private System.Windows.Forms.Label lblCurentPacket;
        private System.Windows.Forms.Label lblHexTop;
        private System.Windows.Forms.Label lblHexSide;
        private System.Windows.Forms.TextBox txtReadable;
        private System.Windows.Forms.Button btnLoadHex;
        private System.Windows.Forms.Button btnDataReport;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}

