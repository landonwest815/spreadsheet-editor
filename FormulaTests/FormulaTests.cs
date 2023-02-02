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
            variable.ToUpper();
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

            vDict = new Dictionary<string, double>();
            vDict.Add("A1", 5.0);
            Formula f1 = new Formula("1+2+A1");
            Assert.AreEqual(8.0, f1.Evaluate(VariableLookup));

        }

        [TestMethod]
        public void TestMethod2()
        {

            Formula f2 = new Formula("((1+3))");
            Assert.AreEqual(4.0, f2.Evaluate(VariableLookup));

        }
    }
}