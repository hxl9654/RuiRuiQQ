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
namespace RuiRuiQQRobot
{

    public partial class FormLogin : Form
    {
        //系统配置相关
        string DicPassword = "";
        string DicServer = "";
        bool NoDicPassword = false;
        //多个函数要用到的变量
        string pin = string.Empty;
        //数据存储相关
        private int Count103 = 0;

        public string[] Badwords;

        private string[] Answer(string message, string uid)
        {
            string[] MessageToSend = new string[20];
            if (message.Equals(""))
                return MessageToSend;
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
            if (message.StartsWith("行情"))
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
                    string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(uid) + "&qunnum=" + HttpUtility.UrlEncode("wechat") + "&action=stock&p1=" + HttpUtility.UrlEncode(tmp[tmp.Length - 1]) + "&p2=NULL&p3=NULL&p4=NULL";
                    HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                    return MessageToSend;
                }
            }
            if (message.StartsWith("汇率"))
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
                    string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(uid) + "&qunnum=" + HttpUtility.UrlEncode("wechat") + "&action=exchangerate&p1=" + HttpUtility.UrlEncode(tmp[1]) + "&p2=" + HttpUtility.UrlEncode(tmp[2]) + "&p3=NULL&p4=NULL";
                    HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                    return MessageToSend;
                }
            }
            if (message.StartsWith("天气"))
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
                    string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(uid) + "&qunnum=" + HttpUtility.UrlEncode("wechat") + "&action=weather&p1=" + HttpUtility.UrlEncode(tmp[1]) + "&p2=NULL&p3=NULL&p4=NULL";
                    HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                    return MessageToSend;
                }
            }
            if (message.StartsWith("城市信息"))
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
                    string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(uid) + "&qunnum=" + HttpUtility.UrlEncode("wechat") + "&action=cityinfo&p1=" + HttpUtility.UrlEncode(tmp[1]) + "&p2=NULL&p3=NULL&p4=NULL";
                    HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                    return MessageToSend;
                }
            }
            if (message.StartsWith("百科"))
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
                    string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(uid) + "&qunnum=" + HttpUtility.UrlEncode("wechat") + "&action=wiki&p1=" + HttpUtility.UrlEncode(tmp[1]) + "&p2=NULL&p3=NULL&p4=NULL";
                    HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                    return MessageToSend;
                }
            }
            if (message.StartsWith("学习"))
            {
                bool StudyFlag = true;
                string[] tmp = message.Split('^');
                if ((!tmp[0].Equals("学习")) || tmp.Length != 3)
                {
                    tmp = message.Split('&');
                    if ((!tmp[0].Equals("学习")) || tmp.Length != 3)
                    {
                        StudyFlag = false;
                    }
                }
                if (tmp.Length != 3 || tmp[1].Replace(" ", "").Equals("") || tmp[2].Replace(" ", "").Equals(""))
                {
                    StudyFlag = false;
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
                        result = AIStudy(tmp[1], tmp[2], uid, "", false);
                    MessageToSend[0] = GetInfo.GetStudyFlagInfo(result, uid, tmp[1], tmp[2]);

                    string url = DicServer + "log.php";
                    string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(uid) + "&qunnum=" + HttpUtility.UrlEncode("wechat") + "&action=study&p1=" + HttpUtility.UrlEncode(tmp[1]) + "&p2=" + HttpUtility.UrlEncode(tmp[2]) + "&p3=NULL&p4=NULL";
                    HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                    return MessageToSend;
                }
            }

            MessageToSend[0] = AIGet(message, uid);
            if (!MessageToSend[0].Equals(""))
            {
                string url = DicServer + "log.php";
                string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(uid) + "&qunnum=" + HttpUtility.UrlEncode("wechat") + "&action=talk&p1=" + HttpUtility.UrlEncode(message) + "&p2=NULL&p3=NULL&p4=NULL";
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
                    MessageToSend[j] = AIGet(tmp1[i], uid, "");
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
                        MessageToSend[j] = AIGet(tmp2[i], uid, "");
                        j++;
                        MsgSendFlag = true;
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

                    }
                    return MessageToSend;
                }
                if (!MessageToSend[0].Equals(""))
                {
                    string url = DicServer + "log.php";
                    string postdata = "password=" + HttpUtility.UrlEncode(DicPassword) + "&qqnum=" + HttpUtility.UrlEncode(uid) + "&qunnum=" + HttpUtility.UrlEncode("wechat") + "&action=talk&p1=" + HttpUtility.UrlEncode(message) + "&p2=NULL&p3=NULL&p4=NULL";
                    HTTP.HttpPost(url, "", postdata, Encoding.UTF8, false);
                }
                return MessageToSend;
            }
            return MessageToSend;
        }

        private void ActionWhenResivedMessage(string uid, string message, string id)
        {
            string[] MessageToSendArray = Answer(message, uid);
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
            if (MessageToSend.Equals(""))
            {
                //SendMessage(id, "～ 小睿睿听不懂你在说什么呢。。。教教我吧～～" + Environment.NewLine + "格式 学习&主人的话&小睿睿的回复" + "\\\"");
            }
            else;//SendMessage(id, MessageToSend);

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
        private void FormLogin_Load(object sender, EventArgs e)
        {
            bool NoFile = false;
            byte[] byData = new byte[100000];
            char[] charData = new char[100000];
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
            if (!NoFile)
            {
                string tmp = "";
                for (int i = 0; i < charData.Length; i++)
                    tmp += charData[i];
                tmp += '\0';
                JosnConfigFileModel dat = (JosnConfigFileModel)JsonConvert.DeserializeObject(tmp, typeof(JosnConfigFileModel));
                textBoxID.Text = dat.ID;
                textBoxSecret.Text = dat.Secrect;
                DicPassword = dat.DicPassword;
                DicServer = dat.DicServer;
            }
            if (DicServer == null || DicServer.Equals(""))
                DicServer = "https://ruiruiqq.hxlxz.com/";
            if (DicPassword == null || DicPassword.Equals(""))
                NoDicPassword = true;
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
        }

        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            Wechat.login(textBoxID.Text, textBoxSecret.Text);
        }
        public FormLogin()
        {
            InitializeComponent();
        }
        private void timerHeart_Tick(object sender, EventArgs e)
        {
            string url = "https://ruirui.hxlxz.com/lookup.php?password=" + DicPassword;
            string temp = HTTP.HttpGet(url);
            string[] tmp = temp.Split('▲');
            for (int i = 0; i < tmp.Length; i++)
            {
                string[] tmp1 = tmp[i].Split('★');
                if (tmp1.Length != 4)
                    continue;
                textBoxMessage.Text +=( GetTime(tmp1[1]) + "   " + tmp1[2] + Environment.NewLine + tmp1[3] + Environment.NewLine);

                ActionWhenResivedMessage(tmp1[2], tmp1[3], tmp1[0]);
            }

        }

        private void listBoxLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxLog.Text = listBoxLog.Items[listBoxLog.SelectedIndex].ToString();
        }
        private void listBoxLog_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxLog.SelectedIndex == -1)
                return;
            Clipboard.SetDataObject(listBoxLog.Items[listBoxLog.SelectedIndex].ToString());
        }
        public static string GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow).ToString();
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
