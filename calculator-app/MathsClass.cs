using System;
using calculatorApp.OperationClass;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace calculatorApp.MathsClass
{
    [XmlRoot(ElementName = "MyMaths")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Maths // to be combined with the above class
    {
        [XmlElement(ElementName = "MyOperation")]
        [JsonProperty("MyOperation")]
        public Operation Operation { get; set; }
    }
}
