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
            this.buttonLogIn = new System.Windows.Forms.Button();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxCAPTCHA = new System.Windows.Forms.TextBox();
            this.pictureBoxCAPTCHA = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCAPTCHA)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonLogIn
            // 
            this.buttonLogIn.Location = new System.Drawing.Point(74, 86);
            this.buttonLogIn.Name = "buttonLogIn";
            this.buttonLogIn.Size = new System.Drawing.Size(75, 23);
            this.buttonLogIn.TabIndex = 0;
            this.buttonLogIn.Text = "登录";
            this.buttonLogIn.UseVisualStyleBackColor = true;
            this.buttonLogIn.Click += new System.EventHandler(this.buttonLogIn_Click);
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(60, 29);
            this.textBoxID.MaxLength = 50;
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(136, 21);
            this.textBoxID.TabIndex = 1;
            this.textBoxID.LostFocus += new System.EventHandler(this.textBoxID_LostFocus);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(60, 59);
            this.textBoxPassword.MaxLength = 30;
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(136, 21);
            this.textBoxPassword.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "账号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "密码";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "验证码";
            this.label3.Visible = false;
            // 
            // textBoxCAPTCHA
            // 
            this.textBoxCAPTCHA.Location = new System.Drawing.Point(14, 156);
            this.textBoxCAPTCHA.Name = "textBoxCAPTCHA";
            this.textBoxCAPTCHA.Size = new System.Drawing.Size(68, 21);
            this.textBoxCAPTCHA.TabIndex = 6;
            this.textBoxCAPTCHA.Visible = false;
            // 
            // pictureBoxCAPTCHA
            // 
            this.pictureBoxCAPTCHA.Location = new System.Drawing.Point(88, 127);
            this.pictureBoxCAPTCHA.Name = "pictureBoxCAPTCHA";
            this.pictureBoxCAPTCHA.Size = new System.Drawing.Size(114, 50);
            this.pictureBoxCAPTCHA.TabIndex = 7;
            this.pictureBoxCAPTCHA.TabStop = false;
            this.pictureBoxCAPTCHA.Visible = false;
            // 
            // FormLogin
            // 
            this.AcceptButton = this.buttonLogIn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 182);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBoxCAPTCHA);
            this.Controls.Add(this.textBoxCAPTCHA);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.buttonLogIn);
            this.MaximumSize = new System.Drawing.Size(230, 220);
            this.MinimumSize = new System.Drawing.Size(230, 160);
            this.Name = "FormLogin";
            this.Text = "登录";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCAPTCHA)).EndInit();
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
    }
}

