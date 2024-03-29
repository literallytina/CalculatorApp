using System;
using calculatorApp.Enum;
using calculatorApp.Interface;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace calculatorApp.OperationClass
{
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
}

