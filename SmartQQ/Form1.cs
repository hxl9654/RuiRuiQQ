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
        //网络通信相关
        CookieContainer cookies = new CookieContainer();
        CookieCollection CookieCollection = new CookieCollection();
        CookieContainer CookieContainer = new CookieContainer();

        public String HeartPackdata;
        bool StopSendingHeartPack = false;
        int AmountOfRunningPosting = 0;
        //系统配置相关
        private int MsgId;
        private int ClientID;
        string StudyPassword = "";
        string DicServer = "";
        bool DisableStudy = false;
        bool DisableWeather = false;
        //通信参数相关
        String ptvsession = "";
        String p_skey, MyUin, skey, p_uin, ptwebqq, vfwebqq, psessionid, hash;
        String pt_uin = "";
        //多个函数要用到的变量
        bool CAPTCHA = false;
        string pin = string.Empty;       
        String CaptchaCode;
        
        bool IsGroupSelent = false, IsFriendSelent = false;
        bool DoNotChangeSelentGroupOrPeople = false;
        //数据存储相关
        JsonGroupModel group;
        JsonFriendModel user;
        JsonWeatherCityCodeModel citycode;
        struct GroupInf
        {
            public String gid;
            public bool EnableRobot;
            public JsonGroupInfoModel inf;
        };
        GroupInf[] groupinfo = new GroupInf[100];
        int groupinfMaxIndex = 0;
        struct FriendInf
        {
            public String uin;
            public JsonFriendInfModel Inf;
        };
        FriendInf[] friendinf = new FriendInf[1000];
        int friendinfMaxIndex = 0;
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

            //一次登录
            String tokentemp = System.Text.RegularExpressions.Regex.Replace(pt_uin, @"\\x", "");
            String token = HexString2Ascii(tokentemp);
            string key = EncodePassword(textBoxPassword.Text, token, textBoxCAPTCHA.Text.ToUpper());

            String url1 = "https://ssl.ptlogin2.qq.com/login?u=";
            String url2 = "&p=";
            String url3 = "&verifycode=";
            String url4 = "&webqq_type=10&remember_uin=1&login2qq=1&aid=501004106&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html";
            String url5 = "%3Flogin2qq%3D1%26webqq_type%3D10&h=1&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1";
            String url6 = "&dumy=&fp=loginerroralert&action=0-15-19190&mibao_css=m_webqq&t=1&g=1&js_type=0&js_ver=10123&";
            String url7 = "login_sig=&pt_randsalt=0&pt_vcode_v1=0&pt_verifysession_v1=";
            String url = url1 + textBoxID.Text + url2 + key + url3 + textBoxCAPTCHA.Text + url4 + url5 + url6 + url7 + ptvsession;

            string temp = HttpGet(url);

            //二次登录准备
            temp = temp.Replace("ptui_checkVC(", "");
            temp = temp.Replace(");", "");
            temp = temp.Replace("'", "");
            string[] tmp = temp.Split(',');
            url = tmp[2];
            if (url == "")
            {
                MessageBox.Show("登录失败，请重试");
                if (CAPTCHA) GetCaptcha();
                return;
            }

            HttpGet(url);

            Uri uri = new Uri("http://web2.qq.com/");
            ptwebqq = cookies.GetCookies(uri)["ptwebqq"].Value;
            p_skey = cookies.GetCookies(uri)["p_skey"].Value;
            MyUin = cookies.GetCookies(uri)["uin"].Value;
            skey = cookies.GetCookies(uri)["skey"].Value;
            p_uin = cookies.GetCookies(uri)["p_uin"].Value;

            //二次登录
            url = "http://d.web2.qq.com/channel/login2";
            url1 = string.Format("r={{\"ptwebqq\":\"{0}\",\"clientid\":{1},\"psessionid\":\"\",\"status\":\"online\"}}", this.ptwebqq, this.ClientID);
            String dat = HttpPost(url, "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2", url1, Encoding.UTF8, true);

            textBoxLog.Text = dat;
            char[] t = new char[2];
            t[0] = ':';
            t[1] = ',';
            dat = dat.Replace("{", "");
            dat = dat.Replace("}", "");
            dat = dat.Replace("\"", "");
            tmp = dat.Split(t);

            vfwebqq = tmp[14];
            psessionid = tmp[16];

            hash = GetHash(textBoxID.Text, ptwebqq);
            getFrienf();
            getGroup();

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
        private void HeartPackAction(string temp)
        {
            string GName = "";
            string MessageFromUin = "";
            textBoxLog.Text = temp;
            if (temp == "{\"retcode\":121,\"t\":\"0\"}\r\n")
            {
                ReLogin();
                return;
            }
            else if (temp == "{\"retcode\":102,\"errmsg\":\"\"}\r\n" || temp == "{\"retcode\":0,\"result\":\"ok\"}\r\n")
            {
                return;
            }
            else if (temp == "{\"retcode\":108,\"errmsg\":\"\"}\r\n")
            {
                return;
            }
            listBoxLog.Items.Insert(0, temp);
            JsonHeartPackResponse result = (JsonHeartPackResponse)JsonConvert.DeserializeObject(temp, typeof(JsonHeartPackResponse));
            for (int i = 0; i < result.result.Count; i++)
            {
                if (result.result[i].poll_type == "buddies_status_change")
                {
                    return;
                }
                else if (result.result[i].poll_type == "kick_message")
                {
                    ReLogin();
                    listBoxLog.Items.Add(result.result[i].value.reason);
                    return;
                }
                else if (result.result[i].poll_type == "message")
                {
                    String message = "";
                    String emojis = "";
                    int j;
                    for(j =1;j<result.result[i].value.content.Count;j++)
                    {
                        string temp1 = result.result[i].value.content[j].ToString();
                        temp1 = temp1.Replace(Environment.NewLine, "");
                        temp1 = temp1.Replace(" ", "");
                        if (temp1.Contains("[\"face\","))
                        {
                            string emojiID = temp1.Replace("[\"face\",", "");
                            emojiID = emojiID.Replace("]", "");
                            emojis += (emojiID + ",");
                        }
                        else message += (result.result[i].value.content[j].ToString() + " ");
                    }
                    message = message.Replace("\\\\n", Environment.NewLine);
                    
                    for (j = 0; j < user.result.info.Count; j++)
                        if (user.result.info[j].uin == result.result[i].value.from_uin)
                        {
                            textBoxResiveMessage.Text += (user.result.info[j].nick + "  " + GetRealQQ(user.result.info[j].uin) + Environment.NewLine + message + "   " + emojis + Environment.NewLine + Environment.NewLine);
                            textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
                            textBoxResiveMessage.ScrollToCaret();
                            break;
                        }
                    if (j == user.result.info.Count)
                    {
                        getFrienf();
                        for (j = 0; j < user.result.info.Count; j++)
                            if (user.result.info[j].uin == result.result[i].value.from_uin)
                            {
                                textBoxResiveMessage.Text += (user.result.info[j].nick + "  " + GetRealQQ(user.result.info[j].uin) + Environment.NewLine + message + "   " + emojis + Environment.NewLine + Environment.NewLine);
                                textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
                                textBoxResiveMessage.ScrollToCaret();
                                break;
                            }
                    }
                    ActionWhenResivedMessage(result.result[i].value.from_uin, message, emojis);
                }
                else if (result.result[i].poll_type == "group_message")
                {
                    String emojis = "";
                    string message = "";
                    int j;
                    for (j = 1; j < result.result[i].value.content.Count; j++)
                    {
                        string temp1 = result.result[i].value.content[j].ToString();
                        temp1 = temp1.Replace(Environment.NewLine, "");
                        temp1 = temp1.Replace(" ", "");
                        if (temp1.Contains("[\"face\","))
                        {
                            string emojiID = temp1.Replace("[\"face\",", "");
                            emojiID = emojiID.Replace("]", "");
                            emojis += (emojiID + ",");
                        }
                        else message += (result.result[i].value.content[j].ToString() + " ");
                    }
                    message = message.Replace("\\\\n", Environment.NewLine);
                    string gid;
                    gid = result.result[i].value.from_uin;
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
                                if (groupinfo[j].inf.result.minfo[k].uin == result.result[i].value.send_uin)
                                {
                                    MessageFromUin = groupinfo[j].inf.result.minfo[k].uin;
                                    textBoxResiveMessage.Text += (GName + "   " + groupinfo[j].inf.result.minfo[k].nick + "  " + GetRealQQ(MessageFromUin) + Environment.NewLine + message + "   " + emojis + Environment.NewLine + Environment.NewLine);
                                    textBoxResiveMessage.SelectionStart = textBoxResiveMessage.TextLength;
                                    textBoxResiveMessage.ScrollToCaret();
                                    break;
                                }
                            break;
                        }
                    }
                    ActionWhenResivedGroupMessage(gid, message, emojis, MessageFromUin);


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
        private string[] Answer(string message, string uin, string gid)
        {            
            string[] MessageToSend = new string[20];
            message = message.Remove(message.Length - 2);
            if (message.Equals(""))
                return MessageToSend;
            string QQNum = GetRealQQ(uin);
            for (int i = 0; i < 20; i++)
                MessageToSend[i] = "";
            bool MsgSendFlag = false;
            if(!gid.Equals(""))
            {
                int i = -1;
                string adminuin = "";
                for (i = 0; i <= groupinfMaxIndex; i++)
                {
                    if (groupinfo[i].gid == gid)
                    {
                        adminuin = groupinfo[i].inf.result.ginfo.owner;
                        break;
                    }
                }
                if (message.Contains("群管理"))
                {
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
                        if (!uin.Equals(adminuin))
                        {
                            MessageToSend[0] = "账号" + GetRealQQ(uin) + "不是群主，无权进行此操作";
                            return MessageToSend;
                        }                           
                        else
                        {
                            if (tmp[1].Equals("关闭机器人"))
                            {
                                if (groupinfo[i].EnableRobot == false)
                                {
                                    MessageToSend[0] = "当前机器人已关闭";
                                    return MessageToSend;
                                }
                                else
                                {
                                    groupinfo[i].EnableRobot = false;
                                    MessageToSend[0] = "机器人关闭成功";
                                    return MessageToSend;
                                }
                            }
                            else if (tmp[1].Equals("启动机器人"))
                            {
                                if (groupinfo[i].EnableRobot == true)
                                {
                                    MessageToSend[0] = "当前机器人已启动";
                                    return MessageToSend;
                                }
                                else
                                {
                                    groupinfo[i].EnableRobot = true;
                                    MessageToSend[0] = "机器人启动成功";
                                    return MessageToSend;
                                }                                                                           
                            }                                
                        }
                        
                    }
                }
                if (i != -1 && groupinfo[i].EnableRobot == false) 
                {
                    MessageToSend[0] = "";
                    return MessageToSend;
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
                        MessageToSend[0] = GetStock(tmp[1], "");
                    else MessageToSend[0] = GetStock(tmp[1], tmp[2]);
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
                    MessageToSend[0] = GetExchangeRate(tmp[1], tmp[2]);
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
                        MessageToSend[0] = GetWeather(tmp[1], "");
                    else
                        MessageToSend[0] = GetWeather(tmp[1], tmp[2]);
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
                    MessageToSend[0] = GetStudyFlagInfo(result, QQNum, tmp[1], tmp[2]);
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
                    MessageToSend[0] = GetStudyFlagInfo(result, QQNum,tmp[1],tmp[2]);                    
                    return MessageToSend;
                }                
            }
            MessageToSend[0] = AIGet(message, QQNum);
            if (!MessageToSend[0].Equals(""))
            {
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

        private string GetStock(string p1, string p2="")
        {
            string url = "";
            
            p1 = p1.Replace(" ", "");
            p1 = p1.Replace("\r", "");
            p1 = p1.Replace("\n", "");
            if(!p2.Equals(""))
            {
                p2 = p2.Replace(" ", "");
                p2 = p2.Replace("\r", "");
                p2 = p2.Replace("\n", "");
            }
            if (p1.Equals("上证指数"))
                url = "http://hq.sinajs.cn/list=s_sh000001";
            else if (p1.Equals("深圳综指"))
                url = "http://hq.sinajs.cn/list=s_sz399106";
            else if (p1.Equals("中小板指数"))
                url = "http://hq.sinajs.cn/list=s_sz399005";
            else if (p1.Equals("创业板指数"))
                url = "http://hq.sinajs.cn/list=s_sz399006";
            else if (p1.Equals("深圳成指"))
                url = "http://hq.sinajs.cn/list=s_sz399001";
            else if (p1.Equals("中小板综指"))
                url = "http://hq.sinajs.cn/list=s_sz399101";
            else if (p1.Equals("创业板综指"))
                url = "http://hq.sinajs.cn/list=s_sz399102";
            else if (p1.ToCharArray()[0] == '6')
                url = "http://hq.sinajs.cn/list=s_sh" + p1;
            else if (p1.ToCharArray()[0] == '0' || p1.ToCharArray()[0] == '3')
                url = "http://hq.sinajs.cn/list=s_sz" + p1;
            else if (p1.Equals("上海") || p1.Equals("沪市") || p1.Equals("上证"))
            {
                url = "http://hq.sinajs.cn/list=s_sh" + p2;
            }
            else if (p1.Equals("深圳") || p1.Equals("深市") || p1.Equals("深证") || p1.Equals("创业板") || p1.Equals("中小板"))
            {
                url = "http://hq.sinajs.cn/list=s_sz" + p2;
            }
            else
                return "参数错误";
            string dat = HttpGet(url,100000,Encoding.GetEncoding("GB2312"));

            string[] tmp = dat.Split('\"');
            tmp = tmp[1].Split(',');
            if(tmp.Length==1)
                return "参数错误";
            string ans = "根据新浪财经的信息，" + tmp[0] + "：现价，" + tmp[1] + "；涨跌" + tmp[2] + "，" + tmp[3] + "%；成交量，" + tmp[4] + "手，" + tmp[5] + "万元。";
            return ans;
        }

        private string GetWeather(string city,string target)
        {
            bool FlagProvinceFound = false;
            bool FlagCityFound = false;
            string citycodestring = "";
            city = city.Replace("省", "");
            city = city.Replace("市", "");
            city = city.Replace(" ", "");
            city = city.Replace("\r", "");
            city = city.Replace("\n", "");

            target = target.Replace(" ", "");
            target = target.Replace("\r", "");
            target = target.Replace("\n", "");

            for (int i = 0; i < citycode.citycodes.Count; i++) 
            {
                if (citycode.citycodes[i].province.Equals(city)) 
                    FlagProvinceFound = true;
                for (int j = 0; j < citycode.citycodes[i].cities.Count; j++)
                {
                    if(citycode.citycodes[i].cities[j].city.Equals(city))
                    {
                        citycodestring = citycode.citycodes[i].cities[j].code;
                        FlagCityFound = true;
                        break;
                    }
                }
                if (FlagCityFound)
                    break;
            }
            if (FlagCityFound)
            {
                string ans="";
                string url = "http://m.weather.com.cn/atad/" + citycodestring + ".html";
                string temp = HttpGet(url);
                JsonWeatherModel weather = (JsonWeatherModel)JsonConvert.DeserializeObject(temp, typeof(JsonWeatherModel));
                if (target.Equals("五天") || target.Equals("5天") || target.Equals("五日") || target.Equals("5日"))
                {
                    string[] week = {"星期一","星期二","星期三","星期四","星期五","星期六","星期日"};
                    int WeekIndex = 0;
                    for (int i = 0; i < 7;i++ )
                        if(weather.weatherinfo.week.Equals(week[i]))
                        {
                            WeekIndex = i;
                            break;
                        } 
                    ans = "根据中国天气网于今天" + weather.weatherinfo.fchh + "时发布的气象预报，" + weather.weatherinfo.city + "的气象信息如下：" + Environment.NewLine;
                    ans = ans + "今天" + weather.weatherinfo.weather1 + "，气温" + weather.weatherinfo.temp1 + "，风力：" + weather.weatherinfo.wind1 + "。" + Environment.NewLine;
                    ans = ans + "明天" + weather.weatherinfo.weather2 + "，气温" + weather.weatherinfo.temp2 + "，风力：" + weather.weatherinfo.wind2 + "。" + Environment.NewLine;
                    ans = ans + "后天" + weather.weatherinfo.weather3 + "，气温" + weather.weatherinfo.temp3 + "，风力：" + weather.weatherinfo.wind3 + "。" + Environment.NewLine;
                    ans = ans + week[(WeekIndex + 3) % 7] + weather.weatherinfo.weather4 + "，气温" + weather.weatherinfo.temp4 + "，风力：" + weather.weatherinfo.wind4 + "。" + Environment.NewLine;
                    ans = ans + week[(WeekIndex + 4) % 7] + weather.weatherinfo.weather5 + "，气温" + weather.weatherinfo.temp5 + "，风力：" + weather.weatherinfo.wind5 + "。" + Environment.NewLine;
                    ans = ans + week[(WeekIndex + 5) % 7] + weather.weatherinfo.weather6 + "，气温" + weather.weatherinfo.temp6 + "，风力：" + weather.weatherinfo.wind6 + "。";
                }
                else if (target.Equals("指数"))
                {
                    ans = "根据中国天气网于今天" + weather.weatherinfo.fchh + "时发布的气象预报，" + weather.weatherinfo.city + "的气象指数如下：" + Environment.NewLine;
                    ans = ans + "天气指数：" + weather.weatherinfo.index + "，穿衣指数：" + weather.weatherinfo.index_d + "。" + Environment.NewLine;
                    ans = ans + "紫外线指数：" + weather.weatherinfo.index_uv + "，洗车指数：" + weather.weatherinfo.index_xc + "，晾晒指数：" + weather.weatherinfo.index_ls + "，旅游指数：" + weather.weatherinfo.index_tr + "。" + Environment.NewLine;
                    ans = ans + "晨练指数：" + weather.weatherinfo.index_cl + "，过敏指数：" + weather.weatherinfo.index_ag + "，舒适指数：" + weather.weatherinfo.index_co + "。";
                }
                else
                {
                    ans = "根据中国天气网于今天" + weather.weatherinfo.fchh + "时发布的气象预报，" + weather.weatherinfo.city + "：" + Environment.NewLine;
                    ans = ans + "今天" + weather.weatherinfo.weather1 + "，气温" + weather.weatherinfo.temp1 + "，风力：" + weather.weatherinfo.wind1 + "。" + Environment.NewLine;
                    ans = ans + "明天" + weather.weatherinfo.weather2 + "，气温" + weather.weatherinfo.temp2 + "，风力：" + weather.weatherinfo.wind2 + "。";
                }
                return ans;
            }
            else if (FlagProvinceFound)
            {
                return "查询天气时，请指定具体的城市，而不是省份。";
            }
            else return "未查询到指定城市 " + city + " 的天气信息";
        }

        private string GetStudyFlagInfo(string result, string QQNum, string tmp1, string tmp2)
        {
            if (result.Equals("Success"))
            {
                return "嗯嗯～小睿睿记住了～～" + Environment.NewLine + "主人说 " + tmp1 + " 时，小睿睿应该回答 " + tmp2;
            }
            else if (result.Equals("Already"))
            {
                return "小睿睿知道了啦～" + Environment.NewLine + "主人说 " + tmp1 + " 时，小睿睿应该回答 " + tmp2;
            }
            else if (result.Equals("DisableStudy"))
            {
                return "当前学习功能未开启";
            }
            else if (result.Equals("IDDisabled"))
            {
                return "小睿睿拒绝学习这句话，原因是：" + Environment.NewLine + "妈麻说，" + QQNum + "是坏人，小睿睿不能听他的话，详询管理员。";
            }
            else if (result.Equals("Waitting"))
            {
                return "小睿睿记下了" + QQNum + "提交的学习请求，不过小睿睿还得去问问语文老师呢～～";
            }
            else if (result.Equals("ForbiddenWord"))
            {
                return "小睿睿拒绝学习这句话，原因是：" + Environment.NewLine + "根据相关法律法规和政策，账号" + QQNum + "提交的学习内容包含敏感词，详询管理员";
            }
            else if (result.Equals("Forbidden"))
            {
                return "小睿睿拒绝学习这句话，原因是：" + Environment.NewLine + "账号" + QQNum + "提交的学习内容被屏蔽，详询管理员";
            }
            else if (result.Equals("NotSuper"))
            {
                return "小睿睿拒绝学习这句话，原因是：" + Environment.NewLine + "账号" + QQNum + "不是特权用户，不能使用特权学习命令。";
            }
            else
            {
                return "小睿睿出错了，也许主人卖个萌就好了～～";
            }
        }

        private string GetExchangeRate(string p1, string p2)
        {
            string url = "https://www.cryptonator.com/api/ticker/" + p1 + "-" + p2;
            string temp = HttpGet(url);
            JsonExchangeRateModel ExchangeRate = (JsonExchangeRateModel)JsonConvert.DeserializeObject(temp, typeof(JsonExchangeRateModel));
            if (ExchangeRate.success == true)
                return p1 + "-" + p2 + "的汇率：" + ExchangeRate.ticker.price;
            else return "Error:" + ExchangeRate.error;
        }
        private void ActionWhenResivedGroupMessage(string gid, string message, string emojis, string uin)
        {
            string[] MessageToSendArray = Answer(message, uin, gid);
            string MessageToSend = "";
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
            SendMessageToGroup(gid, MessageToSend);
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
                    getFrienf();
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
                SendMessageToFriend(uin, "\\\"" + SenderName + Gender + "～ 小睿睿听不懂你在说什么呢。。。教教我吧～～" + Environment.NewLine + "格式 学习^语句^设定的回复" + "\\\"");
            }
            else SendMessageToFriend(uin, MessageToSend);

        }
        private string AIGet(string message, string QQNum)
        {
            String url = DicServer + "gettalk.php?source=" + message + "&qqnum=" + QQNum;
            string temp = HttpGet(url);
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
            string temp = HttpGet(url);
            return temp;
        }

        public void getFrienf()
        {
            String url = "http://s.web2.qq.com/api/get_user_friends2";
            String sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", vfwebqq, this.hash);
            String dat = HttpPost(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1", sendData, Encoding.UTF8, true);
            textBoxLog.Text = dat;

            user = (JsonFriendModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendModel));
            listBoxFriend.Items.Clear();
            for (int i = 0; i < user.result.info.Count; i++)
            {
                string gender;
                friendinf[i].uin = user.result.info[i].uin;
                friendinf[i].Inf = GetFriendInf(user.result.info[i].uin);
                if (friendinfMaxIndex < i)
                    friendinfMaxIndex = i;
                if (friendinf[i].Inf.result.gender.Equals("female"))
                    gender = "妹纸";
                else if (friendinf[i].Inf.result.gender.Equals("male"))
                    gender = "汉子";
                else gender = "未知";
                listBoxFriend.Items.Add(user.result.info[i].uin + ":" + GetRealQQ(user.result.info[i].uin) + ":" + user.result.info[i].nick + ":" + gender);
            }
        }
        public JsonGroupInfoModel GetGroupInfo(string gcode)
        {
            String url = "http://s.web2.qq.com/api/get_group_info_ext2?gcode=" + gcode + "&vfwebqq=" + vfwebqq + "&t=" + GetTimeStamp();
            string dat = HttpGet(url);

            JsonGroupInfoModel ans = (JsonGroupInfoModel)JsonConvert.DeserializeObject(dat, typeof(JsonGroupInfoModel));
            return ans;
        }
        public void getGroup()
        {
            String url = "http://s.web2.qq.com/api/get_group_name_list_mask2";
            String sendData = string.Format("r={{\"vfwebqq\":\"{0}\",\"hash\":\"{1}\"}}", this.vfwebqq, this.hash);
            String dat = HttpPost(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1", sendData, Encoding.UTF8, true);
            textBoxLog.Text = dat;

            group = (JsonGroupModel)JsonConvert.DeserializeObject(dat, typeof(JsonGroupModel));
            listBoxGroup.Items.Clear();
            int i;
            for (i = 0; i < group.result.gnamelist.Count; i++)
            {
                listBoxGroup.Items.Add(group.result.gnamelist[i].gid + ":" + group.result.gnamelist[i].name);
                groupinfo[i].gid = group.result.gnamelist[i].gid;
                groupinfo[i].inf = GetGroupInfo(group.result.gnamelist[i].code);
                groupinfo[i].EnableRobot = true;
            }
            groupinfMaxIndex = i;
        }
        public JsonFriendInfModel GetFriendInf(string uin)
        {
            String url = "http://s.web2.qq.com/api/get_friend_info2?tuin=" + uin + "&vfwebqq=" + vfwebqq + "&clientid=" + ClientID + "&psessionid=" + psessionid + "&t=" + GetTimeStamp();

            string dat = HttpGet(url);
            JsonFriendInfModel ans = (JsonFriendInfModel)JsonConvert.DeserializeObject(dat, typeof(JsonFriendInfModel));
            return ans;
        }
        public string GetXiaoHuangJi(string msg)
        {
            string url = "http://www.xiaohuangji.com/ajax.php";
            string postdata = "para=" + HttpUtility.UrlEncode(msg);
            string MsgGet = HttpPost(url, "http://www.xiaohuangji.com/", postdata, Encoding.UTF8, false, 10000);
            return MsgGet;
        }
        private void textBoxID_LostFocus(object sender, EventArgs e)
        {
            if (textBoxID.Text.Length == 0)
                return;
            String str1 = "https://ssl.ptlogin2.qq.com/check?pt_tea=1&uin=";
            String str2 = "&appid=501004106&js_ver=10121&js_type=0&login_sig=&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html&r=0.4053995015565306";
            String url = str1 + textBoxID.Text + str2;

            string dat = HttpGet(url);

            dat = dat.Replace("ptui_checkVC(", "");
            dat = dat.Replace(");", "");
            dat = dat.Replace("'", "");
            string[] tmp = dat.Split(',');

            pt_uin = tmp[2];

            if (tmp[0] == "1")
            {
                CAPTCHA = true;

                CaptchaCode = tmp[1];
                GetCaptcha();

                pictureBoxCAPTCHA.Visible = true;
                textBoxCAPTCHA.Visible = true;
                label3.Visible = true;
            }
            else
            {
                CAPTCHA = false;
                pictureBoxCAPTCHA.Visible = false;
                textBoxCAPTCHA.Visible = false;
                label3.Visible = false;
                textBoxCAPTCHA.Text = tmp[1];
                ptvsession = tmp[3];
            }

        }
        public void GetCaptcha()
        {
            textBoxCAPTCHA.Text = "";

            String strimg1 = "https://ssl.captcha.qq.com/getimage?aid=501004106&r=0.005933324107900262&uin=";
            String strimg2 = "&cap_cd=";
            String strimg = strimg1 + textBoxID.Text + strimg2 + CaptchaCode;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strimg);
            req.CookieContainer = cookies;
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            pictureBoxCAPTCHA.Image = Image.FromStream(res.GetResponseStream());

            ptvsession = res.Cookies["verifysession"].Value;
            res.Close();
        }
        String GetRealQQ(string uin)
        {

            String url = "http://s.web2.qq.com/api/get_friend_uin2?tuin=" + uin + "&type=1&vfwebqq=" + vfwebqq + "&t=" + GetTimeStamp();

            string dat = HttpGet(url);

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

            return tmp[0];
        }
        //感谢QQBOT群（346167134） 杨小泡的热心帮助！
        internal static string HexString2Ascii(string hexString)
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
        private String GetHash(String no, String ptwebqq)
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
        public string HttpGet(string url, int timeout = 100000, Encoding encode=null)
        {
            string dat;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url); 
            HttpWebResponse res = null;
            req.CookieContainer = cookies;
            req.Timeout = timeout;
            req.Referer = "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2";
            try
            {
                res = (HttpWebResponse)req.GetResponse();
            }
            catch(HttpException)
            {
                return "";
            }
            StreamReader reader;
            if (encode != null)
                reader = new StreamReader(res.GetResponseStream(), encode);
            else
                reader = new StreamReader(res.GetResponseStream());
            dat = reader.ReadToEnd();
            res.Close();
            req.Abort();
            textBoxLog.Text = dat;
            listBoxLog.Items.Insert(0, dat);
            return dat;
        }
        //http://www.itokit.com/2012/0721/74607.html
        public string HttpPost(string url, string Referer, string data, Encoding encode, bool SaveCookie, int timeout = 100000)
        {
            string dat = "";
            if (AmountOfRunningPosting == 0)
                System.GC.Collect();
            AmountOfRunningPosting++;
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.CookieContainer = this.cookies;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:30.0) Gecko/20100101 Firefox/30.0";
            req.Proxy = null;
            req.Timeout = timeout;
            req.ProtocolVersion = HttpVersion.Version10;
            if (!string.IsNullOrEmpty(Referer))
                req.Referer = Referer;
            byte[] mybyte = Encoding.Default.GetBytes(data);
            req.ContentLength = mybyte.Length;
            try
            {
                Stream stream = req.GetRequestStream();
                stream.Write(mybyte, 0, mybyte.Length);


                HttpWebResponse hwr = req.GetResponse() as HttpWebResponse;
                stream.Close();
                if (SaveCookie)
                {
                    this.CookieCollection = hwr.Cookies;
                    this.cookies.GetCookies(req.RequestUri);
                }
                StreamReader SR = new StreamReader(hwr.GetResponseStream(), encode);
                dat = SR.ReadToEnd();
                hwr.Close();
                req.Abort();
            }            
            catch(HttpException)
            {
                return "";
            }
            
            textBoxLog.Text = dat;
            listBoxLog.Items.Insert(0, dat);
            AmountOfRunningPosting--;
            return dat;
        }
        public void HeartPack()
        {
            System.GC.Collect();
            String url = "http://d.web2.qq.com/channel/poll2";
            String sendData1 = "r= {\"ptwebqq\":\"";
            String sendData2 = "\",\"clientid\":";
            String sendData3 = ",\"psessionid\":\"";
            String sendData4 = "\",\"key\":\"\"}";
            HeartPackdata = sendData1 + ptwebqq + sendData2 + ClientID.ToString() + sendData3 + psessionid + sendData4;

            Encoding encode = Encoding.UTF8;
            string Referer = "http://d.web2.qq.com/proxy.html?v=20130916001&callback=1&id=2";

            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.CookieContainer = this.cookies;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            req.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:30.0) Gecko/20100101 Firefox/30.0";
            req.Proxy = null;
            req.ProtocolVersion = HttpVersion.Version10;
            if (!string.IsNullOrEmpty(Referer))
                req.Referer = Referer;

            req.BeginGetRequestStream(new AsyncCallback(RequestProceed), req);
        }
        private void RequestProceed(IAsyncResult asyncResult)
        {

            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            StreamWriter postDataWriter = new StreamWriter(request.EndGetRequestStream(asyncResult));
            postDataWriter.Write(HeartPackdata);
            postDataWriter.Close();
            request.BeginGetResponse(new AsyncCallback(ResponesProceed), request);
        }
        private void ResponesProceed(IAsyncResult ar)
        {
            StreamReader reader = null;
            HttpWebRequest req = ar.AsyncState as HttpWebRequest;
            HttpWebResponse res = req.GetResponse() as HttpWebResponse;
            reader = new StreamReader(res.GetResponseStream());
            String temp = reader.ReadToEnd();
            res.Close();
            req.Abort();
            HeartPackAction(temp);
        }
        private void pictureBoxCAPTCHA_Click(object sender, EventArgs e)
        {
            GetCaptcha();
        }
        private void FormLogin_Load(object sender, EventArgs e)
        {
            bool NoFile = false;
            byte[] byData = new byte[100000];
            char[] charData = new char[100000];
            Control.CheckForIllegalCrossThreadCalls = false;
            System.Net.ServicePointManager.DefaultConnectionLimit = 500;
            Random rd = new Random();
            MsgId = rd.Next(10000000, 50000000);
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
                ClientID = dat.ClientID;
            }
            else
            {
                DicServer = "http://smartqq.hxlxz.com/";
                DisableStudy = true;
                ClientID = rd.Next(1000000, 9999999); ;
            }
            if (textBoxID.Text.Length > 0)
            {
                GetCaptcha();
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
                citycode = (JsonWeatherCityCodeModel)JsonConvert.DeserializeObject(tmp, typeof(JsonWeatherCityCodeModel));
            }
            else DisableWeather = true;
        }
        public FormLogin()
        {
            InitializeComponent();
        }
        private void timerHeart_Tick(object sender, EventArgs e)
        {
            if (!StopSendingHeartPack) HeartPack();
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
            if (CAPTCHA) GetCaptcha();
            listBoxLog.Items.Insert(0, "账号" + textBoxID.Text + "已登出");
        }
        private void buttonLogout_Click(object sender, EventArgs e)
        {
            LogOut();
        }
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string ret = string.Empty;
            ret = Convert.ToInt64(ts.TotalMilliseconds).ToString();

            return ret;
        }

        //http://www.cnblogs.com/lianmin/p/4257421.html
        /// 发送好友消息
        public bool SendMessageToFriend(string uid, string content)
        {
            if (content.Equals(""))
                return false;
            this.MsgId++;
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

                string dat = HttpPost(url, referer, postData, Encoding.UTF8, false);

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
        public bool SendMessageToGroup(string gin, string content)
        {
            if (content.Equals(""))
                return false;
            this.MsgId++;
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

                string dat = HttpPost(url, referer, postData, Encoding.UTF8, false);

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
                SendMessageToGroup(tmp[0], textBoxSendMessage.Text);

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
                SendMessageToFriend(tmp[0], textBoxSendMessage.Text);

                for (int i = 0; i < user.result.info.Count; i++)
                    if (user.result.info[i].uin == tmp[0])
                    {
                        Nick = user.result.info[i].nick;
                        break;
                    }
                textBoxResiveMessage.Text += ("发送至   " + Nick + "   " + GetRealQQ(tmp[0]) + Environment.NewLine + textBoxSendMessage.Text + Environment.NewLine + Environment.NewLine);
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
