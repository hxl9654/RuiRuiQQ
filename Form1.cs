using Jurassic;
using Jurassic.Library;
using Newtonsoft.Json;
using System;
using System.IO;
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

    public partial class FormLogin : Form
    {
        //系统配置相关
        string MasterQQ = "";
        string DicPassword = "";
        string DicServer = "";
        bool NoDicPassword = false;
        public bool StopSendingHeartPack = false;
        //多个函数要用到的变量
        bool Loging = false;
        string pin = string.Empty;
        bool IsGroupSelent = false, IsFriendSelent = false;
        bool DoNotChangeSelentGroupOrPeople = false;
        //数据存储相关
        public JsonGroupModel group;
        public JsonFriendModel user;
        private int Count103 = 0;
        public struct GroupInf
        {
            public string gid;
            public string no;
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
            public JsonGroupInfoModel inf;
            public string[] managers;
            public int GroupManagerIndex;
        };
        public GroupInf[] groupinfo = new GroupInf[200];
        public int groupinfMaxIndex = 0;
        public struct FriendInf
        {
            public string uin;
            public JsonFriendInfModel Inf;
        };
        public FriendInf[] friendinf = new FriendInf[1000];
        public int friendinfMaxIndex = 0;
        string[] Badwords;

        private void LogIn()
        {
            if (textBoxID.Text.Length == 0)
            {
                MessageBox.Show("账号不能为空");
                return;
            }
            for (int i = 0; i < textBoxID.Text.Length; i++)
            {
                if (textBoxID.Text.ToCharArray()[i] < '0' || textBoxID.Text.ToCharArray()[i] > '9')
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
            if (Loging)
                return;
            Loging = true;
            SmartQQ.FirstLogin(textBoxID.Text, textBoxPassword.Text, textBoxCAPTCHA.Text);
            SmartQQ.SecondLogin(textBoxID.Text);

            SmartQQ.getFrienf();
            SmartQQ.getGroup();

            HTTP.HttpGet("https://ruiruiqq.hxlxz.com/infreport.php?qq=" + textBoxID.Text + "&adminqq=" + MasterQQ);

            listBoxLog.Items.Insert(0, "账号" + textBoxID.Text + "登录成功");
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
            AcceptButton = buttonSend;
            Loging = false;
        }

        public void HeartPackAction(string temp)
        {
            string GName = "";
            string MessageFromUin = "";
            textBoxLog.Text = temp;
            JsonHeartPackMessage HeartPackMessage = (JsonHeartPackMessage)JsonConvert.DeserializeObject(temp, typeof(JsonHeartPackMessage));
            int TempCount103 = Count103;
            Count103 = 0;
            if (HeartPackMessage.retcode == 102)
            {
                return;
            }
            else if (HeartPackMessage.retcode == 103)
            {
                listBoxLog.Items.Insert(0, temp);
                Count103 = TempCount103 + 1;
                if (Count103 > 20)
                    ReLogin();
                return;
            }
            else if (HeartPackMessage.retcode == 116)
            {
                listBoxLog.Items.Insert(0, temp);
                SmartQQ.ptwebqq = HeartPackMessage.p;
                return;
            }
            else if (HeartPackMessage.retcode == 108 || HeartPackMessage.retcode == 114)
            {
                listBoxLog.Items.Insert(0, temp);
                ReLogin();
                return;
            }
            else if (HeartPackMessage.retcode == 120 || HeartPackMessage.retcode == 121)
            {
                listBoxLog.Items.Insert(0, temp);
                listBoxLog.Items.Insert(0, HeartPackMessage.t);
                ReLogin();
                return;
            }
            else if (HeartPackMessage.retcode == 100006 || HeartPackMessage.retcode == 100003)
            {
                listBoxLog.Items.Insert(0, temp);
                ReLogin();
                return;
            }
            listBoxLog.Items.Insert(0, temp);

            if (HeartPackMessage.retcode != 0 || HeartPackMessage.result == null)
                return;
            for (int i = 0; i < HeartPackMessage.result.Count; i++)
            {
                if (HeartPackMessage.result[i].poll_type == "buddies_status_change")
                {
                    return;
                }
                else if (HeartPackMessage.result[i].poll_type == "kick_message")
                {
                    ReLogin();
                    listBoxLog.Items.Add(HeartPackMessage.result[i].value.reason);
                    return;
                }
                else if (HeartPackMessage.result[i].poll_type == "message")
                {
                    string message = "";
                    string emojis = "";
                    int j;
                    for (j = 1; j < HeartPackMessage.result[i].value.content.Count; j++)
                    {
                        string temp1 = HeartPackMessage.result[i].value.content[j].ToString();
                        temp1 = temp1.Replace(Environment.NewLine, "");
                        temp1 = temp1.Replace(" ", "");
                        if (temp1.Contains("[\"cface\","))
                            continue;
                        if (temp1.Contains("[\"face\","))
                        {
                            string emojiID = temp1.Replace("[\"face\",", "");
                            emojiID = emojiID.Replace("]", "");
                            emojis += (emojiID + ",");
                        }
                        else message += (HeartPackMessage.result[i].value.content[j].ToString() + " ");
                    }
                    message = message.Replace("\\\\n", Environment.NewLine);
                    message = message.Replace("＆", "&");
                    for (j = 0; j < user.result.info.Count; j++)
                        if (user.result.info[j].uin == HeartPackMessage.result[i].value.from_uin)
                        {
                            textBoxResiveMessage.Text += (user.result.info[j].nick + "  " + SmartQQ.GetRealQQ(user.result.info[j].uin) + Environment.NewLine + message + "   " + emojis + Environment.NewLine + Environment.NewLine);
                            textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
                            textBoxResiveMessage.ScrollToCaret();
                            break;
                        }
                    if (j == user.result.info.Count)
                    {
                        SmartQQ.getFrienf();
                        for (j = 0; j < user.result.info.Count; j++)
                            if (user.result.info[j].uin == HeartPackMessage.result[i].value.from_uin)
                            {
                                textBoxResiveMessage.Text += (user.result.info[j].nick + "  " + SmartQQ.GetRealQQ(user.result.info[j].uin) + Environment.NewLine + message + "   " + emojis + Environment.NewLine + Environment.NewLine);
                                textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
                                textBoxResiveMessage.ScrollToCaret();
                                break;
                            }
                    }
                    ActionWhenResivedMessage(HeartPackMessage.result[i].value.from_uin, message, emojis);
                }
                else if (HeartPackMessage.result[i].poll_type == "group_message")
                {
                    string emojis = "";
                    string message = "";
                    int j;
                    for (j = 1; j < HeartPackMessage.result[i].value.content.Count; j++)
                    {
                        string temp1 = HeartPackMessage.result[i].value.content[j].ToString();
                        temp1 = temp1.Replace(Environment.NewLine, "");
                        temp1 = temp1.Replace(" ", "");
                        if (temp1.Contains("[\"cface\","))
                            continue;
                        if (temp1.Contains("[\"face\","))
                        {
                            string emojiID = temp1.Replace("[\"face\",", "");
                            emojiID = emojiID.Replace("]", "");
                            emojis += (emojiID + ",");
                        }
                        else message += (HeartPackMessage.result[i].value.content[j].ToString() + " ");
                    }
                    message = message.Replace("\\\\n", Environment.NewLine);
                    message = message.Replace("＆", "&");
                    string gno = HeartPackMessage.result[i].value.info_seq;
                    string gid = HeartPackMessage.result[i].value.from_uin;
                    for (j = 0; j < group.result.gnamelist.Count; j++)
                        if (group.result.gnamelist[j].gid == gid)
                        {
                            GName = group.result.gnamelist[j].name;
                            break;
                        }
                    for (j = 0; ; j++)
                    {
                        if (j > groupinfMaxIndex)
                        {
                            SmartQQ.getGroup();
                        }
                        if (j > groupinfMaxIndex)
                        {
                            break;
                        }
                        if (groupinfo[j].gid == gid)
                        {
                            for (int k = 0; k < groupinfo[j].inf.result.minfo.Count; k++)
                                if (groupinfo[j].inf.result.minfo[k].uin == HeartPackMessage.result[i].value.send_uin)
                                {
                                    MessageFromUin = groupinfo[j].inf.result.minfo[k].uin;
                                    textBoxResiveMessage.Text += (GName + "   " + groupinfo[j].inf.result.minfo[k].nick + "  " + SmartQQ.GetRealQQ(MessageFromUin) + Environment.NewLine + message + "   " + emojis + Environment.NewLine + Environment.NewLine);
                                    textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
                                    textBoxResiveMessage.ScrollToCaret();
                                    break;
                                }
                            break;
                        }
                    }
                    ActionWhenResivedGroupMessage(gid, message, emojis, MessageFromUin, gno);
                }
                textBoxLog.Text = temp;
            }
        }

        private string SloveEmoji(string emojiID)
        {
            string temp;
            temp = "[\\\"face\\\"," + emojiID + "]";
            return temp;
        }
        private string[] Answer(string message, string uin, string gid = "", string gno = "")
        {
            string qunnum = gno;
            if (qunnum.Equals(""))
                qunnum = "NULL";
            string[] MessageToSend = new string[20];
            message = message.Remove(message.Length - 2);
            if (message.Equals(""))
                return MessageToSend;
            string QQNum = SmartQQ.GetRealQQ(uin);
            for (int i = 0; i < 20; i++)
                MessageToSend[i] = "";
            bool MsgSendFlag = false;
            if (message.Equals("源码") || message.Equals("作者") || message.Equals("代码"))
            {
                MessageToSend[0] = "本程序作者是何相龙，网站：https://tec.hxlxz.com 。本程序采用GPL v3许可证授权，源码获取地址：https://github.com/hxl9654/RuiRuiQQ";
                return MessageToSend;
            }
            if (message.Equals("帮助") || message.Equals("说明") || message.Equals("使用说明"))
            {
                MessageToSend[0] = "本程序使用说明请参见：https://github.com/hxl9654/RuiRuiQQ/wiki";
                return MessageToSend;
            }
            int j = 0;
            int GroupInfoIndex = -1;
            if (!gid.Equals(""))
            {
                int i = -1;
                string adminuin = "";
                for (i = 0; i <= groupinfMaxIndex; i++)
                {
                    if (groupinfo[i].gid == gid)
                    {
                        GroupInfoIndex = i;
                        adminuin = groupinfo[i].inf.result.ginfo.owner;
                        break;
                    }
                }
            }
            if (message.StartsWith("行情"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (groupinfo[GroupInfoIndex].enableStock == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (groupinfo[GroupInfoIndex].enableStock.Equals("false"))
                        DisableFlag = true;
                }
                if (!DisableFlag)
                {
                    bool StockFlag = true;
                    string[] tmp = message.Split('&');
                    if ((!tmp[0].Equals("行情")) || (tmp.Length != 2 && tmp.Length != 3))
                    {
                        StockFlag = false;
                    }
                    if (StockFlag)
                    {
                        if (tmp.Length == 2)
                            MessageToSend[0] = GetInfo.GetStock(tmp[1], "");
                        else MessageToSend[0] = GetInfo.GetStock(tmp[1], tmp[2]);

                        string url = DicServer + "log.php";
                        string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(qunnum) + "&action=stock&p1=" + HttpUtility.UrlEncode(tmp[tmp.Length - 1]) + "&p2=NULL&p3=NULL&p4=NULL";
                        HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                        return MessageToSend;
                    }
                }
            }
            if (message.StartsWith("汇率"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (groupinfo[GroupInfoIndex].enableExchangeRate == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (groupinfo[GroupInfoIndex].enableExchangeRate.Equals("false"))
                        DisableFlag = true;
                }
                if (!DisableFlag)
                {
                    bool ExchangeRateFlag = true;
                    string[] tmp = message.Split('&');
                    if ((!tmp[0].Equals("汇率")) || tmp.Length != 3)
                    {
                        ExchangeRateFlag = false;
                    }
                    if (ExchangeRateFlag)
                    {
                        MessageToSend[0] = GetInfo.GetExchangeRate(tmp[1], tmp[2]);

                        string url = DicServer + "log.php";
                        string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(qunnum) + "&action=exchangerate&p1=" + HttpUtility.UrlEncode(tmp[1]) + "&p2=" + HttpUtility.UrlEncode(tmp[2]) + "&p3=NULL&p4=NULL";
                        HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                        return MessageToSend;
                    }
                }
            }
            if (message.StartsWith("天气"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (groupinfo[GroupInfoIndex].enableWeather == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (groupinfo[GroupInfoIndex].enableWeather.Equals("false"))
                        DisableFlag = true;
                }
                if (!DisableFlag)
                {
                    bool WeatherFlag = true;
                    string[] tmp = message.Split('&');
                    if ((!tmp[0].Equals("天气")) || (tmp.Length != 2 && tmp.Length != 3))
                    {
                        WeatherFlag = false;
                    }
                    if (WeatherFlag)
                    {
                        if (tmp.Length == 2)
                            MessageToSend[0] = GetInfo.GetWeather(tmp[1], "");
                        else
                            MessageToSend[0] = GetInfo.GetWeather(tmp[1], tmp[2]);

                        string url = DicServer + "log.php";
                        string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(qunnum) + "&action=weather&p1=" + HttpUtility.UrlEncode(tmp[1]) + "&p2=NULL&p3=NULL&p4=NULL";
                        HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                        return MessageToSend;
                    }
                }
            }
            if (message.StartsWith("城市信息"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (groupinfo[GroupInfoIndex].enableCityInfo == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (groupinfo[GroupInfoIndex].enableCityInfo.Equals("false"))
                        DisableFlag = true;
                }
                if (!DisableFlag)
                {
                    bool CityInfoFlag = true;
                    string[] tmp = message.Split('&');
                    if ((!tmp[0].Equals("城市信息")) || (tmp.Length != 2 && tmp.Length != 3))
                    {
                        CityInfoFlag = false;
                    }
                    if (CityInfoFlag)
                    {
                        if (tmp.Length == 2)
                            MessageToSend[0] = GetInfo.GetCityInfo(tmp[1], "");
                        else
                            MessageToSend[0] = GetInfo.GetCityInfo(tmp[1], tmp[2]);

                        string url = DicServer + "log.php";
                        string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(qunnum) + "&action=cityinfo&p1=" + HttpUtility.UrlEncode(tmp[1]) + "&p2=NULL&p3=NULL&p4=NULL";
                        HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                        return MessageToSend;
                    }
                }
            }
            if (message.StartsWith("百科"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (groupinfo[GroupInfoIndex].enableWiki == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (groupinfo[GroupInfoIndex].enableWiki.Equals("false"))
                        DisableFlag = true;
                }
                if (!DisableFlag)
                {
                    bool WikiFlag = true;
                    string[] tmp = message.Split('&');
                    if ((!tmp[0].Equals("百科")) || (tmp.Length != 2) && tmp.Length != 3)
                    {
                        WikiFlag = false;
                    }
                    if (WikiFlag)
                    {
                        if (tmp.Length == 2)
                            MessageToSend[0] = GetInfo.GetWiki(tmp[1], "");
                        else
                            MessageToSend[0] = GetInfo.GetWiki(tmp[1], tmp[2]);

                        string url = DicServer + "log.php";
                        string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(qunnum) + "&action=wiki&p1=" + HttpUtility.UrlEncode(tmp[1]) + "&p2=NULL&p3=NULL&p4=NULL";
                        HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                        return MessageToSend;
                    }
                }
            }
            if (message.StartsWith("学习") || message.StartsWith("特权学习"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (groupinfo[GroupInfoIndex].enableStudy == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (groupinfo[GroupInfoIndex].enableStudy.Equals("false"))
                        DisableFlag = true;
                }
                if (!DisableFlag)
                {
                    bool StudyFlag = true;
                    bool SuperStudy = false;
                    string[] tmp = message.Split('^');
                    if ((!tmp[0].Equals("学习")) || tmp.Length != 3)
                    {
                        if ((tmp[0].Equals("特权学习")) && tmp.Length == 3)
                        {
                            SuperStudy = true;
                        }
                        tmp = message.Split('&');
                        if ((!tmp[0].Equals("学习")) || tmp.Length != 3)
                        {
                            StudyFlag = false;
                            if ((tmp[0].Equals("特权学习")) && tmp.Length == 3)
                            {
                                SuperStudy = true;
                            }
                        }
                    }
                    if (tmp.Length != 3 || tmp[1].Replace(" ", "").Equals("") || tmp[2].Replace(" ", "").Equals(""))
                    {
                        StudyFlag = false;
                        SuperStudy = false;
                    }
                    if (SuperStudy)
                    {
                        string result = "";
                        result = AIStudy(tmp[1], tmp[2], QQNum, gno, true);
                        MessageToSend[0] = GetInfo.GetStudyFlagInfo(result, QQNum, tmp[1], tmp[2]);
                        return MessageToSend;
                    }
                    if (StudyFlag)
                    {
                        string result = "";
                        for (int i = 0; i < Badwords.Length; i++)
                            if (tmp[1].Contains(Badwords[i]) || tmp[2].Contains(Badwords[i]))
                            {
                                result = "ForbiddenWord";
                                break;
                            }
                        if (result.Equals(""))
                            result = AIStudy(tmp[1], tmp[2], QQNum, gno, false);
                        MessageToSend[0] = GetInfo.GetStudyFlagInfo(result, QQNum, tmp[1], tmp[2]);

                        string url = DicServer + "log.php";
                        string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(qunnum) + "&action=study&p1=" + HttpUtility.UrlEncode(tmp[1]) + "&p2=" + HttpUtility.UrlEncode(tmp[2]) + "&p3=NULL&p4=NULL";
                        HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                        return MessageToSend;
                    }
                }
            }
            bool DisableTalkFlag = false;
            if (!gid.Equals(""))
            {
                if (groupinfo[GroupInfoIndex].enableTalk == null)
                    GetGroupSetting(groupinfMaxIndex);
                if (groupinfo[GroupInfoIndex].enableTalk.Equals("false"))
                    DisableTalkFlag = true;
            }
            if (!DisableTalkFlag)
            {

                MessageToSend[0] = AIGet(message, QQNum, gno);
                if (!MessageToSend[0].Equals(""))
                {
                    string url = DicServer + "log.php";
                    string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(qunnum) + "&action=talk&p1=" + HttpUtility.UrlEncode(message) + "&p2=NULL&p3=NULL&p4=NULL";
                    HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                    return MessageToSend;
                }
                string[] tmp1 = message.Split("@#$(),，.。:：;^&；“”～~！!#（）%？?》《、· \r\n\"".ToCharArray());
                j = 0;
                bool RepeatFlag = false;
                for (int i = 0; i < tmp1.Length && i < 10; i++)
                {
                    if (tmp1[i].Equals(message))
                        continue;
                    for (int k = 0; k < i; k++)
                        if (tmp1[k].Equals(tmp1[i]))
                            RepeatFlag = true;
                    if (RepeatFlag)
                    {
                        RepeatFlag = false;
                        continue;
                    }
                    if (!tmp1[i].Equals(""))
                    {
                        MessageToSend[j] = AIGet(tmp1[i], QQNum, gno);
                        j++;
                        MsgSendFlag = true;
                    }
                }
                if (!MsgSendFlag)
                {
                    string[] tmp2 = message.Split("@#$(),，.。:：;^&；“”～~！!#（）%？?》《、· \r\n\"啊喔是的么吧呀恩嗯了呢很吗".ToCharArray());
                    j = 0;
                    RepeatFlag = false;
                    for (int i = 0; i < tmp2.Length && i < 10; i++)
                    {
                        if (tmp2[i].Equals(message))
                            continue;
                        for (int k = 0; k < i; k++)
                            if (tmp2[k].Equals(tmp2[i]))
                                RepeatFlag = true;
                        for (int k = 0; k < tmp1.Length; k++)
                            if (tmp1[k].Equals(tmp2[i]))
                                RepeatFlag = true;

                        if (RepeatFlag)
                        {
                            RepeatFlag = false;
                            continue;
                        }
                        if (!tmp2[i].Equals(""))
                        {
                            MessageToSend[j] = AIGet(tmp2[i], QQNum, gno);
                            j++;
                            MsgSendFlag = true;
                        }
                    }
                }

                if (!MsgSendFlag)
                {
                    bool DisableFlag = false;
                    if (!gid.Equals(""))
                    {
                        if (groupinfo[GroupInfoIndex].enableXHJ == null)
                            GetGroupSetting(groupinfMaxIndex);
                        if (groupinfo[GroupInfoIndex].enableXHJ.Equals("false"))
                            DisableFlag = true;
                    }
                    if (!DisableFlag)
                    {
                        string XiaoHuangJiMsg = GetXiaoHuangJi(message);
                        if (XiaoHuangJiMsg.Length > 1)
                        {
                            for (int i = 0; i < Badwords.Length; i++)
                                if (XiaoHuangJiMsg.Contains(Badwords[i]))
                                    return null;
                            MessageToSend[0] = "隔壁小黄鸡说：" + XiaoHuangJiMsg;

                        }
                        return MessageToSend;
                    }
                }
                if (!MessageToSend[0].Equals(""))
                {
                    string url = DicServer + "log.php";
                    string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(qunnum) + "&action=talk&p1=" + HttpUtility.UrlEncode(message) + "&p2=NULL&p3=NULL&p4=NULL";
                    HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                }
                return MessageToSend;
            }
            return MessageToSend;
        }

        private void SetGroupNum(int GroupInfoIndex, string gno, string gid)
        {
            groupinfo[GroupInfoIndex].no = gno;
            for (int j = 0; j < listBoxGroup.Items.Count; j++)
            {
                string[] tmp = listBoxGroup.Items[j].ToString().Split(':');
                if (tmp[0].Equals(gid))
                {
                    string temp = tmp[0] + ":" + gno;
                    for (int k = 2; k < tmp.Length; k++)
                    {
                        temp += ":" + tmp[k];
                    }
                    listBoxGroup.Items.Remove(listBoxGroup.Items[j]);
                    listBoxGroup.Items.Insert(0, temp);
                    break;
                }
            }
        }
        string GroupManage(string message, string uin, string gid, string gno)
        {
            string adminuin = "";
            int GroupInfoIndex = -1;
            string MessageToSend = "";
            int i;
            if (message.StartsWith("群管理"))
            {
                SmartQQ.getGroup();
                for (i = 0; i <= groupinfMaxIndex; i++)
                {
                    if (groupinfo[i].gid == gid)
                    {
                        GroupInfoIndex = i;
                        adminuin = groupinfo[i].inf.result.ginfo.owner;
                        break;
                    }
                }
                //获取管理员
                if (groupinfo[GroupInfoIndex].managers == null)
                {
                    groupinfo[GroupInfoIndex].inf = SmartQQ.GetGroupInfo(groupinfo[GroupInfoIndex].inf.result.ginfo.code);
                    groupinfo[GroupInfoIndex].managers = new string[30];

                    groupinfo[GroupInfoIndex].GroupManagerIndex = 0;
                    for (i = 0; i < groupinfo[GroupInfoIndex].inf.result.ginfo.members.Count; i++)
                    {
                        if (groupinfo[GroupInfoIndex].inf.result.ginfo.members[i].mflag % 2 == 1)
                        {
                            groupinfo[GroupInfoIndex].managers[groupinfo[GroupInfoIndex].GroupManagerIndex] = groupinfo[GroupInfoIndex].inf.result.ginfo.members[i].muin;
                            groupinfo[GroupInfoIndex].GroupManagerIndex++;
                        }
                    }
                }

                bool GroupManageFlag = true;
                string[] tmp = message.Split('&');
                tmp[1] = tmp[1].Replace("\r", "");
                tmp[1] = tmp[1].Replace("\n", "");
                tmp[1] = tmp[1].Replace(" ", "");
                if ((!tmp[0].Equals("群管理")) || tmp.Length != 2)
                {
                    GroupManageFlag = false;
                }
                if (GroupManageFlag)
                {
                    bool HaveRight = false;
                    if (uin.Equals(adminuin))
                    {
                        HaveRight = true;
                    }
                    if (SmartQQ.GetRealQQ(uin).Equals(MasterQQ))
                    {
                        HaveRight = true;
                    }
                    else
                    {
                        for (i = 0; i <= groupinfo[GroupInfoIndex].GroupManagerIndex; i++)
                        {
                            if (uin.Equals(groupinfo[GroupInfoIndex].managers[i]))
                            {
                                HaveRight = true;
                                break;
                            }
                        }
                    }
                    if (groupinfo[GroupInfoIndex].enable == null)
                    {
                        GetGroupSetting(GroupInfoIndex);
                    }
                    if (tmp.Length != 2 || tmp[1] == null)
                        return "";
                    if ((HaveRight || groupinfo[GroupInfoIndex].enable.Equals("true")) && (tmp[1].Equals("查询状态") || tmp[1].Equals("状态")))
                    {
                        MessageToSend = "机器人启动：" + groupinfo[GroupInfoIndex].enable + Environment.NewLine;
                        MessageToSend += "汇率查询启动：" + groupinfo[GroupInfoIndex].enableExchangeRate + Environment.NewLine;
                        MessageToSend += "百科查询启动：" + groupinfo[GroupInfoIndex].enableWiki + Environment.NewLine;
                        MessageToSend += "天气查询启动：" + groupinfo[GroupInfoIndex].enableWeather + Environment.NewLine;
                        MessageToSend += "城市信息查询启动：" + groupinfo[GroupInfoIndex].enableCityInfo + Environment.NewLine;
                        MessageToSend += "学习启动：" + groupinfo[GroupInfoIndex].enableStudy + Environment.NewLine;
                        MessageToSend += "行情查询启动：" + groupinfo[GroupInfoIndex].enableStock + Environment.NewLine;
                        MessageToSend += "闲聊启动：" + groupinfo[GroupInfoIndex].enableTalk + Environment.NewLine;
                        MessageToSend += "表情启动：" + groupinfo[GroupInfoIndex].enableEmoje + Environment.NewLine;
                        MessageToSend += "小黄鸡启动：" + groupinfo[GroupInfoIndex].enableXHJ;
                        return MessageToSend;
                    }
                    if (HaveRight == false)
                    {
                        MessageToSend = "账号" + SmartQQ.GetRealQQ(uin) + "不是群管理，无权进行此操作";
                        return MessageToSend;
                    }
                    else
                    {
                        tmp[1] = tmp[1].Replace("开启", "启动");
                        tmp[1] = tmp[1].Replace("开起", "启动");
                        if (tmp[1].Equals("启动机器人"))
                        {
                            if (groupinfo[GroupInfoIndex].enable.Equals("true"))
                            {
                                MessageToSend = "当前机器人已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enable", "true");

                                MessageToSend = "机器人启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭机器人"))
                        {
                            if (groupinfo[GroupInfoIndex].enable.Equals("false"))
                            {
                                MessageToSend = "当前机器人已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enable", "false");

                                MessageToSend = "机器人关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动城市信息查询"))
                        {
                            if (groupinfo[GroupInfoIndex].enableCityInfo.Equals("true"))
                            {
                                MessageToSend = "当前城市信息查询已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableCityInfo", "true");

                                MessageToSend = "城市信息查询启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭城市信息查询"))
                        {
                            if (groupinfo[GroupInfoIndex].enableCityInfo.Equals("false"))
                            {
                                MessageToSend = "当前城市信息查询已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableCityInfo", "false");

                                MessageToSend = "城市信息查询关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动天气查询"))
                        {
                            if (groupinfo[GroupInfoIndex].enableWeather.Equals("true"))
                            {
                                MessageToSend = "当前天气查询已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableWeather", "true");

                                MessageToSend = "天气查询启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭天气查询"))
                        {
                            if (groupinfo[GroupInfoIndex].enableWeather.Equals("false"))
                            {
                                MessageToSend = "当前天气查询已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableWeather", "false");

                                MessageToSend = "天气查询关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动百科查询"))
                        {
                            if (groupinfo[GroupInfoIndex].enableWiki.Equals("true"))
                            {
                                MessageToSend = "当前百科查询已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableWiki", "true");

                                MessageToSend = "百科查询启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭百科查询"))
                        {
                            if (groupinfo[GroupInfoIndex].enableWiki.Equals("false"))
                            {
                                MessageToSend = "当前百科查询已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableWiki", "false");

                                MessageToSend = "百科查询关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动汇率查询"))
                        {
                            if (groupinfo[GroupInfoIndex].enableExchangeRate.Equals("true"))
                            {
                                MessageToSend = "当前汇率查询已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableExchangeRate", "true");

                                MessageToSend = "汇率查询启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭汇率查询"))
                        {
                            if (groupinfo[GroupInfoIndex].enableExchangeRate.Equals("false"))
                            {
                                MessageToSend = "当前汇率查询已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableExchangeRate", "false");

                                MessageToSend = "汇率查询关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动行情查询"))
                        {
                            if (groupinfo[GroupInfoIndex].enableStock.Equals("true"))
                            {
                                MessageToSend = "当前行情查询已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableStock", "true");

                                MessageToSend = "行情查询启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭行情查询"))
                        {
                            if (groupinfo[GroupInfoIndex].enableStock.Equals("false"))
                            {
                                MessageToSend = "当前行情查询已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableStock", "false");

                                MessageToSend = "行情查询关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动聊天") || tmp[1].Equals("启动闲聊"))
                        {
                            if (groupinfo[GroupInfoIndex].enableTalk.Equals("true"))
                            {
                                MessageToSend = "当前聊天已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enabletalk", "true");

                                MessageToSend = "聊天启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭聊天") || tmp[1].Equals("关闭闲聊"))
                        {
                            if (groupinfo[GroupInfoIndex].enableTalk.Equals("false"))
                            {
                                MessageToSend = "当前聊天已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enabletalk", "false");

                                MessageToSend = "聊天关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动学习"))
                        {
                            if (groupinfo[GroupInfoIndex].enableStudy.Equals("true"))
                            {
                                MessageToSend = "当前学习已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableStudy", "true");

                                MessageToSend = "学习启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭学习"))
                        {
                            if (groupinfo[GroupInfoIndex].enableStudy.Equals("false"))
                            {
                                MessageToSend = "当前学习已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableStudy", "false");

                                MessageToSend = "学习关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动小黄鸡"))
                        {
                            if (groupinfo[GroupInfoIndex].enableXHJ.Equals("true"))
                            {
                                MessageToSend = "当前小黄鸡已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enablexhj", "true");

                                MessageToSend = "小黄鸡启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭小黄鸡"))
                        {
                            if (groupinfo[GroupInfoIndex].enableXHJ.Equals("false"))
                            {
                                MessageToSend = "当前小黄鸡已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enablexhj", "false");

                                MessageToSend = "小黄鸡关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动表情"))
                        {
                            if (groupinfo[GroupInfoIndex].enableEmoje.Equals("true"))
                            {
                                MessageToSend = "当前表情已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableEmoje", "true");

                                MessageToSend = "表情启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭表情"))
                        {
                            if (groupinfo[GroupInfoIndex].enableEmoje.Equals("false"))
                            {
                                MessageToSend = "当前表情已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableEmoje", "false");

                                MessageToSend = "表情关闭成功";
                                return MessageToSend;
                            }
                        }
                        else
                        {
                            MessageToSend = "没有这条指令。";
                            return MessageToSend;
                        }
                    }
                }
            }
            return MessageToSend;
        }

        private void SetGroupSetting(int GroupInfoIndex, string option, string value)
        {
            if(!NoDicPassword)
            {
                string url = DicServer + "groupmanage.php?password=" + DicPassword + "&action=set&gno=" + groupinfo[GroupInfoIndex].no + "&option=" + option + "&value=" + value;
                string temp = HTTP.HttpGet(url);
                JsonGroupManageModel GroupManageInfo = (JsonGroupManageModel)JsonConvert.DeserializeObject(temp, typeof(JsonGroupManageModel));
                if (GroupManageInfo.statu.Equals("fail"))
                    listBoxLog.Items.Insert(0, GroupManageInfo.statu + GroupManageInfo.error);
            }           
            if (option.Equals("enable"))
                groupinfo[GroupInfoIndex].enable = value;
            else if (option.Equals("enablexhj"))
                groupinfo[GroupInfoIndex].enableXHJ = value;
            else if (option.Equals("enableWeather"))
                groupinfo[GroupInfoIndex].enableWeather = value;
            else if (option.Equals("enabletalk"))
                groupinfo[GroupInfoIndex].enableTalk = value;
            else if (option.Equals("enableStudy"))
                groupinfo[GroupInfoIndex].enableStudy = value;
            else if (option.Equals("enableStock"))
                groupinfo[GroupInfoIndex].enableStock = value;
            else if (option.Equals("enableExchangeRate"))
                groupinfo[GroupInfoIndex].enableExchangeRate = value;
            else if (option.Equals("enableEmoje"))
                groupinfo[GroupInfoIndex].enableEmoje = value;
            else if (option.Equals("enableCityInfo"))
                groupinfo[GroupInfoIndex].enableCityInfo = value;
            else if (option.Equals("enableWiki"))
                groupinfo[GroupInfoIndex].enableWiki = value;
        }

        private void GetGroupSetting(int GroupInfoIndex)
        {
            string url = DicServer + "groupmanage.php?password=" + DicPassword + "&action=get&gno=" + groupinfo[GroupInfoIndex].no;
            string temp = HTTP.HttpGet(url);
            JsonGroupManageModel GroupManageInfo = (JsonGroupManageModel)JsonConvert.DeserializeObject(temp, typeof(JsonGroupManageModel));
            if (GroupManageInfo.statu.Equals("success"))
            {
                groupinfo[GroupInfoIndex].enable = GroupManageInfo.enable;
                groupinfo[GroupInfoIndex].enableXHJ = GroupManageInfo.enablexhj;
                groupinfo[GroupInfoIndex].enableWeather = GroupManageInfo.enableWeather;
                groupinfo[GroupInfoIndex].enableTalk = GroupManageInfo.enabletalk;
                groupinfo[GroupInfoIndex].enableStudy = GroupManageInfo.enableStudy;
                groupinfo[GroupInfoIndex].enableStock = GroupManageInfo.enableStock;
                groupinfo[GroupInfoIndex].enableExchangeRate = GroupManageInfo.enableExchangeRate;
                groupinfo[GroupInfoIndex].enableEmoje = GroupManageInfo.enableEmoje;
                groupinfo[GroupInfoIndex].enableCityInfo = GroupManageInfo.enableCityInfo;
                groupinfo[GroupInfoIndex].enableWiki = GroupManageInfo.enableWiki;

                if (groupinfo[GroupInfoIndex].enable.Equals(""))
                    groupinfo[GroupInfoIndex].enable = "true";
                if (groupinfo[GroupInfoIndex].enableXHJ.Equals(""))
                    groupinfo[GroupInfoIndex].enableXHJ = "true";
                if (groupinfo[GroupInfoIndex].enableWeather.Equals(""))
                    groupinfo[GroupInfoIndex].enableWeather = "true";
                if (groupinfo[GroupInfoIndex].enableTalk.Equals(""))
                    groupinfo[GroupInfoIndex].enableTalk = "true";
                if (groupinfo[GroupInfoIndex].enableStudy.Equals(""))
                    groupinfo[GroupInfoIndex].enableStudy = "true";
                if (groupinfo[GroupInfoIndex].enableStock.Equals(""))
                    groupinfo[GroupInfoIndex].enableStock = "true";
                if (groupinfo[GroupInfoIndex].enableExchangeRate.Equals(""))
                    groupinfo[GroupInfoIndex].enableExchangeRate = "true";
                if (groupinfo[GroupInfoIndex].enableEmoje.Equals(""))
                    groupinfo[GroupInfoIndex].enableEmoje = "true";
                if (groupinfo[GroupInfoIndex].enableCityInfo.Equals(""))
                    groupinfo[GroupInfoIndex].enableCityInfo = "true";
                if (groupinfo[GroupInfoIndex].enableWiki.Equals(""))
                    groupinfo[GroupInfoIndex].enableWiki = "true";
            }
            else
            {
                if (!NoDicPassword)
                    groupinfo[GroupInfoIndex].enable = "false";
                else
                    groupinfo[GroupInfoIndex].enable = "true";
                groupinfo[GroupInfoIndex].enableXHJ = "true";
                groupinfo[GroupInfoIndex].enableWeather = "true";
                groupinfo[GroupInfoIndex].enableTalk = "true";
                groupinfo[GroupInfoIndex].enableStudy = "true";
                groupinfo[GroupInfoIndex].enableStock = "true";
                groupinfo[GroupInfoIndex].enableExchangeRate = "true";
                groupinfo[GroupInfoIndex].enableEmoje = "true";
                groupinfo[GroupInfoIndex].enableCityInfo = "true";
                groupinfo[GroupInfoIndex].enableWiki = "true";
                SmartQQ.SendMessageToGroup(groupinfo[GroupInfoIndex].gid, "\\\"如果需要使用小睿睿机器人，请群管理发送 群管理&启动机器人\\\"");
            }

        }
        private void ActionWhenResivedGroupMessage(string gid, string message, string emojis, string uin, string gno)
        {
            for (int i = 0; i <= groupinfMaxIndex; i++)
            {
                if (groupinfo[i].gid == gid)
                {
                    if (groupinfo[i].no == null || !groupinfo[i].no.Equals(gno))
                    {
                        SetGroupNum(i, gno, gid);
                        break;
                    }
                }
            }
            string MessageToSend = GroupManage(message, uin, gid, gno);

            if (!MessageToSend.Equals(""))
            {
                MessageToSend = "\\\"" + MessageToSend + "\\\"";
                SmartQQ.SendMessageToGroup(gid, MessageToSend);
                return;
            }
            int GroupInfoIndex = -1;
            for (int i = 0; i <= groupinfMaxIndex; i++)
            {
                if (groupinfo[i].gid == gid)
                {
                    GroupInfoIndex = i;
                    if (groupinfo[i].enable == null)
                    {
                        GetGroupSetting(i);
                    }
                    if (groupinfo[i].enable.Equals("false"))
                    {
                        return;
                    }

                }
            }

            string[] MessageToSendArray = Answer(message, uin, gid, gno);
            for (int i = 0; i < 10; i++)
            {
                if (MessageToSendArray[i] != null && !MessageToSendArray[i].Equals("") && !MessageToSendArray[i].Equals("None3"))
                {
                    if (!MessageToSend.Equals(""))
                        MessageToSend += Environment.NewLine;
                    MessageToSend += MessageToSendArray[i];
                    MessageToSendArray[i] = "";
                }
            }
            if (!MessageToSend.Equals(""))
                MessageToSend = "\\\"" + MessageToSend + "\\\"";
            if (groupinfo[GroupInfoIndex].enable.Equals("true") && groupinfo[GroupInfoIndex].enableTalk.Equals("true") && groupinfo[GroupInfoIndex].enableEmoje.Equals("true"))
            {
                string[] tmp = emojis.Split(',');
                for (int i = 0; i < tmp.Length - 1 && i < 10; i++)
                {
                    if (tmp[i].Equals(""))
                        continue;
                    if (!MessageToSend.Equals(""))
                        MessageToSend += ",";
                    MessageToSend += SloveEmoji(tmp[i]);
                }
            }
            SmartQQ.SendMessageToGroup(gid, MessageToSend);
        }
        private void ActionWhenResivedMessage(string uin, string message, string emojis)
        {
            string[] MessageToSendArray = Answer(message, uin, "");
            string MessageToSend = "";
            for (int i = 0; i < 10; i++)
            {
                if (MessageToSendArray[i] != null && !MessageToSendArray[i].Equals(""))
                {
                    if (MessageToSendArray[i].Equals("None3"))
                        MessageToSendArray[i] = "这句话仍在等待审核哟～～如果要大量添加语库，可以向管理员申请白名单的～";
                    if (!MessageToSend.Equals(""))
                        MessageToSend += Environment.NewLine;
                    MessageToSend += MessageToSendArray[i];
                    MessageToSendArray[i] = "";
                }
            }
            if (!MessageToSend.Equals(""))
                MessageToSend = "\\\"" + MessageToSend + "\\\"";
            string[] tmp = emojis.Split(',');
            for (int i = 0; i < tmp.Length - 1 && i < 10; i++)
            {
                if (tmp[i].Equals(""))
                    continue;
                if (!MessageToSend.Equals(""))
                    MessageToSend += ",";
                MessageToSend += SloveEmoji(tmp[i]);
            }
            if (MessageToSend.Equals(""))
            {
                int i;
                string SenderName = "";
                string Gender = "";
                for (i = 0; i <= friendinfMaxIndex; i++)
                    if (friendinf[i].uin == uin)
                    {
                        if (friendinf[i].Inf == null)
                            friendinf[i].Inf = SmartQQ.GetFriendInf(friendinf[i].uin);
                        SenderName = friendinf[i].Inf.result.nick;
                        Gender = friendinf[i].Inf.result.gender;
                        break;
                    }
                if (i > friendinfMaxIndex)
                {
                    SmartQQ.getFrienf();
                    for (i = 0; i <= friendinfMaxIndex; i++)
                        if (friendinf[i].uin == uin)
                        {
                            if (friendinf[i].Inf == null)
                                friendinf[i].Inf = SmartQQ.GetFriendInf(friendinf[i].uin);
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
                SmartQQ.SendMessageToFriend(uin, "\\\"" + SenderName + Gender + "～ 小睿睿听不懂你在说什么呢。。。教教我吧～～" + Environment.NewLine + "格式 学习&主人的话&小睿睿的回复" + "\\\"");
            }
            else SmartQQ.SendMessageToFriend(uin, MessageToSend);

        }
        private string AIGet(string message, string QQNum, string QunNum = "NULL")
        {
            string url = DicServer + "gettalk.php?source=" + message + "&qqnum=" + QQNum + "&qunnum=" + QunNum;
            string temp = HTTP.HttpGet(url);
            if (temp.Equals("None1") || temp.Equals("None2") || temp.Equals("None4"))
                temp = "";
            return temp;
        }

        private string AIStudy(string source, string aim, string QQNum, string QunNum = "", bool superstudy = false)
        {
            listBoxLog.Items.Insert(0, "学习 " + source + " " + aim);
            string url;
            if (NoDicPassword)
                url = DicServer + "AddTalkRequest.php";
            else
                url = DicServer + "addtalk.php";
            string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&source=" + HttpUtility.UrlEncode(source) + "&aim=" + HttpUtility.UrlEncode(aim) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(QunNum);
            if (superstudy)
                postdata = postdata + "&superstudy=true";
            else postdata = postdata + "&superstudy=false";

            string MsgGet = HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
            return MsgGet;
        }



        public string GetXiaoHuangJi(string msg)
        {
            string url = "http://www.xiaohuangji.com/ajax.php";
            string postdata = "para=" + HttpUtility.UrlEncode(msg);
            string MsgGet = HTTP.HttpPost(url, "http://www.xiaohuangji.com/", postdata, Encoding.UTF8, false, 10000);
            for (int i = 0; i < Badwords.Length; i++)
                if (MsgGet.Contains(Badwords[i]))
                    return "";
            if (MsgGet.ToLower().Contains("mysql"))
                return "";
            return MsgGet;
        }


        private void GetCAPTCHA()
        {
            if (textBoxID.Text.Length == 0)
                return;
            textBoxCAPTCHA.Text = "";
            if (SmartQQ.GetCAPTCHAInf(textBoxID.Text) == true)
            {
                pictureBoxCAPTCHA.Visible = true;
                textBoxCAPTCHA.Visible = true;
                label3.Visible = true;
            }
            else
            {
                pictureBoxCAPTCHA.Visible = false;
                textBoxCAPTCHA.Visible = false;
                label3.Visible = false;
            }
        }


        private void pictureBoxCAPTCHA_Click(object sender, EventArgs e)
        {
            SmartQQ.GetCaptcha();
        }
        private void FormLogin_Load(object sender, EventArgs e)
        {
            bool NoFile = false;
            byte[] byData = new byte[100000];
            char[] charData = new char[100000];
            Control.CheckForIllegalCrossThreadCalls = false;
            System.Net.ServicePointManager.DefaultConnectionLimit = 500;
            Random rd = new Random();
            SmartQQ.MsgId = rd.Next(10000000, 50000000);
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
            if (!NoFile)
            {
                string tmp = "";
                for (int i = 0; i < charData.Length; i++)
                    tmp += charData[i];
                tmp += '\0';
                JosnConfigFileModel dat = (JosnConfigFileModel)JsonConvert.DeserializeObject(tmp, typeof(JosnConfigFileModel));
                textBoxID.Text = dat.QQNum;
                textBoxPassword.Text = dat.QQPassword;
                DicPassword = dat.DicPassword;
                DicServer = dat.DicServer;
                MasterQQ = dat.AdminQQ;
                SmartQQ.ClientID = dat.ClientID;
            }
            if (DicServer == null || DicServer.Equals(""))
                DicServer = "https://ruiruiqq.hxlxz.com/";
            if (SmartQQ.ClientID == 0)
                SmartQQ.ClientID = rd.Next(1000000, 9999999);
            if (DicPassword == null || DicPassword.Equals(""))
                NoDicPassword = true;
            if (textBoxID.Text.Length > 0)
            {
                SmartQQ.GetCaptcha();
                if (!textBoxCAPTCHA.Text.Equals(""))
                    buttonLogIn_Click(this, EventArgs.Empty);
            }
            NoFile = false;
            try
            {
                FileStream file = new FileStream(Environment.CurrentDirectory + "\\badwords.txt", FileMode.Open);
                file.Seek(0, SeekOrigin.Begin);
                file.Read(byData, 0, 20000);
                Decoder decoder = Encoding.UTF8.GetDecoder();
                decoder.GetChars(byData, 0, byData.Length, charData, 0);
                file.Close();
            }
            catch (System.IO.FileNotFoundException)
            {
                NoFile = true;
            }
            if (!NoFile)
            {
                string tmp = "";
                for (int i = 0; i < charData.Length; i++)
                    if (charData[i] != '\0') tmp += charData[i];
                Badwords = tmp.Split('|');
            }
            else Badwords = new string[0];
            NoFile = false;

            GetCAPTCHA();
            if (!textBoxCAPTCHA.Text.Equals(""))
                LogIn();
        }

        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            LogIn();
        }
        public FormLogin()
        {
            InitializeComponent();
        }
        private void timerHeart_Tick(object sender, EventArgs e)
        {
            if (!StopSendingHeartPack) HTTP.HeartPack();
        }
        public void ReLogin()
        {
            LogOut();
            GetCAPTCHA();
            if (!textBoxCAPTCHA.Text.Equals(""))
                LogIn();
        }
        public void LogOut()
        {
            groupinfMaxIndex = 0;
            timerHeart.Stop();
            System.GC.Collect();
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
            this.AcceptButton = buttonLogIn;
            if (SmartQQ.NeedCAPTCHA) SmartQQ.GetCaptcha();
            listBoxLog.Items.Insert(0, "账号" + textBoxID.Text + "已登出");
        }
        private void buttonLogout_Click(object sender, EventArgs e)
        {
            LogOut();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (textBoxSendMessage.Text.Equals(""))
                return;
            if (!(IsFriendSelent || IsGroupSelent))
                return;
            StopSendingHeartPack = true;
            if (IsGroupSelent)
            {
                string GName = "";
                string[] tmp = listBoxGroup.SelectedItem.ToString().Split(':');
                string MessageToSend = textBoxSendMessage.Text;
                MessageToSend = "\\\"" + MessageToSend + "\\\"";

                SmartQQ.SendMessageToGroup(tmp[0], MessageToSend);

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
                string MessageToSend = textBoxSendMessage.Text;
                MessageToSend = "\\\"" + MessageToSend + "\\\"";

                SmartQQ.SendMessageToFriend(tmp[0], MessageToSend);

                for (int i = 0; i < user.result.info.Count; i++)
                    if (user.result.info[i].uin == tmp[0])
                    {
                        Nick = user.result.info[i].nick;
                        break;
                    }
                textBoxResiveMessage.Text += ("发送至   " + Nick + "   " + SmartQQ.GetRealQQ(tmp[0]) + Environment.NewLine + textBoxSendMessage.Text + Environment.NewLine + Environment.NewLine);
                textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
                textBoxResiveMessage.ScrollToCaret();
            }
            StopSendingHeartPack = false;
            textBoxSendMessage.Text = "";
        }
        private void textBoxID_LostFocus(object sender, EventArgs e)
        {
            GetCAPTCHA();
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

        private void listBoxLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxLog.Text = listBoxLog.Items[listBoxLog.SelectedIndex].ToString();
        }
        private void listBoxLog_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxGroup.SelectedIndex == -1)
                return;
            Clipboard.SetDataObject(listBoxLog.Items[listBoxLog.SelectedIndex].ToString());
        }
        private void listBoxFriend_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxGroup.SelectedIndex == -1)
                return;
            Clipboard.SetDataObject(listBoxFriend.Items[listBoxFriend.SelectedIndex].ToString());
        }
        private void listBoxGroup_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxGroup.SelectedIndex == -1)
                return;
            Clipboard.SetDataObject(listBoxGroup.Items[listBoxGroup.SelectedIndex].ToString());
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
}
