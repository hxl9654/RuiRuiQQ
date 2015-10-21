using System;
using System.Collections.Generic;
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
    public class JsonFriendModel
    {
        public int retcode;
        public paramResult result;
        public class paramResult
        {
            /// 分组信息
            public List<paramCategories> categories;
            /// 好友汇总
            public List<paramFriends> friends;
            /// 好友信息
            public List<paramInfo> info;
            /// 备注
            public List<paramMarkNames> marknames;
            /// 分组
            public class paramCategories
            {
                public int index;
                public int sort;
                public string name;
            }
            /// 好友汇总 
            public class paramFriends
            {
                public int flag;
                public string uin;
                public int categories;
            }
            /// 好友信息
            public class paramInfo
            {
                public int face;
                public string nick;
                public string uin;
            }
            /// 备注 
            public class paramMarkNames
            {
                public string uin;
                public string markname;
            }
        }
    }
    public class JsonGroupModel
    {
        public int retcode;
        public paramResult result;
        public class paramResult
        {
            public List<paramGnamelist> gnamelist;
            public class paramGnamelist
            {
                public string flag;
                public string gid;
                public string code;
                public string name;
            }
        }
    }
    public class JsonFriendInfModel
    {
        public int retcode;
        public paramResult result;
        public class paramResult
        {
            public paramBirthday birthday;
            public string occupation;
            public string phone;
            public string college;
            public int constel;
            public int blood;
            public string homepage;
            public int stat;
            public string city;
            public string personal;
            public string nick;
            public int shengxiao;
            public string email;
            public string province;
            public string gender;
            public string mobile;
            public class paramBirthday
            {
                public int month;
                public int year;
                public int day;
            }
        }
    }
    public class JsonGroupInfoModel
    {
        public int retcode;
        public paramResult result;
        public class paramResult
        {
            public List<paramMinfo> minfo;
            public paramGinfo ginfo;
            public class paramMinfo
            {
                public string nick;
                public string province;
                public string gender;
                public string uin;
                public string country;
                public string city;
            }
            public class paramGinfo
            {
                public string code;
                public string createtime;
                public string flag;
                public string name;
                public string gid;
                public string owner;
                public List<paramMembers> members;
                public class paramMembers
                {
                    public string muin;
                    public int mflag;
                }
            }
        }
    }
    public class JsonHeartPackMessage
    {
        public int retcode;     //状态码
        public string errmsg;   //错误信息
        public string t;        //被迫下线说明
        public string p;        //需要更换的ptwebqq
        public List<paramResult> result;

        public class paramResult
        {
            public string poll_type;
            public paramValue value;
            public class paramValue
            {
                //收到消息
                public List<object> content;
                public string from_uin;
                //群消息有send_uin，为特征群号；info_seq为群号
                public string send_uin;
                public string info_seq;
                //上线提示
                public string uin;
                public string status;
                //异地登录
                public string reason;
                //临时会话
                public string id;
                public string ruin;
                public string service_type;
            }
        }
    }
    public class JosnConfigFileModel
    {
        public string AdminQQ;
        public string DicServer;
        public string DicPassword;
        public string QQNum;
        public string QQPassword;
        public int ClientID;
        public string YoudaoKeyfrom;
        public string YoudaoKey;
    }
    public class JsonExchangeRateModel
    {
        public bool success;
        public string error;
        public paramTicker ticker;
        public class paramTicker
        {
            public string price;
        }
    }
    public class JsonYahooExchangeRateModel
    {
        public paramQuery query;
        public class paramQuery
        {
            public string created;
            public paramResults results;
            public class paramResults
            {
                public paramRate rate;
                public class paramRate
                {
                    public string id;
                    public string Rate;
                }
            }
        }
    }
    public class JsonGroupManageModel
    {
        public string enable;
        public string enableWeather;
        public string enableExchangeRate;
        public string enableStock;
        public string enableStudy;
        public string enabletalk;
        public string enablexhj;
        public string enableEmoje;
        public string enableCityInfo;
        public string enableWiki;
        public string enableTranslate;

        public string gno;
        public string statu;
        public string error;
    }
    public class JsonWeatherModel
    {
        public paramC c;
        public paramF f;
        public class paramC
        {
            public string c1;   //区域ID
            public string c2;   //城市英文名
            public string c3;   //城市中文名
            public string c4;   //城市所在市英文名
            public string c5;   //城市所在市中文名
            public string c6;   //城市所在省英文名
            public string c7;   //城市所在省中文名
            public string c8;   //城市所在国英文名
            public string c9;   //城市所在国中文名
            public string c10;  //城市级别
            public string c11;  //城市区号
            public string c12;  //邮编
            public string c13;  //经度
            public string c14;  //纬度
            public string c15;  //海拔
            public string c16;  //雷达站号
            public string c17;  //时区
        }
        public class paramF
        {
            public string f0;   //预报发布时间
            public List<paramF1> f1;
        }
        public class paramF1
        {
            public string fa;   //白天天气现象编号
            public string fb;   //晚上天气现象编号
            public string fc;   //白天气温
            public string fd;   //晚上气温
            public string fe;   //白天风向编号
            public string ff;   //晚上风向编号
            public string fg;   //白天风力编号
            public string fh;   //晚上风力编号
            public string fi;   //日出日落时间
        }
    }
    public class JsonWeatherIndexModel
    {
        public List<paramI> i;
        public class paramI
        {
            public string i1;   //指数简称
            public string i2;   //指数名称
            public string i3;   //指数别称
            public string i4;   //指数级别
            public string i5;   //级别说明
        }
    }
    public class JsonWikipediaModel
    {
        public string batchcomplete;
        public paramQuery query;
        public class paramQuery
        {
            public object pages;
        }
    }
    public class JsonWikipediaPageModel
    {
        public int pageid;
        public string title;
        public string extract;
    }
    public class JsonYoudaoTranslateModel
    {
        public int errorcode;
        public string query;
        public List<string> translation;
    }
    public class JsonYahooWeatherModel
    {
        public paramQuery query;
        public class paramQuery
        {
            public string created;
            public paramResults results;
            public class paramResults
            {
                public paramChannel channel;
                public class paramChannel
                {
                    public string description;
                    public paramItem item;
                    public class paramItem
                    {
                        public List<parmForecast> forecast;
                        public class parmForecast
                        {
                            public string code;
                            public string date;
                            public string day;
                            public string high;
                            public string low;
                            public string text;
                        }
                    }
                }
            }
        }
    }
}
