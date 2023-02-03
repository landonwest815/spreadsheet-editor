using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Data;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        // Dictionary for the variables in various tests
        Dictionary<String, double> variables;

        // Normalizer
        public string VariableNormalizer(string variable)
        {
            return variable.ToUpper();
        }

        // Validizer
        public bool VariableValidizer(string variable)
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            return alphabet.Contains(variable);
        }

        // Lookup Delegate
        public double VariableLookup(string variable)
        {
            if (variables.ContainsKey(variable))
            {
                return variables[variable];
            }
            else
            {
                throw new ArgumentException();
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ValidizerTest()
        {
            Formula f = new Formula("1+A2", s => s, VariableValidizer);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidTokenTest() 
        {
            Formula f = new Formula("1&4");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidTokenAfterOpeningParanthesisTest() 
        {
            Formula f = new Formula("(+5)");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void UnbalancedParanthesisTest()
        {
            Formula f = new Formula("(1+2");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EmptyFormulaTest()
        {
            Formula f = new Formula("");
        }

        [TestMethod()]
        public void VariableLookupTest()
        {
            variables = new Dictionary<string, double>() { { "A1", 1.0 }, { "B1", 2.0 }, { "C1", 3.0 } };
            Formula f = new Formula("1 + A1 - B1 + C1 - 2");
            Assert.AreEqual(1.0, f.Evaluate(VariableLookup));
        }

        [TestMethod()]
        public void InconclusiveVariableLookupTest()
        {
            variables = new Dictionary<string, double>() { { "A1", 1.0 } };
            Formula f = new Formula("1+A2", s => s, s => true);
            Assert.IsInstanceOfType(f.Evaluate(VariableLookup), typeof(FormulaError));
        }

        [TestMethod()]
        public void SubtractionTest()
        {
            Formula f = new Formula("20-(5-3)-17");
            Assert.AreEqual(1.0, f.Evaluate(s => 0.0));
        }

        [TestMethod()]
        public void DivisionTest()
        {
            Formula f = new Formula("20/(6/3)");
            Assert.AreEqual(10.0, f.Evaluate(s => 0.0));
        }

        [TestMethod()]
        public void DivisionByZeroTest()
        {
            Formula f = new Formula("20/0*(6/3)");
            Assert.IsInstanceOfType(f.Evaluate(s => 0.0), typeof(FormulaError));
        }

        [TestMethod()]
        public void ComplexDoublesTest()
        {
            Formula f = new Formula("2.71+3.4/2");
            Assert.AreEqual(4.41, f.Evaluate(s => 0.0));
        }

        [TestMethod()]
        public void ScientificNotationTest()
        {
            Formula f1 = new Formula("3.564e-5");
            Formula f2 = new Formula("0.00003564");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod()]
        public void ScientificNotationAdditionTest()
        {
            Formula f1 = new Formula("3.564e-5 + 7");
            Formula f2 = new Formula("7 + 0.00003564");
            Assert.AreEqual(f1.Evaluate(s => 0.0), f2.Evaluate(s => 0.0));
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Formula f1 = new Formula("1 + 3");
            Formula f2 = new Formula("1+3");
            Assert.AreEqual(f1.ToString(), f2.ToString());
        }

        [TestMethod] 
        public void ToStringNormalizedTest() 
        {
            Formula f1 = new Formula("a1 + a2", VariableNormalizer, s => true);
            Formula f2 = new Formula("A1  +  A2");
            Assert.AreEqual(f1.ToString(), f2.ToString());
        }

        [TestMethod]
        public void ToStringNotNormalizedTest()
        {
            Formula f1 = new Formula("a1 + a2");
            Formula f2 = new Formula("A1  +  A2");
            Assert.AreNotEqual(f1.ToString(), f2.ToString());
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Formula f1 = new Formula("a1 + a2", VariableNormalizer, s => true);
            Formula f2 = new Formula("A1  +  A2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void NotEqualsTest()
        {
            Formula f1 = new Formula("a1 + a2");
            Formula f2 = new Formula("A1  +  A2");
            Assert.IsFalse(f1.Equals(f2));
        }

        [TestMethod()]
        public void NonFormulaEqualsTest()
        {
            string s = "1+2";
            Formula f = new Formula(s);
            Assert.IsFalse(f.Equals(s));
        }

        [TestMethod()]
        public void OperatorEqualsTest()
        {
            Formula f1 = new Formula("a1 + a2", VariableNormalizer, s => true);
            Formula f2 = new Formula("A1  +  A2");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod()]
        public void OperatorNotEqualsTest()
        {
            Formula f1 = new Formula("a1 + a2");
            Formula f2 = new Formula("A1  +  A2");
            Assert.IsTrue(f1 != f2);
        }

        [TestMethod()]
        public void EqualHashCodesTest()
        {
            Formula f1 = new Formula("a1 + a2", VariableNormalizer, s => true);
            Formula f2 = new Formula("A1  +  A2");
            Console.WriteLine(f1.GetHashCode());
            Console.WriteLine(f2.GetHashCode());
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
        }

        [TestMethod()]
        public void NotEqualHashCodesTest()
        {
            Formula f1 = new Formula("a1 + a2");
            Formula f2 = new Formula("A1  +  A2");
            Assert.AreNotEqual(f1.GetHashCode(), f2.GetHashCode());
        }

        // PS1 Grading Tests

        [TestMethod(), Timeout(5000)]
        public void TestSingleNumber()
        {
            Formula f = new Formula("5");
            Assert.AreEqual(5.0, f.Evaluate(VariableLookup));
        }

        [TestMethod(), Timeout(5000)]
        public void TestSingleVariable()
        {
            Formula f = new Formula("X5");
            Assert.AreEqual(13.0, f.Evaluate(s => 13.0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestAddition()
        {
            Formula f = new Formula("5+3");
            Assert.AreEqual(8.0, f.Evaluate(VariableLookup));
        }

        [TestMethod(), Timeout(5000)]
        public void TestSubtraction()
        {
            Formula f = new Formula("18-10");
            Assert.AreEqual(8.0, f.Evaluate(VariableLookup));
        }

        [TestMethod(), Timeout(5000)]
        public void TestMultiplication()
        {
            Formula f = new Formula("2*4");
            Assert.AreEqual(8.0, f.Evaluate(VariableLookup));
        }

        [TestMethod(), Timeout(5000)]
        public void TestDivision()
        {
            Formula f = new Formula("16/2");
            Assert.AreEqual(8.0, f.Evaluate(VariableLookup));
        }

        [TestMethod(), Timeout(5000)]
        public void TestArithmeticWithVariable()
        {
            Formula f = new Formula("2+X1");
            Assert.AreEqual(6.0, f.Evaluate(s => 4));
        }

        [TestMethod(), Timeout(5000)]
        public void TestLeftToRight()
        {
            Formula f = new Formula("2*6+3");
            Assert.AreEqual(15.0, f.Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestOrderOperations()
        {
            Formula f = new Formula("2+6*3");
            Assert.AreEqual(20.0, f.Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestParenthesesTimes()
        {
            Formula f = new Formula("(2+6)*3");
            Assert.AreEqual(24.0, f.Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestTimesParentheses()
        {
            Formula f = new Formula("2*(3+5)");
            Assert.AreEqual(16.0, f.Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestPlusParentheses()
        {
            Formula f = new Formula("2+(3+5)");
            Assert.AreEqual(10.0, f.Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestPlusComplex()
        {
            Formula f = new Formula("2+(3+5*9)");
            Assert.AreEqual(50.0, f.Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestOperatorAfterParens()
        {
            Formula f = new Formula("(1*1)-2/2");
            Assert.AreEqual(0.0, f.Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexTimesParentheses()
        {
            Formula f = new Formula("2+3*(3+5)");
            Assert.AreEqual(26.0, f.Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexAndParentheses()
        {
            Formula f = new Formula("2+3*5+(3+4*8)*5+2");
            Assert.AreEqual(194.0, f.Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestDivideByZero()
        {
            Formula f = new Formula("5/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0.0), typeof(FormulaError));
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            Formula f = new Formula("+");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperator()
        {
            Formula f = new Formula("2+5+");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraParentheses()
        {
            Formula f = new Formula("2+5*7)");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestParensNoOperator()
        {
            Formula f = new Formula("5+7+(5)8");
        }


        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            Formula f = new Formula("");
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexNestedParensRight()
        {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6.0, f.Evaluate(s => 1.0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexNestedParensLeft()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12.0, f.Evaluate(s => 2.0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestRepeatedVar()
        {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0.0, f.Evaluate(s => 3.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("30")]
        public void TestClearStacks()
        {
            //Test if code doesn't clear stacks between evaluations
            Formula f = new Formula("2*6+3");
            Assert.AreEqual(15.0, f.Evaluate(s => 0.0));
            Assert.AreEqual(15.0, f.Evaluate(s => 0.0));
        }
    }
}