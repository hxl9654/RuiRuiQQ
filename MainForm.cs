using Jurassic;
using Jurassic.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
namespace RuiRuiQQRobot
{

    public partial class MainForm : Form
    {
        //系统配置相关
        string MasterQQ = "";
        public string YoudaoKeyform;
        public string YoudaoKey;
        //多个函数要用到的变量
        string pin = string.Empty;
        bool IsGroupSelent = false, IsFriendSelent = false;
        bool DoNotChangeSelentGroupOrPeople = false;

        public string[] Badwords;

        /// <summary>
        /// 向主界面右侧的消息框末尾添加文字
        /// </summary>
        /// <param name="text">要添加文字</param>
        internal void AddTextToTextBoxResiveMessage(string text)
        {
            textBoxResiveMessage.Text += (text + Environment.NewLine + Environment.NewLine);
            textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
            textBoxResiveMessage.ScrollToCaret();
        }
        /// <summary>
        /// 更新主界面的QQ群列表
        /// </summary>
        internal void ReNewListBoxGroup()
        {
            foreach (KeyValuePair<string, SmartQQ.GroupInfo> GroupList in SmartQQ.GroupList)
            {
                listBoxGroup.Items.Add(GroupList.Key + "::" + GroupList.Value.name);
            }
        }
        /// <summary>
        /// 更新主界面的QQ好友列表
        /// </summary>
        internal void ReNewListBoxFriend()
        {
            foreach (KeyValuePair<string, SmartQQ.FriendInfo> FriendList in SmartQQ.FriendList)
            {
                listBoxFriend.Items.Add(FriendList.Key + ":" + SmartQQ.Info_RealQQ(FriendList.Key) + ":" + FriendList.Value.nick);
            }
        }


        private string[] Answer(string message, string uin, string gid = "", string gno = "")
        {
            string qunnum = gno;
            if (qunnum.Equals(""))
                qunnum = "NULL";
            string[] MessageToSend = new string[20];

            if (message.Equals(""))
                return MessageToSend;
            string QQNum = SmartQQ.Info_RealQQ(uin);
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
            if (!gid.Equals(""))
            {
                string adminuin = "";
                if (SmartQQ.GroupList.ContainsKey(gid))
                    adminuin = SmartQQ.GroupList[gid].owner;
            }
            if (message.StartsWith("行情"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (SmartQQ.GroupList[gid].GroupManage.enableStock == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (SmartQQ.GroupList[gid].GroupManage.enableStock.Equals("false"))
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
                        HTTP.Post(url, postdata);
                        return MessageToSend;
                    }
                }
            }
            if (message.StartsWith("汇率"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (SmartQQ.GroupList[gid].GroupManage.enableExchangeRate == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (SmartQQ.GroupList[gid].GroupManage.enableExchangeRate.Equals("false"))
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
                        HTTP.Post(url, postdata);
                        return MessageToSend;
                    }
                }
            }
            if (message.StartsWith("天气"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (SmartQQ.GroupList[gid].GroupManage.enableWeather == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (SmartQQ.GroupList[gid].GroupManage.enableWeather.Equals("false"))
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
                        HTTP.Post(url, postdata);
                        return MessageToSend;
                    }
                }
            }
            if (message.StartsWith("城市信息"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (SmartQQ.GroupList[gid].GroupManage.enableCityInfo == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (SmartQQ.GroupList[gid].GroupManage.enableCityInfo.Equals("false"))
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
                        HTTP.Post(url, postdata);
                        return MessageToSend;
                    }
                }
            }
            if (message.StartsWith("百科"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (SmartQQ.GroupList[gid].GroupManage.enableWiki == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (SmartQQ.GroupList[gid].GroupManage.enableWiki.Equals("false"))
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
                        HTTP.Post(url, postdata);
                        return MessageToSend;
                    }
                }
            }
            if (message.StartsWith("翻译"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (SmartQQ.GroupList[gid].GroupManage.enableTranslate == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (SmartQQ.GroupList[gid].GroupManage.enableTranslate.Equals("false"))
                        DisableFlag = true;
                }
                if (!DisableFlag)
                {
                    bool TranslateFlag = true;
                    string[] tmp = message.Split('&');
                    if ((!tmp[0].Equals("翻译")) || tmp.Length != 2)
                    {
                        TranslateFlag = false;
                    }
                    if (TranslateFlag)
                    {
                        MessageToSend[0] = GetInfo.GetTranslate(tmp[1]);

                        string url = DicServer + "log.php";
                        string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(qunnum) + "&action=translate&p1=" + HttpUtility.UrlEncode(tmp[1]) + "&p2=NULL&p3=NULL&p4=NULL";
                        HTTP.Post(url, postdata);
                        return MessageToSend;
                    }
                }
            }
            if (message.StartsWith("弹幕"))
            {
                if (!gid.Equals(""))
                {
                    string[] tmp = message.Split('&');

                    for (int i = 0; i < Badwords.Length; i++)
                        if (tmp[1].Contains(Badwords[i]))
                        {
                            MessageToSend[0] = "保护敏感瓷，发送失败";
                            return MessageToSend;
                        }

                    string url = DicServer + "setcomment.php";
                    string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qunnum=" + HttpUtility.UrlEncode(qunnum) + "&dat=" + HttpUtility.UrlEncode(tmp[1]);
                    HTTP.Post(url, postdata);

                    url = DicServer + "log.php";
                    postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(qunnum) + "&action=comment&p1=" + HttpUtility.UrlEncode(tmp[1]) + "&p2=NULL&p3=NULL&p4=NULL";
                    HTTP.Post(url, postdata);
                    return MessageToSend;
                }
            }
            if (message.StartsWith("学习") || message.StartsWith("特权学习"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (SmartQQ.GroupList[gid].GroupManage.enableStudy == null)
                        GetGroupSetting(groupinfMaxIndex);
                    if (SmartQQ.GroupList[gid].GroupManage.enableStudy.Equals("false"))
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
                        HTTP.Post(url, postdata);
                        return MessageToSend;
                    }
                }
            }
            bool DisableTalkFlag = false;
            if (!gid.Equals(""))
            {
                if (SmartQQ.GroupList[gid].GroupManage.enableTalk == null)
                    GetGroupSetting(groupinfMaxIndex);
                if (SmartQQ.GroupList[gid].GroupManage.enableTalk.Equals("false"))
                    DisableTalkFlag = true;
            }
            if (!DisableTalkFlag)
            {

                MessageToSend[0] = AIGet(message, QQNum, gno);
                if (!MessageToSend[0].Equals(""))
                {
                    string url = DicServer + "log.php";
                    string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(qunnum) + "&action=talk&p1=" + HttpUtility.UrlEncode(message) + "&p2=NULL&p3=NULL&p4=NULL";
                    HTTP.Post(url, postdata);
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
                        if (SmartQQ.GroupList[gid].GroupManage.enableXHJ == null)
                            GetGroupSetting(groupinfMaxIndex);
                        if (SmartQQ.GroupList[gid].GroupManage.enableXHJ.Equals("false"))
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
                    HTTP.Post(url, postdata);
                }
                return MessageToSend;
            }
            return MessageToSend;
        }


        string GroupManage(string message, string uin, string gid, string gno)
        {
            string adminuin = "";
            string MessageToSend = "";
            int i;
            if (message.StartsWith("群管理"))
            {
                SmartQQ.Info_GroupList();
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
                    //groupinfo[GroupInfoIndex].inf = SmartQQ.Info_GroupInfo(groupinfo[GroupInfoIndex].inf.result.ginfo.code);
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
                    if (SmartQQ.Info_RealQQ(uin).Equals(MasterQQ))
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
                    if (SmartQQ.GroupList[gid].GroupManage.enable == null)
                    {
                        GetGroupSetting(GroupInfoIndex);
                    }
                    if (tmp.Length != 2 || tmp[1] == null)
                        return "";
                    if ((HaveRight || SmartQQ.GroupList[gid].GroupManage.enable.Equals("true")) && (tmp[1].Equals("查询状态") || tmp[1].Equals("状态")))
                    {
                        MessageToSend = "机器人启动：" + SmartQQ.GroupList[gid].GroupManage.enable + Environment.NewLine;
                        MessageToSend += "汇率查询启动：" + SmartQQ.GroupList[gid].GroupManage.enableExchangeRate + Environment.NewLine;
                        MessageToSend += "百科查询启动：" + SmartQQ.GroupList[gid].GroupManage.enableWiki + Environment.NewLine;
                        MessageToSend += "天气查询启动：" + SmartQQ.GroupList[gid].GroupManage.enableWeather + Environment.NewLine;
                        MessageToSend += "城市信息查询启动：" + SmartQQ.GroupList[gid].GroupManage.enableCityInfo + Environment.NewLine;
                        MessageToSend += "学习启动：" + SmartQQ.GroupList[gid].GroupManage.enableStudy + Environment.NewLine;
                        MessageToSend += "行情查询启动：" + SmartQQ.GroupList[gid].GroupManage.enableStock + Environment.NewLine;
                        MessageToSend += "翻译启动：" + SmartQQ.GroupList[gid].GroupManage.enableTranslate + Environment.NewLine;
                        MessageToSend += "闲聊启动：" + SmartQQ.GroupList[gid].GroupManage.enableTalk + Environment.NewLine;
                        MessageToSend += "表情启动：" + SmartQQ.GroupList[gid].GroupManage.enableEmoje + Environment.NewLine;
                        MessageToSend += "小黄鸡启动：" + SmartQQ.GroupList[gid].GroupManage.enableXHJ;
                        return MessageToSend;
                    }
                    if (HaveRight == false)
                    {
                        MessageToSend = "账号" + SmartQQ.Info_RealQQ(uin) + "不是群管理，无权进行此操作";
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
                        else if (tmp[1].Equals("启动翻译"))
                        {
                            if (groupinfo[GroupInfoIndex].enableTranslate.Equals("true"))
                            {
                                MessageToSend = "当前翻译已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableTranslate", "true");

                                MessageToSend = "翻译启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭翻译"))
                        {
                            if (groupinfo[GroupInfoIndex].enableTranslate.Equals("false"))
                            {
                                MessageToSend = "当前翻译已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(GroupInfoIndex, "enableTranslate", "false");

                                MessageToSend = "翻译关闭成功";
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

        private void FormLogin_Load(object sender, EventArgs e)
        {
            bool NoFile = false;
            byte[] byData = new byte[100000];
            char[] charData = new char[100000];
            Control.CheckForIllegalCrossThreadCalls = false;
            System.Net.ServicePointManager.DefaultConnectionLimit = 500;
            Random rd = new Random();
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
                RuiRui.DicPassword = dat.DicPassword;
                RuiRui.DicServer = dat.DicServer;
                MasterQQ = dat.AdminQQ;
                YoudaoKey = dat.YoudaoKey;
                YoudaoKeyform = dat.YoudaoKeyfrom;
            }
            if (RuiRui.DicServer == null || RuiRui.DicServer.Equals(""))
                RuiRui.DicServer = "https://ruiruiqq.hxlxz.com/";
            if (RuiRui.DicPassword == null || RuiRui.DicPassword.Equals(""))
                RuiRui.NoDicPassword = true;
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
        }
        private void pictureBoxQRCode_Click(object sender, EventArgs e)
        {
            SmartQQ.Login_GetQRCode();
        }
        private void buttonSend_Click(object sender, EventArgs e)
        {
            if ((textBoxSendMessage.Text.Equals("")) || (!(IsFriendSelent || IsGroupSelent)))
                return;

            if (IsGroupSelent)
            {
                string GName = "";
                string[] tmp = listBoxGroup.SelectedItem.ToString().Split(':');
                string MessageToSend = textBoxSendMessage.Text;

                SmartQQ.Message_Send(1, tmp[0], MessageToSend);

                if (SmartQQ.GroupList.ContainsKey(tmp[0]))
                    GName = SmartQQ.GroupList[tmp[0]].name;
                AddTextToTextBoxResiveMessage("发送至   " + GName + Environment.NewLine + textBoxSendMessage.Text);
            }
            else if (IsFriendSelent)
            {
                string Nick = "";
                string[] tmp = listBoxFriend.SelectedItem.ToString().Split(':');
                string MessageToSend = textBoxSendMessage.Text;

                SmartQQ.Message_Send(0, tmp[0], MessageToSend);

                if (SmartQQ.FriendList.ContainsKey(tmp[0]))
                    Nick = SmartQQ.FriendList[tmp[0]].nick;
                AddTextToTextBoxResiveMessage("发送至   " + Nick + "   " + SmartQQ.Info_RealQQ(tmp[0]) + Environment.NewLine + textBoxSendMessage.Text);
            }
            textBoxSendMessage.Clear();
        }
        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            SmartQQ.Login();
            HTTP.Get("https://ruiruiqq.hxlxz.com/infreport.php?qq=" + SmartQQ.QQNum + "&adminqq=" + MasterQQ);
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
            if (listBoxLog.SelectedIndex != -1)
                Clipboard.SetDataObject(listBoxLog.Items[listBoxLog.SelectedIndex].ToString());
        }
        private void listBoxFriend_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxFriend.SelectedIndex != -1)
                Clipboard.SetDataObject(listBoxFriend.Items[listBoxFriend.SelectedIndex].ToString());
        }

        private void listBoxGroup_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxGroup.SelectedIndex != -1)
                Clipboard.SetDataObject(listBoxGroup.Items[listBoxGroup.SelectedIndex].ToString());
        }
        public MainForm()
        {
            InitializeComponent();
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
