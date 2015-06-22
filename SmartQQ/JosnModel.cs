using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQQ
{
    class JsonFriendModel
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
    class JsonGroupModel
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
            }
        }
    }
    class JsonHeartPackMessage
    {
        public int retcode;     //状态码
        public string errmsg;   //错误信息
        public string t;        //被迫下线说明
        public string p;        //需要更换的ptwebqq
        public List<paramResult> result;

        public class paramResult
        {
            public String poll_type;
            public paramValue value;
            public class paramValue
            {
                //收到消息
                public List<object> content;
                public string from_uin;
                //群消息有send_uin，为群号
                public string send_uin;

                //上线提示
                public string uin;
                public string status;
                //异地登录
                public string reason;
            }
        }
    }
    class JosnConfigFileModel
    {
        public String DicServer;
        public String DicPassword;
        public String QQNum;
        public String QQPassword;
        public int ClientID;
    }
    class JsonExchangeRateModel
    {
        public bool success;
        public string error;
        public paramTicker ticker;
        public class paramTicker
        {
            public String price;
        }
    }
    class JsonWeatherCityCodeModel
    {
        public List<paramCitycode> citycodes;
        public class paramCitycode
        {
            public String province;
            public List<paramcities> cities;
            public class paramcities
            {
                public String city;
                public String code;
            }
        }

    }
    class JsonWeatherModel
    {
        public paramWeatherInfo weatherinfo;
        public class paramWeatherInfo
        {
            public String city;
            public String fchh;         //发布时间
            public String date_y;       //日期
            public String week;         //星期

            public String temp1;        //气温
            public String temp2;
            public String temp3;
            public String temp4;
            public String temp5;
            public String temp6;

            public String weather1;     //天气描述
            public String weather2;
            public String weather3;
            public String weather4;
            public String weather5;
            public String weather6;

            public String wind1;
            public String wind2;
            public String wind3;
            public String wind4;
            public String wind5;
            public String wind6;

            public String index;        //天气指数
            public String index_d;      //穿衣指数
            public String index_uv;     //紫外线指数
            public String index_xc;     //洗车指数
            public String index_tr;     //旅游指数
            public String index_co;     //舒适指数
            public String index_cl;     //晨练指数
            public String index_ls;     //晾晒指数
            public String index_ag;     //过敏指数
        }

    }
}
