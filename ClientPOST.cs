using System;
using System.Net;
using System.IO;
using System.Text;
using System.Web;

// Slightly modded http://samples.gotdotnet.com/quickstart/howto/

namespace HTTPPushSimple
{

    class ClientPOST
    {
        public static void getPage(String url, String payload, out String strResponse) 
        {
            WebResponse result = null;
            strResponse = "";

            try 
            {

                WebRequest req = WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                StringBuilder UrlEncoded = new StringBuilder();
                Char[] reserved = {'?', '=', '&'};
                byte[] SomeBytes = null;

                if (payload != null) 
                {
                    int i=0, j;
                    while(i<payload.Length)
                    {
                        j=payload.IndexOfAny(reserved, i);
                        if (j==-1)
                        {
                            UrlEncoded.Append(HttpUtility.UrlEncode(payload.Substring(i, payload.Length-i)));
                            break;
                        }
                        UrlEncoded.Append(HttpUtility.UrlEncode(payload.Substring(i, j-i)));
                        UrlEncoded.Append(payload.Substring(j,1));
                        i = j+1;
                    }
                    SomeBytes = Encoding.UTF8.GetBytes(UrlEncoded.ToString());
                    req.ContentLength = SomeBytes.Length;
                    Stream newStream = req.GetRequestStream();
                    newStream.Write(SomeBytes, 0, SomeBytes.Length);
                    newStream.Close();
                } 
                else 
                {
                    req.ContentLength = 0;
                }


                result = req.GetResponse();
                Stream ReceiveStream = result.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader sr = new StreamReader( ReceiveStream, encode );
                Console.WriteLine("\r\nResponse stream received");
                Char[] read = new Char[256];
                int count = sr.Read( read, 0, 256 );
                Console.WriteLine("...\r\n");
                StringBuilder sb = new StringBuilder(500);
                while (count > 0) 
                {
                    String str = new String(read, 0, count);
                    sb.Append( str );
                    Console.Write(str);
                    count = sr.Read(read, 0, 256);
                }
                strResponse = sb.ToString();
                Console.WriteLine("");
            } 
            catch(Exception e) 
            {
                Console.WriteLine( e.ToString());
                Console.WriteLine("\r\nThe request URI could not be found or was malformed");
            } 
            finally 
            {
                if ( result != null ) 
                {
                    result.Close();
                }
            }
        }
    }
}



