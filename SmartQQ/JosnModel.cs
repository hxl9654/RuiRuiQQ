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
    public class JsonGroupMemberModel
    {
        public int retcode;
        public paramResult result;
        public class paramResult
        {
            public List<paramMinfo> minfo;
            public class paramMinfo
            {
                public string nick;
                public string province;
                public string gender;
                public string uin;
                public string country;
                public string city;
            }
        }
    }
    class JsonHeartPackResponse
    {
        public int retcode;
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
            public String temp1;
            public String temp2;
            public String weather;
            public String ptime;
        }

    }
}
