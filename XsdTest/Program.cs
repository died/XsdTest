using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace XsdTest
{
    class Program
    {
        private const string Path = "..\\..\\Xsd\\";

        static void Main(string[] args)
        {
            //Check args
            if (args.Length>0 && args.Length != 2)
            {
                Help();
                return;
            }

            var fileName = args.Any()? args[0]: "test.xml";
            var typeName = args.Any() ? args[1] : "Confirmation";
            var rd = XmlReader.Create(fileName);
            var doc = XDocument.Load(rd);

            // Get MD5 hash
            var md5Hash = Tools.GetMd5String(doc.ToString());
            Console.WriteLine($"Validating:\t{fileName}");
            Console.WriteLine($"MD5 hash:\t{md5Hash}");

            //get purpose
            var cultInfo = new CultureInfo("en-US", false).TextInfo;    //avoid case sensitive
            if (!Enum.TryParse(cultInfo.ToTitleCase(typeName), out Enumeration.PurposeType purpose))
            {
                Console.WriteLine($"Can't convert {typeName} to Purpose");
                Console.ReadLine();
                return;
            }

            var success = Tools.XmlValidate(doc, purpose, Path, out var msg);
            if (success)
            {
                Tools.ColorText("Validate Success", ConsoleColor.Green);
                switch (purpose)
                {
                    case Enumeration.PurposeType.Creation:
                        var purchaseOrder = Tools.XmlToGeneric<PurchaseOrder>(doc);
                        Console.WriteLine(
                            $"PoPurpose:\t{purchaseOrder.PoPurpose}\nPoNumber:\t{purchaseOrder.PoNumber}\nFirst Item Qty:\t{purchaseOrder.PoLineItem.First().OrderQty}");
                        break;
                    case Enumeration.PurposeType.Confirmation:
                        var poConfirmation = Tools.XmlToGeneric<PoConfirmation>(doc);
                        Console.WriteLine(
                            $"PoPurpose:\t{poConfirmation.PoResponsePurpose}\nPoNumber:\t{poConfirmation.PoNumber}\nFirst Item ConfirmedDeliveryQty:\t{poConfirmation.PoLineItem.First().ConfirmedDeliveryQty}");
                        break;
                    //and so on
                }

                #region Print Model to XML result
                //var str = ObjToXml(result);
                //Console.WriteLine(str);
                #endregion

            }
            else
            {
                Tools.ColorText($"Validate Fail:{msg}", ConsoleColor.Red);
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

        private static void Help()
        {
            Tools.ColorText("XSD Validate Test Tool Guide\n",ConsoleColor.Yellow);
            Console.WriteLine(
                "XsdText.exe <File> <Purpose>\n\n" +
                "<File>:\t\tThe file you want to test.\n"+
                "<Purpose>:\tPurpose: Creation, Confirmation, T1Amendment, T1AmendmentConfirmation, T2Amendment,\n" +
                "\t\tT2AmendmentAcknowledgement, Cancellation, CancellationConfirmation\n");
            Console.WriteLine("Enter any key to exit...");
            Console.ReadLine();
        }
    }
}
