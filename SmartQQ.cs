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
        public static int ClientID = 53999199;
        public static string ptvsession = "";
        public static string p_skey, MyUin, skey, p_uin, vfwebqq, hash;
        public static string ptwebqq, psessionid;
        public static string pt_uin = "";

        static string[,] realQQ = new string[10000, 2];
        static int realQQIndex = 0;
        public static void SecondLogin()
        {
            string url0 = string.Format("http://s.web2.qq.com/api/getvfwebqq?ptwebqq={0}&clientid={1}&psessionid=&t={2}", ptwebqq, ClientID, GetTimeStamp());
            string temp = HTTP.HttpGet(url0);

            Uri uri = new Uri("http://web2.qq.com/");

            //二次登录
            string url = "http://d1.web2.qq.com/channel/login2";

            Uri uri1 = new Uri("http://qq.com/");
            string url1 = "{\"ptwebqq\":\"" + ptwebqq + "\",\"clientid\":" + ClientID + ",\"psessionid\":\"\",\"status\":\"online\"}";
            url1 = "r=" + HttpUtility.UrlEncode(url1);
            string dat = HTTP.HttpPost(url, "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2", url1, Encoding.UTF8, true);

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

            hash = GetHash(Program.formlogin.QQNum, ptwebqq);
        }
        public static void GetQRCode()
        {
            Random rd = new Random();
            int t1 = rd.Next(100000000, 999999999);
            int t2 = rd.Next(1000000, 9999999);
            string url = "https://ssl.ptlogin2.qq.com/ptqrshow?appid=501004106&e=0&l=M&s=5&d=72&v=4&t=0." + t1.ToString() + t2.ToString();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = HTTP.cookies;

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Program.formlogin.pictureBoxQRCode.Image = Image.FromStream(res.GetResponseStream());
        }
        public static void FirstLogin()
        {
            GetQRCode();
            Program.formlogin.timerLogin.Enabled = true;
            Program.formlogin.timerLogin.Start();
        }

        public static string CheckStatu()
        {
            string url = "https://ssl.ptlogin2.qq.com/ptqrlogin?webqq_type=10&remember_uin=1&login2qq=1&aid=501004106&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1&dumy=&fp=loginerroralert&action=0-0-18099&mibao_css=m_webqq&t=1&g=1&js_type=0&js_ver=10135&login_sig=&pt_randsalt=0";
            string temp = HTTP.HttpGet(url);
            string[] tmp = temp.Split('\'');
            return tmp[1] + tmp[5];
        }

        //http://www.cnblogs.com/lianmin/p/4257421.html
        /// 发送好友消息
        public static bool SendMessageToFriend(string uid, string content, string specialMessage = "")
        {
            if (content.Equals(""))
                return false;
            if (uid.Equals(""))
                return false;
            string[] temp = specialMessage.Split(','); ;
            MsgId++;
            content = content.Replace("\r\n", "\n");
            content = content.Replace("\n\r", "\n");
            content = content.Replace("\r", "\n");
            content = content.Replace("\n", Environment.NewLine);
            try
            {
                string postData = "";
                if (temp[0] != "disscuss")
                    postData = "{\"to\":" + uid;
                else postData = "{\"did\":" + uid;
                postData += ",\"content\":\"[" + content.Replace(Environment.NewLine, "\\\\n");
                postData += ",[\\\"font\\\",{\\\"name\\\":\\\"宋体\\\",\\\"size\\\":10,\\\"style\\\":[0,0,0],\\\"color\\\":\\\"000000\\\"}]]\",\"clientid\":" + ClientID + ",\"face\":";
                if (specialMessage == "")
                    postData += "585";
                else if (temp[0] == "disscuss")
                    postData += "564";
                else postData += "594";
                postData += ",\"msg_id\":" + MsgId;
                postData += ",\"psessionid\":\"" + psessionid + "\"";

                if (temp[0] != "" && temp[0] != "disscuss")
                {
                    postData += ",\"group_sig\":\"" + GetGroupSig(temp[0], temp[1], temp[2]);
                    postData += "\",\"service_type\":" + temp[2];
                }
                postData += "}";
                string url;
                string referer = "http://d1.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2";
                if (specialMessage == "")
                    url = "http://d1.web2.qq.com/channel/send_buddy_msg2";
                else if (temp[0] == "disscuss")
                    url = "http://d1.web2.qq.com/channel/send_discu_msg2";
                else url = "http://d1.web2.qq.com/channel/send_sess_msg2";
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
        public static string GetGroupSig(string id, string uin, string service_type)
        {
            string url = "http://d1.web2.qq.com/channel/get_c2cmsg_sig2?id=" + id + "&to_uin=" + uin + "&clientid=" + ClientID + "&psessionid=" + psessionid + "&service_type=" + service_type + "&t=" + GetTimeStamp();
            string temp = HTTP.HttpGet(url);
            string[] tmp = temp.Split('\"');
            return tmp[9];
        }
        public static void GetDissInfo(string did, int index)
        {
            string url = "http://d1.web2.qq.com/channel/get_discu_info?did=" + did + "&vfwebqq=" + vfwebqq + "&clientid=" + ClientID + "&psessionid=" + psessionid + "&t=" + GetTimeStamp();
            string temp = HTTP.HttpGet(url);
            JsonDissgussModel inf = (JsonDissgussModel)JsonConvert.DeserializeObject(temp, typeof(JsonDissgussModel));
            Program.formlogin.discussInfo[index].did = inf.result.info.did;
            Program.formlogin.discussInfo[index].dname = inf.result.info.discu_name;
            Program.formlogin.discussInfo[index].downer = inf.result.info.discu_owner;
            Program.formlogin.discussInfo[index].inf = inf;
        }
        /// 发送群消息
        public static bool SendMessageToGroup(string gin, string content)
        {
            if (content.Equals(""))
                return false;
            if (gin.Equals(""))
                return false;
            MsgId++;
            content = content.Replace("\r\n", "\n");
            content = content.Replace("\n\r", "\n");
            content = content.Replace("\r", "\n");
            content = content.Replace("\n", Environment.NewLine);
            try
            {
                string postData = "{\"group_uin\":" + gin
                    + ",\"content\":\"[" + content.Replace(Environment.NewLine, "\\\\n")
                    + ",[\\\"font\\\",{\\\"name\\\":\\\"宋体\\\",\\\"size\\\":11,\\\"style\\\":[0,0,0],\\\"color\\\":\\\"00FF00\\\"}]]\",\"face\":549,\"clientid\":" + ClientID
                    + ",\"msg_id\":" + MsgId
                    + ",\"psessionid\":\"" + psessionid
                    + "\"}";
                postData = "r=" + HttpUtility.UrlEncode(postData);

                string referer = "http://d1.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2";
                string url = "http://d1.web2.qq.com/channel/send_qun_msg2";

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
        public static void getFrienf()
        {
            string url = "http://s.web2.qq.com/api/get_user_friends2";
            string sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", vfwebqq, hash);
            string dat = HTTP.HttpPost(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1", sendData, Encoding.UTF8, true);
            Program.formlogin.textBoxLog.Text = dat;

            Program.formlogin.user = (JsonFriendModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendModel));
            for (int i = 0; i < Program.formlogin.user.result.info.Count; i++)
            {
                bool flag = false;
                for (int j = 0; j <= Program.formlogin.friendinfMaxIndex; j++)
                {
                    if (Program.formlogin.friendinf[j].uin == Program.formlogin.user.result.info[i].uin)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    continue;
                else
                {
                    Program.formlogin.friendinfMaxIndex++;
                    Program.formlogin.friendinf[Program.formlogin.friendinfMaxIndex].uin = Program.formlogin.user.result.info[i].uin;
                    Program.formlogin.listBoxFriend.Items.Add(Program.formlogin.user.result.info[i].uin + ":" + GetRealQQ(Program.formlogin.user.result.info[i].uin) + ":" + Program.formlogin.user.result.info[i].nick);
                }
            }
        }
        public static string GetRealQQ(string uin)
        {
            for (int i = 0; i < realQQIndex; i++)
            {
                if (realQQ[i, 0] == uin && (!realQQ[i, 1].Equals("")))
                    return realQQ[i, 1];
            }
            string url = "http://s.web2.qq.com/api/get_friend_uin2?tuin=" + uin + "&type=1&vfwebqq=" + vfwebqq + "&t=" + GetTimeStamp();

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
            string res = string.Empty;
            for (int a = 0; a < hexString.Length; a = a + 2)
            {
                string Char2Convert = hexString.Substring(a, 2);
                int n = Convert.ToInt32(Char2Convert, 16);
                char c = (char)n;
                res += c.ToString();
            }
            return res;
        }
        public static string GetHash(string no, string ptwebqq)
        {
            var scriptEngine = new Jurassic.ScriptEngine();
            scriptEngine.EnableDebugging = true;
            scriptEngine.SetGlobalValue("window", new WindowObject(scriptEngine));
            scriptEngine.ExecuteFile(System.AppDomain.CurrentDomain.BaseDirectory + "hash.js");
            var ret = scriptEngine.CallGlobalFunction<string>("friendsHash", no, ptwebqq, 0);
            return ret;
        }
        public static JsonGroupInfoModel GetGroupInfo(string gcode)
        {
            string url = "http://s.web2.qq.com/api/get_group_info_ext2?gcode=" + gcode + "&vfwebqq=" + vfwebqq + "&t=" + GetTimeStamp();
            string dat = HTTP.HttpGet(url);

            JsonGroupInfoModel ans = (JsonGroupInfoModel)JsonConvert.DeserializeObject(dat, typeof(JsonGroupInfoModel));
            return ans;
        }
        public static void getGroup()
        {
            string url = "http://s.web2.qq.com/api/get_group_name_list_mask2";
            string sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", vfwebqq, hash);
            string dat = HTTP.HttpPost(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1", sendData, Encoding.UTF8, true);
            Program.formlogin.textBoxLog.Text = dat;

            Program.formlogin.group = (JsonGroupModel)JsonConvert.DeserializeObject(dat, typeof(JsonGroupModel));
            int i;
            for (i = 0; i < Program.formlogin.group.result.gnamelist.Count; i++)
            {
                bool Already = false;
                for (int j = 0; j < Program.formlogin.groupinfMaxIndex; j++)
                    if (Program.formlogin.groupinfo[j].gid.Equals(Program.formlogin.group.result.gnamelist[i].gid))
                        Already = true;
                if (!Already)
                {
                    Program.formlogin.listBoxGroup.Items.Add(Program.formlogin.group.result.gnamelist[i].gid + "::" + Program.formlogin.group.result.gnamelist[i].name);
                    Program.formlogin.groupinfo[Program.formlogin.groupinfMaxIndex].gid = Program.formlogin.group.result.gnamelist[i].gid;
                    Program.formlogin.groupinfo[Program.formlogin.groupinfMaxIndex].inf = GetGroupInfo(Program.formlogin.group.result.gnamelist[i].code);
                    Program.formlogin.groupinfMaxIndex++;
                }
            }
        }
        public static JsonFriendInfModel GetFriendInf(string uin)
        {
            string url = "http://s.web2.qq.com/api/get_friend_info2?tuin=" + uin + "&vfwebqq=" + vfwebqq + "&clientid=" + ClientID + "&psessionid=" + psessionid + "&t=" + GetTimeStamp();

            string dat = HTTP.HttpGet(url);
            JsonFriendInfModel ans = (JsonFriendInfModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendInfModel));
            return ans;
        }
    }
}
