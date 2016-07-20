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

namespace RuiRuiQQRobot
{
    public static class SmartQQ
    {
        private static System.Timers.Timer Login_QRStatuTimer = new System.Timers.Timer();
        private static string vfwebqq, ptwebqq, psessionid, uin, hash;
        public static string QQNum;
        public static FriendInfo SelfInfo = new FriendInfo();
        public static Dictionary<string, FriendInfo> FriendList = new Dictionary<string, FriendInfo>();
        public static Dictionary<string, GroupInfo> GroupList = new Dictionary<string, GroupInfo>();
        public static Dictionary<string, DiscussInfo> DisscussList = new Dictionary<string, DiscussInfo>();
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
        /// 登录第一步：获取登陆用二维码
        /// </summary>
        /// <returns>成功：true</returns>
        public static bool Login_GetQRCode()
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://ssl.ptlogin2.qq.com/ptqrshow?appid=501004106&e=0&l=M&s=5&d=72&v=4&t=0.1");
                req.CookieContainer = HTTP.cookies;

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Program.MainForm.pictureBoxQRCode.Visible = true;
                Program.MainForm.pictureBoxQRCode.Image = Image.FromStream(res.GetResponseStream());
            }
            catch (Exception) { return false; }
            return true;
        }
        /// <summary>
        /// 每秒检查一次二维码状态
        /// </summary>
        private static void Login_QRStatuTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Login_GetQRStatu();
        }
        /// <summary>
        /// 登录第二步：检查二维码状态
        /// </summary>
        private static void Login_GetQRStatu()
        {
            string dat;
            dat = HTTP.Get("https://ssl.ptlogin2.qq.com/ptqrlogin?webqq_type=10&remember_uin=1&login2qq=1&aid=501004106 &u1=http%3A%2F%2Fw.qq.com%2Fproxy.html%3Flogin2qq%3D1%26webqq_type%3D10 &ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1&dumy=&fp=loginerroralert &action=0-0-157510&mibao_css=m_webqq&t=1&g=1&js_type=0&js_ver=10143&login_sig=&pt_randsalt=0", "https://ui.ptlogin2.qq.com/cgi-bin/login?daid=164&target=self&style=16&mibao_css=m_webqq&appid=501004106&enable_qlogin=0&no_verifyimg=1 &s_url=http%3A%2F%2Fw.qq.com%2Fproxy.html&f_url=loginerroralert &strong_login=1&login_state=10&t=20131024001");
            string[] temp = dat.Split('\'');
            switch (temp[1])
            {
                case ("65"):                                            //二维码失效
                    Program.MainForm.labelQRStatu.Text = "失效";
                    Login_GetQRCode();
                    break;
                case ("66"):                                            //等待扫描
                    Program.MainForm.labelQRStatu.Text = "有效";
                    break;
                case ("67"):                                            //等待确认
                    Program.MainForm.labelQRStatu.Text = "等待";
                    break;
                case ("0"):                                             //已经确认
                    Program.MainForm.labelQRStatu.Text = "成功";
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
            Program.MainForm.labelQRStatu.Text = "";
            Program.MainForm.pictureBoxQRCode.Visible = false;
            Info_FriendList();
            Info_GroupList();
            Info_DisscussList();
            Message_Request();

            Program.MainForm.listBoxLog.Items.Insert(0, "登录成功");
            Program.MainForm.buttonSend.Enabled = true;
            Program.MainForm.buttonLogIn.Enabled = false;
            Program.MainForm.AcceptButton = Program.MainForm.buttonSend;
            Program.MainForm.labelQQNum.Text = SmartQQ.QQNum;
        }
        /// <summary>
        /// 登录第三步：获取ptwebqq值
        /// </summary>
        /// <param name="url">获取ptwebqq的跳转地址</param>
        private static void Login_GetPtwebqq(string url)
        {
            string dat = HTTP.Get(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
            Uri uri = new Uri("http://web2.qq.com/");
            ptwebqq = HTTP.cookies.GetCookies(uri)["ptwebqq"].Value;
            //p_skey = HTTP.cookies.GetCookies(uri)["p_skey"].Value;
            //MyUin = HTTP.cookies.GetCookies(uri)["uin"].Value;
            //skey = HTTP.cookies.GetCookies(uri)["skey"].Value;
            //p_uin = HTTP.cookies.GetCookies(uri)["p_uin"].Value;
            //QQNum = p_uin.Remove(0, 1).TrimStart('0');
        }
        /// <summary>
        /// 登录第四步：获取vfwebqq的值
        /// </summary>
        private static void Login_GetVfwebqq()
        {
            string url = "http://s.web2.qq.com/api/getvfwebqq?ptwebqq=#{ptwebqq}&clientid=53999199&psessionid=&t=0.1".Replace("#{ptwebqq}", ptwebqq);
            string dat = HTTP.Get(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
            vfwebqq = dat.Split('\"')[7];
        }
        /// <summary>
        /// 登录第五步：获取pessionid
        /// </summary>
        private static void Login_GetPsessionid()
        {
            string url = "http://d1.web2.qq.com/channel/login2";
            string url1 = "{\"ptwebqq\":\"#{ptwebqq}\",\"clientid\":53999199,\"psessionid\":\"\",\"status\":\"online\"}".Replace("#{ptwebqq}", ptwebqq);
            url1 = "r=" + HttpUtility.UrlEncode(url1);
            string dat = HTTP.Post(url, url1, "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");
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
                HTTP.Post_Async_Action action = Message_Get;
                HTTP.Post_Async(url, HeartPackdata, action);
            }
            catch (Exception) { Message_Request(); }
        }
        /// <summary>
        /// 接收到消息的回调函数
        /// </summary>
        /// <param name="data">接收到的数据（json）</param>
        private static bool Running = true;
        private static void Message_Get(string data)
        {
            Task.Run(() => Message_Request());
            if (Running)
                Task.Run(() => Message_Process(data));
        }
        /// <summary>
        /// 处理收到的消息
        /// </summary>
        /// <param name="data">收到的消息（JSON）</param>
        private static void Message_Process(string data)
        {
            Program.MainForm.textBoxLog.Text = data;
            JsonPollMessage poll = (JsonPollMessage)JsonConvert.DeserializeObject(data, typeof(JsonPollMessage));
            if (poll.retcode != 0)
                Message_Process_Error(poll);
            else if (poll.result != null && poll.result.Count > 0)
                for (int i = 0; i < poll.result.Count; i++)
                {
                    switch (poll.result[i].poll_type)
                    {
                        case "kick_message":
                            Running = false;
                            MessageBox.Show(poll.result[i].value.reason);
                            break;
                        case "message":
                            Message_Process_Message(poll.result[i].value);
                            break;
                        case "group_message":
                            Message_Process_GroupMessage(poll.result[i].value);
                            break;
                        case "discu_message":
                            Message_Process_DisscussMessage(poll.result[i].value);
                            break;
                        default:
                            Program.MainForm.listBoxLog.Items.Add(poll.result[i].poll_type);
                            break;
                    }
                }
        }
        /// <summary>
        /// 处理poll包中的消息数组
        /// </summary>
        /// <param name="content">消息数组</param>
        /// <returns></returns>
        private static string Message_Process_GetMessageText(List<object> content)
        {
            string message = "";
            for (int i = 1; i < content.Count; i++)
            {
                if (content[i].ToString().Contains("[\"cface\","))
                    continue;
                else if (content[i].ToString().Contains("\"face\","))
                    message += ("{..[face" + content[i].ToString().Replace("\"face\",", "").Replace("]", "").Replace("[", "").Replace(" ", "").Replace("\r", "").Replace("\n", "") + "]..}");
                else
                    message += content[i].ToString();
            }
            message = message.Replace("\\\\n", Environment.NewLine).Replace("＆", "&");
            return message;
        }
        /// <summary>
        /// 私聊消息处理
        /// </summary>
        /// <param name="value">poll包中的value</param>
        private static void Message_Process_Message(JsonPollMessage.paramResult.paramValue value)
        {
            string message = Message_Process_GetMessageText(value.content);
            string nick = "未知";
            if (!FriendList.ContainsKey(value.from_uin))
                Info_FriendList();
            if (FriendList.ContainsKey(value.from_uin))
                nick = FriendList[value.from_uin].nick;
            Program.MainForm.AddTextToTextBoxResiveMessage(nick + "  " + Info_RealQQ(value.from_uin) + Environment.NewLine + message);
            RuiRui.AnswerMessage(value.from_uin, message, 0);
        }
        /// <summary>
        /// 群聊消息处理
        /// </summary>
        /// <param name="value">poll包中的value</param>
        private static void Message_Process_GroupMessage(JsonPollMessage.paramResult.paramValue value)
        {
            string message = Message_Process_GetMessageText(value.content);
            string gid = value.from_uin;
            string gno = AID_GroupKey(gid);
            if (gno.Equals("FAIL"))
                return;
            string nick = "未知";
            if (GroupList[gid].MemberList.ContainsKey(value.send_uin))
                nick = GroupList[gid].MemberList[value.send_uin].nick;
            if (Info_RealQQ(value.send_uin).Equals("1000000"))
                nick = "系统消息";
            Program.MainForm.AddTextToTextBoxResiveMessage(GroupList[gid].name + "   " + nick + "  " + Info_RealQQ(value.send_uin) + Environment.NewLine + message);

            RuiRui.AnswerGroupMessage(gid, message, value.send_uin, gno);
        }

        /// <summary>
        /// 讨论组消息处理
        /// </summary>
        /// <param name="value">poll包中的value</param>
        private static void Message_Process_DisscussMessage(JsonPollMessage.paramResult.paramValue value)
        {
            string message = Message_Process_GetMessageText(value.content);
            string DName = "讨论组";
            string SenderNick = "未知";
            if (!DisscussList.ContainsKey(value.did))
                Info_DisscussList();
            if (DisscussList.ContainsKey(value.did))
            {
                DName += DisscussList[value.did].name;
                if (DisscussList[value.did].MemberList.ContainsKey(value.send_uin))
                    SenderNick = DisscussList[value.did].MemberList[value.send_uin].nick;
            }
            else DName = "未知讨论组";
            if (Info_RealQQ(value.send_uin).Equals("1000000"))
                SenderNick = "系统消息";
            Program.MainForm.AddTextToTextBoxResiveMessage(DName + "   " + SenderNick + "  " + Info_RealQQ(value.send_uin) + Environment.NewLine + message);
            RuiRui.AnswerMessage(value.did, message, 2);
        }
        /// <summary>
        /// 错误信息处理
        /// </summary>
        /// <param name="poll">poll包</param>
        private static int Count103 = 0;
        private static void Message_Process_Error(JsonPollMessage poll)
        {
            int TempCount103 = Count103;
            Count103 = 0;
            if (poll.retcode == 102)
            {
                return;
            }
            else if (poll.retcode == 103)
            {
                Program.MainForm.listBoxLog.Items.Insert(0, "retcode:103");
                Count103 = TempCount103 + 1;
                if (Count103 > 20)
                {
                    Running = false;
                    MessageBox.Show("retcode:" + poll.retcode);
                }
                return;
            }
            else if (poll.retcode == 116)
            {
                Program.MainForm.listBoxLog.Items.Insert(0, "retcode:" + poll.retcode + poll.p);
                ptwebqq = poll.p;
                return;
            }
            else if (poll.retcode == 108 || poll.retcode == 114)
            {
                Program.MainForm.listBoxLog.Items.Insert(0, "retcode:" + poll.retcode);
                Running = false;
                MessageBox.Show("retcode:" + poll.retcode);
                return;
            }
            else if (poll.retcode == 120 || poll.retcode == 121)
            {
                Program.MainForm.listBoxLog.Items.Insert(0, "retcode:" + poll.retcode);
                Program.MainForm.listBoxLog.Items.Insert(0, poll.t);
                Running = false;
                MessageBox.Show("retcode:" + poll.retcode);
                return;
            }
            else if (poll.retcode == 100006 || poll.retcode == 100003)
            {
                Program.MainForm.listBoxLog.Items.Insert(0, "retcode:" + poll.retcode);
                Running = false;
                MessageBox.Show("retcode:" + poll.retcode);
                return;
            }
            Program.MainForm.listBoxLog.Items.Insert(0, "retcode:" + poll.retcode);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="type">接受者类型：0，用户；1，群；2，讨论组</param>
        /// <param name="id">用户：uid；群：qid；讨论组：did</param>
        /// <param name="messageToSend">要发送的消息</param>
        /// <returns></returns>
        internal static bool Message_Send(int type, string id, string messageToSend)
        {
            Program.MainForm.listBoxLog.Items.Add(type + ":" + id + ":" + messageToSend);
            if (messageToSend.Equals("") || id.Equals(""))
                return false;

            string[] tmp = messageToSend.Split("{}".ToCharArray());
            messageToSend = "";
            for (int i = 0; i < tmp.Length; i++)
                if (!tmp[i].Trim().StartsWith("..[face") || !tmp[i].Trim().EndsWith("].."))
                    messageToSend += "\\\"" + tmp[i] + "\\\",";
                else
                    messageToSend += tmp[i].Replace("..[face", "[\\\"face\\\",").Replace("]..", "],");
            messageToSend = messageToSend.Remove(messageToSend.LastIndexOf(','));
            messageToSend = messageToSend.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
            try
            {
                string to_groupuin_did, url;
                switch (type)
                {
                    case 0:
                        to_groupuin_did = "to";
                        url = "http://d1.web2.qq.com/channel/send_buddy_msg2";
                        break;
                    case 1:
                        to_groupuin_did = "group_uin";
                        url = "http://d1.web2.qq.com/channel/send_qun_msg2";
                        break;
                    case 2:
                        to_groupuin_did = "did";
                        url = "http://d1.web2.qq.com/channel/send_discu_msg2";
                        break;
                    default:
                        return false;
                }
                string postData = "{\"#{type}\":#{id},\"content\":\"[#{msg},[\\\"font\\\",{\\\"name\\\":\\\"宋体\\\",\\\"size\\\":10,\\\"style\\\":[0,0,0],\\\"color\\\":\\\"000000\\\"}]]\",\"face\":522,\"clientid\":53999199,\"msg_id\":65890001,\"psessionid\":\"#{psessionid}\"}";
                postData = "r=" + HttpUtility.UrlEncode(postData.Replace("#{type}", to_groupuin_did).Replace("#{id}", id).Replace("#{msg}", messageToSend).Replace("#{psessionid}", psessionid));

                string dat = HTTP.Post(url, postData, "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");

                return dat.Equals("{\"errCode\":0,\"msg\":\"send ok\"}");
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取好友列表并保存
        /// </summary>
        internal static void Info_FriendList()
        {
            string url = "http://s.web2.qq.com/api/get_user_friends2";
            string sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", vfwebqq, hash);
            string dat = HTTP.Post(url, sendData, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");

            JsonFriendModel friend = (JsonFriendModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendModel));
            for (int i = 0; i < friend.result.info.Count; i++)
            {
                if (!FriendList.ContainsKey(friend.result.info[i].uin))
                    FriendList.Add(friend.result.info[i].uin, new FriendInfo());
                FriendList[friend.result.info[i].uin].face = friend.result.info[i].face;
                FriendList[friend.result.info[i].uin].nick = friend.result.info[i].nick;
                Info_FriendInfo(friend.result.info[i].uin);
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
            Program.MainForm.ReNewListBoxFriend();
        }
        /// <summary>
        /// 获取好友的详细信息
        /// </summary>
        /// <param name="uin"></param>
        internal static void Info_FriendInfo(string uin)
        {
            string url = "http://s.web2.qq.com/api/get_friend_info2?tuin=#{uin}&vfwebqq=#{vfwebqq}&clientid=53999199&psessionid=#{psessionid}&t=0.1";
            url = url.Replace("#{uin}", uin).Replace("#{vfwebqq}", vfwebqq).Replace("#{psessionid}", psessionid);
            string dat = HTTP.Get(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
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
            if (inf.result.birthday.year != 0 && inf.result.birthday.month != 0 && inf.result.birthday.day != 0)
                FriendList[uin].birthday = new DateTime(inf.result.birthday.year, inf.result.birthday.month, inf.result.birthday.day);
        }
        /// <summary>
        /// 获取自己的信息
        /// </summary>
        internal static void Info_SelfInfo()
        {
            string url = "http://s.web2.qq.com/api/get_self_info2&t=0.1";
            string dat = HTTP.Get(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
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
            if (inf.result.birthday.year != 0 && inf.result.birthday.month != 0 && inf.result.birthday.day != 0)
                SelfInfo.birthday = new DateTime(inf.result.birthday.year, inf.result.birthday.month, inf.result.birthday.day);
        }
        /// <summary>
        /// 获取群列表并保存
        /// </summary>
        internal static void Info_GroupList()
        {
            string url = "http://s.web2.qq.com/api/get_group_name_list_mask2";
            string sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", vfwebqq, hash);
            string dat = HTTP.Post(url, sendData, "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");

            JsonGroupModel group = (JsonGroupModel)JsonConvert.DeserializeObject(dat, typeof(JsonGroupModel));
            for (int i = 0; i < group.result.gnamelist.Count; i++)
            {
                if (!GroupList.ContainsKey(group.result.gnamelist[i].gid))
                    GroupList.Add(group.result.gnamelist[i].gid, new GroupInfo());
                GroupList[group.result.gnamelist[i].gid].name = group.result.gnamelist[i].name;
                GroupList[group.result.gnamelist[i].gid].code = group.result.gnamelist[i].code;
                Info_GroupInfo(group.result.gnamelist[i].gid);
                RuiRui.GetGroupSetting(group.result.gnamelist[i].gid);
            }
            Program.MainForm.ReNewListBoxGroup();
        }
        /// <summary>
        /// 获取群的详细信息
        /// </summary>
        /// <param name="gid"></param>
        internal static void Info_GroupInfo(string gid)
        {
            if (!GroupList.ContainsKey(gid))
                return;
            string gcode = GroupList[gid].code;
            string url = "http://s.web2.qq.com/api/get_group_info_ext2?gcode=#{group_code}&vfwebqq=#{vfwebqq}&t=0.1".Replace("#{group_code}", gcode).Replace("#{vfwebqq}", vfwebqq);
            string dat = HTTP.Get(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");
            JsonGroupInfoModel groupInfo = (JsonGroupInfoModel)JsonConvert.DeserializeObject(dat, typeof(JsonGroupInfoModel));
            GroupList[gid].name = groupInfo.result.ginfo.name;
            GroupList[gid].createtime = groupInfo.result.ginfo.createtime;
            GroupList[gid].face = groupInfo.result.ginfo.face;
            GroupList[gid].owner = groupInfo.result.ginfo.owner;
            GroupList[gid].memo = groupInfo.result.ginfo.memo;
            GroupList[gid].markname = groupInfo.result.ginfo.markname;
            GroupList[gid].level = groupInfo.result.ginfo.level;
            for (int i = 0; i < groupInfo.result.minfo.Count; i++)
            {
                if (!GroupList[gid].MemberList.ContainsKey(groupInfo.result.minfo[i].uin))
                    GroupList[gid].MemberList.Add(groupInfo.result.minfo[i].uin, new GroupInfo.MenberInfo());
                GroupList[gid].MemberList[groupInfo.result.minfo[i].uin].city = groupInfo.result.minfo[i].city;
                GroupList[gid].MemberList[groupInfo.result.minfo[i].uin].province = groupInfo.result.minfo[i].province;
                GroupList[gid].MemberList[groupInfo.result.minfo[i].uin].country = groupInfo.result.minfo[i].country;
                GroupList[gid].MemberList[groupInfo.result.minfo[i].uin].gender = groupInfo.result.minfo[i].gender;
                GroupList[gid].MemberList[groupInfo.result.minfo[i].uin].nick = groupInfo.result.minfo[i].nick;
            }
            if (groupInfo.result.cards != null)
                for (int i = 0; i < groupInfo.result.cards.Count; i++)
                {
                    if (!GroupList[gid].MemberList.ContainsKey(groupInfo.result.cards[i].muin))
                        GroupList[gid].MemberList.Add(groupInfo.result.cards[i].muin, new GroupInfo.MenberInfo());
                    GroupList[gid].MemberList[groupInfo.result.cards[i].muin].card = groupInfo.result.cards[i].card;
                }
            for (int i = 0; i < groupInfo.result.ginfo.members.Count; i++)
                if (groupInfo.result.ginfo.members[i].mflag % 2 == 1)
                    GroupList[gid].MemberList[groupInfo.result.ginfo.members[i].muin].isManager = true;
                else GroupList[gid].MemberList[groupInfo.result.ginfo.members[i].muin].isManager = false;
        }
        /// <summary>
        /// 获取讨论组列表并保存
        /// </summary>
        internal static void Info_DisscussList()
        {
            string url = "http://s.web2.qq.com/api/get_discus_list?clientid=53999199&psessionid=#{psessionid}&vfwebqq=#{vfwebqq}&t=0.1".Replace("#{psessionid}", psessionid).Replace("#{vfwebqq}", vfwebqq);
            string dat = HTTP.Get(url, "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");
            JsonDisscussModel disscuss = (JsonDisscussModel)JsonConvert.DeserializeObject(dat, typeof(JsonDisscussModel));
            for (int i = 0; i < disscuss.result.dnamelist.Count; i++)
            {
                if (!DisscussList.ContainsKey(disscuss.result.dnamelist[i].did))
                    DisscussList.Add(disscuss.result.dnamelist[i].did, new DiscussInfo());
                DisscussList[disscuss.result.dnamelist[i].did].name = disscuss.result.dnamelist[i].name;
                Info_DisscussInfo(disscuss.result.dnamelist[i].did);
            }
        }
        /// <summary>
        /// 获取讨论组详细信息
        /// </summary>
        /// <param name="did"></param>
        internal static void Info_DisscussInfo(string did)
        {
            string url = "http://d1.web2.qq.com/channel/get_discu_info?did=#{discuss_id}&psessionid=#{psessionid}&vfwebqq=#{vfwebqq}&clientid=53999199&t=0.1";
            url = url.Replace("#{discuss_id}", did).Replace("#{psessionid}", psessionid).Replace("#{vfwebqq}", vfwebqq);
            string dat = HTTP.Get(url, "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");
            JsonDisscussInfoModel inf = (JsonDisscussInfoModel)JsonConvert.DeserializeObject(dat, typeof(JsonDisscussInfoModel));

            for (int i = 0; i < inf.result.mem_info.Count; i++)
            {
                if (!DisscussList[did].MemberList.ContainsKey(inf.result.mem_info[i].uin))
                    DisscussList[did].MemberList.Add(inf.result.mem_info[i].uin, new DiscussInfo.MenberInfo());
                DisscussList[did].MemberList[inf.result.mem_info[i].uin].nick = inf.result.mem_info[i].nick;
            }
            for (int i = 0; i < inf.result.mem_status.Count; i++)
            {
                if (!DisscussList[did].MemberList.ContainsKey(inf.result.mem_status[i].uin))
                    DisscussList[did].MemberList.Add(inf.result.mem_status[i].uin, new DiscussInfo.MenberInfo());
                DisscussList[did].MemberList[inf.result.mem_status[i].uin].status = inf.result.mem_status[i].status;
                DisscussList[did].MemberList[inf.result.mem_status[i].uin].client_type = inf.result.mem_status[i].client_type;
            }
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
            string dat = HTTP.Get(url);
            string temp = dat.Split('\"')[10].Split(',')[0].Replace(":", "");
            if (temp != "")
            {
                RealQQNum.Add(uin, temp);
                return temp;
            }
            else return "";
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
        /// 生成由群主QQ和群创建世界构成的群标识码
        /// </summary>
        /// <param name="gid"></param>
        /// <returns></returns>
        internal static string AID_GroupKey(string gid)
        {
            if (!GroupList.ContainsKey(gid))
                Info_GroupList();
            if (GroupList.ContainsKey(gid))
                return Info_RealQQ(GroupList[gid].owner) + ":" + GroupList[gid].createtime;
            else return "FAIL";
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
            public string memo;
            public int face;
            public string createtime;
            public int level;
            public string owner;
            public GroupManageClass GroupManage = new GroupManageClass();
            public class GroupManageClass
            {
                public string enable;
                public string enableWeather;
                public string enableExchangeRate;
                public string enableStock;
                public string enableStudy;
                public string enableTalk;
                public string enableXHJ;
                public string enableEmoje;
                public string enableCityInfo;
                public string enableWiki;
                public string enableTranslate;
            }
            public Dictionary<string, MenberInfo> MemberList = new Dictionary<string, MenberInfo>();
            public class MenberInfo
            {
                public string nick;
                public string country;
                public string province;
                public string city;
                public string gender;
                public string card;
                public bool isManager;
            }
        }
        /// <summary>
        /// 讨论组资料类
        /// </summary>
        public class DiscussInfo
        {
            public string name;
            public Dictionary<string, MenberInfo> MemberList = new Dictionary<string, MenberInfo>();
            public class MenberInfo
            {
                public string nick;
                public string status;
                public int client_type;
            }
        }
    }
}
