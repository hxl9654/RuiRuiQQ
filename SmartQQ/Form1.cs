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

        string StudyPassword = "";
        string DicServer = "";
        bool DisableStudy = false;
        bool DisableWeather = false;
        public bool StopSendingHeartPack = false;
        //多个函数要用到的变量
        
        string pin = string.Empty;       
        
        
        bool IsGroupSelent = false, IsFriendSelent = false;
        bool DoNotChangeSelentGroupOrPeople = false;
        //数据存储相关
        public JsonGroupModel group;
        public JsonFriendModel user;

        public struct GroupInf
        {
            public String gid;
            public String no;
            public String enable;
            public JsonGroupInfoModel inf;
            public String[] managers;
            public int GroupManagerIndex;
        };
        public GroupInf[] groupinfo = new GroupInf[100];
        public int groupinfMaxIndex = 0;
        public struct FriendInf
        {
            public String uin;
            public JsonFriendInfModel Inf;
        };
        public FriendInf[] friendinf = new FriendInf[1000];
        public int friendinfMaxIndex = 0;
        string[] Badwords;
        private void buttonLogIn_Click(object sender, EventArgs e)
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

            SmartQQ.FirstLogin(textBoxID.Text, textBoxPassword.Text, textBoxCAPTCHA.Text);
            SmartQQ.SecondLogin(textBoxID.Text);

            SmartQQ.getFrienf();
            SmartQQ.getGroup();

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
            this.AcceptButton = this.buttonSend;
        }
        public void HeartPackAction(string temp)
        {
            string GName = "";
            string MessageFromUin = "";
            textBoxLog.Text = temp;
            JsonHeartPackMessage HeartPackMessage = (JsonHeartPackMessage)JsonConvert.DeserializeObject(temp, typeof(JsonHeartPackMessage));
           
            if (HeartPackMessage.retcode == 102)
            {
                return;
            }
            else if (HeartPackMessage.retcode == 103)
            {
                listBoxLog.Items.Insert(0, temp);
                return;
            }
            else if (HeartPackMessage.retcode == 116)
            {
                listBoxLog.Items.Insert(0, temp);
                SmartQQ.ptwebqq = HeartPackMessage.p;
                return;
            }
            else if (HeartPackMessage.retcode == 108)
            {
                listBoxLog.Items.Insert(0, temp);
                return;
            }
            else if (HeartPackMessage.retcode == 120 || HeartPackMessage.retcode == 121)
            {
                listBoxLog.Items.Insert(0, temp);
                listBoxLog.Items.Insert(0, HeartPackMessage.t);
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
                    String message = "";
                    String emojis = "";
                    int j;
                    for(j =1;j<HeartPackMessage.result[i].value.content.Count;j++)
                    {
                        string temp1 = HeartPackMessage.result[i].value.content[j].ToString();
                        temp1 = temp1.Replace(Environment.NewLine, "");
                        temp1 = temp1.Replace(" ", "");
                        if (temp1.Contains("[\"face\","))
                        {
                            string emojiID = temp1.Replace("[\"face\",", "");
                            emojiID = emojiID.Replace("]", "");
                            emojis += (emojiID + ",");
                        }
                        else message += (HeartPackMessage.result[i].value.content[j].ToString() + " ");
                    }
                    message = message.Replace("\\\\n", Environment.NewLine);
                    
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
                    String emojis = "";
                    string message = "";
                    int j;
                    for (j = 1; j < HeartPackMessage.result[i].value.content.Count; j++)
                    {
                        string temp1 = HeartPackMessage.result[i].value.content[j].ToString();
                        temp1 = temp1.Replace(Environment.NewLine, "");
                        temp1 = temp1.Replace(" ", "");
                        if (temp1.Contains("[\"face\","))
                        {
                            string emojiID = temp1.Replace("[\"face\",", "");
                            emojiID = emojiID.Replace("]", "");
                            emojis += (emojiID + ",");
                        }
                        else message += (HeartPackMessage.result[i].value.content[j].ToString() + " ");
                    }
                    message = message.Replace("\\\\n", Environment.NewLine);
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
        private string[] Answer(string message, string uin, string gid="",string gno="")
        {            
            string[] MessageToSend = new string[20];
            message = message.Remove(message.Length - 2);
            if (message.Equals(""))
                return MessageToSend;
            string QQNum = SmartQQ.GetRealQQ(uin);
            for (int i = 0; i < 20; i++)
                MessageToSend[i] = "";
            bool MsgSendFlag = false;
            if (message.Equals("源码") || message.Equals("作者") || message.Equals("代码") || message.Equals("开源许可证") || message.Equals("许可证"))
            {
                MessageToSend[0] = "本程序作者是何相龙，网站：https://tec.hxlxz.com 。本程序采用GPL v3许可证授权，源码获取地址：https://github.com/qwgg9654/RuiRuiQQ";
                return MessageToSend;
            }
            int j = 0;
            if(!gid.Equals(""))
            {
                int i = -1;
                string adminuin = "";
                int GroupInfoIndex = -1;
                for (i = 0; i <= groupinfMaxIndex; i++)
                {
                    if (groupinfo[i].gid == gid)
                    {
                        GroupInfoIndex = i;
                        adminuin = groupinfo[i].inf.result.ginfo.owner;
                        //获取群号
                        if (groupinfo[i].no == null || !groupinfo[i].no.Equals(gno)) 
                        {
                            groupinfo[i].no = gno;
                            for (j = 0; j < listBoxGroup.Items.Count; j++) 
                            {
                                string[] tmp = listBoxGroup.Items[j].ToString().Split(':');
                                if(tmp[0].Equals(gid))
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
                        break;
                    }
                }                               
            }
            if(message.Contains("行情"))
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
                    return MessageToSend;
                }
            }
            if (message.Contains("汇率"))
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
                    return MessageToSend;                       
                }
            }
            if (message.Contains("天气") && DisableWeather == false) 
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
                    return MessageToSend;
                }
            }
            if (message.Contains("学习"))
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
                        if((tmp[0].Equals("特权学习")) && tmp.Length == 3)
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
                    result = AIStudy(tmp[1], tmp[2], QQNum, true);
                    MessageToSend[0] = GetInfo.GetStudyFlagInfo(result, QQNum, tmp[1], tmp[2]);
                    return MessageToSend;
                }
                if(StudyFlag)
                {
                    string result = "";
                    for (int i = 0; i < Badwords.Length; i++)
                        if (tmp[1].Contains(Badwords[i]) || tmp[2].Contains(Badwords[i]))
                            result = "ForbiddenWord";
                    if (result.Equals(""))
                        result = AIStudy(tmp[1], tmp[2], QQNum, false);
                    MessageToSend[0] = GetInfo.GetStudyFlagInfo(result, QQNum, tmp[1], tmp[2]);                    
                    return MessageToSend;
                }                
            }
            MessageToSend[0] = AIGet(message, QQNum);
            if (!MessageToSend[0].Equals(""))
            {
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
                    MessageToSend[j] = AIGet(tmp1[i], QQNum);
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
                    if (tmp1[i].Equals(message))
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
                        MessageToSend[j] = AIGet(tmp2[i], QQNum);
                        j++;
                        MsgSendFlag = true;
                    }
                }
            }
            
            
            if (!MsgSendFlag)
            {
                string XiaoHuangJiMsg = GetXiaoHuangJi(message);
                if (XiaoHuangJiMsg.Length > 1)
                {
                    for (int i = 0; i < Badwords.Length; i++)
                        if (XiaoHuangJiMsg.Contains(Badwords[i]))
                            return null;
                    MessageToSend[0] = "隔壁小黄鸡说：" + XiaoHuangJiMsg;
                    
                    return MessageToSend;
                }
                else
                {
                    return MessageToSend;
                }
            }
            return MessageToSend;
        }
        string GroupManage(string message, string uin, string gid = "", string gno = "")
        {
            string adminuin = "";
            int GroupInfoIndex = -1;
            string MessageToSend = "";
            int i;
            if (message.Contains("群管理"))
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
                groupinfo[GroupInfoIndex].inf = SmartQQ.GetGroupInfo(groupinfo[GroupInfoIndex].inf.result.ginfo.code);
                //获取管理员
                if (groupinfo[GroupInfoIndex].managers == null)
                    groupinfo[GroupInfoIndex].managers = new string[30];
                    
                groupinfo[GroupInfoIndex].GroupManagerIndex = 0;
                for (i = 0; i < groupinfo[GroupInfoIndex].inf.result.ginfo.members.Count; i++)
                {
                    if (groupinfo[GroupInfoIndex].inf.result.ginfo.members[i].mflag == 1)
                    {
                        groupinfo[GroupInfoIndex].managers[groupinfo[GroupInfoIndex].GroupManagerIndex] = groupinfo[GroupInfoIndex].inf.result.ginfo.members[i].muin;
                        groupinfo[GroupInfoIndex].GroupManagerIndex++;
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
                    if (HaveRight == false)
                    {
                        MessageToSend = "账号" + SmartQQ.GetRealQQ(uin) + "不是群管理，无权进行此操作";
                        return MessageToSend;
                    }
                    else
                    {
                        if (groupinfo[GroupInfoIndex].enable == null)
                        {
                            string url = DicServer + "groupmanage.php?password=" + StudyPassword + "&action=get&gno=" + groupinfo[GroupInfoIndex].no;
                            string temp = HTTP.HttpGet(url);
                            JsonGroupManageModel GroupManageInfo = (JsonGroupManageModel)JsonConvert.DeserializeObject(temp, typeof(JsonGroupManageModel));
                            if (GroupManageInfo.statu.Equals("success"))
                                groupinfo[GroupInfoIndex].enable = GroupManageInfo.enable;
                            else
                                groupinfo[GroupInfoIndex].enable = "true";
                        }
                        if (tmp[1].Equals("关闭机器人"))
                        {
                            if (groupinfo[GroupInfoIndex].enable.Equals("false"))
                            {
                                MessageToSend = "当前机器人已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                groupinfo[GroupInfoIndex].enable = "false";

                                string url = DicServer + "groupmanage.php?password=" + StudyPassword + "&action=set&gno=" + groupinfo[i].no + "&option=enable&value=false";
                                string temp = HTTP.HttpGet(url);
                                JsonGroupManageModel GroupManageInfo = (JsonGroupManageModel)JsonConvert.DeserializeObject(temp, typeof(JsonGroupManageModel));
                                if (GroupManageInfo.statu.Equals("fail"))
                                    listBoxLog.Items.Insert(0, GroupManageInfo.statu + GroupManageInfo.error);

                                MessageToSend = "机器人关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动机器人"))
                        {
                            if (groupinfo[GroupInfoIndex].enable.Equals("true"))
                            {
                                MessageToSend = "当前机器人已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                groupinfo[GroupInfoIndex].enable = "true";

                                string url = DicServer + "groupmanage.php?password=" + StudyPassword + "&action=set&gno=" + groupinfo[i].no + "&option=enable&value=true";
                                string temp = HTTP.HttpGet(url);
                                JsonGroupManageModel GroupManageInfo = (JsonGroupManageModel)JsonConvert.DeserializeObject(temp, typeof(JsonGroupManageModel));
                                if (GroupManageInfo.statu.Equals("fail"))
                                    listBoxLog.Items.Insert(0, GroupManageInfo.statu + GroupManageInfo.error);

                                MessageToSend = "机器人启动成功";
                                return MessageToSend;
                            }
                        }
                    }

                }
            }
            return MessageToSend;
        }
        private void ActionWhenResivedGroupMessage(string gid, string message, string emojis, string uin, string gno)
        {
            string MessageToSend = GroupManage(message, uin, gid, gno);
            if (!MessageToSend.Equals(""))
            {
                MessageToSend = "\\\"" + MessageToSend + "\\\"";
                SmartQQ.SendMessageToGroup(gid, MessageToSend);
                return;
            }                
            for (int i = 0; i <= groupinfMaxIndex; i++)
            {
                if (groupinfo[i].gid == gid)
                {
                    if (groupinfo[i].enable == null) 
                    {
                        string url = DicServer + "groupmanage.php?password=" + StudyPassword + "&action=get&gno=" + groupinfo[i].no;
                        string temp = HTTP.HttpGet(url);
                        JsonGroupManageModel GroupManageInfo = (JsonGroupManageModel)JsonConvert.DeserializeObject(temp, typeof(JsonGroupManageModel));
                        if (GroupManageInfo.statu.Equals("success"))
                            groupinfo[i].enable = GroupManageInfo.enable;
                        else
                            groupinfo[i].enable = "true";
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
                    if (i != 0)
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
                    if (i != 0)
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
                SmartQQ.SendMessageToFriend(uin, "\\\"" + SenderName + Gender + "～ 小睿睿听不懂你在说什么呢。。。教教我吧～～" + Environment.NewLine + "格式 学习^语句^设定的回复" + "\\\"");
            }
            else SmartQQ.SendMessageToFriend(uin, MessageToSend);

        }
        private string AIGet(string message, string QQNum)
        {
            String url = DicServer + "gettalk.php?source=" + message + "&qqnum=" + QQNum;
            string temp = HTTP.HttpGet(url);
            if (temp.Equals("None1") || temp.Equals("None2") || temp.Equals("None4"))
                temp = "";
            return temp;
        }

        private string AIStudy(string source, string aim, string QQNum, bool superstudy = false)
        {
            listBoxLog.Items.Insert(0, "学习 " + source + " " + aim);
            if (DisableStudy)
            {
                return "DisableStudy";
            }
            String url = DicServer + "addtalk.php?password=" + StudyPassword + "&source=" + source + "&aim=" + aim + "&qqnum=" + QQNum;
            if (superstudy)
                url = url + "&superstudy=true";
            else url = url + "&superstudy=false";
            string temp = HTTP.HttpGet(url);
            return temp;
        }

        
        
        public string GetXiaoHuangJi(string msg)
        {
            string url = "http://www.xiaohuangji.com/ajax.php";
            string postdata = "para=" + HttpUtility.UrlEncode(msg);
            string MsgGet = HTTP.HttpPost(url, "http://www.xiaohuangji.com/", postdata, Encoding.UTF8, false, 10000);
            return MsgGet;
        }
        private void textBoxID_LostFocus(object sender, EventArgs e)
        {
            if (textBoxID.Text.Length == 0)
                return;
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
                //tmp.Replace(Environment.NewLine, "");
                //tmp.Replace(" ", "");
                JosnConfigFileModel dat = (JosnConfigFileModel)JsonConvert.DeserializeObject(tmp, typeof(JosnConfigFileModel));
                textBoxID.Text = dat.QQNum;
                textBoxPassword.Text = dat.QQPassword;
                StudyPassword = dat.DicPassword;
                DicServer = dat.DicServer;
                SmartQQ.ClientID = dat.ClientID;
            }
            else
            {
                DicServer = "http://smartqq.hxlxz.com/";
                DisableStudy = true;
                SmartQQ.ClientID = rd.Next(1000000, 9999999); ;
            }
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
            try
            {
                FileStream file = new FileStream(Environment.CurrentDirectory + "\\cityweathercode.txt", FileMode.Open);
                file.Seek(0, SeekOrigin.Begin);
                file.Read(byData, 0, 100000);
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
                GetInfo.citycode = (JsonWeatherCityCodeModel)JsonConvert.DeserializeObject(tmp, typeof(JsonWeatherCityCodeModel));
            }
            else DisableWeather = true;
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
            if(!textBoxCAPTCHA.Text.Equals(""))
                buttonLogIn_Click(this, EventArgs.Empty);
        }
        public void LogOut()
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
                SmartQQ.SendMessageToGroup(tmp[0], textBoxSendMessage.Text);

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
                SmartQQ.SendMessageToFriend(tmp[0], textBoxSendMessage.Text);

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
            Clipboard.SetDataObject(listBoxLog.Items[listBoxLog.SelectedIndex].ToString());
        }
        private void listBoxFriend_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(listBoxFriend.Items[listBoxFriend.SelectedIndex].ToString());
        }
        private void listBoxGroup_DoubleClick(object sender, EventArgs e)
        {
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
