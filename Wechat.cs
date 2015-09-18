using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace RuiRuiQQRobot
{
    static class Wechat
    {
        public static string login(string AppID, string AppSecret)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + AppID + "&secret=" + AppSecret;
            string temp = HTTP.HttpGet(url);
            WechatTokenModel WechatToken = (WechatTokenModel)JsonConvert.DeserializeObject(temp, typeof(WechatTokenModel));
            if (WechatToken.errcode == 0)
                return WechatToken.access_token;
            else return login(AppID, AppSecret);
        }
    }
}
