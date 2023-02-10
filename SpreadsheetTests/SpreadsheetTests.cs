using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void SpreadsheetSizeTest()
        {
            // spreadsheet data
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1.0);
            sheet.SetCellContents("B1", 2.0);
            sheet.SetCellContents("C1", 3.0);

            Assert.AreEqual(3.0, sheet.GetNamesOfAllNonemptyCells().ToList().Count);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod] 
        public void GetNamesOfAllNonemptyCellsTest() 
        { 
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1.0);
            sheet.SetCellContents("B1", 2.0);
            sheet.SetCellContents("C1", 3.0);

            List<string> nonemptyCells = new List<string>() { "A1", "B1", "C1" };

            Assert.IsTrue(nonemptyCells.SequenceEqual(sheet.GetNamesOfAllNonemptyCells()));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void SetCellContentsTest()
        {
            // spreadsheet data
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1.0);
            sheet.SetCellContents("B1", new Formula("2.0"));
            sheet.SetCellContents("C1", "word");
            sheet.SetCellContents("D1", new Formula("A1*2"));
            sheet.SetCellContents("E1", new Formula("D1-5"));

            List<string> expected = new List<string>() { "A1", "D1", "E1" };

            Assert.IsTrue(expected.SequenceEqual(sheet.SetCellContents("A1", 5.0)));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetCellContentsNumberTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1.0);

            Assert.AreEqual(1.0, sheet.GetCellContents("A1"));

            sheet.SetCellContents("A1", 2.0);

            Assert.AreEqual(2.0, sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetCellContentsStringTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", "words");

            Assert.AreEqual("words", sheet.GetCellContents("A1"));

            sheet.SetCellContents("A1", "new words");

            Assert.AreEqual("new words", sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetCellContentsFormulaTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", new Formula("2+1"));

            Assert.AreEqual(new Formula("2+1"), sheet.GetCellContents("A1"));

            sheet.SetCellContents("A1", new Formula("8/4"));

            Assert.AreEqual(new Formula("8/4"), sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// See title
        /// </summary>
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

        /// <summary>
        /// See title
        /// </summary>
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

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CircularExceptionTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1.0);
            sheet.SetCellContents("B1", new Formula("A1"));
            sheet.SetCellContents("A1", new Formula("B1"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void DependenciesTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1.0);
            sheet.SetCellContents("B1", new Formula("A1+3"));
            sheet.SetCellContents("C1", new Formula("B1-3"));
            sheet.SetCellContents("D1", new Formula("A1-1"));

            List<string> list = sheet.SetCellContents("A1", 5.0).ToList();

            list.ForEach(Console.Write);

            Assert.IsTrue(list.OrderBy(x => x).SequenceEqual(new List<string>() { "A1", "B1", "C1", "D1" }.OrderBy(x => x)));

            List<string> list2 = sheet.SetCellContents("B1", 3.0).ToList();

            Assert.IsTrue(list2.SequenceEqual(new List<string>() { "B1", "C1" }));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void EmptyCellNameTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("", 1.0);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidCellNameTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("$a", 1.0);
        }

        /// <summary>
        /// See title
        /// </summary>
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