using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
// *   This program is free software: you can redistribute it and/or modify
// *   it under the terms of the GNU General Public License as published by
// *   the Free Software Foundation, either version 3 of the License, or
// *   (at your option) any later version.
// *
// *   This program is distributed in the hope that it will be useful,
// *   but WITHOUT ANY WARRANTY; without even the implied warranty of
// *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// *   GNU General Public License for more details.
// *
// *   You should have received a copy of the GNU General Public License
// *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// *
// * @author     Xianglong He
// * @copyright  Copyright (c) 2015 Xianglong He. (http://tec.hxlxz.com)
// * @license    http://www.gnu.org/licenses/     GPL v3
// * @version    1.0
// * @discribe   RuiRuiQQRobot服务端
// * 本软件作者是何相龙，使用GPL v3许可证进行授权。
namespace SmartQQ
{
    public static class SmartQQ
    {
        //通信参数相关
        public static int MsgId;
        public static int ClientID;
        public static String ptvsession = "";
        public static String p_skey, MyUin, skey, p_uin, vfwebqq, hash;
        public static String ptwebqq, psessionid;
        public static String pt_uin = "";

        public static bool NeedCAPTCHA = false;
        public static String CaptchaCode;

        static string[,] realQQ = new string[10000, 2];
        static int realQQIndex = 0;
        public static void SecondLogin(string ID)
        {
            //二次登录
            string url = "http://d.web2.qq.com/channel/login2";
            string url1 = string.Format("r={{\"ptwebqq\":\"{0}\",\"clientid\":{1},\"psessionid\":\"\",\"status\":\"online\"}}", ptwebqq, ClientID);
            String dat = HTTP.HttpPost(url, "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2", url1, Encoding.UTF8, true);

            Program.formlogin.textBoxLog.Text = dat;
            char[] t = new char[2];
            t[0] = ':';
            t[1] = ',';
            dat = dat.Replace("{", "");
            dat = dat.Replace("}", "");
            dat = dat.Replace("\"", "");
            string[] tmp = dat.Split(t);

            vfwebqq = tmp[14];
            psessionid = tmp[16];

            hash = GetHash(ID, ptwebqq);
        }

        public static bool FirstLogin(string ID, string Password, string CAPTCHA)
        {
            //一次登录
            String tokentemp = System.Text.RegularExpressions.Regex.Replace(pt_uin, @"\\x", "");
            String token = HexString2Ascii(tokentemp);
            string key = EncodePassword(Password, token, CAPTCHA.ToUpper());

            String url1 = "https://ssl.ptlogin2.qq.com/login?u=";
            String url2 = "&p=";
            String url3 = "&verifycode=";
            String url4 = "&webqq_type=10&remember_uin=1&login2qq=1&aid=501004106&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html";
            String url5 = "%3Flogin2qq%3D1%26webqq_type%3D10&h=1&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1";
            String url6 = "&dumy=&fp=loginerroralert&action=0-15-19190&mibao_css=m_webqq&t=1&g=1&js_type=0&js_ver=10123&";
            String url7 = "login_sig=&pt_randsalt=0&pt_vcode_v1=0&pt_verifysession_v1=";
            String url = url1 + ID + url2 + key + url3 + CAPTCHA + url4 + url5 + url6 + url7 + ptvsession;

            string temp = HTTP.HttpGet(url);

            //二次登录准备
            temp = temp.Replace("ptui_checkVC(", "");
            temp = temp.Replace(");", "");
            temp = temp.Replace("'", "");
            string[] tmp = temp.Split(',');
            url = tmp[2];
            if (url == "")
            {
                MessageBox.Show("登录失败，请重试");
                if (NeedCAPTCHA) GetCaptcha();
                return false;
            }

            HTTP.HttpGet(url);

            Uri uri = new Uri("http://web2.qq.com/");
            ptwebqq = HTTP.cookies.GetCookies(uri)["ptwebqq"].Value;
            p_skey = HTTP.cookies.GetCookies(uri)["p_skey"].Value;
            MyUin = HTTP.cookies.GetCookies(uri)["uin"].Value;
            skey = HTTP.cookies.GetCookies(uri)["skey"].Value;
            p_uin = HTTP.cookies.GetCookies(uri)["p_uin"].Value;

            return true;
        }
        //http://www.cnblogs.com/lianmin/p/4257421.html
        /// 发送好友消息
        public static bool SendMessageToFriend(string uid, string content)
        {
            if (content.Equals(""))
                return false;
            if (uid.Equals(""))
                return false;
            MsgId++;
            try
            {
                string postData = "{\"to\":" + uid;
                postData += ",\"content\":\"[" + content.Replace(Environment.NewLine, "\\\\n");
                postData += ",[\\\"font\\\",{\\\"name\\\":\\\"宋体\\\",\\\"size\\\":10,\\\"style\\\":[0,0,0],\\\"color\\\":\\\"000000\\\"}]]\",\"face\":585,\"clientid\":" + ClientID;
                postData += ",\"msg_id\":" + MsgId;
                postData += ",\"psessionid\":\"" + psessionid;
                postData += "\"}";

                string referer = "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2";
                string url = "http://d.web2.qq.com/channel/send_buddy_msg2";
                postData = "r=" + HttpUtility.UrlEncode(postData);

                string dat = HTTP.HttpPost(url, referer, postData, Encoding.UTF8, false);

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
        public static bool SendMessageToGroup(string gin, string content)
        {
            if (content.Equals(""))
                return false;
            if (gin.Equals(""))
                return false;
            MsgId++;
            try
            {
                string postData = "{\"group_uin\":" + gin
                    + ",\"content\":\"[" + content.Replace(Environment.NewLine, "\\\\n")
                    + ",[\\\"font\\\",{\\\"name\\\":\\\"宋体\\\",\\\"size\\\":13,\\\"style\\\":[0,0,0],\\\"color\\\":\\\"000000\\\"}]]\",\"face\":549,\"clientid\":" + ClientID
                    + ",\"msg_id\":" + MsgId
                    + ",\"psessionid\":\"" + psessionid
                    + "\"}";
                postData = "r=" + HttpUtility.UrlEncode(postData);

                string referer = "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2";
                string url = "http://d.web2.qq.com/channel/send_qun_msg2";

                string dat = HTTP.HttpPost(url, referer, postData, Encoding.UTF8, false);

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
        public static void GetCaptcha()
        {
            Program.formlogin.textBoxCAPTCHA.Text = "";

            String strimg1 = "https://ssl.captcha.qq.com/getimage?aid=501004106&r=0.005933324107900262&uin=";
            String strimg2 = "&cap_cd=";
            String strimg = strimg1 + Program.formlogin.textBoxID.Text + strimg2 + CaptchaCode;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strimg);
            req.CookieContainer = HTTP.cookies;
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Program.formlogin.pictureBoxCAPTCHA.Image = Image.FromStream(res.GetResponseStream());

            ptvsession = res.Cookies["verifysession"].Value;
            res.Close();
        }
        public static bool GetCAPTCHAInf(string ID)
        {
            String str1 = "https://ssl.ptlogin2.qq.com/check?pt_tea=1&uin=";
            String str2 = "&appid=501004106&js_ver=10121&js_type=0&login_sig=&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html&r=0.4053995015565306";
            String url = str1 + ID + str2;

            string dat = HTTP.HttpGet(url);

            dat = dat.Replace("ptui_checkVC(", "");
            dat = dat.Replace(");", "");
            dat = dat.Replace("'", "");
            string[] tmp = dat.Split(',');

            pt_uin = tmp[2];

            if (tmp[0] == "1")
            {
                NeedCAPTCHA = true;

                CaptchaCode = tmp[1];
                GetCaptcha();

                return true;
            }
            else
            {
                NeedCAPTCHA = false;
                Program.formlogin.textBoxCAPTCHA.Text = tmp[1];
                ptvsession = tmp[3];
                return false;
            }
        }
        public static void getFrienf()
        {
            String url = "http://s.web2.qq.com/api/get_user_friends2";
            String sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", vfwebqq, hash);
            String dat = HTTP.HttpPost(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1", sendData, Encoding.UTF8, true);
            Program.formlogin.textBoxLog.Text = dat;

            Program.formlogin.user = (JsonFriendModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendModel));
            Program.formlogin.listBoxFriend.Items.Clear();
            for (int i = 0; i < Program.formlogin.user.result.info.Count; i++)
            {
                string gender;
                Program.formlogin.friendinf[i].uin = Program.formlogin.user.result.info[i].uin;
                Program.formlogin.friendinf[i].Inf = SmartQQ.GetFriendInf(Program.formlogin.user.result.info[i].uin);
                if (Program.formlogin.friendinfMaxIndex < i)
                    Program.formlogin.friendinfMaxIndex = i;
                if (Program.formlogin.friendinf[i].Inf.result.gender.Equals("female"))
                    gender = "妹纸";
                else if (Program.formlogin.friendinf[i].Inf.result.gender.Equals("male"))
                    gender = "汉子";
                else gender = "未知";
                Program.formlogin.listBoxFriend.Items.Add(Program.formlogin.user.result.info[i].uin + ":" + GetRealQQ(Program.formlogin.user.result.info[i].uin) + ":" + Program.formlogin.user.result.info[i].nick + ":" + gender);
            }
        }
        public static String GetRealQQ(string uin)
        {
            for (int i = 0; i < realQQIndex; i++)
            {
                if (realQQ[i, 0] == uin && (!realQQ[i, 1].Equals("")))
                    return realQQ[i, 1];
            }
            String url = "http://s.web2.qq.com/api/get_friend_uin2?tuin=" + uin + "&type=1&vfwebqq=" + vfwebqq + "&t=" + GetTimeStamp();

            string dat = HTTP.HttpGet(url);

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

            realQQ[realQQIndex, 0] = uin;
            realQQ[realQQIndex, 1] = tmp[0];
            realQQIndex++;
            return tmp[0];
        }
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string ret = string.Empty;
            ret = Convert.ToInt64(ts.TotalMilliseconds).ToString();

            return ret;
        }
        //感谢QQBOT群（346167134） 杨小泡的热心帮助！
        public static string HexString2Ascii(string hexString)
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
        public static String GetHash(String no, String ptwebqq)
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
        public static JsonGroupInfoModel GetGroupInfo(string gcode)
        {
            String url = "http://s.web2.qq.com/api/get_group_info_ext2?gcode=" + gcode + "&vfwebqq=" + vfwebqq + "&t=" + GetTimeStamp();
            string dat = HTTP.HttpGet(url);

            JsonGroupInfoModel ans = (JsonGroupInfoModel)JsonConvert.DeserializeObject(dat, typeof(JsonGroupInfoModel));
            return ans;
        }
        public static void getGroup()
        {
            String url = "http://s.web2.qq.com/api/get_group_name_list_mask2";
            String sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", vfwebqq, hash);
            String dat = HTTP.HttpPost(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1", sendData, Encoding.UTF8, true);
            Program.formlogin.textBoxLog.Text = dat;

            Program.formlogin.group = (JsonGroupModel)JsonConvert.DeserializeObject(dat, typeof(JsonGroupModel));
            Program.formlogin.listBoxGroup.Items.Clear();
            int i;
            for (i = 0; i < Program.formlogin.group.result.gnamelist.Count; i++)
            {
                Program.formlogin.listBoxGroup.Items.Add(Program.formlogin.group.result.gnamelist[i].gid + "::" + Program.formlogin.group.result.gnamelist[i].name);
                Program.formlogin.groupinfo[i].gid = Program.formlogin.group.result.gnamelist[i].gid;
                Program.formlogin.groupinfo[i].inf = GetGroupInfo(Program.formlogin.group.result.gnamelist[i].code);
            }
            Program.formlogin.groupinfMaxIndex = i;
        }
        public static JsonFriendInfModel GetFriendInf(string uin)
        {
            String url = "http://s.web2.qq.com/api/get_friend_info2?tuin=" + uin + "&vfwebqq=" + vfwebqq + "&clientid=" + ClientID + "&psessionid=" + psessionid + "&t=" + GetTimeStamp();

            string dat = HTTP.HttpGet(url);
            JsonFriendInfModel ans = (JsonFriendInfModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendInfModel));
            return ans;
        }
    }
}
