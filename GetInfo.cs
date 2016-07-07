using Newtonsoft.Json;
using System;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
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
    public static class GetInfo
    {
        public static string GetTranslate(string str)
        {
            string lang = "";
            int strLen = str.Length;
            int bytLeng = System.Text.Encoding.UTF8.GetBytes(str).Length;
            if (strLen < bytLeng)
                lang = "en";
            if (lang.Equals(""))
                lang = "zh-CN";

            string messagetosend = "原文：" + str;

            string url = "https://translate.google.com/translate_a/single?client=t&sl=auto&tl=";
            url = url + lang + "&hl=zh-CN&dt=bd&dt=ex&dt=ld&dt=md&dt=qca&dt=rw&dt=rm&dt=ss&dt=t&dt=at&ie=UTF-8&oe=UTF-8&ssel=3&tsel=3&kc=0&tk=346111|219373&q=" + str;
            string temp = HTTP.Get(url,"", 2000);
            string[] tmp = temp.Split('\"');
            if (tmp.Length != 0 && tmp[1] != null)
                messagetosend = messagetosend + Environment.NewLine + "谷歌翻译：" + tmp[1];
            else
                messagetosend = messagetosend + Environment.NewLine + "谷歌翻译：异常";

            url = " http://fanyi.youdao.com/openapi.do?keyfrom=" + Program.formlogin.YoudaoKeyform + "&key=" + Program.formlogin.YoudaoKey + "&type=data&doctype=json&version=1.1&q=" + str;
            temp = HTTP.Get(url);
            JsonYoudaoTranslateModel dat = (JsonYoudaoTranslateModel)JsonConvert.DeserializeObject(temp, typeof(JsonYoudaoTranslateModel));
            if (dat.errorcode == 0)
            {
                if (dat.translation[0] != null)
                    messagetosend = messagetosend + Environment.NewLine + "有道翻译：" + dat.translation[0];
                else messagetosend = messagetosend + Environment.NewLine + "有道翻译：异常";
            }
            else if (dat.errorcode == 20)
                messagetosend = messagetosend + Environment.NewLine + "有道翻译：不支持或文本过长";
            else if (dat.errorcode == 50)
                messagetosend = messagetosend + Environment.NewLine + "有道翻译：有道API密钥错误";

            for (int i = 0; i < Program.formlogin.Badwords.Length; i++)
                if (messagetosend.Contains(Program.formlogin.Badwords[i]))
                {
                    messagetosend = messagetosend.Replace(Program.formlogin.Badwords[i], "***");
                }
            return messagetosend;
        }
        public static string GetWeather(string city, string target)
        {
            if ((!city.Equals("呼市郊区")) && (!city.Equals("津市")) && (!city.Equals("沙市")))
            {
                city = city.Replace("省", "");
                city = city.Replace("市", "");
            }
            city = city.Replace(" ", "");
            city = city.Replace("\r", "");
            city = city.Replace("\n", "");

            target = target.Replace(" ", "");
            target = target.Replace("\r", "");
            target = target.Replace("\n", "");
            string ans = "";
            string url, temp;
            if (target.Equals("雅虎"))
            {
                url = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20%28select%20woeid%20from%20geo.places%281%29%20where%20text=\"" + city + "\") and%20u=%22c%22&format=json";
                temp = HTTP.Get(url);
                JsonYahooWeatherModel weather = (JsonYahooWeatherModel)JsonConvert.DeserializeObject(temp, typeof(JsonYahooWeatherModel));
                if (weather.query.results == null)
                    return "未查询到指定城市 " + city + " 的天气信息";
                else
                {
                    ans = weather.query.results.channel.description + "（请核对城市名是否正确）";
                    for (int i = 0; i < weather.query.results.channel.item.forecast.Count; i++)
                        ans = ans + Environment.NewLine + "周" + getYahooWeak(weather.query.results.channel.item.forecast[i].day) + "：" + getYahooWeatherCode(weather.query.results.channel.item.forecast[i].code) + "，最高气温：" + weather.query.results.channel.item.forecast[i].high + "摄氏度，最低气温：" + weather.query.results.channel.item.forecast[i].low + "摄氏度";
                    return ans;
                }
            }
            if (target.Equals("指数"))
                target = "index";
            else
                target = "forecast";
            url = "https://ruiruiqq.hxlxz.com/weather.php?city=" + city + "&type=" + target;
            temp = HTTP.Get(url);
            if (temp.Equals("NoCity"))
            {
                return GetWeather(city, "雅虎");
            }

            if (target.Equals("forecast"))
            {
                JsonWeatherModel weather = (JsonWeatherModel)JsonConvert.DeserializeObject(temp, typeof(JsonWeatherModel));
                ans = "根据中国天气网于" + weather.f.f0 + "发布的气象预报，" + weather.c.c3 + "的气象信息如下：" + Environment.NewLine;
                if (weather.f.f1[0].fa != null && !weather.f.f1[0].fa.Equals(""))
                    ans = ans + "今天白天：" + SloveWeather(weather.f.f1[0].fa) + "，" + weather.f.f1[0].fc + "摄氏度，" + SloveWind(weather.f.f1[0].fe) + SloveWindPower(weather.f.f1[0].fg) + "。";
                else
                    ans = ans + "今天";
                ans = ans + "晚上：" + SloveWeather(weather.f.f1[0].fb) + "，" + weather.f.f1[0].fd + "摄氏度，" + SloveWind(weather.f.f1[0].ff) + SloveWindPower(weather.f.f1[0].fh) + "。日出日落时间：" + weather.f.f1[0].fi + Environment.NewLine;
                ans = ans + "明天白天：" + SloveWeather(weather.f.f1[1].fa) + "，" + weather.f.f1[1].fc + "摄氏度，" + SloveWind(weather.f.f1[1].fe) + SloveWindPower(weather.f.f1[1].fg) + "。";
                ans = ans + "晚上：" + SloveWeather(weather.f.f1[1].fb) + "，" + weather.f.f1[1].fd + "摄氏度，" + SloveWind(weather.f.f1[1].ff) + SloveWindPower(weather.f.f1[1].fh) + "。日出日落时间：" + weather.f.f1[1].fi + Environment.NewLine;
                ans = ans + "后天白天：" + SloveWeather(weather.f.f1[2].fa) + "，" + weather.f.f1[2].fc + "摄氏度，" + SloveWind(weather.f.f1[2].fe) + SloveWindPower(weather.f.f1[2].fg) + "。";
                ans = ans + "晚上：" + SloveWeather(weather.f.f1[2].fb) + "，" + weather.f.f1[2].fd + "摄氏度，" + SloveWind(weather.f.f1[2].ff) + SloveWindPower(weather.f.f1[2].fh) + "。日出日落时间：" + weather.f.f1[2].fi;
            }
            else if (target.Equals("index"))
            {
                JsonWeatherIndexModel WeatherIndex = (JsonWeatherIndexModel)JsonConvert.DeserializeObject(temp, typeof(JsonWeatherIndexModel));
                ans = "根据中国天气网发布的气象预报，" + city + "的气象信息如下：" + Environment.NewLine;
                ans = ans + WeatherIndex.i[0].i2 + "：" + WeatherIndex.i[0].i4 + "；" + WeatherIndex.i[0].i5 + Environment.NewLine;
                ans = ans + WeatherIndex.i[1].i2 + "：" + WeatherIndex.i[1].i4 + "；" + WeatherIndex.i[1].i5 + Environment.NewLine;
                ans = ans + WeatherIndex.i[2].i2 + "：" + WeatherIndex.i[2].i4 + "；" + WeatherIndex.i[2].i5;
            }
            return ans;

        }

        private static string getYahooWeatherCode(string code)
        {
            switch (code)
            {
                case "0": return "龙卷风";
                case "1": return "热带风暴";
                case "2": return "暴风";
                case "3": return "大雷雨";
                case "4": return "雷阵雨";
                case "5": return "雨夹雪";
                case "6": return "雨夹雹";
                case "7": return "雪夹雹";
                case "8": return "冻雾雨";
                case "9": return "细雨";
                case "10": return "冻雨";
                case "11": return "阵雨";
                case "12": return "阵雨";
                case "13": return "阵雪";
                case "14": return "小阵雪";
                case "15": return "高吹雪";
                case "16": return "雪";
                case "17": return "冰雹";
                case "18": return "雨淞";
                case "19": return "粉尘";
                case "20": return "雾";
                case "21": return "薄雾";
                case "22": return "烟雾";
                case "23": return "大风";
                case "24": return "风";
                case "25": return "冷";
                case "26": return "阴";
                case "27": return "多云";
                case "28": return "多云";
                case "29": return "局部多云";
                case "30": return "局部多云";
                case "31": return "晴";
                case "32": return "晴";
                case "33": return "转晴";
                case "34": return "转晴";
                case "35": return "雨夹冰雹";
                case "36": return "热";
                case "37": return "局部雷雨";
                case "38": return "偶有雷雨";
                case "39": return "偶有雷雨";
                case "40": return "偶有阵雨";
                case "41": return "大雪";
                case "42": return "零星阵雪";
                case "43": return "大雪";
                case "44": return "局部多云";
                case "45": return "雷阵雨";
                case "46": return "阵雪";
                case "47": return "局部雷阵雨";
                default: return "水深火热";
            }
        }

        private static string getYahooWeak(string day)
        {
            switch (day.ToLower())
            {
                case "mon": return "一";
                case "tue": return "二";
                case "wed": return "三";
                case "thu": return "四";
                case "fri": return "五";
                case "sat": return "六";
                case "sun": return "日";
                default: return day;
            }
        }

        private static string SloveWind(string code)
        {
            switch (code)
            {
                case ("0"): return "";
                case ("1"): return "东北风";
                case ("2"): return "东风";
                case ("3"): return "东南风";
                case ("4"): return "南风";
                case ("5"): return "西南风";
                case ("6"): return "西风";
                case ("7"): return "西北风";
                case ("8"): return "北风";
                case ("9"): return "旋转风";
                default: return "";
            }
        }

        private static string SloveWindPower(string code)
        {
            switch (code)
            {
                case ("0"): return "微风";
                case ("1"): return "3-4级";
                case ("2"): return "4-5级";
                case ("3"): return "5-6级";
                case ("4"): return "6-7级";
                case ("5"): return "7-8级";
                case ("6"): return "8-9级";
                case ("7"): return "9-10级";
                case ("8"): return "10-11级";
                case ("9"): return "11-12级";
                default: return "";
            }
        }
        public static string SloveWeather(string code)
        {
            switch (code)
            {
                case ("00"): return "晴";
                case ("01"): return "多云";
                case ("02"): return "阴";
                case ("03"): return "阵雨";
                case ("04"): return "雷阵雨";
                case ("05"): return "雷阵雨伴有冰雹";
                case ("06"): return "雨夹雪";
                case ("07"): return "小雨";
                case ("08"): return "中雨";
                case ("09"): return "大雨";
                case ("10"): return "暴雨";
                case ("11"): return "大暴雨";
                case ("12"): return "特大暴雨";
                case ("13"): return "阵雪";
                case ("14"): return "小雪";
                case ("15"): return "中雪";
                case ("16"): return "大雪";
                case ("17"): return "暴雪";
                case ("18"): return "雾";
                case ("19"): return "冻雨";
                case ("20"): return "沙尘暴";
                case ("21"): return "小到中雨";
                case ("22"): return "中到大雨";
                case ("23"): return "大到暴雨";
                case ("24"): return "暴雨到大暴雨";
                case ("25"): return "大暴雨到特大暴雨";
                case ("26"): return "小到中雪";
                case ("27"): return "中到大雪";
                case ("28"): return "大到暴雪";
                case ("29"): return "浮尘";
                case ("30"): return "扬沙";
                case ("31"): return "强沙尘暴";
                case ("53"): return "霾";
                case ("99"): return "无";
                default: return "暂时无法获取";
            }
        }
        public static string GetStudyFlagInfo(string result, string QQNum, string tmp1, string tmp2)
        {
            switch (result)
            {
                case ("Success"): return "嗯嗯～小睿睿记住了～～" + Environment.NewLine + "主人说 " + tmp1 + " 时，小睿睿应该回答 " + tmp2;
                case ("Already"): return "小睿睿知道了啦～" + Environment.NewLine + "主人说 " + tmp1 + " 时，小睿睿应该回答 " + tmp2;
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

        public static string GetExchangeRate(string p1, string p2)
        {
            string url = "https://query.yahooapis.com/v1/public/yql?q=select%20id,Rate%20from%20yahoo.finance.xchange%20where%20pair%20in%20%28%22";
            url += p1 + p2 + "%22%29&env=store://datatables.org/alltableswithkeys&format=json";
            string temp = HTTP.Get(url, "", 100000);
            JsonYahooExchangeRateModel ExchangeRateYahoo = (JsonYahooExchangeRateModel)JsonConvert.DeserializeObject(temp, typeof(JsonYahooExchangeRateModel));
            if (!ExchangeRateYahoo.query.results.rate.Rate.Equals("N/A"))
            {
                return "根据Yahoo!的信息，" + ExchangeRateYahoo.query.results.rate.id + "在UTC" + ExchangeRateYahoo.query.created + "的汇率是：" + ExchangeRateYahoo.query.results.rate.Rate + "。";
            }
            url = "https://www.cryptonator.com/api/ticker/" + p1 + "-" + p2;
            temp = HTTP.Get(url);
            JsonExchangeRateModel ExchangeRate = (JsonExchangeRateModel)JsonConvert.DeserializeObject(temp, typeof(JsonExchangeRateModel));
            if (ExchangeRate.success == true)
                return "根据cryptonator的信息，" + p1 + "-" + p2 + "的汇率：" + ExchangeRate.ticker.price;
            else return "Error:" + ExchangeRate.error;
        }
        public static string GetWiki(string keyword, string aim = "")
        {
            if (aim.Equals("互动百科") || aim.Equals("互动"))
            {
                string url = "http://www.baike.com/wiki/" + keyword;
                string temp = HTTP.Get(url);
                if (temp.Contains("尚未收录"))
                    return "没有找到这个词条哦～";
                temp = temp.Replace("<meta content=\"", "&");
                temp = temp.Replace("\" name=\"description\">", "&");
                string[] tmp = temp.Split('&');
                if (!tmp[1].Equals(""))
                    return tmp[1] + Environment.NewLine + "详情请查看http://www.baike.com/wiki/" + HttpUtility.UrlEncode(keyword);
                else
                    return "";
            }
            else if (aim.Equals("维基百科") || aim.Equals("维基"))
            {
                string url = "https://zh.wikipedia.org/w/api.php?action=query&prop=extracts&format=json&exsentences=2&exintro=&explaintext=&exsectionformat=plain&exvariant=zh&titles=" + keyword;
                string temp = HTTP.Get(url);
                for (int i = 0; i < Program.formlogin.Badwords.Length; i++)
                    if (temp.Contains(Program.formlogin.Badwords[i]) || keyword.Contains(Program.formlogin.Badwords[i]))
                    {
                        return "这个Wiki被河蟹吃掉了 QAQ";
                    }
                JsonWikipediaModel temp1 = (JsonWikipediaModel)JsonConvert.DeserializeObject(temp, typeof(JsonWikipediaModel));
                string[] tmp = temp1.query.pages.ToString().Split("{}".ToCharArray());
                JsonWikipediaPageModel pages = (JsonWikipediaPageModel)JsonConvert.DeserializeObject("{" + tmp[2] + "}", typeof(JsonWikipediaPageModel));

                if (pages.extract != null)
                    return pages.extract + Environment.NewLine + "详情请查看https://zh.wikipedia.org/wiki/" + HttpUtility.UrlEncode(keyword);
                else
                    return "没有找到这个Wiki哦～";
            }
            else if (aim.Equals("百度百科") || aim.Equals("百度"))
            {
                string url = "http://wapbaike.baidu.com/item/" + keyword;
                string temp = HTTP.Get(url);

                if (temp.Contains("您所访问的页面不存在"))
                    return "没有找到这个词条哦～";
                if (temp.Contains("百科名片"))
                {
                    temp = temp.Replace("&quot;", "");
                    temp = temp.Replace("&", "");
                    temp = temp.Replace("百科名片", "&");
                    string[] tmp = temp.Split('&');

                    temp = tmp[1];
                    temp = temp.Replace("<p>", "&");
                    temp = temp.Replace("</p>", "&");
                    tmp = temp.Split('&');

                    temp = tmp[1].Replace("</a>", "");
                    temp = temp.Replace("<b>", "");
                    temp = temp.Replace("</b>", "");
                    temp = temp.Replace("<i>", "");
                    temp = temp.Replace("</i>", "");

                    temp = temp.Replace("<a", "&");
                    temp = temp.Replace("\">", "&");
                    tmp = temp.Split('&');

                    temp = "";
                    for (int i = 0; i < tmp.Length; i += 2)
                        if ((!tmp[i].Contains("card-info")) && (!tmp[i].Contains("div class")))
                            temp += tmp[i];
                    if (!temp.Equals(""))
                        return temp + Environment.NewLine + "详情请查看http://wapbaike.baidu.com/item/" + HttpUtility.UrlEncode(keyword);
                    else
                        return "词条 " + keyword + " 请查看http://wapbaike.baidu.com/item/" + HttpUtility.UrlEncode(keyword);
                }
                else return "没有找到这个词条哦～";
            }
            else
            {
                string temp1 = GetWiki(keyword, "百度");
                if (temp1.Contains("查看"))
                    return temp1 + " --百度百科";

                temp1 = GetWiki(keyword, "互动");
                if (temp1.Contains("查看"))
                    return temp1 + " --互动百科";

                temp1 = GetWiki(keyword, "维基");
                if (temp1.Contains("查看"))
                    return temp1 + " --维基百科";

                else return "没有找到这个词条哦～";
            }
        }
        public static string GetStock(string p1, string p2 = "")
        {
            string url = "";

            p1 = p1.Replace(" ", "");
            p1 = p1.Replace("\r", "");
            p1 = p1.Replace("\n", "");
            if (!p2.Equals(""))
            {
                p2 = p2.Replace(" ", "");
                p2 = p2.Replace("\r", "");
                p2 = p2.Replace("\n", "");
            }
            switch (p1)
            {
                case ("上证指数"): url = "http://hq.sinajs.cn/list=s_sh000001"; break;
                case ("深证综指"): url = "http://hq.sinajs.cn/list=s_sz399106"; break;
                case ("中小板指数"): url = "http://hq.sinajs.cn/list=s_sz399005"; break;
                case ("创业板指数"): url = "http://hq.sinajs.cn/list=s_sz399006"; break;
                case ("深证成指"): url = "http://hq.sinajs.cn/list=s_sz399001"; break;
                case ("中小板综指"): url = "http://hq.sinajs.cn/list=s_sz399101"; break;
                case ("创业板综指"): url = "http://hq.sinajs.cn/list=s_sz399102"; break;
                default:
                    {
                        if (p1.ToCharArray()[0] == '6')
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
                        break;
                    }
            }
            string dat = HTTP.Get(url,"", 100000, Encoding.GetEncoding("GB2312"));

            string[] tmp = dat.Split('\"');
            tmp = tmp[1].Split(',');
            if (tmp.Length == 1)
                return "参数错误";
            string ans = "根据新浪财经的信息，" + tmp[0] + "：现价，" + tmp[1] + "；涨跌" + tmp[2] + "，" + tmp[3] + "%；成交量，" + tmp[4] + "手，" + tmp[5] + "万元。";
            return ans;
        }

        internal static string GetCityInfo(string city, string target)
        {
            if ((!city.Equals("呼市郊区")) && (!city.Equals("津市")) && (!city.Equals("沙市")))
            {
                city = city.Replace("省", "");
                city = city.Replace("市", "");
            }
            city = city.Replace(" ", "");
            city = city.Replace("\r", "");
            city = city.Replace("\n", "");

            target = target.Replace(" ", "");
            target = target.Replace("\r", "");
            target = target.Replace("\n", "");
            string ans = "";

            string url = "https://ruiruiqq.hxlxz.com/weather.php?city=" + city + "&type=forecast";
            string temp = HTTP.Get(url);
            if (temp.Equals("NoCity"))
                return "未查询到指定城市 " + city + " 的信息";

            JsonWeatherModel weather = (JsonWeatherModel)JsonConvert.DeserializeObject(temp, typeof(JsonWeatherModel));

            ans = "城市 " + weather.c.c3 + "（" + weather.c.c2 + "） 的信息如下：" + Environment.NewLine;
            ans += "所在省市：" + weather.c.c7 + "省" + weather.c.c5 + "市" + "（" + weather.c.c6 + " " + weather.c.c4 + "）" + Environment.NewLine;
            ans += "区号：" + weather.c.c11 + "，邮编：" + weather.c.c12 + "。城市级别：" + weather.c.c10 + "级城市" + Environment.NewLine;
            ans += "经度：" + weather.c.c13 + "，纬度：" + weather.c.c14 + "，海拔：" + weather.c.c15 + "。雷达站" + weather.c.c16;
            return ans;
        }
    }
}
