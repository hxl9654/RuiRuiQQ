using System;
using System.IO;
using System.Net;
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

    public static class HTTP
    {
        //网络通信相关
        public static CookieContainer cookies = new CookieContainer();
        static CookieCollection CookieCollection = new CookieCollection();
        static CookieContainer CookieContainer = new CookieContainer();

        public static string Get(string url, string referer = "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2", int timeout = 100000, Encoding encode = null)
        {
            string dat;
            HttpWebResponse res = null;
            HttpWebRequest req;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.CookieContainer = cookies;
                req.AllowAutoRedirect = false;
                req.Timeout = timeout;
                req.Referer = referer;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0;%20WOW64; rv:47.0) Gecko/20100101 Firefox/47.0";
                res = (HttpWebResponse)req.GetResponse();

                cookies.Add(res.Cookies);
            }
            catch (HttpException)
            {
                return "";
            }
            catch (WebException)
            {
                return "";
            }
            StreamReader reader;

            reader = new StreamReader(res.GetResponseStream(), encode == null ? Encoding.UTF8 : encode);
            dat = reader.ReadToEnd();

            res.Close();
            req.Abort();
            if (Program.formlogin != null)
            {
                Program.formlogin.textBoxLog.Text = dat;
                if (!dat.Equals(""))
                    Program.formlogin.listBoxLog.Items.Insert(0, dat);
            }
            return dat;
        }
        //http://www.itokit.com/2012/0721/74607.html
        public static string Post(string url, string data, string Referer = "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2", int timeout = 100000, Encoding encode = null)
        {
            string dat = "";
            HttpWebRequest req;
            try
            {
                req = WebRequest.Create(url) as HttpWebRequest;
                req.CookieContainer = cookies;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                req.Proxy = null;
                req.Timeout = timeout;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0;%20WOW64; rv:47.0) Gecko/20100101 Firefox/47.0";
                req.ProtocolVersion = HttpVersion.Version10;
                req.Referer = Referer;

                byte[] mybyte = Encoding.Default.GetBytes(data);
                req.ContentLength = mybyte.Length;

                Stream stream = req.GetRequestStream();
                stream.Write(mybyte, 0, mybyte.Length);


                HttpWebResponse res = req.GetResponse() as HttpWebResponse;

                cookies.Add(res.Cookies);
                stream.Close();

                StreamReader SR = new StreamReader(res.GetResponseStream(), encode == null ? Encoding.UTF8 : encode);
                dat = SR.ReadToEnd();
                res.Close();
                req.Abort();
            }
            catch (HttpException)
            {
                return "";
            }
            catch (WebException)
            {
                return "";
            }
            if (Program.formlogin != null)
            {
                Program.formlogin.textBoxLog.Text = dat;
                if (!dat.Equals(""))
                    Program.formlogin.listBoxLog.Items.Insert(0, dat);
            }
            return dat;
        }
        public delegate void Post_Async_Action(string data);
        private class Post_Async_Data
        {
            public HttpWebRequest req;
            public Post_Async_Action post_Async_Action;
        }
        public static void Post_Async(string url, string PostData, Post_Async_Action action, string Referer = "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2", int timeout = 100000)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.CookieContainer = cookies;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            req.Referer = Referer;
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0;%20WOW64; rv:47.0) Gecko/20100101 Firefox/47.0";
            req.Proxy = null;
            req.ProtocolVersion = HttpVersion.Version10;
            req.ContinueTimeout = timeout;

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(PostData);
            Stream stream = req.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            Post_Async_Data dat = new Post_Async_Data();
            dat.req = req;
            dat.post_Async_Action = action;
            req.BeginGetResponse(new AsyncCallback(Post_Async_ResponesProceed), dat);
        }

        private static void Post_Async_ResponesProceed(IAsyncResult ar)
        {
            StreamReader reader = null;
            Post_Async_Data dat = ar.AsyncState as Post_Async_Data;
            HttpWebRequest req = dat.req;
            HttpWebResponse res = req.GetResponse() as HttpWebResponse;
            reader = new StreamReader(res.GetResponseStream());
            string temp = reader.ReadToEnd();
            res.Close();
            req.Abort();
            dat.post_Async_Action(temp);
        }
    }
}
