using System;
using System.IO;
using System.Net;


namespace CamNect.Camera
{
    public class HttpReq
    {

        public static String HttpGet(string url)
        {
            String result;
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Credentials = new NetworkCredential("root", "root");          

            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            
            StreamReader reader = new StreamReader(resp.GetResponseStream());
            result = reader.ReadToEnd();

            resp.Close();
            reader.Close();
  
            return result;
        }

    }
}
