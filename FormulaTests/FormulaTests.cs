using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        Dictionary<String, double> vDict;

        // Normalize function
        public string VariableNormalize(string variable)
        {
            variable = variable.ToUpper();
            Console.WriteLine(variable);
            return variable;
        }

        // isValid function
        public bool VariableValidizer(string variable)
        {
            if (vDict.ContainsKey(variable))
            {
                return true;
            }
            return false;
        }

        public double VariableLookup(string variable)
        {
            return vDict[variable];
        }

        // Lookup function

        [TestMethod]
        public void TestMethod1()
        {

            vDict = new Dictionary<string, double>
            {
                { "A1", 5.0 }
            };
            Formula f1 = new Formula("1+2+A1");
            Assert.AreEqual(8.0, f1.Evaluate(VariableLookup));

        }

        [TestMethod]
        public void TestMethod2()
        {

            vDict = new Dictionary<string, double>
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
            vDict = new Dictionary<string, double>
            {
                { "A1", 1.0 },
                { "a1", 2.0 }
            };
            Formula f3 = new Formula("a1 + A1", VariableNormalize, VariableValidizer);
            Assert.AreEqual(2.0, f3.Evaluate(VariableLookup));
        }

        [TestMethod]
        public void TestMethod4()
        {
            vDict = new Dictionary<string, double>();
            vDict.Add("A1", 1.0);
        }
    }
}