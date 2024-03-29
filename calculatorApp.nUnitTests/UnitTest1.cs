using Newtonsoft.Json;
using System.Xml.Serialization;
using NUnit.Framework.Constraints;
using calculatorApp.Core;
using calculatorApp.OperationClass;
using calculatorApp.Enum;

namespace calculatorApp.nUnitTests
{ 
    public class MathsOperationsTests
    {
        [Test]
        public void TestAdditionOperation()
        {
            // Arrange
            Operation operation = new Operation
            {
                ID = Operator.Addition,
                Value = new List<double> { 2, 3 }
            };

            // Act
            double result = operation.Calculate();

            // Assert
            Assert.AreEqual(5, result);
        }

        [Test]
        public void TestMultiplicationWithNestedAddition()
        {
            // Arrange
            Operation nestedOperation = new Operation
            {
                ID = Operator.Addition,
                Value = new List<double> { 2, 3 } // This should result in 5
            };

            Operation operation = new Operation
            {
                ID = Operator.Multiplication,
                Value = new List<double> { 4 },
                NestedOperation = nestedOperation
            };

            // Act
            double result = operation.Calculate();

            // Assert
            Assert.AreEqual(20, result); // 4 * (2 + 3) = 20
        }

        [Test]
        public void TestDivisionByZeroThrowsException()
        {
            // Arrange
            var operation = new Operation
            {
                ID = Operator.Division,
                Value = new List<double> { 10, 0 } // Attempting to divide by zero
            };

            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => operation.Calculate());
        }


    }
}
