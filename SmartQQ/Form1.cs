using Jurassic;
using Jurassic.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
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
        String p_skey, MyUin, skey, p_uin, ptwebqq, vfwebqq, psessionid, hash;
        public String HeartPackdata;
        int ClientID = 1659243;
        JsonGroupModel group;
        JsonFriendModel user;
        bool IsGroupSelent = false, IsFriendSelent = false;
        bool DoNotChangeSelentGroupOrPeople = false;
        bool StopSendingHeartPack = false;
        string StudyPassword = "";
        string DicServer = "";
        bool DisableStudy = false;
        int AmountOfRunningPosting = 0;
        StreamReader streamreader;
        struct GroupMember
        {
            public String gid;
            public JsonGroupMemberModel Menber;
        };
        GroupMember[] groupmember = new GroupMember[100];
        struct FriendInf
        {
            public String uin;
            public JsonFriendInfModel Inf;
        };
        FriendInf[] friendinf = new FriendInf[1000];
        int friendinfMaxIndex = 0;
        private int MsgId = 76523245;
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

            string temp = HttpGet(url);

            //二次登录准备
            temp = temp.Replace("ptui_checkVC(", "");
            temp = temp.Replace(");", "");
            temp = temp.Replace("'", "");
            string[] tmp = temp.Split(',');
            url = tmp[2];
            if(url == "")
            {
                MessageBox.Show("登录失败，请重试");
                if (CAPTCHA) GetCaptcha();
                return;
            }

            HttpGet(url);

            Uri uri = new Uri("http://web2.qq.com/");
            ptwebqq = cookies.GetCookies(uri)["ptwebqq"].Value;
            p_skey = cookies.GetCookies(uri)["p_skey"].Value;
            MyUin = cookies.GetCookies(uri)["uin"].Value;
            skey = cookies.GetCookies(uri)["skey"].Value;
            p_uin = cookies.GetCookies(uri)["p_uin"].Value;

            //二次登录
            url = "http://d.web2.qq.com/channel/login2";
            url1 = string.Format("r={{\"ptwebqq\":\"{0}\",\"clientid\":{1},\"psessionid\":\"\",\"status\":\"online\"}}", this.ptwebqq, this.ClientID);
            String dat = HttpPost(url, "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2", url1, Encoding.UTF8, true);

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

            listBoxLog.Items.Insert(0,"账号" + textBoxID.Text + "登录成功");
            timerHeart.Enabled = true;
            timerHeart.Start();

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
            string MessageFromUin="";            
            textBoxLog.Text = temp;
            if (temp == "{\"retcode\":121,\"t\":\"0\"}\r\n")
            {
                ReLogin();                
                MessageBox.Show("账号在其他地点登录，被迫退出");
                return;
            }
            else if (temp == "{\"retcode\":102,\"errmsg\":\"\"}\r\n" || temp == "{\"retcode\":0,\"result\":\"ok\"}\r\n")
            {
                return;
            }
            else if (temp == "{\"retcode\":108,\"errmsg\":\"\"}\r\n")
            {
                return;
            }
            listBoxLog.Items.Insert(0,temp);
            JsonHeartPackResponse result = (JsonHeartPackResponse)JsonConvert.DeserializeObject(temp, typeof(JsonHeartPackResponse));
            for (int i = 0; i < result.result.Count; i++ )
            {
                if (result.result[i].poll_type == "buddies_status_change")
                {
                    for (int j = 0; j < user.result.info.Count; j++)
                        if (user.result.info[j].uin == result.result[i].value.uin)
                        {
                            listBoxLog.Items.Insert(0,user.result.info[j].nick + "  " + result.result[i].value.status);
                            break;
                        }
                            
                }
                else if (result.result[i].poll_type == "kick_message")
                {
                    ReLogin();
                    MessageBox.Show(result.result[i].value.reason);
                    return;
                }
                else if (result.result[i].poll_type == "message")
                {
                    String message = result.result[i].value.content[1].ToString();
                    message.Replace("\\\\n", Environment.NewLine);
                    int j;                    
                    for (j = 0; j < user.result.info.Count; j++)
                        if (user.result.info[j].uin == result.result[i].value.from_uin)
                        {
                            textBoxResiveMessage.Text += (user.result.info[j].nick + "  " + GetRealQQ(user.result.info[j].uin) + Environment.NewLine + result.result[i].value.content[1].ToString() + Environment.NewLine + Environment.NewLine);
                            textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
                            textBoxResiveMessage.ScrollToCaret();
                            break;
                        }
                    if(j == user.result.info.Count)
                    {
                        getFrienf();
                        for (j = 0; j < user.result.info.Count; j++)
                            if (user.result.info[j].uin == result.result[i].value.from_uin)
                            {
                                textBoxResiveMessage.Text += (user.result.info[j].nick + "  " + GetRealQQ(user.result.info[j].uin) + Environment.NewLine + result.result[i].value.content[1].ToString() + Environment.NewLine + Environment.NewLine);
                                textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
                                textBoxResiveMessage.ScrollToCaret();
                                break;
                            }
                    }
                    ActionWhenResivedMessage(result.result[i].value.from_uin, message);
                }
                else if (result.result[i].poll_type == "group_message")
                {
                    String message = result.result[i].value.content[1].ToString();
                    message.Replace("\\\\n", Environment.NewLine);
                    string gid;
                    gid = result.result[i].value.from_uin;                   
                    for (int j = 0; j < group.result.gnamelist.Count; j++)
                        if (group.result.gnamelist[j].gid == gid)
                        {
                            GName = group.result.gnamelist[j].name;
                            break;
                        }
                    for (int j = 0; ;j++ )
                    {
                        if (groupmember[j].gid == gid)
                        {
                            for (int k = 0; k < groupmember[j].Menber.result.minfo.Count; k++)
                                if (groupmember[j].Menber.result.minfo[k].uin == result.result[i].value.send_uin)
                                {
                                    MessageFromUin = GetRealQQ(groupmember[j].Menber.result.minfo[k].uin);
                                    textBoxResiveMessage.Text += (GName + "   " + groupmember[j].Menber.result.minfo[k].nick + "  " + MessageFromUin + Environment.NewLine + result.result[i].value.content[1].ToString() + Environment.NewLine + Environment.NewLine);
                                    textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
                                    textBoxResiveMessage.ScrollToCaret();
                                    break;
                                }
                            break;
                        }
                    }
                    ActionWhenResivedGroupMessage(gid, message, MessageFromUin);
                    

                }
                textBoxLog.Text = temp;
            }               
        }
        private string[] Answer(string message, string QQNum)
        {
            string[] MessageToSend = new string[20];
            for (int i = 0; i < 20; i++)
                MessageToSend[i] = "";
            bool MsgSendFlag = false;
            if (message.Contains("学习"))
            {
                string[] tmp = message.Split('^');
                if (tmp[0].Equals("学习") && tmp.Length == 3)
                {
                    string result = AIStudy(tmp[1], tmp[2], QQNum);
                    if (result.Equals("Success"))
                    {
                        MessageToSend[0] = "嗯嗯～小睿睿记住了～～" + Environment.NewLine + "主人说 " + tmp[1] + " 时，小睿睿应该回答 " + tmp[2];
                    }
                    else if (result.Equals("Already"))
                    {
                        MessageToSend[0] = "小睿睿知道了啦～" + Environment.NewLine + "主人说 " + tmp[1] + " 时，小睿睿应该回答 " + tmp[2];
                    }
                    else if (result.Equals("DisableStudy"))
                    {
                        MessageToSend[0] = "当前学习功能未开启";
                    }
                    else
                    {
                        MessageToSend[0] = "小睿睿出错了，也许主人卖个萌就好了～～";
                    }
                    return MessageToSend;
                }
                
            }
            MessageToSend[0] = AIGet(message, QQNum);
            if (!MessageToSend[0].Equals(""))
            {
                return MessageToSend;
            }
            string[] tmp1 = message.Split("@#$(),，.。:：;；“”～~！!#（）%？?》《、· \r\n\"啊么吧呀恩嗯了呢很".ToCharArray());
            int j = 0;
            bool RepeatFlag = false;
            for (int i = 0; i < tmp1.Length && i < 10; i++)
            {
                for (int k = 0; k < i; k++)
                    if (tmp1[k].Equals(tmp1[i]))
                        RepeatFlag = true;
                if(RepeatFlag)
                {
                    RepeatFlag = false;
                    continue;
                }
                if (!tmp1[i].Equals(""))
                {
                    MessageToSend[j] = AIGet(tmp1[i], QQNum);
                    j++;
                    MsgSendFlag = true;
                }
            }

            if (!MsgSendFlag)
            {
                string XiaoHuangJiMsg = GetXiaoHuangJi(message);
                if (XiaoHuangJiMsg.Length > 1)
                {
                    MessageToSend[0] = "隔壁小黄鸡说：" + XiaoHuangJiMsg;
                    return MessageToSend;
                }
                else
                {
                    return null;
                }
            }
            return MessageToSend;
        }
        private void ActionWhenResivedGroupMessage(string gid, string message, string uin)
        {
            string[] MessageToSendArray = Answer(message, GetRealQQ(uin));
            string MessageToSend = "";
            for (int i = 0; i < 10; i++)
            {
                if (!MessageToSendArray[i].Equals(""))
                {
                    MessageToSend += MessageToSendArray[i] + Environment.NewLine;
                    MessageToSendArray[i] = "";
                }
            }
            SendMessageToGroup(gid, MessageToSend);
        }
        private void ActionWhenResivedMessage(string uin, string message)
        {
            string[] MessageToSendArray = Answer(message, GetRealQQ(uin));
            string MessageToSend = "";
            for (int i = 0; i < 10; i++)
            {
                if (!MessageToSendArray[i].Equals(""))
                {
                    MessageToSend += MessageToSendArray[i] + Environment.NewLine;
                    MessageToSendArray[i] = "";
                }
            }
            if (MessageToSend.Equals(""))
            {
                int i;
                string SenderName = "";
                string Gender = "";
                for (i = 0; i <= friendinfMaxIndex; i++)
                    if (friendinf[i].uin == uin)
                    {
                        SenderName = friendinf[i].Inf.result.nick;
                        Gender = friendinf[i].Inf.result.gender;
                        break;
                    }
                if (i > friendinfMaxIndex)
                {
                    getFrienf();
                    for (i = 0; i <= friendinfMaxIndex; i++)
                        if (friendinf[i].uin == uin)
                        {
                            SenderName = friendinf[i].Inf.result.nick;
                            Gender = friendinf[i].Inf.result.gender;
                            break;
                        }
                }
                if (Gender == "female")
                    Gender = "姐姐 ";
                else if (Gender == "male")
                    Gender = "哥哥 ";
                else
                    Gender = " ";
                SendMessageToFriend(uin, SenderName + Gender + "～ 小睿睿听不懂你在说什么呢。。。教教我吧～～" + Environment.NewLine + "格式 学习^语句^设定的回复");
            }
            else SendMessageToFriend(uin, MessageToSend);

        }
        private string AIGet(string message, string QQNum)
        {
            String url = DicServer+"gettalk.php?source=" + message + "&qqnum=" + QQNum;
            string temp = HttpGet(url);
            if (temp.Contains("None"))
                temp = "";
            return temp;
        }

        private string AIStudy(string source, string aim, string QQNum)
        {
            listBoxLog.Items.Insert(0,"学习 " + source + " " + aim);
            if (DisableStudy)
            {
                return "DisableStudy";
            }
            String url = DicServer+"addtalk.php?password=" + StudyPassword + "&source=" + source + "&aim=" + aim + "&qqnum=" + QQNum;
            string temp = HttpGet(url);
            return temp;
        }
        
        public void getFrienf()
        {
            String url = "http://s.web2.qq.com/api/get_user_friends2";
            String sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", vfwebqq, this.hash);
            String dat = HttpPost(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1", sendData, Encoding.UTF8, true);
            textBoxLog.Text = dat;

            user = (JsonFriendModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendModel));
            listBoxFriend.Items.Clear();
            for(int i=0;i<user.result.info.Count;i++)
            {
                string gender;
                friendinf[i].uin = user.result.info[i].uin;
                friendinf[i].Inf = GetFriendInf(user.result.info[i].uin);
                if (friendinfMaxIndex < i)
                    friendinfMaxIndex = i;
                if (friendinf[i].Inf.result.gender.Equals("female"))
                    gender = "妹纸";
                else if (friendinf[i].Inf.result.gender.Equals("male"))
                    gender = "汉子";
                else gender = "未知";
                listBoxFriend.Items.Add(user.result.info[i].uin + ":" + GetRealQQ(user.result.info[i].uin) + ":" + user.result.info[i].nick + ":" + gender);
            }
        }
        public JsonGroupMemberModel GetGroupMenber(string gcode)
        {
            String url = "http://s.web2.qq.com/api/get_group_info_ext2?gcode=" + gcode + "&vfwebqq=" + vfwebqq + "&t=" + GetTimeStamp();
            string dat = HttpGet(url);

            JsonGroupMemberModel ans = (JsonGroupMemberModel)JsonConvert.DeserializeObject(dat, typeof(JsonGroupMemberModel));
            return ans;
        }
        public void getGroup()
        {
            String url = "http://s.web2.qq.com/api/get_group_name_list_mask2";
            String sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", this.vfwebqq, this.hash);
            String dat = HttpPost(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1", sendData, Encoding.UTF8, true);
            textBoxLog.Text = dat;

            group = (JsonGroupModel)JsonConvert.DeserializeObject(dat, typeof(JsonGroupModel));
            listBoxGroup.Items.Clear();
            for (int i = 0; i < group.result.gnamelist.Count; i++)
            {
                listBoxGroup.Items.Add(group.result.gnamelist[i].gid + ":" + group.result.gnamelist[i].name);
                groupmember[i].gid = group.result.gnamelist[i].gid;
                groupmember[i].Menber = GetGroupMenber(group.result.gnamelist[i].code);               
            }
        }
        public JsonFriendInfModel GetFriendInf(string uin)
        {        
            String url = "http://s.web2.qq.com/api/get_friend_info2?tuin=" + uin + "&vfwebqq=" + vfwebqq +"&clientid="+ClientID+"&psessionid="+psessionid+ "&t=" + GetTimeStamp();

            string dat = HttpGet(url);
            JsonFriendInfModel ans = (JsonFriendInfModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendInfModel));
            return ans;
        }
        public string GetXiaoHuangJi(string msg)
        {
            string url = "http://www.xiaohuangji.com/ajax.php";
            string postdata = "para=" + HttpUtility.UrlEncode(msg);
            string MsgGet = HttpPost(url, "http://www.xiaohuangji.com/", postdata, Encoding.UTF8, false);
            return MsgGet;
        }
        private void textBoxID_LostFocus(object sender, EventArgs e)
        {
            String str1 = "https://ssl.ptlogin2.qq.com/check?pt_tea=1&uin=";
            String str2 = "&appid=501004106&js_ver=10121&js_type=0&login_sig=&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html&r=0.4053995015565306";
            String url = str1 + textBoxID.Text + str2;

            string dat = HttpGet(url);

            dat = dat.Replace("ptui_checkVC(", "");
            dat = dat.Replace(");", "");
            dat = dat.Replace("'", "");
            string[] tmp = dat.Split(',');

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
            res.Close();
        }
        String GetRealQQ(string uin)
        {

            String url = "http://s.web2.qq.com/api/get_friend_uin2?tuin=" + uin + "&type=1&vfwebqq=" + vfwebqq + "&t=" + GetTimeStamp();

            string dat = HttpGet(url);

            if (dat == "{\"retcode\":100101}")
            {
                return GetRealQQ(uin);
            }
            dat = dat.Replace("{\"retcode\":0,\"result\":{\"uiuin\":\"\",\"account\":", "");
            dat = dat.Replace("}", "");
            dat = dat.Replace("\"uin\":", "");
            dat = dat.Replace("\r", "");
            dat = dat.Replace("\n", "");
            string[] tmp = dat.Split(',');

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
        public string HttpGet(string url)
        {
            string dat;
            req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cookies;
            req.Referer = "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2";
            res = (HttpWebResponse)req.GetResponse();
            reader = new StreamReader(res.GetResponseStream());

            dat = reader.ReadToEnd();
            res.Close();
            req.Abort();
            textBoxLog.Text = dat;
            listBoxLog.Items.Insert(0,dat);
            return dat;
        }
        //http://www.itokit.com/2012/0721/74607.html
        public string HttpPost(string url, string Referer, string data, Encoding encode, bool SaveCookie)
        {
            if(AmountOfRunningPosting == 0)
                System.GC.Collect();
            AmountOfRunningPosting++;
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

            Stream stream = req.GetRequestStream();
            stream.Write(mybyte, 0, mybyte.Length);
            

            HttpWebResponse hwr = req.GetResponse() as HttpWebResponse;
            stream.Close();

            if (SaveCookie)
            {
                this.CookieCollection = hwr.Cookies;
                this.cookies.GetCookies(req.RequestUri);
            }
            StreamReader SR = new StreamReader(hwr.GetResponseStream(), encode);
            string dat = SR.ReadToEnd();
            hwr.Close();
            req.Abort();
            textBoxLog.Text = dat;
            listBoxLog.Items.Insert(0,dat);
            AmountOfRunningPosting--;
            return dat;
        }
        public void HeartPack()
        {
            System.GC.Collect();
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
            res.Close();
            req.Abort();
            HeartPackAction(temp);       
        }
        private void pictureBoxCAPTCHA_Click(object sender, EventArgs e)
        {
            GetCaptcha();
        }
        private void FormLogin_Load(object sender, EventArgs e)
        {
            bool NoFile = false;
            byte[] byData = new byte[1000];
            char[] charData = new char[1000];
            Control.CheckForIllegalCrossThreadCalls = false;
            System.Net.ServicePointManager.DefaultConnectionLimit = 500;
            try
            {
                FileStream file = new FileStream(Environment.CurrentDirectory + "\\RuiRuiRobot.conf", FileMode.Open);
                file.Seek(0, SeekOrigin.Begin);
                file.Read(byData, 0, 1000); 
                Decoder decoder = Encoding.Default.GetDecoder();
                decoder.GetChars(byData, 0, byData.Length, charData, 0);
                file.Close();
            }
            catch (System.IO.FileNotFoundException)
            {
                NoFile = true;
            }
            if(!NoFile)
            {
                string tmp = "";
                for (int i = 0; i < charData.Length; i++)
                    tmp += charData[i];
                tmp += '\0';
                //tmp.Replace(Environment.NewLine, "");
                //tmp.Replace(" ", "");
                JosnConfigFileModel dat = (JosnConfigFileModel)JsonConvert.DeserializeObject(tmp, typeof(JosnConfigFileModel));
                textBoxID.Text = dat.QQNum;
                textBoxPassword.Text = dat.QQPassword;
                StudyPassword = dat.DicPassword;
                DicServer = dat.DicServer;
            }
            else
            {
                DicServer = "http://smartqq.hxlxz.com/";
                DisableStudy = true; 
            }
               
        }
        public FormLogin()
        {
            InitializeComponent();
        }
        private void timerHeart_Tick(object sender, EventArgs e)
        {
            if(!StopSendingHeartPack)HeartPack();          
        }
        public void ReLogin()
        {
            timerHeart.Stop();
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
            listBoxLog.Items.Insert(0,"账号" + textBoxID.Text + "已登出");
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

        //http://www.cnblogs.com/lianmin/p/4257421.html
        /// 发送好友消息
        public bool SendMessageToFriend(string uid, string content)
        {
            this.MsgId++;
            try
            {
                string postData = "{\"to\":" + uid;
                postData += ",\"content\":\"[\\\"" + content.Replace(Environment.NewLine, "\\\\n");
                postData += "\\\",[\\\"font\\\",{\\\"name\\\":\\\"宋体\\\",\\\"size\\\":10,\\\"style\\\":[0,0,0],\\\"color\\\":\\\"000000\\\"}]]\",\"face\":585,\"clientid\":" + ClientID;
                postData += ",\"msg_id\":" + MsgId;
                postData += ",\"psessionid\":\"" + psessionid;
                postData += "\"}";

                string referer = "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2";
                string url = "http://d.web2.qq.com/channel/send_buddy_msg2";
                postData = "r=" + HttpUtility.UrlEncode(postData);

                string dat = HttpPost(url, referer, postData, Encoding.UTF8, false);

                dat = dat.Replace("{\"retcode\":", "");
                dat = dat.Replace("\"result\":\"", "");
                dat = dat.Replace("\"}", "");
                string[] tmp = dat.Split(',');               
                if (tmp[0] == "0" && tmp[0] == "ok")
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        /// 发送群消息
        public bool SendMessageToGroup(string gin, string content)
        {
            this.MsgId++;
            try
            {
                string postData = "{\"group_uin\":" + gin
                    + ",\"content\":\"[\\\"" + content.Replace(Environment.NewLine, "\\\\n")
                    + "\\\",[\\\"font\\\",{\\\"name\\\":\\\"宋体\\\",\\\"size\\\":13,\\\"style\\\":[0,0,0],\\\"color\\\":\\\"000000\\\"}]]\",\"face\":549,\"clientid\":" + ClientID
                    + ",\"msg_id\":" + MsgId
                    + ",\"psessionid\":\"" + psessionid
                    + "\"}";
                postData = "r=" + HttpUtility.UrlEncode(postData);
               
                string referer = "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2";
                string url = "http://d.web2.qq.com/channel/send_qun_msg2";

                string dat = HttpPost(url, referer, postData, Encoding.UTF8, false);

                dat = dat.Replace("{\"retcode\":", "");
                dat = dat.Replace("\"result\":\"", "");
                dat = dat.Replace("\"}", "");
                string[] tmp = dat.Split(',');
                if (tmp[0] == "0" && tmp[0] == "ok")
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (textBoxSendMessage.Text.Equals(""))
                return;
            if (!(IsFriendSelent || IsGroupSelent))
                return;
            StopSendingHeartPack = true;
            if(IsGroupSelent)
            {
                string GName = "";
                string[] tmp = listBoxGroup.SelectedItem.ToString().Split(':');
                SendMessageToGroup(tmp[0], textBoxSendMessage.Text); 

                for (int i = 0; i < group.result.gnamelist.Count; i++)
                    if (group.result.gnamelist[i].gid == tmp[0])
                        {
                            GName = group.result.gnamelist[i].name;
                            break;
                        }
                textBoxResiveMessage.Text += ("发送至   " + GName + Environment.NewLine + textBoxSendMessage.Text + Environment.NewLine + Environment.NewLine);
                textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
                textBoxResiveMessage.ScrollToCaret();
            }
            else if (IsFriendSelent)
            {
                string Nick = "";
                string[] tmp = listBoxFriend.SelectedItem.ToString().Split(':');
                SendMessageToFriend(tmp[0], textBoxSendMessage.Text);

                for (int i = 0; i < user.result.info.Count; i++)
                    if (user.result.info[i].uin == tmp[0])
                    {
                        Nick = user.result.info[i].nick;
                        break;
                    }
                textBoxResiveMessage.Text += ("发送至   " + Nick + "   " + GetRealQQ(tmp[0]) + Environment.NewLine + textBoxSendMessage.Text + Environment.NewLine + Environment.NewLine);
                textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
                textBoxResiveMessage.ScrollToCaret();
            }
            StopSendingHeartPack = false;
            textBoxSendMessage.Text = "";
        }

        private void listBoxFriend_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DoNotChangeSelentGroupOrPeople) return;
            IsGroupSelent = false;
            IsFriendSelent = true;
            DoNotChangeSelentGroupOrPeople = true;
            listBoxGroup.SelectedItem = (ListBox.SelectedObjectCollection)null;
            DoNotChangeSelentGroupOrPeople = false;
        }

        private void listBoxGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DoNotChangeSelentGroupOrPeople) return;
            IsGroupSelent = true;
            IsFriendSelent = false;
            DoNotChangeSelentGroupOrPeople = true;
            listBoxFriend.SelectedItem = (ListBox.SelectedObjectCollection)null;
            DoNotChangeSelentGroupOrPeople = false;
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
    public class JsonFriendInfModel
    {
        public int retcode;
        public paramResult result;
        public class paramResult
        {
            public paramBirthday birthday;
            public string occupation;
            public string phone;
            public string college;
            public int constel;
            public int blood;
            public string homepage;
            public int stat;
            public string city;
            public string personal;
            public string nick;
            public int shengxiao;
            public string email;
            public string province;
            public string gender;
            public string mobile;
            public class paramBirthday
            {
                public int month;
                public int year;
                public int day;
            }
        }
    }
    public class JsonGroupMemberModel
    {
        public int retcode;
        public paramResult result;
        public class paramResult
        {
            public List<paramMinfo> minfo;
            public class paramMinfo
            {
                public string nick;
                public string province;
                public string gender;
                public string uin;
                public string country;
                public string city;
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
    class JosnConfigFileModel
    {
        public String DicServer;
        public String DicPassword;
        public String QQNum;
        public String QQPassword;
    }
}
