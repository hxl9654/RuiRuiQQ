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
namespace RuiRuiQQRobot
{
    public class WechatTokenModel
    {
        public string access_token;
        public int expires_in;
        public int errcode;
        public string errmsg;
    }
    public class JosnConfigFileModel
    {
        public string DicServer;
        public string DicPassword;
        public string ID;
        public string Secrect;
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
}
