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
using System.Security;
using Jurassic;
using Jurassic.Library;
using Newtonsoft.Json;
using System.Threading;
using System.Timers;
namespace SmartQQ
{

    public partial class FormLogin : Form
    {
        CookieContainer cookies = new CookieContainer();
        CookieCollection CookieCollection = new CookieCollection();
        CookieContainer CookieContainer = new CookieContainer();
        String ptvsession = "";
        bool CAPTCHA = false;
        string pin = string.Empty;
        String pt_uin = "";
        HttpWebRequest req = null;
        HttpWebResponse res = null;
        StreamReader reader = null;
        String CaptchaCode;
        String p_skey, uin, skey, p_uin, ptwebqq, vfwebqq, psessionid, hash;
        public String HeartPackdata;
        int ClientID = 94659243;
        JsonGroupModel group;
        JsonFriendModel user;
        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            if (textBoxID.Text.Length == 0)
            {
                MessageBox.Show("账号不能为空");
                return;
            }
            for (int i = 0; i < textBoxID.Text.Length; i++ )
            {
                if(textBoxID.Text.ToCharArray()[i] < '0' || textBoxID.Text.ToCharArray()[i] > '9')
                {
                    MessageBox.Show("账号只能为数字");
                    return;
                }
            }
            if (textBoxPassword.Text.Length == 0)
            {
                MessageBox.Show("密码不能为空");
                return;
            }
            if (textBoxCAPTCHA.Text.Length != 4)
            {
                MessageBox.Show("验证码错误");
                return;
            }

            //一次登录
            String tokentemp = System.Text.RegularExpressions.Regex.Replace(pt_uin, @"\\x", "");
            String token = HexString2Ascii(tokentemp);
            string key = EncodePassword(textBoxPassword.Text, token, textBoxCAPTCHA.Text.ToUpper());

            String url1 = "https://ssl.ptlogin2.qq.com/login?u=";
            String url2 = "&p=";
            String url3 = "&verifycode=";
            String url4 = "&webqq_type=10&remember_uin=1&login2qq=1&aid=501004106&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html";
            String url5 = "%3Flogin2qq%3D1%26webqq_type%3D10&h=1&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1";
            String url6 = "&dumy=&fp=loginerroralert&action=0-15-19190&mibao_css=m_webqq&t=1&g=1&js_type=0&js_ver=10123&";
            String url7 = "login_sig=&pt_randsalt=0&pt_vcode_v1=0&pt_verifysession_v1=";
            String url = url1 + textBoxID.Text + url2 + key + url3 + textBoxCAPTCHA.Text + url4 + url5 + url6 + url7 + ptvsession;

            req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cookies;
            res = (HttpWebResponse)req.GetResponse();
            reader = new StreamReader(res.GetResponseStream());

            String temp = reader.ReadToEnd();
            textBoxLog.Text = temp;
            //二次登录准备
            temp = temp.Replace("ptui_checkVC(", "");
            temp = temp.Replace(");", "");
            temp = temp.Replace("'", "");
            string[] tmp = temp.Split(',');
            url = tmp[2];
            if(url == "")
            {
                buttonLogIn_Click(this, EventArgs.Empty);
                return;
            }
            req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cookies;
            res = (HttpWebResponse)req.GetResponse();

            Uri uri = new Uri("http://web2.qq.com/");
            ptwebqq = cookies.GetCookies(uri)["ptwebqq"].Value;
            p_skey = cookies.GetCookies(uri)["p_skey"].Value;
            uin = cookies.GetCookies(uri)["uin"].Value;
            skey = cookies.GetCookies(uri)["skey"].Value;
            p_uin = cookies.GetCookies(uri)["p_uin"].Value;

            //二次登录
            url = "http://d.web2.qq.com/channel/login2";
            url1 = string.Format("r={{\"ptwebqq\":\"{0}\",\"clientid\":{1},\"psessionid\":\"\",\"status\":\"online\"}}", this.ptwebqq, this.ClientID);
            String dat = PostHtml(url, "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2", url1, Encoding.UTF8, true);

            textBoxLog.Text = dat;
            char[] t = new char[2];
            t[0]=':';
            t[1]=',';
            dat = dat.Replace("{", "");
            dat = dat.Replace("}", "");
            dat = dat.Replace("\"", "");
            tmp = dat.Split(t);

            vfwebqq = tmp[14];
            psessionid = tmp[16];

            hash = GetHash(textBoxID.Text,ptwebqq);
            getFrienf();
            getGroup();

            listBoxLog.Items.Add("账号" + textBoxID.Text + "登录成功");
            timerHeart.Enabled = true;
            timerTimeOut.Enabled = true;
            timerHeart.Start();
            timerTimeOut.Start();

            textBoxID.Enabled = false;
            textBoxPassword.Enabled = false;
            buttonSend.Enabled = true;
            buttonLogIn.Enabled = false;
            buttonLogout.Enabled = true;
            label3.Visible = false;
            pictureBoxCAPTCHA.Visible = false;
            textBoxCAPTCHA.Visible = false;
            textBoxCAPTCHA.Text = "";
        }
        private void HeartPackAction(string temp)
        {
            string GName="";
            if (temp == "{\"retcode\":121,\"t\":\"0\"}\r\n")
            {
                ReLogin();
                MessageBox.Show("账号在其他地点登录，被迫退出");
                return;
            }
            else if (temp == "{\"retcode\":102,\"errmsg\":\"\"}\r\n" || temp == "{\"retcode\":0,\"result\":\"ok\"}\r\n")
            {
                ReInitTimerTimeOur();
                return;
            }
            else if (temp == "{\"retcode\":108,\"errmsg\":\"\"}\r\n")
                return;
            JsonHeartPackResponse result = (JsonHeartPackResponse)JsonConvert.DeserializeObject(temp, typeof(JsonHeartPackResponse));
            for (int i = 0; i < result.result.Count; i++ )
            {
                if (result.result[i].poll_type == "buddies_status_change")
                {
                    for (int j = 0; j < user.result.info.Count; j++)
                        if (user.result.info[j].uin == result.result[i].value.uin)
                            listBoxLog.Items.Add(user.result.info[j].nick + "  " + result.result[i].value.status);
                }
                else if (result.result[i].poll_type == "kick_message")
                {
                    ReLogin();
                    MessageBox.Show(result.result[i].value.reason);
                    return;
                }
                else if (result.result[i].poll_type == "message")
                {
                    for (int j = 0; j < user.result.info.Count; j++)
                        if (user.result.info[j].uin == result.result[i].value.from_uin)
                        {
                            textBoxResiveMessage.Text += (user.result.info[j].nick + "  " + GetRealQQ(user.result.info[j].uin) + Environment.NewLine + result.result[i].value.content[1].ToString() + Environment.NewLine);
                        }
                }
                else if (result.result[i].poll_type == "group_message")
                {
                    for (int j = 0; j < group.result.gnamelist.Count; j++)
                        if (group.result.gnamelist[j].gid == result.result[i].value.from_uin)
                        {
                            GName = group.result.gnamelist[j].name;
                        }
                    for (int j = 0; j < user.result.info.Count; j++)
                        if (user.result.info[j].uin == result.result[i].value.send_uin)
                        {
                            textBoxResiveMessage.Text += (GName + "   " + user.result.info[j].nick + "  " + GetRealQQ(user.result.info[j].uin) + Environment.NewLine + result.result[i].value.content[1].ToString() + Environment.NewLine);
                        }
                }
                ReInitTimerTimeOur();
                textBoxLog.Text = temp;
            }               
        }
        public void getFrienf()
        {
            String url = "http://s.web2.qq.com/api/get_user_friends2";
            String sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", vfwebqq, this.hash);
            String dat = PostHtml(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1", sendData, Encoding.UTF8, true);
            textBoxLog.Text = dat;

            user = (JsonFriendModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendModel));
            listBoxFriend.Items.Clear();
            for(int i=0;i<user.result.info.Count;i++)
            {
                listBoxFriend.Items.Add(user.result.info[i].uin+ ":" + GetRealQQ(user.result.info[i].uin) + ":" + user.result.info[i].nick );
            }
        }
        
        public void getGroup()
        {
            String url = "http://s.web2.qq.com/api/get_group_name_list_mask2";
            String sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", this.vfwebqq, this.hash);
            String dat = PostHtml(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1", sendData, Encoding.UTF8, true);
            textBoxLog.Text = dat;

            group = (JsonGroupModel)JsonConvert.DeserializeObject(dat, typeof(JsonGroupModel));
            listBoxGroup.Items.Clear();
            for (int i = 0; i < group.result.gnamelist.Count; i++)
            {
                listBoxGroup.Items.Add(group.result.gnamelist[i].gid + ":" + group.result.gnamelist[i].name);
            }
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

            textBoxLog.Text = pin;
            pin = pin.Replace("ptui_checkVC(", "");
            pin = pin.Replace(");", "");
            pin = pin.Replace("'", "");
            string[] tmp = pin.Split(',');

            pt_uin = tmp[2];

            if (tmp[0] == "1")
            {
                CAPTCHA = true;

                CaptchaCode = tmp[1];
                GetCaptcha();

                pictureBoxCAPTCHA.Visible = true;
                textBoxCAPTCHA.Visible = true;
                label3.Visible = true;
            }
            else
            {
                CAPTCHA = false;
                pictureBoxCAPTCHA.Visible = false;
                textBoxCAPTCHA.Visible = false;
                label3.Visible = false;
                textBoxCAPTCHA.Text = tmp[1];
                ptvsession = tmp[3];
            }

        }
        public void GetCaptcha()
        {
            textBoxCAPTCHA.Text = "";

            String strimg1 = "https://ssl.captcha.qq.com/getimage?aid=501004106&r=0.005933324107900262&uin=";
            String strimg2 = "&cap_cd=";
            String strimg = strimg1 + textBoxID.Text + strimg2 + CaptchaCode;

            req = (HttpWebRequest)WebRequest.Create(strimg);
            req.CookieContainer = cookies;
            res = (HttpWebResponse)req.GetResponse();

            pictureBoxCAPTCHA.Image = Image.FromStream(res.GetResponseStream());

            ptvsession = res.Cookies["verifysession"].Value;

        }
        String GetRealQQ(string uin)
        {

            String url = "http://s.web2.qq.com/api/get_friend_uin2?tuin=" + uin + "&type=1&vfwebqq=" + vfwebqq + "&t=" + GetTimeStamp();
        
            req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cookies;
            req.Referer = "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2";
            res = (HttpWebResponse)req.GetResponse();
            reader = new StreamReader(res.GetResponseStream());

            pin = reader.ReadToEnd();

            textBoxLog.Text = pin;

            if (pin == "{\"retcode\":100101}")
            {
                return GetRealQQ(uin);
            }
            pin = pin.Replace("{\"retcode\":0,\"result\":{\"uiuin\":\"\",\"account\":","");
            pin = pin.Replace("}", "");
            pin = pin.Replace("\"uin\":", "");
            pin = pin.Replace("\r", "");
            pin = pin.Replace("\n", "");
            string[] tmp = pin.Split(',');

            return tmp[0];
        }
        //感谢QQBOT群（346167134） 杨小泡的热心帮助！
        internal static string HexString2Ascii(string hexString)
        {
            string res = String.Empty;
            for (int a = 0; a < hexString.Length; a = a + 2)
            {
                string Char2Convert = hexString.Substring(a, 2);
                int n = Convert.ToInt32(Char2Convert, 16);
                char c = (char)n;
                res += c.ToString();
            }
            return res;
        }
        private String GetHash(String no, String ptwebqq)
        {
            var scriptEngine = new Jurassic.ScriptEngine();
            scriptEngine.EnableDebugging = true;
            scriptEngine.SetGlobalValue("window", new WindowObject(scriptEngine));
            scriptEngine.ExecuteFile(System.AppDomain.CurrentDomain.BaseDirectory + "hash.js");
            var ret = scriptEngine.CallGlobalFunction<string>("friendsHash", no, ptwebqq, 0);
            return ret;
        }
        internal static string EncodePassword(string password, string token, string bits)
        {
            var scriptEngine = new Jurassic.ScriptEngine();
            scriptEngine.EnableDebugging = true;
            scriptEngine.SetGlobalValue("window", new WindowObject(scriptEngine));
            scriptEngine.ExecuteFile(System.AppDomain.CurrentDomain.BaseDirectory + "encode.js");
            var ret = scriptEngine.CallGlobalFunction<string>("getEncryption", password, token, bits, 0);
            return ret;
        }
        //http://www.itokit.com/2012/0721/74607.html
        public string PostHtml(string url, string Referer, string data, Encoding encode, bool SaveCookie)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.CookieContainer = this.cookies;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:30.0) Gecko/20100101 Firefox/30.0";
            req.Proxy = null;
            req.ProtocolVersion = HttpVersion.Version10;
            if (!string.IsNullOrEmpty(Referer))
                req.Referer = Referer;
            byte[] mybyte = Encoding.Default.GetBytes(data);
            req.ContentLength = mybyte.Length;
            using (Stream stream = req.GetRequestStream())
            {
                stream.Write(mybyte, 0, mybyte.Length);
            }
            using (HttpWebResponse hwr = req.GetResponse() as HttpWebResponse)
            {
                if (SaveCookie)
                {
                    this.CookieCollection = hwr.Cookies;
                    this.cookies.GetCookies(req.RequestUri);
                }
                using (StreamReader SR = new StreamReader(hwr.GetResponseStream(), encode))
                {
                    return SR.ReadToEnd();
                }
            }
        }
        public void HeartPack()
        {
            String url = "http://d.web2.qq.com/channel/poll2";
            String sendData1 = "r= {\"ptwebqq\":\"";
            String sendData2 = "\",\"clientid\":";
            String sendData3 = ",\"psessionid\":\"";
            String sendData4 = "\",\"key\":\"\"}";
            HeartPackdata = sendData1 + ptwebqq + sendData2 + ClientID.ToString() + sendData3 + psessionid + sendData4;

            Encoding encode = Encoding.UTF8;
            string Referer = "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2";

            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.CookieContainer = this.cookies;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:30.0) Gecko/20100101 Firefox/30.0";
            req.Proxy = null;
            req.ProtocolVersion = HttpVersion.Version10;
            if (!string.IsNullOrEmpty(Referer))
                req.Referer = Referer;

            req.BeginGetRequestStream(new AsyncCallback(RequestProceed), req);
        }
        private void RequestProceed(IAsyncResult asyncResult)
        {

            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            StreamWriter postDataWriter = new StreamWriter(request.EndGetRequestStream(asyncResult));
            postDataWriter.Write(HeartPackdata);
            postDataWriter.Close();
            request.BeginGetResponse(new AsyncCallback(ResponesProceed), request);
        }
        private void ResponesProceed(IAsyncResult ar)
        {
            StreamReader reader = null;
            HttpWebRequest req = ar.AsyncState as HttpWebRequest;
            HttpWebResponse res = req.GetResponse() as HttpWebResponse;
            reader = new StreamReader(res.GetResponseStream());
            String temp = reader.ReadToEnd();
           
            HeartPackAction(temp);       
        }

        private void label3_Click(object sender, EventArgs e)
        {
            GetCaptcha();
        }
        private void pictureBoxCAPTCHA_Click(object sender, EventArgs e)
        {
            GetCaptcha();
        }
        private void FormLogin_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        public FormLogin()
        {
            InitializeComponent();
        }
        private void ReInitTimerTimeOur()
        {
            timerTimeOut.Stop();
            timerTimeOut.Interval = 60000;
            timerTimeOut.Start();
        }
        private void timerHeart_Tick(object sender, EventArgs e)
        {
            int n = 0;
            n++;
            if (n == 7) getFrienf();
            else if (n == 14)
            {
                getGroup();
                n = 0;
            }
            HeartPack();          
        }
        private void timerTimeOut_Tick(object sender, EventArgs e)
        {

            ReLogin();
            MessageBox.Show("网络异常，请重新登录");
            
        }
        public void ReLogin()
        {
            timerHeart.Stop();
            timerTimeOut.Stop();
            listBoxFriend.Items.Clear();
            listBoxGroup.Items.Clear();
            textBoxID.Enabled = true;
            textBoxPassword.Enabled = true;
            buttonSend.Enabled = false;
            buttonLogIn.Enabled = true;
            buttonLogout.Enabled = false;
            label3.Visible = true;
            pictureBoxCAPTCHA.Visible = true;
            textBoxCAPTCHA.Visible = true;
            if (CAPTCHA) GetCaptcha();
            listBoxLog.Items.Add("账号" + textBoxID.Text + "已登出");
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            ReLogin();
        }
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string ret = string.Empty;
            ret = Convert.ToInt64(ts.TotalMilliseconds).ToString();

            return ret;
        }  
    }
    public class WindowObject : ObjectInstance
    {
        public WindowObject(ScriptEngine engine)
            : base(engine.Global)
        {
            this.PopulateFunctions();
        }
    }
    //http://www.cnblogs.com/lianmin/p/4237723.html (有较大修改）
    class JsonFriendModel
    {
        public int retcode ;
        public paramResult result;
        public class paramResult
        {
            /// 分组信息
            public List<paramCategories> categories;
            /// 好友汇总
            public List<paramFriends> friends;
            /// 好友信息
            public List<paramInfo> info;
            /// 备注
            public List<paramMarkNames> marknames;
            /// 分组
            public class paramCategories
            {
                public int index;
                public int sort;
                public string name;
            }
            /// 好友汇总 
            public class paramFriends
            {
                public int flag;
                public string uin;
                public int categories;
            }
            /// 好友信息
            public class paramInfo
            {
                public int face;
                public string nick;
                public string uin;
            }
            /// 备注 
            public class paramMarkNames
            {
                public string uin;
                public string markname;
            }
        }
    }
    class JsonGroupModel
    {
        public int retcode;
        public paramResult result;
        public class paramResult
        {
            public List<paramGnamelist> gnamelist;
            public class paramGnamelist
            {
                public string flag;
                public string gid;
                public string code;
                public string name;
            }
        }
    }
    class JsonHeartPackResponse
    {
        public int retcode;
        public List<paramResult> result;
        public class paramResult
        {
            public String poll_type;
            public paramValue value;
            public class paramValue
            {
                //收到消息
                public List<object> content;
                public string from_uin;
                //群消息有send_uin，为群号
                public string send_uin;

                //上线提示
                public string uin;
                public string status;
                //异地登录
                public string reason;
            }
        }
    } 
}
