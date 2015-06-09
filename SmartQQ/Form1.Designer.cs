namespace SmartQQ
{
    partial class FormLogin
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
            this.buttonLogIn = new System.Windows.Forms.Button();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxCAPTCHA = new System.Windows.Forms.TextBox();
            this.pictureBoxCAPTCHA = new System.Windows.Forms.PictureBox();
            this.timerHeart = new System.Windows.Forms.Timer(this.components);
            this.timerTimeOut = new System.Windows.Forms.Timer(this.components);
            this.groupbox1 = new System.Windows.Forms.GroupBox();
            this.buttonLogout = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCAPTCHA)).BeginInit();
            this.groupbox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonLogIn
            // 
            this.buttonLogIn.Location = new System.Drawing.Point(20, 70);
            this.buttonLogIn.Name = "buttonLogIn";
            this.buttonLogIn.Size = new System.Drawing.Size(75, 23);
            this.buttonLogIn.TabIndex = 0;
            this.buttonLogIn.Text = "登录";
            this.buttonLogIn.UseVisualStyleBackColor = true;
            this.buttonLogIn.Click += new System.EventHandler(this.buttonLogIn_Click);
            // 
            // textBoxID
            // 
            this.textBoxID.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxID.Location = new System.Drawing.Point(41, 14);
            this.textBoxID.MaxLength = 50;
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(136, 21);
            this.textBoxID.TabIndex = 1;
            this.textBoxID.Text = "3286028350";
            this.textBoxID.LostFocus += new System.EventHandler(this.textBoxID_LostFocus);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(41, 43);
            this.textBoxPassword.MaxLength = 30;
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(136, 21);
            this.textBoxPassword.TabIndex = 2;
            this.textBoxPassword.Text = "qwgg9654";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "账号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "密码";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "验证码";
            this.label3.Visible = false;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // textBoxCAPTCHA
            // 
            this.textBoxCAPTCHA.Location = new System.Drawing.Point(8, 142);
            this.textBoxCAPTCHA.MaxLength = 4;
            this.textBoxCAPTCHA.Name = "textBoxCAPTCHA";
            this.textBoxCAPTCHA.Size = new System.Drawing.Size(68, 21);
            this.textBoxCAPTCHA.TabIndex = 6;
            this.textBoxCAPTCHA.Visible = false;
            // 
            // pictureBoxCAPTCHA
            // 
            this.pictureBoxCAPTCHA.Location = new System.Drawing.Point(86, 113);
            this.pictureBoxCAPTCHA.Name = "pictureBoxCAPTCHA";
            this.pictureBoxCAPTCHA.Size = new System.Drawing.Size(114, 50);
            this.pictureBoxCAPTCHA.TabIndex = 7;
            this.pictureBoxCAPTCHA.TabStop = false;
            this.pictureBoxCAPTCHA.Visible = false;
            this.pictureBoxCAPTCHA.Click += new System.EventHandler(this.pictureBoxCAPTCHA_Click);
            // 
            // timerHeart
            // 
            this.timerHeart.Interval = 1000;
            this.timerHeart.Tick += new System.EventHandler(this.timerHeart_Tick);
            // 
            // timerTimeOut
            // 
            this.timerTimeOut.Interval = 60000;
            this.timerTimeOut.Tick += new System.EventHandler(this.timerTimeOut_Tick);
            // 
            // groupbox1
            // 
            this.groupbox1.Controls.Add(this.buttonLogout);
            this.groupbox1.Controls.Add(this.label1);
            this.groupbox1.Controls.Add(this.textBoxCAPTCHA);
            this.groupbox1.Controls.Add(this.pictureBoxCAPTCHA);
            this.groupbox1.Controls.Add(this.textBoxID);
            this.groupbox1.Controls.Add(this.label2);
            this.groupbox1.Controls.Add(this.label3);
            this.groupbox1.Controls.Add(this.textBoxPassword);
            this.groupbox1.Controls.Add(this.buttonLogIn);
            this.groupbox1.Location = new System.Drawing.Point(12, 12);
            this.groupbox1.Name = "groupbox1";
            this.groupbox1.Size = new System.Drawing.Size(208, 173);
            this.groupbox1.TabIndex = 8;
            this.groupbox1.TabStop = false;
            this.groupbox1.Text = "登录";
            // 
            // buttonLogout
            // 
            this.buttonLogout.Enabled = false;
            this.buttonLogout.Location = new System.Drawing.Point(102, 70);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(75, 23);
            this.buttonLogout.TabIndex = 8;
            this.buttonLogout.Text = "注销";
            this.buttonLogout.UseVisualStyleBackColor = true;
            this.buttonLogout.Click += new System.EventHandler(this.buttonLogout_Click);
            // 
            // listBoxLog
            // 
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.ItemHeight = 12;
            this.listBoxLog.Location = new System.Drawing.Point(8, 20);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(192, 244);
            this.listBoxLog.TabIndex = 9;
            // 
            // textBoxResiveMessage
            // 
            this.textBoxResiveMessage.AcceptsTab = true;
            this.textBoxResiveMessage.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxResiveMessage.Location = new System.Drawing.Point(6, 17);
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
            this.groupBox2.Location = new System.Drawing.Point(12, 191);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(208, 270);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "系统日志";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.listBoxFriend);
            this.groupBox3.Location = new System.Drawing.Point(226, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(264, 270);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "好友列表";
            // 
            // listBoxFriend
            // 
            this.listBoxFriend.ItemHeight = 12;
            this.listBoxFriend.Location = new System.Drawing.Point(10, 20);
            this.listBoxFriend.Name = "listBoxFriend";
            this.listBoxFriend.Size = new System.Drawing.Size(248, 244);
            this.listBoxFriend.TabIndex = 9;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.listBoxGroup);
            this.groupBox4.Location = new System.Drawing.Point(226, 288);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(264, 173);
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
            this.listBoxGroup.Size = new System.Drawing.Size(252, 148);
            this.listBoxGroup.TabIndex = 9;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.buttonSend);
            this.groupBox5.Controls.Add(this.textBoxSendMessage);
            this.groupBox5.Controls.Add(this.textBoxResiveMessage);
            this.groupBox5.Location = new System.Drawing.Point(496, 12);
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
            // 
            // textBoxSendMessage
            // 
            this.textBoxSendMessage.AcceptsTab = true;
            this.textBoxSendMessage.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxSendMessage.Location = new System.Drawing.Point(0, 356);
            this.textBoxSendMessage.Multiline = true;
            this.textBoxSendMessage.Name = "textBoxSendMessage";
            this.textBoxSendMessage.ReadOnly = true;
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
            this.textBoxLog.Size = new System.Drawing.Size(810, 32);
            this.textBoxLog.TabIndex = 15;
            // 
            // FormLogin
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 502);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupbox1);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(774, 511);
            this.Name = "FormLogin";
            this.Text = "Smart QQ Robot";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCAPTCHA)).EndInit();
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

        private System.Windows.Forms.Button buttonLogIn;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxCAPTCHA;
        private System.Windows.Forms.PictureBox pictureBoxCAPTCHA;
        private System.Windows.Forms.Timer timerHeart;
        private System.Windows.Forms.Timer timerTimeOut;
        private System.Windows.Forms.GroupBox groupbox1;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.TextBox textBoxResiveMessage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox listBoxFriend;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox listBoxGroup;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxSendMessage;
        private System.Windows.Forms.Button buttonLogout;
        private System.Windows.Forms.TextBox textBoxLog;
    }
}

