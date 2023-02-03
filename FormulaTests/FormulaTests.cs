using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Data;
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
            return (variables.ContainsKey(variable));
        }

        // Lookup Delegate
        public double VariableLookup(string variable)
        {
            return variables[variable];
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
        [TestCategory("12")]
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