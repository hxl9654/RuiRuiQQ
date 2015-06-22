using Newtonsoft.Json;
using System;
using System.Text;
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
        public static JsonWeatherCityCodeModel citycode;
        public static string GetWeather(string city, string target)
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
                    if (citycode.citycodes[i].cities[j].city.Equals(city))
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
                string ans = "";
                string url = "http://m.weather.com.cn/atad/" + citycodestring + ".html";
                string temp = HTTP.HttpGet(url);
                JsonWeatherModel weather = (JsonWeatherModel)JsonConvert.DeserializeObject(temp, typeof(JsonWeatherModel));
                if (target.Equals("五天") || target.Equals("5天") || target.Equals("五日") || target.Equals("5日"))
                {
                    string[] week = { "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期日" };
                    int WeekIndex = 0;
                    for (int i = 0; i < 7; i++)
                        if (weather.weatherinfo.week.Equals(week[i]))
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
            else
            {
                return "小睿睿出错了，也许主人卖个萌就好了～～";
            }
        }

        public static string GetExchangeRate(string p1, string p2)
        {
            string url = "https://www.cryptonator.com/api/ticker/" + p1 + "-" + p2;
            string temp = HTTP.HttpGet(url);
            JsonExchangeRateModel ExchangeRate = (JsonExchangeRateModel)JsonConvert.DeserializeObject(temp, typeof(JsonExchangeRateModel));
            if (ExchangeRate.success == true)
                return p1 + "-" + p2 + "的汇率：" + ExchangeRate.ticker.price;
            else return "Error:" + ExchangeRate.error;
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
    }
}
