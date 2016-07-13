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
        internal static string DicPassword = "";
        internal static string DicServer = "";
        internal static bool NoDicPassword = false;
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
                if (SmartQQ.GroupList[gid].GroupManage.enable.Equals("false"))
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
                string[] tmp = MessageToSend.Split(',');
                MessageToSend = "";
                for (int i = 0; i < tmp.Length; i++)
                    if (!tmp[i].StartsWith("[\"face\","))
                        MessageToSend += tmp[i];
            }
            SmartQQ.Message_Send(1, gid, MessageToSend);
        }
        /// <summary>
        /// 获取指定群的配置信息
        /// </summary>
        /// <param name="gid"></param>
        private static void GetGroupSetting(string gid)
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
            else
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
        private static string AIStudy(string source, string aim, string QQNum, string QunNum = "", bool superstudy = false)
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
