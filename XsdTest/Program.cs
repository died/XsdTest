using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XsdTest
{
    class Program
    {
        private static readonly string Path = "..\\..\\Xsd\\";
        static void Main()
        {
            var rd = XmlReader.Create("input.xml");
            var doc = XDocument.Load(rd);
            var success = XmlValidate(doc, Enumeration.PurposeType.Creation, Path, out string msg);
            if (success)
            {
                var result = XmlToGeneric<PurchaseOrder>(doc);
                //var result2 = (PurchaseOrder)XmlToObj(doc, typeof(PurchaseOrder)); //old way

                Console.WriteLine($"PoPurpose:\t{result.PoPurpose}");
                Console.WriteLine($"PoNumber:\t{result.PoNumber}");
                Console.WriteLine($"First Item Qty:\t{result.PoLineItem.First().OrderQty}" );

                #region Print Model to XML result
                //var str = ObjToXml(result);
                //Console.WriteLine(str);
                #endregion

            }
            else
            {
                Console.WriteLine($"Validate Fail:{msg}");   
            }

            #region If didn't convert to movel, old way to get value
            //foreach (var x in doc.Descendants("PurchaseOrder"))
            //{
            //    Console.WriteLine(x.Element("POPurpose"));
            //    Console.WriteLine(x.Element("PONumber")?.Value);
            //}
            #endregion
            Console.ReadLine();
        }

        #region Obj Handler
        /// <summary>
        /// Convert Xml to Model
        /// </summary>
        /// <typeparam name="T">Model Type</typeparam>
        /// <param name="xml">Xml XDocument</param>
        /// <returns></returns>
        static T XmlToGeneric<T>(XDocument xml)
        {
            T obj;
            try
            {
                using (TextReader reader = new StringReader(xml.ToString()))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    obj = (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                obj = default(T);
            }
            return obj;
        }

        static object XmlToObj(XDocument xml, Type objType)
        {
            object obj;
            try
            {
                XmlSerializer serializer = new XmlSerializer(objType);
                using (TextReader reader = new StringReader(xml.ToString()))
                {
                    obj = serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return obj;
        }

        static string ObjToXml(object obj)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var tw = new XmlTextWriter(stream, Encoding.UTF8))
                    {
                        var serializer = new XmlSerializer(obj.GetType());
                        serializer.Serialize(tw, obj);
                        using (var reader = new StreamReader(stream, Encoding.UTF8, true))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }
        #endregion

        #region Validate
        static bool XmlValidate(XDocument xml, Enumeration.PurposeType purpose, string path, out string message)
        {
            var result = true;
            message = string.Empty;

            var schema = GetSchema(purpose, path);
            if (schema == null)
            {
                Console.WriteLine("wrong purpose");
                return false;
            }

            try
            {
                xml.Validate(schema, ValidationEventHandler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                message = e.Message;
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Get XML schema by purpose
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        static XmlSchemaSet GetSchema(Enumeration.PurposeType purpose,string path)
        {
            var schema = new XmlSchemaSet();
            switch (purpose)
            {
                case Enumeration.PurposeType.Creation:      //step 1
                case Enumeration.PurposeType.T1Amendment:   //step 3
                    schema.Add("", path + "PurchaseOrderv1_0.xsd");
                    break;
                case Enumeration.PurposeType.Confirmation:  //step 2
                case Enumeration.PurposeType.T1AmendmentConfirmation:   //step 4
                case Enumeration.PurposeType.T2Amendment:   //step 5
                    schema.Add("", path + "POResponseAndAmendmentv1_0.xsd");
                    break;
                case Enumeration.PurposeType.T2AmendmentAcknowledgement://step 6
                case Enumeration.PurposeType.CancellationConfirmation:  //step 8
                    schema.Add("", path + "Acknowledgementv1_0.xsd");
                    break;
                case Enumeration.PurposeType.Cancellation:  //step 7
                    schema.Add("", path + "POCancellationv1_0.xsd");
                    break;
                default:
                    schema = null;
                    break;
            }
            return schema;
        }
        #endregion

        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    throw new Exception(e.Message);
                case XmlSeverityType.Warning:
                    Console.WriteLine(e.Message);
                    break;
            }
        }
    }
}
