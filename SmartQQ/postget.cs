using System;
using System.Text;
public class postget
{
    public static string GetUrltoHtml(string Url, string type)
    {
        try
        {
            System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
            System.Net.WebResponse wResp = wReq.GetResponse();
            System.IO.Stream respStream = wResp.GetResponseStream();
            using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
            {
                return reader.ReadToEnd();
            }
        }
        catch (System.Exception ex)
        {
            String errorMsg = ex.Message;
        }
        return "";
    }  
}
