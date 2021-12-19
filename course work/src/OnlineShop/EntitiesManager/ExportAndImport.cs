using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace EntitiesManager
{
    public static class ExportAndImport
    {
        public static void Export(long id, string path, OrderRep orderRep, ProductRep productRep)
        {
            new DirectoryInfo(path + "/Export").Create();
            Order order = orderRep.GetOrderById(id);
            XDocument doc = new XDocument();
            XElement element = new XElement($"Order_{id}", new List<XAttribute>()
            {
                new XAttribute("ID",order.id),
                new XAttribute("CustomerID",order.customerId),
                new XAttribute("OrderDate",order.orderDate),
                new XAttribute("Amount",order.amount)
                });
            doc.Add(element);
            doc.Save(path + "/Export/ExportOrder.xml");
            doc = new XDocument();
            element = new XElement("Products");
            foreach (long l in orderRep.GetProductIds(order))
            {
                Product p = productRep.GetProductById(l);
                XElement product = new XElement($"{p.productName.Replace(" ", "_").Replace("'", "")}");
                XAttribute xId = new XAttribute("ID", p.id);
                XAttribute xPrice = new XAttribute("Price", p.price);
                XAttribute xAvailability = new XAttribute("Availability", p.availability);
                XAttribute xData = new XAttribute("Created_At", p.createdAt);

                product.Add(xId, xPrice, xAvailability, xData);
                element.Add(product);
            }
            doc.Add(element);
            doc.Save(path + "/Export/ExportProducts.xml");

            ZipFile.CreateFromDirectory(path + "/Export", path + "/Export.zip");
        }
        public static void Import(long id, string fullName, OrderRep orderRep, out bool error)
        {
            try
            {
                XDocument doc = XDocument.Load(fullName);
                XElement el = doc.Element("Connections");
                foreach (XElement element in el.Elements())
                {
                    if (element.Attribute("ProductID").Value == id.ToString())
                        orderRep.AddProductConection(id, Convert.ToInt64(element.Attribute("OrderID").Value));
                }
                error = false;
            }
            catch
            {
                error = true;
            }
        }
    }
}
