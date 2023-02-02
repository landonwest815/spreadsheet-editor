using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Reflection.Metadata.Ecma335;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        Dictionary<String, double> variables;

        // Normalize function
        public string VariableNormalizer(string variable)
        {
            return variable.ToUpper();
        }

        // isValid function
        public bool VariableValidizer(string variable)
        {
            return (variables.ContainsKey(variable));
        }

        public double VariableLookup(string variable)
        {
            return variables[variable];
        }

        // Lookup function

        [TestMethod]
        public void TestMethod1()
        {

            variables = new Dictionary<string, double>
            {
                { "A1", 5.0 }
            };
            Formula f1 = new Formula("1+2+A1");
            Assert.AreEqual(8.0, f1.Evaluate(VariableLookup));

        }

        [TestMethod]
        public void TestMethod2()
        {

            variables = new Dictionary<string, double>
            {
                { "a1", 1.0 },
                { "A1", 2.0 }
            };
            Formula f2 = new Formula("a1 + A1");
            Assert.AreEqual(3.0, f2.Evaluate(VariableLookup));

        }

        [TestMethod]
        public void TestMethod3()
        {
            variables = new Dictionary<string, double>
            {
                { "A1", 1.0 },
                { "a1", 2.0 }
            };
            Formula f3 = new Formula("a1 + A1", VariableNormalizer, VariableValidizer);
            Assert.AreEqual(2.0, f3.Evaluate(VariableLookup));
        }

        [TestMethod]
        public void TestMethod4()
        {
            variables = new Dictionary<string, double>();
            variables.Add("A1", 1.0);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Formula f1 = new Formula("1.00 + 2.00");
            Formula f2 = new Formula("1.0+2.0");

            Assert.IsTrue(f1.Equals(f2));
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }
    }
}