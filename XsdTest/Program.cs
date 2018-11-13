using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace XsdTest
{
    class Program
    {
        private static readonly string Path = "..\\..\\Xsd\\";
        static void Main(string[] args)
        {
            
            var fileName = args.Any()? args.First(): "input.xml";
            var rd = XmlReader.Create(fileName);
            var doc = XDocument.Load(rd);

            // Get MD5 hash
            var md5Hash = Tools.GetMd5String(doc.ToString());
            Console.WriteLine($"Validating:\t{fileName}");
            Console.WriteLine($"MD5 hash:\t{md5Hash}");

            var success = Tools.XmlValidate(doc, Enumeration.PurposeType.Creation, Path, out string msg);
            if (success)
            {
                var result = Tools.XmlToGeneric<PurchaseOrder>(doc);


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

            #region If didn't convert to model, old way to get value
            //foreach (var x in doc.Descendants("PurchaseOrder"))
            //{
            //    Console.WriteLine(x.Element("POPurpose"));
            //    Console.WriteLine(x.Element("PONumber")?.Value);
            //}
            #endregion
            Console.ReadLine();
        }


    }
}
