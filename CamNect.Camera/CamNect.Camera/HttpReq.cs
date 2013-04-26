using System;
using System.IO;
using System.Net;


namespace CamNect.Camera
{
    public class HttpReq
    {

        public static string HttpGet(string url, string id, string pwd)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Credentials = new NetworkCredential(id, pwd);
            string result = null;


            // TODO Gérer l'erreur et la faire remonter
            try
            {
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

                StreamReader reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();

                resp.Close();
                reader.Close();
            }
            catch
            {
                string erreur = "requête http n'a pas reçu de réponse";
                System.Console.WriteLine(erreur);
            }


            return result;
        }

    }
}
