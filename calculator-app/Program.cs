using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.IO;

namespace calculatorApp.Core
{
    // Interface for math operations
    public interface IOperation
    {
        double Calculate();
    }

    // Define enum for the 4 different operators
    public enum Operator
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }


    public class Operation : IOperation
    {
        [XmlElement(ElementName = "Value")]
        [JsonProperty("Value")]
        public List<double> Value { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        [JsonProperty("ID")]
        public Operator ID { get; set; }

        [XmlElement(ElementName = "MyOperation")]
        [JsonProperty("MyOperation")]
        public Operation NestedOperation { get; set; }

        public Operation()
        {
            Value = new List<double>();
        }

        public virtual double Calculate()
        {
            double result = 0.0;

            switch (ID)
            {
                case Operator.Addition:
                    AdditionOperation additionOperation = new AdditionOperation(Value);
                    result = additionOperation.Calculate();
                    break;
                case Operator.Subtraction:
                    SubtractionOperation subtractionOperation = new SubtractionOperation(Value);
                    result = subtractionOperation.Calculate();
                    break;
                case Operator.Multiplication:
                    MultiplicationOperation multiplicationOperation = new MultiplicationOperation(Value);
                    result = multiplicationOperation.Calculate();
                    break;
                case Operator.Division:
                    DivisionOperation divisionOperation = new DivisionOperation(Value);
                    result = divisionOperation.Calculate();
                    break;
                default:
                    throw new InvalidOperationException("Unsupported operation");
            }

            if (NestedOperation != null)
            {
                double nestedResult = NestedOperation.Calculate();

                switch (ID)
                {
                    case Operator.Addition:
                        result += nestedResult;
                        break;
                    case Operator.Subtraction:
                        result -= nestedResult;
                        break;
                    case Operator.Multiplication:
                        result *= nestedResult;
                        break;
                    case Operator.Division:
                        if (nestedResult == 0)
                            throw new DivideByZeroException("Cannot divide by zero in nested operation");
                        else
                            result /= nestedResult;
                        break;
                }
            }
            return result;
        }

    }

    // Addition Operation inheriting Operation
    public class AdditionOperation : Operation
    {
        // Constructor to initialize the Value property
        public AdditionOperation(List<double> values)
        {
            Value = values;
        }

        public override double Calculate()
        {
            double result = 0;

            foreach (var value in Value)
                result += value;

            return result;
        }
    }

    // Subtraction Operation inheriting Operation
    public class SubtractionOperation : Operation
    {

        // Constructor to initialize the Value property
        public SubtractionOperation(List<double> values)
        {
            Value = values;
        }

        public override double Calculate()
        {
            double result = Value[0];

            for (int i = 1; i < Value.Count(); i++)
                result -= Value[i];

            return result;
        }
    }

    //Multiplication Operation inheriting Operation
    public class MultiplicationOperation : Operation
    {
        // Constructor to initialize the Value property
        public MultiplicationOperation(List<double> values)
        {
            Value = values;
        }

        public override double Calculate()
        {
            double result = 1;

            foreach (var value in Value)
                result *= value;

            return result;
        }
    }

    //Division Operation inheriting Operation
    public class DivisionOperation : Operation
    {
        // Constructor to initialize the Value property
        public DivisionOperation(List<double> values)
        {
            Value = values;
        }

        public override double Calculate()
        {
            double result = Value[0];

            for (int i = 1; i < Value.Count(); i++)
                if (Value[i] == 0)
                    throw new DivideByZeroException("Cannot divide by zero");
                else
                    result /= Value[i];

            return result;
        }
    }

    [XmlRoot(ElementName = "MyMaths")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Maths // to be combined with the above class
    {
        [XmlElement(ElementName = "MyOperation")]
        [JsonProperty("MyOperation")]
        public Operation Operation { get; set; }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            //XML string example
            string xmlString = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <MyMaths>
                        <MyOperation ID=""Addition"">
                            <Value>2</Value>
                            <Value>3</Value>
                            <MyOperation ID=""Multiplication"">
                                <Value>4</Value>
                                <Value>5</Value>
                            </MyOperation>
                        </MyOperation>
                    </MyMaths>";

            XmlSerializer serializer = new XmlSerializer(typeof(Maths));

            Maths xmlMaths;

            // Deserialize XML
            using (StringReader reader = new StringReader(xmlString))
            {
                xmlMaths = (Maths)serializer.Deserialize(reader);
            }

            double xmlResult = xmlMaths.Operation.Calculate();

            //JSON string example
            string jsonString = @"{
            'MyOperation': {
                'ID': 'Addition',
                'Value': [2, 3],
                'MyOperation': {
                    'ID': 'Multiplication',
                    'Value': [4, 5]
                }
            }
        }";

            // Deserialize JSON
            Maths jsonMaths = JsonConvert.DeserializeObject<Maths>(jsonString.Replace('\'', '\"'));

            double jsonResult = jsonMaths.Operation.Calculate();

            Console.WriteLine("XML result: " + xmlResult);
            Console.WriteLine("JSON result: " + jsonResult);

            // TEST CASES
            //--------------------------------------------------------

            // Case 1 - different operations
            // XML
            string xmlTest1 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <MyMaths>
                        <MyOperation ID=""Multiplication"">
                            <Value>5</Value>
                            <Value>2</Value>
                            <MyOperation ID=""Division"">
                                <Value>6</Value>
                                <Value>4.2</Value>
                            </MyOperation>
                        </MyOperation>
                    </MyMaths>";

            XmlSerializer serializerT1 = new XmlSerializer(typeof(Maths));

            Maths xmlMathsT1;

            using (StringReader readerT1 = new StringReader(xmlTest1))
            {
                xmlMathsT1 = (Maths)serializerT1.Deserialize(readerT1);
            }

            double xmlResultT1 = xmlMathsT1.Operation.Calculate();

            // JSON
            string jsonStringT1 = @"{
            'MyOperation': {
                'ID': 'Multiplication',
                'Value': [5, 2],
                'MyOperation': {
                    'ID': 'Division',
                    'Value': [6, 4.2]
                }
            }
        }";

            Maths jsonMathsT1 = JsonConvert.DeserializeObject<Maths>(jsonStringT1.Replace('\'', '\"'));

            double jsonResultT1 = jsonMathsT1.Operation.Calculate();

            Console.WriteLine("XML result: " + xmlResultT1);
            Console.WriteLine("JSON result: " + jsonResultT1);

            // Case 2 - more layers of operations
            // XML
            string xmlTest2 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <MyMaths>
                        <MyOperation ID=""Addition"">
                            <Value>7.5</Value>
                            <Value>8</Value>
                            <MyOperation ID=""Multiplication"">
                                <Value>3.3</Value>
                                <Value>2.1</Value>
                                <MyOperation ID=""Subtraction"">
                                    <Value>4</Value>
                                    <Value>1</Value>
                                </MyOperation>
                            </MyOperation>
                        </MyOperation>
                    </MyMaths>";

            XmlSerializer serializerT2 = new XmlSerializer(typeof(Maths));

            Maths xmlMathsT2;

            using (StringReader readerT2 = new StringReader(xmlTest2))
            {
                xmlMathsT2 = (Maths)serializerT2.Deserialize(readerT2);
            }

            double xmlResultT2 = xmlMathsT2.Operation.Calculate();

            // JSON
            string jsonStringT2 = @"{
            'MyOperation': {
                'ID': 'Addition',
                'Value': [7.5, 8],
                'MyOperation': {
                    'ID': 'Multiplication',
                    'Value': [3.3, 2.1],
                    'MyOperation': {
                        'ID': 'Subtraction',
                        'Value': [4, 1]
                    }
                }
            }
        }";

            Maths jsonMathsT2 = JsonConvert.DeserializeObject<Maths>(jsonStringT2.Replace('\'', '\"'));

            double jsonResultT2 = jsonMathsT2.Operation.Calculate();

            Console.WriteLine("XML result: " + xmlResultT2);
            Console.WriteLine("JSON result: " + jsonResultT2);

            // Case 3 - more values
            // XML
            string xmlTest3 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <MyMaths>
                        <MyOperation ID=""Addition"">
                            <Value>7.5</Value>
                            <Value>8</Value>
                            <Value>3.6</Value>
                            <MyOperation ID=""Multiplication"">
                                <Value>3.3</Value>
                                <Value>2.1</Value>
                            </MyOperation>
                        </MyOperation>
                    </MyMaths>";

            XmlSerializer serializerT3 = new XmlSerializer(typeof(Maths));

            Maths xmlMathsT3;

            using (StringReader readerT3 = new StringReader(xmlTest3))
            {
                xmlMathsT3 = (Maths)serializerT3.Deserialize(readerT3);
            }

            double xmlResultT3 = xmlMathsT3.Operation.Calculate();

            // JSON
            string jsonStringT3 = @"{
            'MyOperation': {
                'ID': 'Addition',
                'Value': [7.5, 8, 3.6],
                'MyOperation': {
                    'ID': 'Multiplication',
                    'Value': [3.3, 2.1]
                }
            }
        }";

            Maths jsonMathsT3 = JsonConvert.DeserializeObject<Maths>(jsonStringT3.Replace('\'', '\"'));

            double jsonResultT3 = jsonMathsT3.Operation.Calculate();

            Console.WriteLine("XML result: " + xmlResultT3);
            Console.WriteLine("JSON result: " + jsonResultT3);

            Console.ReadLine();
        }
    }
}










