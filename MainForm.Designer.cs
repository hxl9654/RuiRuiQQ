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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pictureBoxQRCode = new System.Windows.Forms.PictureBox();
            this.groupbox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelQRStatu = new System.Windows.Forms.Label();
            this.buttonLogIn = new System.Windows.Forms.Button();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listBoxFriend = new System.Windows.Forms.ListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.listBoxGroup = new System.Windows.Forms.ListBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.buttonFriendSend = new System.Windows.Forms.Button();
            this.textBoxFriendSend = new System.Windows.Forms.TextBox();
            this.textBoxFriendChat = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.buttonGroupSend = new System.Windows.Forms.Button();
            this.textBoxGroupSend = new System.Windows.Forms.TextBox();
            this.textBoxGroupChat = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.buttonDiscussSend = new System.Windows.Forms.Button();
            this.textBoxDiscussSend = new System.Windows.Forms.TextBox();
            this.textBoxDiscussChat = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.listBoxDiscuss = new System.Windows.Forms.ListBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQRCode)).BeginInit();
            this.groupbox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxQRCode
            // 
            this.pictureBoxQRCode.Enabled = false;
            this.pictureBoxQRCode.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxQRCode.Image")));
            this.pictureBoxQRCode.Location = new System.Drawing.Point(6, 174);
            this.pictureBoxQRCode.Name = "pictureBoxQRCode";
            this.pictureBoxQRCode.Size = new System.Drawing.Size(207, 191);
            this.pictureBoxQRCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxQRCode.TabIndex = 7;
            this.pictureBoxQRCode.TabStop = false;
            this.pictureBoxQRCode.Click += new System.EventHandler(this.pictureBoxQRCode_Click);
            // 
            // groupbox1
            // 
            this.groupbox1.Controls.Add(this.label1);
            this.groupbox1.Controls.Add(this.labelQRStatu);
            this.groupbox1.Controls.Add(this.buttonLogIn);
            this.groupbox1.Controls.Add(this.pictureBoxQRCode);
            this.groupbox1.Location = new System.Drawing.Point(6, 6);
            this.groupbox1.Name = "groupbox1";
            this.groupbox1.Size = new System.Drawing.Size(219, 371);
            this.groupbox1.TabIndex = 8;
            this.groupbox1.TabStop = false;
            this.groupbox1.Text = "登录";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(215, 60);
            this.label1.TabIndex = 11;
            this.label1.Text = "欢迎使用RuiRuiQQ，本程序需登录QQ。\r\n1，使用手机QQ登录您的QQ小号。\r\n2，点击下方的登录按钮。\r\n3，用手机QQ“扫一扫”扫描二维码。\r\n4，在手" +
    "机QQ上点击“同意登录”按钮。";
            // 
            // labelQRStatu
            // 
            this.labelQRStatu.AutoSize = true;
            this.labelQRStatu.Location = new System.Drawing.Point(3, 86);
            this.labelQRStatu.Name = "labelQRStatu";
            this.labelQRStatu.Size = new System.Drawing.Size(173, 12);
            this.labelQRStatu.TabIndex = 9;
            this.labelQRStatu.Text = "当前登录状态：请点击登陆按钮";
            // 
            // buttonLogIn
            // 
            this.buttonLogIn.Location = new System.Drawing.Point(6, 101);
            this.buttonLogIn.Name = "buttonLogIn";
            this.buttonLogIn.Size = new System.Drawing.Size(207, 67);
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
            this.listBoxLog.Size = new System.Drawing.Size(525, 220);
            this.listBoxLog.TabIndex = 9;
            this.listBoxLog.SelectedIndexChanged += new System.EventHandler(this.listBoxLog_SelectedIndexChanged);
            this.listBoxLog.DoubleClick += new System.EventHandler(this.listBoxLog_DoubleClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBoxLog);
            this.groupBox2.Controls.Add(this.textBoxLog);
            this.groupBox2.Location = new System.Drawing.Point(3, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(539, 371);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "系统日志";
            // 
            // textBoxLog
            // 
            this.textBoxLog.Location = new System.Drawing.Point(8, 246);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(525, 119);
            this.textBoxLog.TabIndex = 15;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.listBoxFriend);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(189, 370);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "好友列表";
            // 
            // listBoxFriend
            // 
            this.listBoxFriend.ItemHeight = 12;
            this.listBoxFriend.Location = new System.Drawing.Point(6, 21);
            this.listBoxFriend.Name = "listBoxFriend";
            this.listBoxFriend.Size = new System.Drawing.Size(177, 340);
            this.listBoxFriend.TabIndex = 9;
            this.listBoxFriend.SelectedIndexChanged += new System.EventHandler(this.listBoxFriend_SelectedIndexChanged);
            this.listBoxFriend.DoubleClick += new System.EventHandler(this.listBoxFriend_DoubleClick);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.listBoxGroup);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(189, 370);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "群列表";
            // 
            // listBoxGroup
            // 
            this.listBoxGroup.FormattingEnabled = true;
            this.listBoxGroup.ItemHeight = 12;
            this.listBoxGroup.Location = new System.Drawing.Point(6, 21);
            this.listBoxGroup.Name = "listBoxGroup";
            this.listBoxGroup.Size = new System.Drawing.Size(177, 340);
            this.listBoxGroup.TabIndex = 9;
            this.listBoxGroup.SelectedIndexChanged += new System.EventHandler(this.listBoxGroup_SelectedIndexChanged);
            this.listBoxGroup.DoubleClick += new System.EventHandler(this.listBoxGroup_DoubleClick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "RuiRuiQQ";
            this.notifyIcon1.Visible = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(556, 409);
            this.tabControl1.TabIndex = 16;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.groupBox8);
            this.tabPage1.Controls.Add(this.groupbox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(548, 383);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "账号登陆";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(548, 383);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "好友";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.buttonFriendSend);
            this.groupBox7.Controls.Add(this.textBoxFriendSend);
            this.groupBox7.Controls.Add(this.textBoxFriendChat);
            this.groupBox7.Location = new System.Drawing.Point(201, 6);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(341, 370);
            this.groupBox7.TabIndex = 17;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "聊天信息";
            // 
            // buttonFriendSend
            // 
            this.buttonFriendSend.Enabled = false;
            this.buttonFriendSend.Location = new System.Drawing.Point(286, 260);
            this.buttonFriendSend.Name = "buttonFriendSend";
            this.buttonFriendSend.Size = new System.Drawing.Size(47, 101);
            this.buttonFriendSend.TabIndex = 12;
            this.buttonFriendSend.Text = "发送";
            this.buttonFriendSend.UseVisualStyleBackColor = true;
            this.buttonFriendSend.Click += new System.EventHandler(this.buttonFriendSend_Click);
            // 
            // textBoxFriendSend
            // 
            this.textBoxFriendSend.AcceptsTab = true;
            this.textBoxFriendSend.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxFriendSend.Location = new System.Drawing.Point(6, 260);
            this.textBoxFriendSend.Multiline = true;
            this.textBoxFriendSend.Name = "textBoxFriendSend";
            this.textBoxFriendSend.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxFriendSend.Size = new System.Drawing.Size(274, 101);
            this.textBoxFriendSend.TabIndex = 11;
            // 
            // textBoxFriendChat
            // 
            this.textBoxFriendChat.AcceptsTab = true;
            this.textBoxFriendChat.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxFriendChat.Location = new System.Drawing.Point(6, 15);
            this.textBoxFriendChat.MaxLength = 0;
            this.textBoxFriendChat.Multiline = true;
            this.textBoxFriendChat.Name = "textBoxFriendChat";
            this.textBoxFriendChat.ReadOnly = true;
            this.textBoxFriendChat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxFriendChat.Size = new System.Drawing.Size(329, 239);
            this.textBoxFriendChat.TabIndex = 10;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.groupBox5);
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(548, 383);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "群";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.buttonGroupSend);
            this.groupBox5.Controls.Add(this.textBoxGroupSend);
            this.groupBox5.Controls.Add(this.textBoxGroupChat);
            this.groupBox5.Location = new System.Drawing.Point(201, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(341, 370);
            this.groupBox5.TabIndex = 17;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "聊天信息";
            // 
            // buttonGroupSend
            // 
            this.buttonGroupSend.Enabled = false;
            this.buttonGroupSend.Location = new System.Drawing.Point(286, 260);
            this.buttonGroupSend.Name = "buttonGroupSend";
            this.buttonGroupSend.Size = new System.Drawing.Size(47, 101);
            this.buttonGroupSend.TabIndex = 12;
            this.buttonGroupSend.Text = "发送";
            this.buttonGroupSend.UseVisualStyleBackColor = true;
            this.buttonGroupSend.Click += new System.EventHandler(this.buttonGroupSend_Click);
            // 
            // textBoxGroupSend
            // 
            this.textBoxGroupSend.AcceptsTab = true;
            this.textBoxGroupSend.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxGroupSend.Location = new System.Drawing.Point(6, 260);
            this.textBoxGroupSend.Multiline = true;
            this.textBoxGroupSend.Name = "textBoxGroupSend";
            this.textBoxGroupSend.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxGroupSend.Size = new System.Drawing.Size(274, 101);
            this.textBoxGroupSend.TabIndex = 11;
            // 
            // textBoxGroupChat
            // 
            this.textBoxGroupChat.AcceptsTab = true;
            this.textBoxGroupChat.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxGroupChat.Location = new System.Drawing.Point(6, 15);
            this.textBoxGroupChat.MaxLength = 0;
            this.textBoxGroupChat.Multiline = true;
            this.textBoxGroupChat.Name = "textBoxGroupChat";
            this.textBoxGroupChat.ReadOnly = true;
            this.textBoxGroupChat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxGroupChat.Size = new System.Drawing.Size(329, 239);
            this.textBoxGroupChat.TabIndex = 10;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage4.Controls.Add(this.groupBox9);
            this.tabPage4.Controls.Add(this.groupBox6);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(548, 383);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "讨论组";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.buttonDiscussSend);
            this.groupBox9.Controls.Add(this.textBoxDiscussSend);
            this.groupBox9.Controls.Add(this.textBoxDiscussChat);
            this.groupBox9.Location = new System.Drawing.Point(201, 6);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(341, 370);
            this.groupBox9.TabIndex = 16;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "聊天信息";
            // 
            // buttonDiscussSend
            // 
            this.buttonDiscussSend.Enabled = false;
            this.buttonDiscussSend.Location = new System.Drawing.Point(286, 260);
            this.buttonDiscussSend.Name = "buttonDiscussSend";
            this.buttonDiscussSend.Size = new System.Drawing.Size(47, 101);
            this.buttonDiscussSend.TabIndex = 12;
            this.buttonDiscussSend.Text = "发送";
            this.buttonDiscussSend.UseVisualStyleBackColor = true;
            this.buttonDiscussSend.Click += new System.EventHandler(this.buttonDiscussSend_Click);
            // 
            // textBoxDiscussSend
            // 
            this.textBoxDiscussSend.AcceptsTab = true;
            this.textBoxDiscussSend.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxDiscussSend.Location = new System.Drawing.Point(6, 260);
            this.textBoxDiscussSend.Multiline = true;
            this.textBoxDiscussSend.Name = "textBoxDiscussSend";
            this.textBoxDiscussSend.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDiscussSend.Size = new System.Drawing.Size(274, 101);
            this.textBoxDiscussSend.TabIndex = 11;
            // 
            // textBoxDiscussChat
            // 
            this.textBoxDiscussChat.AcceptsTab = true;
            this.textBoxDiscussChat.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxDiscussChat.Location = new System.Drawing.Point(6, 15);
            this.textBoxDiscussChat.MaxLength = 0;
            this.textBoxDiscussChat.Multiline = true;
            this.textBoxDiscussChat.Name = "textBoxDiscussChat";
            this.textBoxDiscussChat.ReadOnly = true;
            this.textBoxDiscussChat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDiscussChat.Size = new System.Drawing.Size(329, 239);
            this.textBoxDiscussChat.TabIndex = 10;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.listBoxDiscuss);
            this.groupBox6.Location = new System.Drawing.Point(6, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(189, 370);
            this.groupBox6.TabIndex = 14;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "讨论组列表";
            // 
            // listBoxDiscuss
            // 
            this.listBoxDiscuss.FormattingEnabled = true;
            this.listBoxDiscuss.ItemHeight = 12;
            this.listBoxDiscuss.Location = new System.Drawing.Point(6, 21);
            this.listBoxDiscuss.Name = "listBoxDiscuss";
            this.listBoxDiscuss.Size = new System.Drawing.Size(177, 340);
            this.listBoxDiscuss.TabIndex = 9;
            this.listBoxDiscuss.SelectedIndexChanged += new System.EventHandler(this.listBoxDiscuss_SelectedIndexChanged);
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(548, 383);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "系统设置";
            // 
            // tabPage6
            // 
            this.tabPage6.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage6.Controls.Add(this.groupBox2);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(548, 383);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "系统日志";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 424);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(580, 22);
            this.statusStrip1.TabIndex = 17;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // groupBox8
            // 
            this.groupBox8.Location = new System.Drawing.Point(231, 6);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(311, 371);
            this.groupBox8.TabIndex = 12;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "RuiRui QQ需要您的资助";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 446);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(596, 485);
            this.MinimumSize = new System.Drawing.Size(596, 485);
            this.Name = "MainForm";
            this.Text = "RuiRui QQ Robot";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQRCode)).EndInit();
            this.groupbox1.ResumeLayout(false);
            this.groupbox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.PictureBox pictureBoxQRCode;
        private System.Windows.Forms.GroupBox groupbox1;
        public System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.ListBox listBoxFriend;
        private System.Windows.Forms.GroupBox groupBox4;
        public System.Windows.Forms.ListBox listBoxGroup;
        public System.Windows.Forms.TextBox textBoxLog;
        internal System.Windows.Forms.Button buttonLogIn;
        public System.Windows.Forms.Label labelQRStatu;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.GroupBox groupBox6;
        public System.Windows.Forms.ListBox listBoxDiscuss;
        private System.Windows.Forms.GroupBox groupBox7;
        internal System.Windows.Forms.Button buttonFriendSend;
        private System.Windows.Forms.TextBox textBoxFriendSend;
        private System.Windows.Forms.TextBox textBoxFriendChat;
        private System.Windows.Forms.GroupBox groupBox5;
        internal System.Windows.Forms.Button buttonGroupSend;
        private System.Windows.Forms.TextBox textBoxGroupSend;
        private System.Windows.Forms.TextBox textBoxGroupChat;
        private System.Windows.Forms.GroupBox groupBox9;
        internal System.Windows.Forms.Button buttonDiscussSend;
        private System.Windows.Forms.TextBox textBoxDiscussSend;
        private System.Windows.Forms.TextBox textBoxDiscussChat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox8;
    }
}

