    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Diagnostics;

    namespace HTTPPushSimple
    {

        public class XMLUtil
        {
            private XMLUtil()
            {
            }

            public static String StripReturns( String str ) 
            {
                String strNew="";
                for (int i = 0; i < str.Length; i++ )
                {
                    char ch = str[i];
                    if ( ch != '\n' || 
                        ch != '\r' )
                        strNew += ch;
                
                }
                return strNew;
            }

		

            public static void CheckValid( MemoryStream ms, out bool blnValid, out string strReasonIfNot )
            {
                long pos = ms.Position;
                ms.Seek( 0, SeekOrigin.Begin);

                XmlValidatingReader myXmlValidatingReader = new XmlValidatingReader( new XmlTextReader(ms) );
                //myXmlValidatingReader.Schemas.Add(null,"C:\\afds.xsd");
                myXmlValidatingReader.ValidationType = ValidationType.Auto;
                CheckValidXML( myXmlValidatingReader );

                ms.Seek( pos, SeekOrigin.Begin);

                blnValid = m_blnValidXML;
                strReasonIfNot = m_strReasonIfNot;
            }


            #region CheckValid
            private static bool m_blnValidXML = true;
            private static string m_strReasonIfNot = "";
            private static bool CheckValidXML(XmlValidatingReader myXmlValidatingReader)
            {
                try
                {
                    // Set the validation event handler
                    myXmlValidatingReader.ValidationEventHandler += new ValidationEventHandler (ValidationEventHandle);

                    // Read XML data
                    while (myXmlValidatingReader.Read())
                    {}

                    m_blnValidXML = true;
                    m_strReasonIfNot = "";

                }
                catch (XmlException e)
                {
                    m_blnValidXML = false;
                    m_strReasonIfNot =  e.Message;
                }

                catch (Exception e)
                {
                    m_blnValidXML = false;
                    m_strReasonIfNot =  e.Message;
                }

                return m_blnValidXML;
            }

            private static void ValidationEventHandle (object sender, ValidationEventArgs args)
            {
                m_blnValidXML = false;

                m_strReasonIfNot = args.Message;

                if (args.Severity == XmlSeverityType.Warning)
                {
                    m_strReasonIfNot = "No schema found to enforce validation.";
                } 
                else
                    if (args.Severity == XmlSeverityType.Error)
                {
                    m_strReasonIfNot = "validation error occurred when validating the instance document.";
                } 

                if (args.Exception != null) // XSD schema validation error
                {
                    m_strReasonIfNot = args.Exception.SourceUri + "," +  args.Exception.LinePosition + "," +  args.Exception.LineNumber;
                }

            }

            #endregion CheckValid

            public static void GetDTDName ( ref XmlTextReader xmlTextReader , ref string strDTD )
            {

                System.Xml.XmlNodeType nt = System.Xml.XmlNodeType.CDATA;
                try 
                {
                    while (xmlTextReader.Read())
                    {
                        nt = xmlTextReader.NodeType;

                        switch ( nt )
                        {
                            case XmlNodeType.Element: // The node is an Element
                                //m_Response.Write("TooFar");
                                goto End_While_Loop;

                            case XmlNodeType.DocumentType: // The node is a DocumentType
                                strDTD = xmlTextReader.GetAttribute("SYSTEM");
                                //m_Response.Write("reader.GetAttribute(SYSTEM) = " + reader.GetAttribute("SYSTEM") );
                                goto End_While_Loop;
                            default :
                                break;
                        }
                    }
                End_While_Loop:
                    ;

                }
                catch (	XmlException exXML )
                {
                    Trace.WriteLine( "exXML = " + exXML );
                }
                catch (	FileNotFoundException exFNF )
                {
                    Trace.WriteLine( "exXML = " + exFNF );
                }
                finally 
                {
                    ;
                }
			
            }


        }
    }
