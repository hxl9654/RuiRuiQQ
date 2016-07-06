using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
// * @version    2.0
// * @discribe   RuiRuiQQRobot服务端
// * 本软件作者是何相龙，使用GPL v3许可证进行授权。

namespace SmartQQ
{
    public static class SmartQQ
    {

        private static System.Timers.Timer Login_QRStatuTimer = new System.Timers.Timer();
        private static string vfwebqq, ptwebqq, psessionid, uin, QQNum, hash;
        public static FriendInfo SelfInfo = new FriendInfo();
        //private static string p_skey, skey, p_uin, MyUin;
        public static Dictionary<string, FriendInfo> FriendList = new Dictionary<string, FriendInfo>();
        public static Dictionary<string, GroupInfo> GroupList = new Dictionary<string, GroupInfo>();
        public static Dictionary<string, string> RealQQNum = new Dictionary<string, string>();
        public static string[] FriendCategories = new string[100];
        /// <summary>
        /// 开始登录SmartQQ
        /// </summary>
        public static void Login()
        {
            Login_GetQRCode();
            Login_QRStatuTimer.AutoReset = true;
            Login_QRStatuTimer.Elapsed += Login_QRStatuTimer_Elapsed;
            Login_QRStatuTimer.Interval = 1000;
            Login_QRStatuTimer.Start();
        }
        /// <summary>
        /// 每秒检查一次二维码状态
        /// </summary>
        private static void Login_QRStatuTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Login_GetQRStatu();
        }
        /// <summary>
        /// 获取登陆用二维码
        /// </summary>
        /// <returns>成功：true</returns>
        public static bool Login_GetQRCode()
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://ssl.ptlogin2.qq.com/ptqrshow?appid=501004106&e=0&l=M&s=5&d=72&v=4&t=0.1");
                req.CookieContainer = HTTP.cookies;

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Program.formlogin.pictureBoxQRCode.Image = Image.FromStream(res.GetResponseStream());
            }
            catch (Exception) { return false; }
            return true;
        }
        /// <summary>
        /// 检查二维码状态
        /// </summary>
        private static void Login_GetQRStatu()
        {
            string dat;
            dat = HTTP.HttpGet("https://ssl.ptlogin2.qq.com/ptqrlogin?webqq_type=10&remember_uin=1&login2qq=1&aid=501004106 &u1=http%3A%2F%2Fw.qq.com%2Fproxy.html%3Flogin2qq%3D1%26webqq_type%3D10 &ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1&dumy=&fp=loginerroralert &action=0-0-157510&mibao_css=m_webqq&t=1&g=1&js_type=0&js_ver=10143&login_sig=&pt_randsalt=0", "https://ui.ptlogin2.qq.com/cgi-bin/login?daid=164&target=self&style=16&mibao_css=m_webqq&appid=501004106&enable_qlogin=0&no_verifyimg=1 &s_url=http%3A%2F%2Fw.qq.com%2Fproxy.html&f_url=loginerroralert &strong_login=1&login_state=10&t=20131024001");
            string[] temp = dat.Split('\'');
            switch (temp[1])
            {
                case ("65"):                                            //二维码失效
                    Program.formlogin.labelQRStatu.Text = "失效";
                    Login_GetQRCode();
                    break;
                case ("66"):                                            //等待扫描
                    Program.formlogin.labelQRStatu.Text = "有效";
                    break;
                case ("67"):                                            //等待确认
                    Program.formlogin.labelQRStatu.Text = "等待";
                    break;
                case ("0"):                                             //已经确认
                    Program.formlogin.labelQRStatu.Text = "成功";
                    Login_Process(temp[5]);
                    break;

                default: break;
            }

        }
        /// <summary>
        /// 处理扫描二维码并确认后的登录流程
        /// </summary>
        /// <param name="url">获取ptwebqq的跳转地址</param>
        private static void Login_Process(string url)
        {
            Login_QRStatuTimer.Stop();
            Login_GetPtwebqq(url);
            Login_GetVfwebqq();
            Login_GetPsessionid();
            Info_FriendList();
            Info_GroupList();
            Message_Request();
        }
        private static void Login_GetPtwebqq(string url)
        {
            string dat = HTTP.HttpGet(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
            Uri uri = new Uri("http://web2.qq.com/");
            ptwebqq = HTTP.cookies.GetCookies(uri)["ptwebqq"].Value;
            //p_skey = HTTP.cookies.GetCookies(uri)["p_skey"].Value;
            //MyUin = HTTP.cookies.GetCookies(uri)["uin"].Value;
            //skey = HTTP.cookies.GetCookies(uri)["skey"].Value;
            //p_uin = HTTP.cookies.GetCookies(uri)["p_uin"].Value;
            //QQNum = p_uin.Remove(0, 1).TrimStart('0');
        }

        private static void Login_GetVfwebqq()
        {
            string url = "http://s.web2.qq.com/api/getvfwebqq?ptwebqq=#{ptwebqq}&clientid=53999199&psessionid=&t=0.1".Replace("#{ptwebqq}", ptwebqq);
            string dat = HTTP.HttpGet(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
            vfwebqq = dat.Split('\"')[7];
        }

        private static void Login_GetPsessionid()
        {
            string url = "http://d1.web2.qq.com/channel/login2";
            string url1 = "{\"ptwebqq\":\"#{ptwebqq}\",\"clientid\":53999199,\"psessionid\":\"\",\"status\":\"online\"}".Replace("#{ptwebqq}", ptwebqq);
            url1 = "r=" + HttpUtility.UrlEncode(url1);
            string dat = HTTP.HttpPost(url, url1, "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");
            psessionid = dat.Replace(":", ",").Replace("{", "").Replace("}", "").Replace("\"", "").Split(',')[10];
            QQNum = uin = dat.Replace(":", ",").Replace("{", "").Replace("}", "").Replace("\"", "").Split(',')[14];
            hash = AID_Hash(QQNum, ptwebqq);
        }
        /// <summary>
        /// 发送poll包，请求消息
        /// </summary>
        private static void Message_Request()
        {
            try
            {
                string url = "http://d1.web2.qq.com/channel/poll2";
                string HeartPackdata = "{\"ptwebqq\":\"#{ptwebqq}\",\"clientid\":53999199,\"psessionid\":\"#{psessionid}\",\"key\":\"\"}";
                HeartPackdata = HeartPackdata.Replace("#{ptwebqq}", ptwebqq).Replace("#{psessionid}", psessionid);
                HeartPackdata = "r=" + HttpUtility.UrlEncode(HeartPackdata);
                HTTP.HttpPost_Async_Action action = Message_Get;
                HTTP.HttpPost_Async(url, HeartPackdata, action);
            }
            catch (Exception) { Message_Request(); }
        }
        /// <summary>
        /// 接收到消息的回调函数
        /// </summary>
        /// <param name="data">接收到的数据（json）</param>
        private static void Message_Get(string data)
        {
            Task.Run(() => Message_Request());
            Task.Run(() => Message_Process(data));
        }

        private static void Message_Process(string data)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 根据QQ号和ptwebqq值获取hash值，用于获取好友列表和群列表
        /// </summary>
        /// <param name="QQNum">QQ号</param>
        /// <param name="ptwebqq">ptwebqq</param>
        /// <returns>hash值</returns>
        private static string AID_Hash(string QQNum, string ptwebqq)
        {
            int[] N = new int[4];
            long QQNum_Long = long.Parse(QQNum);
            for (int T = 0; T < ptwebqq.Length; T++)
            {
                N[T % 4] ^= ptwebqq.ToCharArray()[T];
            }
            string[] U = { "EC", "OK" };
            long[] V = new long[4];
            V[0] = QQNum_Long >> 24 & 255 ^ U[0].ToCharArray()[0];
            V[1] = QQNum_Long >> 16 & 255 ^ U[0].ToCharArray()[1];
            V[2] = QQNum_Long >> 8 & 255 ^ U[1].ToCharArray()[0];
            V[3] = QQNum_Long & 255 ^ U[1].ToCharArray()[1];

            long[] U1 = new long[8];

            for (int T = 0; T < 8; T++)
            {
                U1[T] = T % 2 == 0 ? N[T >> 1] : V[T >> 1];
            }

            string[] N1 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
            string V1 = "";

            for (int i = 0; i < U1.Length; i++)
            {
                V1 += N1[(int)((U1[i] >> 4) & 15)];
                V1 += N1[(int)(U1[i] & 15)];
            }
            return V1;
        }
        /// <summary>
        /// 根据uin获取真实QQ号
        /// </summary>
        /// <param name="uin"></param>
        /// <returns></returns>
        internal static string Info_RealQQ(string uin)
        {
            if (RealQQNum.ContainsKey(uin))
                return RealQQNum[uin];

            string url = "http://s.web2.qq.com/api/get_friend_uin2?tuin=#{uin}&type=1&vfwebqq=#{vfwebqq}&t=0.1".Replace("#{uin}", uin).Replace("#{vfwebqq}", vfwebqq);
            string dat = HTTP.HttpGet(url);
            string temp = dat.Split('\"')[10].Split(',')[0].Replace(":", "");
            if (temp != "")
            {
                RealQQNum.Add(uin, temp);
                return temp;
            }
            else return "";
        }
        /// <summary>
        /// 获取好友列表并保存
        /// </summary>
        internal static void Info_FriendList()
        {
            string url = "http://s.web2.qq.com/api/get_user_friends2";
            string sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", vfwebqq, hash);
            string dat = HTTP.HttpPost(url, sendData, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");

            JsonFriendModel friend = (JsonFriendModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendModel));
            for (int i = 0; i < friend.result.info.Count; i++)
            {
                if (!FriendList.ContainsKey(friend.result.info[i].uin))
                    FriendList.Add(friend.result.info[i].uin, new FriendInfo());
                FriendList[friend.result.info[i].uin].face = friend.result.info[i].face;
                FriendList[friend.result.info[i].uin].nick = friend.result.info[i].nick;
            }
            for (int i = 0; i < friend.result.friends.Count; i++)
            {
                if (!FriendList.ContainsKey(friend.result.friends[i].uin))
                    FriendList.Add(friend.result.friends[i].uin, new FriendInfo());
                FriendList[friend.result.friends[i].uin].categories = friend.result.friends[i].categories;
            }
            for (int i = 0; i < friend.result.categories.Count; i++)
            {
                FriendCategories[friend.result.categories[i].index] = friend.result.categories[i].name;
            }
            Program.formlogin.ReNewListBoxFriend();
        }
        /// <summary>
        /// 获取自己的信息
        /// </summary>
        internal static void Info_SelfInfo()
        {
            string url = "http://s.web2.qq.com/api/get_self_info2&t=0.1";
            string dat = HTTP.HttpGet(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
            JsonFriendInfModel inf = (JsonFriendInfModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendInfModel));

            SelfInfo.face = inf.result.face;
            SelfInfo.occupation = inf.result.occupation;
            SelfInfo.phone = inf.result.phone;
            SelfInfo.college = inf.result.college;
            SelfInfo.blood = inf.result.blood;
            SelfInfo.homepage = inf.result.homepage;
            SelfInfo.vip_info = inf.result.vip_info;
            SelfInfo.country = inf.result.country;
            SelfInfo.city = inf.result.city;
            SelfInfo.personal = inf.result.personal;
            SelfInfo.nick = inf.result.nick;
            SelfInfo.shengxiao = inf.result.shengxiao;
            SelfInfo.email = inf.result.email;
            SelfInfo.province = inf.result.province;
            SelfInfo.gender = inf.result.gender;
            SelfInfo.birthday = new DateTime(inf.result.birthday.year, inf.result.birthday.month, inf.result.birthday.day);
        }
        /// <summary>
        /// 获取群列表并保存
        /// </summary>
        internal static void Info_GroupList()
        {
            string url = "http://s.web2.qq.com/api/get_group_name_list_mask2";
            string sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", vfwebqq, hash);
            string dat = HTTP.HttpPost(url, sendData, "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");

            JsonGroupModel group = (JsonGroupModel)JsonConvert.DeserializeObject(dat, typeof(JsonGroupModel));
            for (int i = 0; i < group.result.gnamelist.Count; i++)
            {
                if (!GroupList.ContainsKey(group.result.gnamelist[i].gid))
                    GroupList.Add(group.result.gnamelist[i].gid, new GroupInfo());
                GroupList[group.result.gnamelist[i].gid].name = group.result.gnamelist[i].name;
                GroupList[group.result.gnamelist[i].gid].code = group.result.gnamelist[i].code;
            }
            Program.formlogin.ReNewListBoxGroup();
        }

        internal static void GetDissInfo(string did, int j)
        {
            throw new NotImplementedException();
        }

        internal static JsonGroupInfoModel GetGroupInfo(string code)
        {
            throw new NotImplementedException();
        }

        internal static void SendMessageToGroup(string gid, string v)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 获取好友的详细信息
        /// </summary>
        /// <param name="uin"></param>
        internal static void Info_FriendInfo(string uin)
        {
            string url = "http://s.web2.qq.com/api/get_friend_info2?tuin=#{uin}&vfwebqq=#{vfwebqq}&clientid=53999199&psessionid=#{psessionid}&t=0.1";
            url = url.Replace("#{uin}", uin).Replace("#{vfwebqq}", vfwebqq).Replace("#{psessionid}", psessionid);
            string dat = HTTP.HttpGet(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
            JsonFriendInfModel inf = (JsonFriendInfModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendInfModel));
            if (!FriendList.ContainsKey(uin))
                FriendList.Add(uin, new FriendInfo());
            FriendList[uin].face = inf.result.face;
            FriendList[uin].occupation = inf.result.occupation;
            FriendList[uin].phone = inf.result.phone;
            FriendList[uin].college = inf.result.college;
            FriendList[uin].blood = inf.result.blood;
            FriendList[uin].homepage = inf.result.homepage;
            FriendList[uin].vip_info = inf.result.vip_info;
            FriendList[uin].country = inf.result.country;
            FriendList[uin].city = inf.result.city;
            FriendList[uin].personal = inf.result.personal;
            FriendList[uin].nick = inf.result.nick;
            FriendList[uin].shengxiao = inf.result.shengxiao;
            FriendList[uin].email = inf.result.email;
            FriendList[uin].province = inf.result.province;
            FriendList[uin].gender = inf.result.gender;
            FriendList[uin].birthday = new DateTime(inf.result.birthday.year, inf.result.birthday.month, inf.result.birthday.day);
        }

        internal static void SendMessageToFriend(string v, string messageToSend)
        {
            throw new NotImplementedException();
        }

        internal static void SendMessageToFriend(string uin, string messageToSend, string specialMessage)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 好友资料类
        /// </summary>
        public class FriendInfo
        {
            public string markname;
            public string nick;
            public string gender;

            public int face;
            public int client_type;
            public int categories;
            public string status;

            public string occupation;   //职业                    
            public string college;
            public string country;
            public string province;
            public string city;
            public string personal;     //简介

            public string homepage;
            public string email;
            public string mobile;
            public string phone;

            public DateTime birthday;
            public int blood;
            public int shengxiao;
            public int vip_info;
        }
        /// <summary>
        /// 群资料类
        /// </summary>
        public class GroupInfo
        {
            public string name;
            public string code;
            public string markname;
        }
    }
}
