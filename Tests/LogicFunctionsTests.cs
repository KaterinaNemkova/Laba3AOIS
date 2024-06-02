using Microsoft.VisualStudio.TestTools.UnitTesting;
using laba2AOIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace laba2AOIS.Tests
{
    [TestClass()]
    public class LogicFunctionsTests
    {
        [TestMethod]
        public void OperationAnd_ShouldReturnTrue_WhenBothOperandsAreTrue()
        {
            bool result = LogicFunctions.OperationAnd(true, true);
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void OperationAnd_ShouldReturnFalse_WhenOneOperandIsFalse()
        {
            bool result = LogicFunctions.OperationAnd(true, false);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void OperationOr_ShouldReturnTrue_WhenOneOperandIsTrue()
        {
            bool result = LogicFunctions.OperationOr(true, false);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void OperationOr_ShouldReturnFalse_WhenBothOperandsAreFalse()
        {
            bool result = LogicFunctions.OperationOr(false, false);
            Assert.IsFalse(result);
        }

        

        [TestMethod]
        public void OperationNot_ShouldReturnTrue_WhenOperandIsFalse()
        {
            bool result = LogicFunctions.OperationNot(false);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void OperationNot_ShouldReturnFalse_WhenOperandIsTrue()
        {
            bool result = LogicFunctions.OperationNot(true);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void OperationImplication_ShouldReturnTrue_WhenFirstOperandIsFalse()
        {
            bool result = LogicFunctions.OperationImplication(false, true);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void OperationImplication_ShouldReturnFalse_WhenFirstOperandIsTrueAndSecondIsFalse()
        {
            bool result = LogicFunctions.OperationImplication(true, false);
            Assert.IsFalse(result);
        }

       
        [TestMethod]
        public void OperationEquivalence_ShouldReturnTrue_WhenBothOperandsAreEqual()
        {
            bool result = LogicFunctions.OperationEquivalence(true, true);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void OperationEquivalence_ShouldReturnFalse_WhenOperandsAreDifferent()
        {
            bool result = LogicFunctions.OperationEquivalence(true, false);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void OPZ_ShouldReturnCorrectPostfixExpression()
        {
            string expression = "a&b";
            string result = LogicFunctions.OPZ(expression);
            Assert.AreEqual("ab&", result);
        }
        [TestMethod]
        public void TestOperationOr()
        {
            bool operand1 = true;
            bool operand2 = false;
            bool result = LogicFunctions.OperationOr(operand1, operand2);
            Assert.IsTrue(result);

            operand1 = false;
            operand2 = false;
            result = LogicFunctions.OperationOr(operand1, operand2);
            Assert.IsFalse(result);

            operand1 = true;
            operand2 = true;
            result = LogicFunctions.OperationOr(operand1, operand2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestOperationEquivalence()
        {
            bool operand1 = true;
            bool operand2 = true;
            bool result = LogicFunctions.OperationEquivalence(operand1, operand2);
            Assert.IsTrue(result);

            operand1 = true;
            operand2 = false;
            result = LogicFunctions.OperationEquivalence(operand1, operand2);
            Assert.IsFalse(result);

            operand1 = false;
            operand2 = true;
            result = LogicFunctions.OperationEquivalence(operand1, operand2);
            Assert.IsFalse(result);

            operand1 = false;
            operand2 = false;
            result = LogicFunctions.OperationEquivalence(operand1, operand2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestOperationImplication()
        {
            bool operand1 = true;
            bool operand2 = true;
            bool result = LogicFunctions.OperationImplication(operand1, operand2);
            Assert.IsTrue(result);

            operand1 = true;
            operand2 = false;
            result = LogicFunctions.OperationImplication(operand1, operand2);
            Assert.IsFalse(result);

            operand1 = false;
            operand2 = true;
            result = LogicFunctions.OperationImplication(operand1, operand2);
            Assert.IsTrue(result);

            operand1 = false;
            operand2 = false;
            result = LogicFunctions.OperationImplication(operand1, operand2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ConvertBinaryToDecimal_ShouldReturnCorrectDecimalValue()
        {
            var binaryResult = new List<int> { 1, 0, 1 };
            int result = LogicFunctions.ConvertBinaryToDecimal(binaryResult);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void PopulateResultsAndIndices_ShouldPopulateCorrectly()
        {
            int n = 2;
            string postfixExpression = "ab&";
            var truthTable = LogicFunctions.GenerateTruthTable(n, "a&b", postfixExpression);
            string SKNF = "";
            string SDNF = "";
            var sknfIndices = new List<int>();
            var sdnfIndices = new List<int>();
            var decimalResult = new List<int>();

            LogicFunctions.PopulateResultsAndIndices(truthTable, postfixExpression, n, ref SKNF, ref SDNF, sknfIndices, sdnfIndices, decimalResult);

            Assert.AreEqual("(a | b) & (a | !b) & (!a | b) & ", SKNF);
            Assert.AreEqual("(a & b) | ", SDNF);
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2 }, sknfIndices);
            CollectionAssert.AreEqual(new List<int> { 3 }, sdnfIndices);
            CollectionAssert.AreEqual(new List<int> { 0, 0, 0, 1 }, decimalResult);
        }

        [TestMethod]
        public void GetClauses_ShouldReturnCorrectClauses()
        {
            string sknf = "(a | !b) & (c | d)";
            var result = LogicFunctions.GetClauses(sknf);
            CollectionAssert.AreEqual(new List<string> { "(a | !b)", "(c | d)" }, result);
        }
        [TestMethod]
        public void SplitImplicants_ShouldReturnCorrectLiterals()
        {
            string input = "(a & !b)";
            var result = LogicFunctions.SplitImplicants(input);
            CollectionAssert.AreEqual(new List<string> { "a", "!b" }, result);
        }
        [TestMethod]
        public void IsSubstringInOriginal_ShouldReturnTrue_WhenSubConstituentIsInOriginal()
        {
            string subConstituent = "(a & !b)";
            string originalConstituent = "(a & !b & c)";
            bool result = LogicFunctions.IsSubstringInOriginal(subConstituent, originalConstituent);
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void MinimizeSKNF_Table_ShouldReturnMinimizedExpression()
        {
            string sknf = "(a | !b) & (c | d)";
            string result = LogicFunctions.MinimizeSKNF_Table(sknf);
            Assert.AreEqual("((a | !b)) & ((c | d))", result);
        }
        [TestMethod]
        public void MinimizeSDNF_Table_ShouldReturnMinimizedExpression()
        {
            string sdnf = "(a & !b) | (c & d)";
            string result = LogicFunctions.MinimizeSDNF_Table(sdnf);
            Assert.AreEqual("((a & !b)) | ((c & d))", result);
        }
        [TestMethod]
        public void ConstructSKNF_ShouldReturnCorrectExpression()
        {
            var values = new List<bool> { true, false };
            string result = LogicFunctions.ConstructSKNF(values, 2);
            Assert.AreEqual("(!a | b) & ", result);
        }

        [TestMethod]
        public void ConstructSDNF_ShouldReturnCorrectExpression()
        {
            var values = new List<bool> { true, false };
            string result = LogicFunctions.ConstructSDNF(values, 2);
            Assert.AreEqual("(a & !b) | ", result);
        }
        [TestMethod]
        public void GenerateTruthTable_ShouldReturnCorrectTable()
        {
            int n = 2;
            var result = LogicFunctions.GenerateTruthTable(n, "a&b", "ab&");
            var expected = new List<List<bool>>
            {
            new List<bool> { false, false },
            new List<bool> { false, true },
            new List<bool> { true, false },
            new List<bool> { true, true }
            };

            for (int i = 0; i < expected.Count; i++)
            {
                CollectionAssert.AreEqual(expected[i], result[i]);
            }
        }

        [TestMethod]
        public void MinimizeSDNF_Table_Merging_Test()
        {
            // Arrange
            string sdnf = "(a & b) | (a & !c) | (!b & c)";
            string expected = "((a & b)) | ((a & !c)) | ((!b & c))";

            // Act
            string result = LogicFunctions.MinimizeSDNF_Table(sdnf);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestExtractVariablesSKNF()
        {
            string sknf = "(!a | b) & (a | !b)";
            var variables = LogicFunctions.ExtractVariablesSKNF(sknf);
            CollectionAssert.AreEquivalent(new List<string> { "a", "b" }, variables);
        }

        [TestMethod]
        public void TestExtractVariablesSDNF()
        {
            string sdnf = "(a & !b) | (!a & b)";
            var variables = LogicFunctions.ExtractVariablesSDNF(sdnf);
            CollectionAssert.AreEquivalent(new List<string> { "a", "b" }, variables);
        }

        [TestMethod]
        public void TestToGray()
        {
            Assert.AreEqual(0, LogicFunctions.ToGray(0));
            Assert.AreEqual(1, LogicFunctions.ToGray(1));
            Assert.AreEqual(3, LogicFunctions.ToGray(2));
            Assert.AreEqual(2, LogicFunctions.ToGray(3));
        }


        [TestMethod]
        public void TestMinimizeSKNFbyKarnaugh()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string sknf = "(!a | !b | !c) & (!a | b | c) & (a | !b | c)";
                List<int> index = new List<int> { 1, 0, 1, 1, 0, 0, 1, 1 }; 
                LogicFunctions.MinimizeSKNFbyKarnaugh(sknf, index);

                string expectedOutput =
                    "Karnaugh Map:\r\n" +
                    "1   0   1   1   \r\n\r\n" +
                    "0   0   1   1   \r\n\r\n";

                Assert.AreEqual(expectedOutput, sw.ToString());
            }
        }

        [TestMethod]
        public void TestMinimizeSDNFbyKarnaugh()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                string sdnf = "(a & b & c) | (a & !b & !c) | (!a & b & c)";
                List<int> index = new List<int> { 1, 0, 1, 1, 0, 0, 1, 1 };
                LogicFunctions.MinimizeSDNFbyKarnaugh(sdnf, index);

                string expectedOutput =
                    "Karnaugh Map for SDNF:\r\n" +
                    "1   0   1   1   \r\n\r\n" +
                    "0   0   1   1   \r\n\r\n";

                Assert.AreEqual(expectedOutput, sw.ToString());
            }
        }
        
    }

}