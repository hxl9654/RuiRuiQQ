namespace RuiRuiQQRobot
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pictureBoxQRCode = new System.Windows.Forms.PictureBox();
            this.groupbox1 = new System.Windows.Forms.GroupBox();
            this.labelQQNum = new System.Windows.Forms.Label();
            this.labelQRStatu = new System.Windows.Forms.Label();
            this.buttonLogIn = new System.Windows.Forms.Button();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.textBoxResiveMessage = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listBoxFriend = new System.Windows.Forms.ListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.listBoxGroup = new System.Windows.Forms.ListBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBoxSendMessage = new System.Windows.Forms.TextBox();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQRCode)).BeginInit();
            this.groupbox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxQRCode
            // 
            this.pictureBoxQRCode.Enabled = false;
            this.pictureBoxQRCode.Location = new System.Drawing.Point(8, 20);
            this.pictureBoxQRCode.Name = "pictureBoxQRCode";
            this.pictureBoxQRCode.Size = new System.Drawing.Size(165, 165);
            this.pictureBoxQRCode.TabIndex = 7;
            this.pictureBoxQRCode.TabStop = false;
            this.pictureBoxQRCode.Click += new System.EventHandler(this.pictureBoxQRCode_Click);
            // 
            // groupbox1
            // 
            this.groupbox1.Controls.Add(this.labelQQNum);
            this.groupbox1.Controls.Add(this.labelQRStatu);
            this.groupbox1.Controls.Add(this.buttonLogIn);
            this.groupbox1.Controls.Add(this.pictureBoxQRCode);
            this.groupbox1.Location = new System.Drawing.Point(12, 12);
            this.groupbox1.Name = "groupbox1";
            this.groupbox1.Size = new System.Drawing.Size(208, 193);
            this.groupbox1.TabIndex = 8;
            this.groupbox1.TabStop = false;
            this.groupbox1.Text = "登录";
            // 
            // labelQQNum
            // 
            this.labelQQNum.AutoSize = true;
            this.labelQQNum.Location = new System.Drawing.Point(19, 36);
            this.labelQQNum.Name = "labelQQNum";
            this.labelQQNum.Size = new System.Drawing.Size(0, 12);
            this.labelQQNum.TabIndex = 10;
            // 
            // labelQRStatu
            // 
            this.labelQRStatu.AutoSize = true;
            this.labelQRStatu.Location = new System.Drawing.Point(179, 20);
            this.labelQRStatu.Name = "labelQRStatu";
            this.labelQRStatu.Size = new System.Drawing.Size(0, 12);
            this.labelQRStatu.TabIndex = 9;
            // 
            // buttonLogIn
            // 
            this.buttonLogIn.Location = new System.Drawing.Point(179, 118);
            this.buttonLogIn.Name = "buttonLogIn";
            this.buttonLogIn.Size = new System.Drawing.Size(23, 67);
            this.buttonLogIn.TabIndex = 8;
            this.buttonLogIn.Text = "登　录";
            this.buttonLogIn.UseVisualStyleBackColor = true;
            this.buttonLogIn.Click += new System.EventHandler(this.buttonLogIn_Click);
            // 
            // listBoxLog
            // 
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.ItemHeight = 12;
            this.listBoxLog.Location = new System.Drawing.Point(8, 20);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(192, 220);
            this.listBoxLog.TabIndex = 9;
            this.listBoxLog.SelectedIndexChanged += new System.EventHandler(this.listBoxLog_SelectedIndexChanged);
            this.listBoxLog.DoubleClick += new System.EventHandler(this.listBoxLog_DoubleClick);
            // 
            // textBoxResiveMessage
            // 
            this.textBoxResiveMessage.AcceptsTab = true;
            this.textBoxResiveMessage.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxResiveMessage.Location = new System.Drawing.Point(6, 17);
            this.textBoxResiveMessage.MaxLength = 0;
            this.textBoxResiveMessage.Multiline = true;
            this.textBoxResiveMessage.Name = "textBoxResiveMessage";
            this.textBoxResiveMessage.ReadOnly = true;
            this.textBoxResiveMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxResiveMessage.Size = new System.Drawing.Size(314, 333);
            this.textBoxResiveMessage.TabIndex = 10;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBoxLog);
            this.groupBox2.Location = new System.Drawing.Point(12, 211);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(208, 250);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "系统日志";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.listBoxFriend);
            this.groupBox3.Location = new System.Drawing.Point(226, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(330, 270);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "好友列表";
            // 
            // listBoxFriend
            // 
            this.listBoxFriend.ItemHeight = 12;
            this.listBoxFriend.Location = new System.Drawing.Point(10, 20);
            this.listBoxFriend.Name = "listBoxFriend";
            this.listBoxFriend.Size = new System.Drawing.Size(314, 244);
            this.listBoxFriend.TabIndex = 9;
            this.listBoxFriend.SelectedIndexChanged += new System.EventHandler(this.listBoxFriend_SelectedIndexChanged);
            this.listBoxFriend.DoubleClick += new System.EventHandler(this.listBoxFriend_DoubleClick);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.listBoxGroup);
            this.groupBox4.Location = new System.Drawing.Point(226, 288);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(330, 173);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "群组列表";
            // 
            // listBoxGroup
            // 
            this.listBoxGroup.FormattingEnabled = true;
            this.listBoxGroup.ItemHeight = 12;
            this.listBoxGroup.Location = new System.Drawing.Point(6, 19);
            this.listBoxGroup.Name = "listBoxGroup";
            this.listBoxGroup.Size = new System.Drawing.Size(318, 148);
            this.listBoxGroup.TabIndex = 9;
            this.listBoxGroup.SelectedIndexChanged += new System.EventHandler(this.listBoxGroup_SelectedIndexChanged);
            this.listBoxGroup.DoubleClick += new System.EventHandler(this.listBoxGroup_DoubleClick);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.buttonSend);
            this.groupBox5.Controls.Add(this.textBoxSendMessage);
            this.groupBox5.Controls.Add(this.textBoxResiveMessage);
            this.groupBox5.Location = new System.Drawing.Point(562, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(326, 449);
            this.groupBox5.TabIndex = 14;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "聊天信息";
            // 
            // buttonSend
            // 
            this.buttonSend.Enabled = false;
            this.buttonSend.Location = new System.Drawing.Point(269, 356);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(51, 87);
            this.buttonSend.TabIndex = 12;
            this.buttonSend.Text = "发送";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBoxSendMessage
            // 
            this.textBoxSendMessage.AcceptsTab = true;
            this.textBoxSendMessage.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxSendMessage.Location = new System.Drawing.Point(0, 356);
            this.textBoxSendMessage.Multiline = true;
            this.textBoxSendMessage.Name = "textBoxSendMessage";
            this.textBoxSendMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxSendMessage.Size = new System.Drawing.Size(263, 87);
            this.textBoxSendMessage.TabIndex = 11;
            // 
            // textBoxLog
            // 
            this.textBoxLog.Location = new System.Drawing.Point(12, 467);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(876, 32);
            this.textBoxLog.TabIndex = 15;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 501);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupbox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(916, 540);
            this.MinimumSize = new System.Drawing.Size(916, 540);
            this.Name = "MainForm";
            this.Text = "RuiRui QQ Robot";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQRCode)).EndInit();
            this.groupbox1.ResumeLayout(false);
            this.groupbox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.PictureBox pictureBoxQRCode;
        private System.Windows.Forms.GroupBox groupbox1;
        public System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.TextBox textBoxResiveMessage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.ListBox listBoxFriend;
        private System.Windows.Forms.GroupBox groupBox4;
        public System.Windows.Forms.ListBox listBoxGroup;
        private System.Windows.Forms.GroupBox groupBox5;
        internal System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxSendMessage;
        public System.Windows.Forms.TextBox textBoxLog;
        internal System.Windows.Forms.Button buttonLogIn;
        public System.Windows.Forms.Label labelQRStatu;
        internal System.Windows.Forms.Label labelQQNum;
    }
}

