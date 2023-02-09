using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        public void SpreadsheetSize()
        {
            // spreadsheet data
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1.0);
            sheet.SetCellContents("B1", 2.0);
            sheet.SetCellContents("C1", 3.0);

            Assert.AreEqual(3.0, sheet.GetNamesOfAllNonemptyCells().ToList().Count);
        }

        [TestMethod]
        public void SetCellContentsTest()
        {
            // spreadsheet data
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1.0);
            sheet.SetCellContents("B1", new Formula("2.0"));
            sheet.SetCellContents("C1", "word");
            sheet.SetCellContents("D1", new Formula("A1*2"));

            List<string> expected = new List<string>() { "A1", "D1" };

            Assert.IsTrue(expected.SequenceEqual(sheet.SetCellContents("A1", 5.0)));
        }

        [TestMethod]
        public void GetCellContentsTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1.0);

            Assert.AreEqual(1.0, sheet.GetCellContents("A1"));

            sheet.SetCellContents("A1", 2.0);

            Assert.AreEqual(2.0, sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void SettingNewCellContent()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1.0);
            sheet.SetCellContents("B1", new Formula("2.0"));
            sheet.SetCellContents("C1", "word");

            sheet.SetCellContents("A1", 5.0);
            sheet.SetCellContents("B1", new Formula("10.0"));
            sheet.SetCellContents("C1", "new word");

            Assert.AreEqual(5.0, sheet.GetCellContents("A1"));
            Assert.AreEqual(new Formula("10.0"), sheet.GetCellContents("B1"));
            Assert.AreEqual("new word", sheet.GetCellContents("C1"));
        }

        [TestMethod]
        public void RemovingDependenciesTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1.0);
            sheet.SetCellContents("B1", new Formula("A1+3"));

            Assert.IsTrue(sheet.SetCellContents("A1", 2.0).SequenceEqual(new List<string>() { "A1", "B1" }));

            sheet.SetCellContents("B1", 3.0);

            Assert.IsTrue(sheet.SetCellContents("A1", 3.0).SequenceEqual(new List<string>() { "A1" }));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullNameTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("", 1.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTextTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", "");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("$a", 1.0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameGetCellContentsTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1.0);
            sheet.GetCellContents("B1");
        }
    }
}