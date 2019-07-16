using System.Xml.Linq;
using System.Xml.Serialization;

namespace CmSmsGateway.Api
{
    internal class CmXmlGenerator
    {
        public static string Generate(string productionToken, CmMessage message)
        {
            XElement xml = new XElement("MESSAGES", new XElement("AUTHENTICATION", new XElement("PRODUCTTOKEN", productionToken)),
                                new XElement("MSG",
                                    new XElement("FROM", message.From),
                                    new XElement("TO", message.To),
                                    new XElement("DCS", message.ContentType == ContentType.Text ? ContentType.Text.ToString("D") : ContentType.UnicodeText.ToString("D") ),
                                    new XElement("BODY", message.Body),
                                    new XElement("MINIMUMNUMBEROFMESSAGEPARTS", message.MinimumNumberofMessageParts),
                                    new XElement("MAXIMUMNUMBEROFMESSAGEPARTS", message.MaximumNumberofMessageParts)
                                    ));
           
            return xml.ToString();
        }
    }
}
