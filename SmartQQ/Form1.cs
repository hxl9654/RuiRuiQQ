using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace SmartQQ
{

    public partial class FormLogin : Form
    {
        String ptvsession = "";
        bool CAPTCHA = false;
        string pin = string.Empty;
        String pt_uin = "";
        HttpWebRequest req = null;
        HttpWebResponse res = null;
        StreamReader reader = null;
        private HttpWebResponse response;
        public FormLogin()
        {
            InitializeComponent();
        }

        private void textBoxID_LostFocus(object sender, EventArgs e)
        {
            String str1 = "https://ssl.ptlogin2.qq.com/check?pt_tea=1&uin=";
            String str2 = "&appid=501004106&js_ver=10121&js_type=0&login_sig=&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html&r=0.4053995015565306";
            String tagUrl = str1 + textBoxID.Text + str2;

            req = (HttpWebRequest)WebRequest.Create(tagUrl);
            res = (HttpWebResponse)req.GetResponse();
            reader = new StreamReader(res.GetResponseStream());
            pin = reader.ReadToEnd();

            pin = pin.Replace("ptui_checkVC(", "");
            pin = pin.Replace(");", "");
            pin = pin.Replace("'", "");
            string[] tmp = pin.Split(',');

            pt_uin = tmp[2];

            if (tmp[0] == "1")
            {
                CAPTCHA = true;
                textBoxCAPTCHA.Text = "";

                this.Size = new Size(230, 220);
                String strimg1 = "https://ssl.captcha.qq.com/getimage?aid=501004106&r=0.005933324107900262&uin=";
                String strimg2 = "&cap_cd=";
                String strimg = strimg1 + textBoxID.Text + strimg2 + tmp[1];

                req = (HttpWebRequest)WebRequest.Create(strimg);
                req.CookieContainer = new CookieContainer();
                res = (HttpWebResponse)req.GetResponse();

                pictureBoxCAPTCHA.Image = Image.FromStream(res.GetResponseStream());
                pictureBoxCAPTCHA.Visible = true;
                textBoxCAPTCHA.Visible = true;
                label3.Visible = true;

                ptvsession = res.Cookies["verifysession"].Value;
            }
            else
            {
                CAPTCHA = false;
                this.Size = new Size(230, 160);
                pictureBoxCAPTCHA.Visible = false;
                textBoxCAPTCHA.Visible = false;
                label3.Visible = false;
                textBoxCAPTCHA.Text = tmp[1];
                ptvsession = tmp[3];
            }

        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            this.Size = new Size(230, 160);
        }

        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            if (textBoxID.Text.Length == 0)
            {
                MessageBox.Show("账号不能为空");
                return;
            }
            if (textBoxPassword.Text.Length == 0)
            {
                MessageBox.Show("密码不能为空");
                return;
            }
            if (textBoxCAPTCHA.Text.Length == 0)
            {
                MessageBox.Show("验证码不能为空");
                return;
            }
            String key = MD5.MD5_QQ_2_Encrypt(pt_uin, textBoxPassword.Text, textBoxCAPTCHA.Text);

            String url1 = "https://ssl.ptlogin2.qq.com/login?u=";
            String url2 = "&p=";
            String url3 = "&verifycode=";
            String url4 = "&webqq_type=10&remember_uin=1&login2qq=1&aid=501004106&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html";
            String url5 = "%3Flogin2qq%3D1%26webqq_type%3D10&h=1&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1";
            String url6 = "&dumy=&fp=loginerroralert&action=0-15-19190&mibao_css=m_webqq&t=1&g=1&js_type=0&js_ver=10123&";
            String url7 = "login_sig=&pt_randsalt=0&pt_vcode_v1=0&pt_verifysession_v1=";

            String url = url1 + textBoxID.Text + url2 + key + url3 + textBoxCAPTCHA.Text + url4 + url5 + url6 + url7 + ptvsession;

            MessageBox.Show(url);
            req = (HttpWebRequest)WebRequest.Create(url);
            res = (HttpWebResponse)req.GetResponse();
            reader = new StreamReader(res.GetResponseStream());
            pin = reader.ReadToEnd();

            MessageBox.Show(pin);
        }

    }
}
