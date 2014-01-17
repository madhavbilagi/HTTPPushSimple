using System;
using System.Text;
using System.Net;

namespace HTTPPushSimple
{
    /// <summary>
    /// Summary description for StartUp.
    /// </summary>
    public class StartUp
    {
        public StartUp()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static void showusage() 
        {
            Console.WriteLine("Attempts to POST into to a URL - POSTed content is hard coded.");
            Console.WriteLine();
            //Console.WriteLine("Usage:");
            //Console.WriteLine("ClientPOST URL [postdata]");
            //Console.WriteLine();
            //Console.WriteLine("Examples:");
            //Console.WriteLine("ClientPOST http://www.microsoft.com s1=food&s2=bart");
        }


        [STAThread]
        public static void Main(string[] args) 
        {
            /*
                        if (args.Length < 1) 
                        {
                            showusage();
                        } 
                        else 
                        {
                            if (args.Length < 2 ) 
                            {
                                getPage(args[0], "s1=food&s2=bart");
                            } 
                            else 
                            {
                                getPage(args[0], args[1]);
                            }
                        }

                        Console.WriteLine();
                        Console.WriteLine("Press return to continue...");
                        Console.ReadLine();
            */
			ServicePointManager.CertificatePolicy = new AcceptUntrustedCertificatePolicy();
			// String strURL = "https://gateway3.go2mobile.net:14430/gateway/v3/gateway.aspx";

            String strURL = "http://gateway3.go2mobile.net:10030/gateway/v3/test.aspx";
            //String strURL = "http://localhost:8083/gateway/v3/gateway.aspx";
            // Proxy on localhost to see raw POST & response :  http://www.pocketsoap.com/tcpTrace/

            string strDestination_Addr = "+448234123456";

            //string strGreek = getGreekString();
            //string strText = strGreek ;
            
            string strText = "HTTPPushSimple " + DateTime.Now + " TEST MSG";

            String  strXML;
            int     iResult;
            XMLParse.getXML ( strDestination_Addr , strText, out strXML, out iResult );

            String strUser     = "aaa123";
            String strPassword = "bbb456";
            int intTabID       = 123; 
            int intSeqID       = 456; // combination to provide a unique reference of the POST

            String strPayload = "User=" + strUser + "&Password=" + strPassword + "&RequestID=" + intTabID + "_" + intSeqID + "&WIN_XML=" + strXML;

            Console.WriteLine("strURL = "     + strURL);
            Console.WriteLine("strPayload = " + strPayload);
            
            String strResponse;
            ClientPOST.getPage( strURL, strPayload, out strResponse );
            
            ProcessResponse ( strResponse );
            Console.WriteLine();
            
            Console.WriteLine("Press return to continue...");
            Console.ReadLine();

            return;
        }

        private static void ProcessResponse( String strXML )
        {

            String strTPTransactionID;
            bool blnXMLResponseOK;
            XMLParse.ParseResponse ( strXML, out strTPTransactionID, out blnXMLResponseOK  );

            Console.WriteLine();
            Console.WriteLine("strTPTransactionID = " + strTPTransactionID);
            Console.WriteLine("blnXMLResponseOK   = " + blnXMLResponseOK);

            String strCommsType      ="";
            int intTabID             =0;
            int intSeqID             =0;
            int intResponseValue     =0;
            try 
            {
                String[] strA = strTPTransactionID.Split('_');
                strCommsType         = strA[0];
                intTabID             = Int32.Parse(strA[1].TrimStart('T'));
                intSeqID             = Int32.Parse(strA[2].TrimStart('I').TrimStart('D'));
                intResponseValue     = Int32.Parse(strA[3].TrimStart('R'));
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine( ex.ToString());
                Console.WriteLine("\r\nIncorrect format response");
            }

            Console.WriteLine();
            Console.WriteLine("Comms Type         = " + strCommsType);
            Console.WriteLine("TabID              = " + intTabID);
            Console.WriteLine("SeqID              = " + intSeqID);
            Console.WriteLine("Response Value     = " + intResponseValue);

            Console.WriteLine();
            bool blnOverallResultOK =  blnXMLResponseOK  && (intResponseValue==0);
            Console.WriteLine("blnOverallResultOK = " + blnOverallResultOK );

            /* Some of the possible Response Values 
             * (more specific error nos may be added - see Interface Definition Document
              0 = Successfull acceptance of request (inc. XML is valid)
            301 = Invalid XML
            302 = Invalid Login
            406 = Encoding Types unmatched
            407 = DTD or XML Schema not known
            409 = Invalid HTTP Content Type, should start with "application/x-www-form-urlencoded"
            410 = DTD not found.
            416 = WIN website Internal - exSQL
            417 = WIN website Internal - exGen
            418 = WIN website Internal - exUA
            419 = Max_no_of_simultaneous_connections_exceeded
            423 = Configuration Database not currently available
            424 = Target Database not currently available
            425 = Configuration Database connection string invalid format
            426 = Target Database connection string invalid format 
            */
        }



    }
}
