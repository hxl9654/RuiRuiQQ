using Newtonsoft.Json;
using System;
using System.Text;
using System.Web;
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

            if (target.Equals("指数"))
                target = "index";
            else
                target = "forecast";
            string url = "https://ruiruiqq.hxlxz.com/weather.php?city=" + city + "&type=" + target;
            string temp = HTTP.HttpGet(url);
            if (temp.Equals("NoCity"))
                return "未查询到指定城市 " + city + " 的天气信息";

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

        private static string SloveWind(string code)
        {
            if (code == "0")
                return "";
            else if (code == "1")
                return "东北风";
            else if (code == "2")
                return "东风";
            else if (code == "3")
                return "东南风";
            else if (code == "4")
                return "南风";
            else if (code == "5")
                return "西南风";
            else if (code == "6")
                return "西风";
            else if (code == "7")
                return "西北风";
            else if (code == "8")
                return "北风";
            else if (code == "9")
                return "旋转风";
            else
                return "";
        }

        private static string SloveWindPower(string code)
        {
            if (code == "0")
                return "微风";
            else if (code == "1")
                return "3-4级";
            else if (code == "2")
                return "4-5级";
            else if (code == "3")
                return "5-6级";
            else if (code == "4")
                return "6-7级";
            else if (code == "5")
                return "7-8级";
            else if (code == "6")
                return "8-9级";
            else if (code == "7")
                return "9-10级";
            else if (code == "8")
                return "10-11级";
            else if (code == "9")
                return "11-12级";
            else
                return "";
        }
        public static string SloveWeather(string code)
        {
            if (code == "00")
                return "晴";
            else if (code == "01")
                return "多云";
            else if (code == "02")
                return "阴";
            else if (code == "03")
                return "阵雨";
            else if (code == "04")
                return "雷阵雨";
            else if (code == "05")
                return "雷阵雨伴有冰雹";
            else if (code == "06")
                return "雨夹雪";
            else if (code == "07")
                return "小雨";
            else if (code == "08")
                return "中雨";
            else if (code == "09")
                return "大雨";
            else if (code == "10")
                return "暴雨";
            else if (code == "11")
                return "大暴雨";
            else if (code == "12")
                return "特大暴雨";
            else if (code == "13")
                return "阵雪";
            else if (code == "14")
                return "小雪";
            else if (code == "15")
                return "中雪";
            else if (code == "16")
                return "大雪";
            else if (code == "17")
                return "暴雪";
            else if (code == "18")
                return "雾";
            else if (code == "19")
                return "冻雨";
            else if (code == "20")
                return "沙尘暴";
            else if (code == "21")
                return "小到中雨";
            else if (code == "22")
                return "中到大雨";
            else if (code == "23")
                return "大到暴雨";
            else if (code == "24")
                return "暴雨到大暴雨";
            else if (code == "25")
                return "大暴雨到特大暴雨";
            else if (code == "26")
                return "小到中雪";
            else if (code == "27")
                return "中到大雪";
            else if (code == "28")
                return "大到暴雪";
            else if (code == "29")
                return "浮尘";
            else if (code == "30")
                return "扬沙";
            else if (code == "31")
                return "强沙尘暴";
            else if (code == "53")
                return "霾";
            else if (code == "99")
                return "无";
            else
                return "暂时无法获取";
        }
        public static string GetStudyFlagInfo(string result, string QQNum, string tmp1, string tmp2)
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
            else if (result.Equals("pending"))
            {
                return "小睿睿记录下了账号" + QQNum + "提交的学习请求，请耐心等待审核，欢迎加入小睿睿的小窝，群137777833。";
            }
            else
            {
                return "小睿睿出错了，也许主人卖个萌就好了～～";
            }
        }

        public static string GetExchangeRate(string p1, string p2)
        {
            string url = "https://query.yahooapis.com/v1/public/yql?q=select%20id,Rate%20from%20yahoo.finance.xchange%20where%20pair%20in%20%28%22";
            url += p1 + p2 + "%22%29&env=store://datatables.org/alltableswithkeys&format=json";
            string temp = HTTP.HttpGet(url, 100000, null, "");
            JsonYahooExchangeRateModel ExchangeRateYahoo = (JsonYahooExchangeRateModel)JsonConvert.DeserializeObject(temp, typeof(JsonYahooExchangeRateModel));
            if (!ExchangeRateYahoo.query.results.rate.Rate.Equals("N/A"))
            {
                return "根据Yahoo的信息，" + ExchangeRateYahoo.query.results.rate.id + "在UTC" + ExchangeRateYahoo.query.created + "的汇率是：" + ExchangeRateYahoo.query.results.rate.Rate + "。";
            }
            url = "https://www.cryptonator.com/api/ticker/" + p1 + "-" + p2;
            temp = HTTP.HttpGet(url);
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
                string temp = HTTP.HttpGet(url);
                temp = temp.Replace("<meta content=\"", "&");
                temp = temp.Replace("\" name=\"description\">", "&");
                string[] tmp = temp.Split('&');
                if (!tmp[1].Equals(""))
                    return tmp[1] + Environment.NewLine + "详情请查看http://www.baike.com/wiki/" + HttpUtility.UrlEncode(keyword);
                else
                    return "";
            }
            if (aim.Equals("维基百科") || aim.Equals("维基"))
            {
                string url = "https://zh.wikipedia.org/w/api.php?action=query&prop=extracts&format=json&exsentences=2&exintro=&explaintext=&exsectionformat=plain&exvariant=zh&titles=" + keyword;
                string temp = HTTP.HttpGet(url);
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
            else
            {
                string url = "http://wapbaike.baidu.com/item/" + keyword;
                string temp = HTTP.HttpGet(url);

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
                    for (int i = 0; i < tmp.Length; i+=2)
                        if ((!tmp[i].Contains("card-info"))&&(!tmp[i].Contains("div class")))
                            temp += tmp[i];
                    if (!temp.Equals(""))
                        return temp + Environment.NewLine + "详情请查看http://wapbaike.baidu.com/item/" + HttpUtility.UrlEncode(keyword);
                    else
                        return "词条 " + keyword + " 请查看http://wapbaike.baidu.com/item/" + HttpUtility.UrlEncode(keyword);
                }
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
            if (p1.Equals("上证指数"))
                url = "http://hq.sinajs.cn/list=s_sh000001";
            else if (p1.Equals("深证综指"))
                url = "http://hq.sinajs.cn/list=s_sz399106";
            else if (p1.Equals("中小板指数"))
                url = "http://hq.sinajs.cn/list=s_sz399005";
            else if (p1.Equals("创业板指数"))
                url = "http://hq.sinajs.cn/list=s_sz399006";
            else if (p1.Equals("深证成指"))
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
            string dat = HTTP.HttpGet(url, 100000, Encoding.GetEncoding("GB2312"));

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
            string temp = HTTP.HttpGet(url);
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
