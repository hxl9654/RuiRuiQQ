using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RuiRuiQQRobot
{
    public static class RuiRui
    {
        //系统配置相关
        internal static string MasterQQ = "";
        internal static string DicPassword = "";
        internal static string DicServer = "";
        internal static bool NoDicPassword = false;
        public static string[] Badwords;

        private static string[] Answer(string message, string uin, string gid = "", string gno = "")
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
            if (message.StartsWith("行情"))
            {
                bool DisableFlag = false;
                if (!gid.Equals(""))
                {
                    if (SmartQQ.GroupList[gid].GroupManage.enableStock == null)
                        GetGroupSetting(gid);
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
                        GetGroupSetting(gid);
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
                        GetGroupSetting(gid);
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
                        GetGroupSetting(gid);
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
                        GetGroupSetting(gid);
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
                        GetGroupSetting(gid);
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
                            MessageToSend[0] = QQNum + "发送的文字包含敏感瓷，小睿睿不喜欢敏感词哦~";
                            return MessageToSend;
                        }
                    string gname = SmartQQ.Info_RealQQ(SmartQQ.GroupList[gid].owner) + SmartQQ.GroupList[gid].name;
                    string url = DicServer + "setcomment.php";
                    string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qunnum=" + HttpUtility.UrlEncode(gname) + "&dat=" + HttpUtility.UrlEncode(tmp[1]);
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
                        GetGroupSetting(gid);
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
                        MessageToSend[0] = GetStudyFlagInfo(result, QQNum, tmp[1], tmp[2]);
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
                        MessageToSend[0] = GetStudyFlagInfo(result, QQNum, tmp[1], tmp[2]);

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
                    GetGroupSetting(gid);
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
                int j = 0;
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
                            GetGroupSetting(gid);
                        if (SmartQQ.GroupList[gid].GroupManage.enableXHJ.Equals("false"))
                            DisableFlag = true;
                    }
                    if (!DisableFlag)
                    {
                        string XiaoHuangJiMsg = GetInfo.GetTuLin(message, QQNum);
                        if (XiaoHuangJiMsg.Length > 1)
                        {
                            for (int i = 0; i < Badwords.Length; i++)
                                if (XiaoHuangJiMsg.Contains(Badwords[i]))
                                    return null;
                            MessageToSend[0] = "隔壁图灵机器人说：" + XiaoHuangJiMsg;

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


        private static string GroupManage(string message, string uin, string gid, string gno)
        {
            string adminuin = "";
            string MessageToSend = "";
            if (message.StartsWith("群管理"))
            {
                SmartQQ.Info_GroupInfo(gid);
                if (!gid.Equals(""))
                {
                    adminuin = "";
                    if (SmartQQ.GroupList.ContainsKey(gid))
                        adminuin = SmartQQ.GroupList[gid].owner;
                }

                bool GroupManageFlag = true;
                string[] tmp = message.Split('&');
                tmp[1] = tmp[1].Replace("\r", "").Replace("\n", "").Replace(" ", "");
                if ((!tmp[0].Equals("群管理")) || tmp.Length != 2)
                {
                    GroupManageFlag = false;
                }
                if (GroupManageFlag)
                {
                    bool HaveRight = false;
                    if (uin.Equals(adminuin) || SmartQQ.Info_RealQQ(uin).Equals(MasterQQ))
                        HaveRight = true;
                    else if (SmartQQ.GroupList[gid].MemberList[uin].isManager)
                        HaveRight = true;
                    else HaveRight = false;
                    if (SmartQQ.GroupList[gid].GroupManage.enable == null)
                    {
                        GetGroupSetting(gid);
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
                            if (SmartQQ.GroupList[gid].GroupManage.enable.Equals("true"))
                            {
                                MessageToSend = "当前机器人已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enable", "true");

                                MessageToSend = "机器人启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭机器人"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enable.Equals("false"))
                            {
                                MessageToSend = "当前机器人已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enable", "false");

                                MessageToSend = "机器人关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动城市信息查询"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableCityInfo.Equals("true"))
                            {
                                MessageToSend = "当前城市信息查询已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableCityInfo", "true");

                                MessageToSend = "城市信息查询启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭城市信息查询"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableCityInfo.Equals("false"))
                            {
                                MessageToSend = "当前城市信息查询已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableCityInfo", "false");

                                MessageToSend = "城市信息查询关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动天气查询"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableWeather.Equals("true"))
                            {
                                MessageToSend = "当前天气查询已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableWeather", "true");

                                MessageToSend = "天气查询启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭天气查询"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableWeather.Equals("false"))
                            {
                                MessageToSend = "当前天气查询已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableWeather", "false");

                                MessageToSend = "天气查询关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动百科查询"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableWiki.Equals("true"))
                            {
                                MessageToSend = "当前百科查询已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableWiki", "true");

                                MessageToSend = "百科查询启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭百科查询"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableWiki.Equals("false"))
                            {
                                MessageToSend = "当前百科查询已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableWiki", "false");

                                MessageToSend = "百科查询关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动汇率查询"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableExchangeRate.Equals("true"))
                            {
                                MessageToSend = "当前汇率查询已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableExchangeRate", "true");

                                MessageToSend = "汇率查询启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭汇率查询"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableExchangeRate.Equals("false"))
                            {
                                MessageToSend = "当前汇率查询已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableExchangeRate", "false");

                                MessageToSend = "汇率查询关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动行情查询"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableStock.Equals("true"))
                            {
                                MessageToSend = "当前行情查询已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableStock", "true");

                                MessageToSend = "行情查询启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭行情查询"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableStock.Equals("false"))
                            {
                                MessageToSend = "当前行情查询已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableStock", "false");

                                MessageToSend = "行情查询关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动聊天") || tmp[1].Equals("启动闲聊"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableTalk.Equals("true"))
                            {
                                MessageToSend = "当前聊天已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enabletalk", "true");

                                MessageToSend = "聊天启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭聊天") || tmp[1].Equals("关闭闲聊"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableTalk.Equals("false"))
                            {
                                MessageToSend = "当前聊天已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enabletalk", "false");

                                MessageToSend = "聊天关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动学习"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableStudy.Equals("true"))
                            {
                                MessageToSend = "当前学习已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableStudy", "true");

                                MessageToSend = "学习启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭学习"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableStudy.Equals("false"))
                            {
                                MessageToSend = "当前学习已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableStudy", "false");

                                MessageToSend = "学习关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动小黄鸡"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableXHJ.Equals("true"))
                            {
                                MessageToSend = "当前小黄鸡已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enablexhj", "true");

                                MessageToSend = "小黄鸡启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭小黄鸡"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableXHJ.Equals("false"))
                            {
                                MessageToSend = "当前小黄鸡已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enablexhj", "false");

                                MessageToSend = "小黄鸡关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动表情"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableEmoje.Equals("true"))
                            {
                                MessageToSend = "当前表情已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableEmoje", "true");

                                MessageToSend = "表情启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭表情"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableEmoje.Equals("false"))
                            {
                                MessageToSend = "当前表情已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableEmoje", "false");

                                MessageToSend = "表情关闭成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("启动翻译"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableTranslate.Equals("true"))
                            {
                                MessageToSend = "当前翻译已启动";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableTranslate", "true");

                                MessageToSend = "翻译启动成功";
                                return MessageToSend;
                            }
                        }
                        else if (tmp[1].Equals("关闭翻译"))
                        {
                            if (SmartQQ.GroupList[gid].GroupManage.enableTranslate.Equals("false"))
                            {
                                MessageToSend = "当前翻译已关闭";
                                return MessageToSend;
                            }
                            else
                            {
                                SetGroupSetting(gid, "enableTranslate", "false");

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
        /// <summary>
        /// 收到私聊和讨论组消息时调用的回复函数
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        internal static void AnswerMessage(string uin, string message, int type = 0)
        {
            string[] MessageToSendArray = Answer(message, uin);
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
                SmartQQ.Message_Send(type, uin, MessageToSend);
            else if (type == 0)
            {
                string SenderName = "";
                string Gender = "";
                if (!SmartQQ.FriendList.ContainsKey(uin))
                    SmartQQ.Info_FriendList();
                if (SmartQQ.FriendList.ContainsKey(uin))
                {
                    SenderName = SmartQQ.FriendList[uin].nick;
                    Gender = SmartQQ.FriendList[uin].gender;
                }
                if (Gender == "female")
                    Gender = "姐姐 ";
                else if (Gender == "male")
                    Gender = "哥哥 ";
                SmartQQ.Message_Send(0, uin, SenderName + Gender + "～ 小睿睿听不懂你在说什么呢。。。教教我吧～～" + Environment.NewLine + "格式 学习&主人的话&小睿睿的回复");
            }
        }
        /// <summary>
        /// 收到群消息时调用的回复函数
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="message"></param>
        /// <param name="uin"></param>
        /// <param name="gno"></param>
        internal static void AnswerGroupMessage(string gid, string message, string uin, string gno)
        {
            string MessageToSend = GroupManage(message, uin, gid, gno);

            if (!MessageToSend.Equals(""))
            {
                SmartQQ.Message_Send(1, gid, MessageToSend);
                return;
            }
            if (!SmartQQ.GroupList.ContainsKey(gid))
                SmartQQ.Info_GroupList();
            if (SmartQQ.GroupList.ContainsKey(gid))
            {
                if (SmartQQ.GroupList[gid].GroupManage == null)
                    GetGroupSetting(gid);
                if (SmartQQ.GroupList[gid].GroupManage.enable == null || SmartQQ.GroupList[gid].GroupManage.enable.Equals("false"))
                    return;
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
            if (SmartQQ.GroupList[gid].GroupManage.enableEmoje.Equals("false"))
            {
                string[] tmp = MessageToSend.Split('{');
                MessageToSend = "";
                for (int i = 0; i < tmp.Length; i++)
                    if (!tmp[i].StartsWith("..[face"))
                        MessageToSend += ("{" + tmp[i]);
                    else MessageToSend += tmp[i].Remove(0, 7);
            }
            SmartQQ.Message_Send(1, gid, MessageToSend);
        }
        /// <summary>
        /// 获取指定群的配置信息
        /// </summary>
        /// <param name="gid"></param>
        internal static void GetGroupSetting(string gid)
        {
            string url = DicServer + "groupmanage.php?password=" + DicPassword + "&action=get&gno=" + SmartQQ.AID_GroupKey(gid);
            string temp = HTTP.Get(url);
            JsonGroupManageModel GroupManageInfo = (JsonGroupManageModel)JsonConvert.DeserializeObject(temp, typeof(JsonGroupManageModel));
            if (GroupManageInfo.statu.Equals("success"))
            {
                SmartQQ.GroupList[gid].GroupManage.enable = GroupManageInfo.enable;
                SmartQQ.GroupList[gid].GroupManage.enableXHJ = GroupManageInfo.enablexhj;
                SmartQQ.GroupList[gid].GroupManage.enableWeather = GroupManageInfo.enableWeather;
                SmartQQ.GroupList[gid].GroupManage.enableTalk = GroupManageInfo.enabletalk;
                SmartQQ.GroupList[gid].GroupManage.enableStudy = GroupManageInfo.enableStudy;
                SmartQQ.GroupList[gid].GroupManage.enableStock = GroupManageInfo.enableStock;
                SmartQQ.GroupList[gid].GroupManage.enableExchangeRate = GroupManageInfo.enableExchangeRate;
                SmartQQ.GroupList[gid].GroupManage.enableEmoje = GroupManageInfo.enableEmoje;
                SmartQQ.GroupList[gid].GroupManage.enableCityInfo = GroupManageInfo.enableCityInfo;
                SmartQQ.GroupList[gid].GroupManage.enableWiki = GroupManageInfo.enableWiki;
                SmartQQ.GroupList[gid].GroupManage.enableTranslate = GroupManageInfo.enableTranslate;

                if (SmartQQ.GroupList[gid].GroupManage.enable.Equals(""))
                    SmartQQ.GroupList[gid].GroupManage.enable = "true";
                if (SmartQQ.GroupList[gid].GroupManage.enableXHJ.Equals(""))
                    SmartQQ.GroupList[gid].GroupManage.enableXHJ = "true";
                if (SmartQQ.GroupList[gid].GroupManage.enableWeather.Equals(""))
                    SmartQQ.GroupList[gid].GroupManage.enableWeather = "true";
                if (SmartQQ.GroupList[gid].GroupManage.enableTalk.Equals(""))
                    SmartQQ.GroupList[gid].GroupManage.enableTalk = "true";
                if (SmartQQ.GroupList[gid].GroupManage.enableStudy.Equals(""))
                    SmartQQ.GroupList[gid].GroupManage.enableStudy = "true";
                if (SmartQQ.GroupList[gid].GroupManage.enableStock.Equals(""))
                    SmartQQ.GroupList[gid].GroupManage.enableStock = "true";
                if (SmartQQ.GroupList[gid].GroupManage.enableExchangeRate.Equals(""))
                    SmartQQ.GroupList[gid].GroupManage.enableExchangeRate = "true";
                if (SmartQQ.GroupList[gid].GroupManage.enableEmoje.Equals(""))
                    SmartQQ.GroupList[gid].GroupManage.enableEmoje = "true";
                if (SmartQQ.GroupList[gid].GroupManage.enableCityInfo.Equals(""))
                    SmartQQ.GroupList[gid].GroupManage.enableCityInfo = "true";
                if (SmartQQ.GroupList[gid].GroupManage.enableWiki.Equals(""))
                    SmartQQ.GroupList[gid].GroupManage.enableWiki = "true";
                if (SmartQQ.GroupList[gid].GroupManage.enableTranslate.Equals(""))
                    SmartQQ.GroupList[gid].GroupManage.enableTranslate = "true";
            }
            else if (Program.MainForm.buttonFriendSend != null && Program.MainForm.buttonFriendSend.Enabled != false)
            {
                if (!NoDicPassword)
                    SmartQQ.GroupList[gid].GroupManage.enable = "false";
                else
                    SmartQQ.GroupList[gid].GroupManage.enable = "true";
                SmartQQ.GroupList[gid].GroupManage.enableXHJ = "true";
                SmartQQ.GroupList[gid].GroupManage.enableWeather = "true";
                SmartQQ.GroupList[gid].GroupManage.enableTalk = "true";
                SmartQQ.GroupList[gid].GroupManage.enableStudy = "true";
                SmartQQ.GroupList[gid].GroupManage.enableStock = "true";
                SmartQQ.GroupList[gid].GroupManage.enableExchangeRate = "true";
                SmartQQ.GroupList[gid].GroupManage.enableEmoje = "true";
                SmartQQ.GroupList[gid].GroupManage.enableCityInfo = "true";
                SmartQQ.GroupList[gid].GroupManage.enableWiki = "true";
                SmartQQ.GroupList[gid].GroupManage.enableTranslate = "true";

                SmartQQ.Message_Send(1, gid, "如果需要使用小睿睿机器人，请群管理发送 群管理&启动机器人");
            }
        }
        /// <summary>
        /// 设置服务器上存储的群配置信息
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="option">配置项名称</param>
        /// <param name="value">配置项值</param>
        private static void SetGroupSetting(string gid, string option, string value)
        {
            if (!NoDicPassword)
            {
                string url = DicServer + "groupmanage.php?password=" + DicPassword + "&action=set&gno=" + SmartQQ.AID_GroupKey(gid) + "&option=" + option + "&value=" + value;
                string temp = HTTP.Get(url);
                JsonGroupManageModel GroupManageInfo = (JsonGroupManageModel)JsonConvert.DeserializeObject(temp, typeof(JsonGroupManageModel));
                if (GroupManageInfo.statu.Equals("fail"))
                    Program.MainForm.listBoxLog.Items.Insert(0, GroupManageInfo.statu + GroupManageInfo.error);
            }
            if (option.Equals("enable"))
                SmartQQ.GroupList[gid].GroupManage.enable = value;
            else if (option.Equals("enablexhj"))
                SmartQQ.GroupList[gid].GroupManage.enableXHJ = value;
            else if (option.Equals("enableWeather"))
                SmartQQ.GroupList[gid].GroupManage.enableWeather = value;
            else if (option.Equals("enabletalk"))
                SmartQQ.GroupList[gid].GroupManage.enableTalk = value;
            else if (option.Equals("enableStudy"))
                SmartQQ.GroupList[gid].GroupManage.enableStudy = value;
            else if (option.Equals("enableStock"))
                SmartQQ.GroupList[gid].GroupManage.enableStock = value;
            else if (option.Equals("enableExchangeRate"))
                SmartQQ.GroupList[gid].GroupManage.enableExchangeRate = value;
            else if (option.Equals("enableEmoje"))
                SmartQQ.GroupList[gid].GroupManage.enableEmoje = value;
            else if (option.Equals("enableCityInfo"))
                SmartQQ.GroupList[gid].GroupManage.enableCityInfo = value;
            else if (option.Equals("enableWiki"))
                SmartQQ.GroupList[gid].GroupManage.enableWiki = value;
            else if (option.Equals("enableTranslate"))
                SmartQQ.GroupList[gid].GroupManage.enableTranslate = value;
        }
        /// <summary>
        /// 从服务器获取AI的回复
        /// </summary>
        /// <param name="message">源语句</param>
        /// <param name="QQNum">发言用户的QQ</param>
        /// <param name="QunNum">发言的群</param>
        /// <returns></returns>
        private static string AIGet(string message, string QQNum, string QunNum = "NULL")
        {
            string url = DicServer + "gettalk.php?source=" + message + "&qqnum=" + QQNum + "&qunnum=" + QunNum;
            string temp = HTTP.Get(url);
            if (temp.Equals("None1") || temp.Equals("None2") || temp.Equals("None4"))
                temp = "";
            return temp;
        }
        /// <summary>
        /// 向服务器提交AI学习请求
        /// </summary>
        /// <param name="source">源语句</param>
        /// <param name="aim">目标语句</param>
        /// <param name="QQNum">发起学习用户的QQ</param>
        /// <param name="QunNum">发起学习的群</param>
        /// <param name="superstudy">是否为特权学习</param>
        /// <returns>用户友好的提示语</returns>
        public static string AIStudy(string source, string aim, string QQNum, string QunNum = "", bool superstudy = false)
        {
            Program.MainForm.listBoxLog.Items.Insert(0, "学习 " + source + " " + aim);
            string url;
            if (NoDicPassword)
                url = DicServer + "AddTalkRequest.php";
            else
                url = DicServer + "addtalk.php";
            string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&source=" + HttpUtility.UrlEncode(source) + "&aim=" + HttpUtility.UrlEncode(aim) + "&qqnum=" + HttpUtility.UrlEncode(QQNum) + "&qunnum=" + HttpUtility.UrlEncode(QunNum);
            if (superstudy)
                postdata = postdata + "&superstudy=true";
            else postdata = postdata + "&superstudy=false";

            string MsgGet = HTTP.Post(url, postdata);
            return MsgGet;
        }
        /// <summary>
        /// 将AI学习指令的返回代码翻译为用户友好的提示语
        /// </summary>
        /// <param name="result">代码</param>
        /// <param name="QQNum">发起学习用户QQ</param>
        /// <param name="source">源语句</param>
        /// <param name="aim">目标语句</param>
        /// <returns>用户友好的提示语</returns>
        public static string GetStudyFlagInfo(string result, string QQNum, string source, string aim)
        {
            switch (result)
            {
                case ("Success"): return "嗯嗯～小睿睿记住了～～" + Environment.NewLine + "主人说 " + source + " 时，小睿睿应该回答 " + aim;
                case ("Already"): return "小睿睿知道了啦～" + Environment.NewLine + "主人说 " + source + " 时，小睿睿应该回答 " + aim;
                case ("DisableStudy"): return "当前学习功能未开启";
                case ("IDDisabled"): return "小睿睿拒绝学习这句话，原因是：" + Environment.NewLine + "妈麻说，" + QQNum + "是坏人，小睿睿不能听他的话，详询管理员。";
                case ("Waitting"): return "小睿睿记下了" + QQNum + "提交的学习请求，不过小睿睿还得去问问语文老师呢～～主人先等等吧～～";
                case ("ForbiddenWord"): return "小睿睿拒绝学习这句话，原因是：" + Environment.NewLine + "根据相关法律法规和政策，账号" + QQNum + "提交的学习内容包含敏感词，详询管理员";
                case ("Forbidden"): return "小睿睿拒绝学习这句话，原因是：" + Environment.NewLine + "账号" + QQNum + "提交的学习内容被屏蔽，详询管理员";
                case ("NotSuper"): return "小睿睿拒绝学习这句话，原因是：" + Environment.NewLine + "账号" + QQNum + "不是特权用户，不能使用特权学习命令。";
                case ("pending"): return "小睿睿记录下了账号" + QQNum + "提交的学习请求，请耐心等待审核，欢迎加入小睿睿的小窝，群137777833。";
                default: return "小睿睿出错了，也许主人卖个萌就好了～～";
            }

        }
    }
}
