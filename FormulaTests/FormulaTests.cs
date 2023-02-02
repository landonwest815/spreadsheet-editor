using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {

        // Normalize function



        // isValid function



        // Lookup function

        [TestMethod]
        public void TestMethod1()
        {

            Formula f1 = new Formula("1+2+A2");

        }

        [TestMethod]
        public void TestMethod2()
        {

            Formula f2 = new Formula("((1+3)");

        }
    }
}