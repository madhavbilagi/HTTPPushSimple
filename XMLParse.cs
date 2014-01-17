using System.Diagnostics;
using System;

using System.Data;

using System.IO;
using System.Xml;
using System.Text;


// http://www.fileformat.info/info/unicode/block/greek_and_coptic/images.htm

namespace HTTPPushSimple
{
    /// <summary>
    /// Summary description for XMLParse.
    /// </summary>
    public class XMLParse
    {
        public XMLParse()
        {
            //
            // TODO: Add constructor logic here  jhhjh  ghgh hggh hjfh
            //
        }


        public static void getXML ( 
            string                        strDestination_Addr,
            string                        strText,
            out String                    strXML,
            out int                       iResult )
        {
            int				          iEncodingPageNo;
            iEncodingPageNo = Encoding.UTF8.CodePage; 
            strXML          = "";
            iResult         = -123;
            /*
                <?xml version="1.0" standalone="no"?>
                <!DOCTYPE WIN_DELIVERY_2_SMS SYSTEM "winbound_messages_v1.dtd">
                <WIN_DELIVERY_2_SMS>
                <!-- E.g. minimal set of elements -->
                <SMSMESSAGE>
                    <DESTINATION_ADDR>+448234123456</DESTINATION_ADDR>
                    <TEXT><![CDATA[Dynamic Spoof Test 1]]></TEXT>
                    <TRANSACTIONID>111222333</TRANSACTIONID>
                    <TYPEID>2</TYPEID>
                    <SERVICEID>1</SERVICEID>
                    <COSTID>1</COSTID>
                </SMSMESSAGE>
            */
            
            //String strText = "Hello World";
            int intTPTransactionID = 1;
            int intTPTypeID        = 2;
            int intTPServiceID     = 1;
            int intCostID          = 1;

            MemoryStream  ms = null;
            Encoding enc = null;
            XmlTextWriter xmlTextReader = null;

            try
            {
                enc = System.Text.Encoding.GetEncoding( iEncodingPageNo ); 
                ms = new MemoryStream();
                xmlTextReader = new XmlTextWriter ( ms, enc ); 
                xmlTextReader.Formatting = Formatting.Indented; 
                xmlTextReader.WriteStartDocument(false);
                xmlTextReader.WriteDocType("WIN_DELIVERY_2_SMS", null, "winbound_messages_v1.dtd", null);

                xmlTextReader.WriteStartElement("WIN_DELIVERY_2_SMS");

                xmlTextReader.WriteStartElement("SMSMESSAGE");
                xmlTextReader.WriteElementString("DESTINATION_ADDR", null, strDestination_Addr);
                xmlTextReader.WriteElementString("TEXT", null, strText);
                xmlTextReader.WriteElementString("TRANSACTIONID", null, intTPTransactionID.ToString() );
                xmlTextReader.WriteElementString("TYPEID", null,        intTPTypeID.ToString() );
                xmlTextReader.WriteElementString("SERVICEID", null,     intTPServiceID.ToString() );
                xmlTextReader.WriteElementString("COSTID", null,        intCostID.ToString() );
                xmlTextReader.WriteEndElement(); // SMSMESSAGE

                xmlTextReader.WriteEndElement(); // WIN_DELIVERY_2_SMS

                xmlTextReader.WriteEndDocument();
                xmlTextReader.Flush();

                bool blnValid;
                string strReasonIfNot;
                XMLUtil.CheckValid( ms, out blnValid, out strReasonIfNot  );
                if ( !blnValid )
                {
                    Console.WriteLine( "Invalid XML generated \n" + strReasonIfNot + "\n" + strXML );

                    Console.WriteLine();
                    Console.WriteLine("Press return to continue...");
                    Console.ReadLine();

                    int exitCode = 0;
                    Environment.Exit(exitCode);
                }

                Byte[] plaintextbyte = ms.ToArray();
                xmlTextReader.Close();
                strXML = enc.GetString(plaintextbyte);

                if ( strXML[0] == 65279 ) 
                    strXML = strXML.Remove(0,1);
            }
            catch (Exception ex )
            {
                Console.WriteLine( "Exception ex :" + ex );

                Console.WriteLine();
                Console.WriteLine("Press return to continue...");
                Console.ReadLine();

                int exitCode = 0;
                Environment.Exit(exitCode);
            }


            return;
        }

        public static void ParseResponse ( String strXML, out String strTPTransactionID, out bool blnXMLResponseOK  )
        {
            blnXMLResponseOK = false;
            strTPTransactionID = "unassigned";
            // httpWebResponse.ContentEncoding

            StringReader stringReader;
            stringReader = new StringReader( strXML );

            XmlTextReader xmlTextReader = new XmlTextReader ( stringReader );
			
            XmlValidatingReader myXmlValidatingReader = new XmlValidatingReader( xmlTextReader );
            myXmlValidatingReader.ValidationType = ValidationType.DTD;

            bool blnValid = true;
            string strReasonIfNot;
            try 
            {
                while (myXmlValidatingReader.Read());
            }
            catch (Exception ex)
            {
                blnValid = false;
                strReasonIfNot = ex.Message;
            }


            if ( blnValid ) 
            {
                stringReader = new StringReader( strXML );
                xmlTextReader = new XmlTextReader ( stringReader );
                //J xmlTextReader.XmlResolver = r; 

                String strDTD = "winbound_messages_v1.dtd"; // initialise with the default
                XMLUtil.GetDTDName ( ref xmlTextReader, ref strDTD );
			
                stringReader  = new StringReader( strXML );
                xmlTextReader = new XmlTextReader ( stringReader  );
                //J xmlTextReader.XmlResolver = r; 			

                // farm out to process the different DTDs
                switch ( strDTD )
                {
                    case "response_generic_v1.dtd" :
						
                        /*
                            <SMSRESPONSE>
                            <REQUESTID>123</REQUESTID>
                            </SMSRESPONSE>
                            */


                        while (xmlTextReader.Read())
                        {
								
                            if (xmlTextReader.MoveToContent() == XmlNodeType.Element )
                            {
                                String strReaderName = xmlTextReader.Name;
					
                                if (strReaderName == "REQUESTID") 
                                {
                                    strTPTransactionID = xmlTextReader.ReadString();
                                    blnXMLResponseOK = true;
                                    break;
                                }
                            }
                        }
                        break;

                    default :
                        strTPTransactionID = "TPRefOutOfXML";
                        break;
                }
            }
            else
                strTPTransactionID = "TPRefInvalidResponse";

			
        }


    }
}
