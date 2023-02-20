using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace SpreadsheetTests
{
    /// <summary>
    /// Author:    Landon West
    /// Partner:   None
    /// Date:      16-Feb-2023
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Landon West - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Landon West, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents
    ///
    ///    This tester class tests the functionality of the Spreadsheet file.
    /// </summary>
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// Normalize Delegate
        /// </summary>
        /// <param name="input"> cell name to be normalized </param>
        /// <returns> the normalized version of a given cell name </returns>
        public static string Normalize(string input)
        {
            return input.ToUpper();
        }

        /// <summary>
        /// Validize Delegate
        /// </summary>
        /// <param name="input"> cell name to be validized </param>
        /// <returns> bool of whether the cell name is valid or not </returns>
        // Validize Delegate
        public static bool Validize(string input)
        {
            if (input == "A1")
                return true;
            else
                return false;
        }

        /// <summary>
        /// See Title
        /// </summary>
        [TestMethod]
        public void SavingDoubleTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "5.0");
            sheet.Save("save.txt");
            Assert.AreEqual("default", sheet.GetSavedVersion("save.txt"));
        }

        /// <summary>
        /// See Title
        /// </summary>
        [TestMethod]
        public void SavingFormulaTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=10/2");
            sheet.Save("sheetwithformula.txt");
            Assert.AreEqual("default", sheet.GetSavedVersion("sheetwithformula.txt"));
        }

        /// <summary>
        /// See Title
        /// </summary>
        [TestMethod]
        public void SavingStringTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "yay");
            sheet.Save("sheetwithstring.txt");
            Assert.AreEqual("default", sheet.GetSavedVersion("sheetwithstring.txt"));
        }

        /// <summary>
        /// See Title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void NonsensePathTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.Save("/some/nonsense/path.xml");
        }

        /// <summary>
        /// See Title
        /// </summary>
        [TestMethod]
        public void LoadSpreadsheetTest() 
        {
            using (XmlWriter writer = XmlWriter.Create("save2.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cells");

                // Cell Name: A1    Cell Contents: "hello"
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                // Cell Name: B1    Cell Contents: "5.0"
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B1");
                writer.WriteElementString("contents", "5.0");
                writer.WriteEndElement();

                // Cell Name: C1    Cell Contents: "=5*20"
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "C1");
                writer.WriteElementString("contents", "=5*20");
                writer.WriteEndElement();

                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");

            Assert.AreEqual("hello", ss.GetCellValue("A1"));
            Assert.AreEqual(5.0, ss.GetCellValue("B1"));
            Assert.AreEqual(100.0, ss.GetCellValue("C1"));
        }

        /// <summary>
        /// See Title
        /// </summary>
        [TestMethod]
        public void NormalizedCellNameTests()
        {
            Spreadsheet sheet1 = new Spreadsheet();
            sheet1.SetContentsOfCell("a1", "5.0");
            sheet1.SetContentsOfCell("A1", "10.0");

            Assert.AreEqual(2.0, sheet1.GetNamesOfAllNonemptyCells().Count());

            Spreadsheet sheet2 = new Spreadsheet(s => true, Normalize, "");
            sheet2.SetContentsOfCell("a1", "5.0");
            sheet2.SetContentsOfCell("A1", "10.0");

            Assert.AreEqual(1.0, sheet2.GetNamesOfAllNonemptyCells().Count());
        }

        /// <summary>
        /// See Title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void ValidizedCellNameTest()
        {
            Spreadsheet sheet = new Spreadsheet(Validize, s => s, "");
            sheet.SetContentsOfCell("A1", "valid");

            Assert.AreEqual(1.0, sheet.GetNamesOfAllNonemptyCells().Count());

            sheet.SetContentsOfCell("C1", "invalid");
        }

        /// <summary>
        /// See Title
        /// </summary>
        [TestMethod]
        public void ChangedTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "2.0");

            Assert.IsTrue(sheet.Changed);

            sheet.Save("newsheet.txt");

            Assert.IsFalse(sheet.Changed);
        }

        /// <summary>
        /// See Title
        /// </summary>
        [TestMethod]
        public void SimpleEvaluateTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=5+3");
            Console.WriteLine(sheet.GetCellValue("A1"));
        }

        /// <summary>
        /// See Title
        /// </summary>
        [TestMethod]
        public void MultiCellEvaluateTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "5");
            sheet.SetContentsOfCell("B1", "=A1+2");
            sheet.SetContentsOfCell("C1", "=B1+3");
            Console.WriteLine(sheet.GetCellValue("C1"));
        }

        /// <summary>
        /// See Title
        /// </summary>
        [TestMethod]
        public void ChangingDataInMultiCellEvaluateTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "5");
            sheet.SetContentsOfCell("B1", "=A1+2");
            sheet.SetContentsOfCell("C1", "=B1+3");
            Console.WriteLine(sheet.GetCellValue("C1"));

            sheet.SetContentsOfCell("A1", "10");
            Console.WriteLine(sheet.GetCellValue("C1"));

            sheet.SetContentsOfCell("B1", "7");
            Console.WriteLine(sheet.GetCellValue("C1"));
        }


        // AS4 Grading Tests

        [TestMethod]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestEmptyGetContents()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("1AA");
        }

        [TestMethod]
        [TestCategory("3")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetEmptyContents()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod]
        [TestCategory("5")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetInvalidNameDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1A1A", "1.5");
        }

        [TestMethod]
        [TestCategory("6")]
        public void TestSimpleSetDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.5");
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod]
        [TestCategory("9")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetSimpleString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "hello");
        }

        [TestMethod]
        [TestCategory("10")]
        public void TestSetGetSimpleString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA
        [TestMethod]
        [TestCategory("13")]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetSimpleForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "=2");
        }

        [TestMethod]
        [TestCategory("14")]
        public void TestSetGetForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "=3");
            Formula f = (Formula)s.GetCellContents("Z7");
            Assert.AreEqual(new Formula("3"), f);
            Assert.AreNotEqual(new Formula("2"), f);
        }

        // CIRCULAR FORMULA DETECTION
        [TestMethod]
        [TestCategory("15")]
        [ExpectedException(typeof(CircularException))]
        public void TestSimpleCircular()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2");
            s.SetContentsOfCell("A2", "=A1");
        }

        [TestMethod]
        [TestCategory("16")]
        [ExpectedException(typeof(CircularException))]
        public void TestComplexCircular()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A3", "=A4+A5");
            s.SetContentsOfCell("A5", "=A6+A7");
            s.SetContentsOfCell("A7", "=A1+A1");
        }

        [TestMethod]
        [TestCategory("17")]
        [ExpectedException(typeof(CircularException))]
        public void TestUndoCircular()
        {
            Spreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", "=A3*A1");
            }
            catch (CircularException e)
            {
                throw e;
            }
        }

        [TestMethod]
        [TestCategory("17b")]
        [ExpectedException(typeof(CircularException))]
        public void TestUndoCellsCircular()
        {
            Spreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=A2");
                s.SetContentsOfCell("A2", "=A1");
            }
            catch (CircularException e)
            {
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod]
        [TestCategory("18")]
        public void TestEmptyNames()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod]
        [TestCategory("19")]
        public void TestExplicitEmptySet()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod]
        [TestCategory("20")]
        public void TestSimpleNamesString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod]
        [TestCategory("21")]
        public void TestSimpleNamesDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "52.25");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod]
        [TestCategory("22")]
        public void TestSimpleNamesFormula()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "=3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod]
        [TestCategory("23")]
        public void TestMixedNames()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "hello");
            s.SetContentsOfCell("B1", "=3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod]
        [TestCategory("24")]
        public void TestSetSingletonDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            s.SetContentsOfCell("C1", "=5");
            Assert.IsTrue(s.SetContentsOfCell("A1", "17.2").SequenceEqual(new List<string>() { "A1" }));
        }

        [TestMethod]
        [TestCategory("25")]
        public void TestSetSingletonString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "=5");
            Assert.IsTrue(s.SetContentsOfCell("B1", "hello").SequenceEqual(new List<string>() { "B1" }));
        }

        [TestMethod]
        [TestCategory("26")]
        public void TestSetSingletonFormula()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(s.SetContentsOfCell("C1", "=5").SequenceEqual(new List<string>() { "C1" }));
        }

        [TestMethod]
        [TestCategory("27")]
        public void TestSetChain()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", "=A2+A4");
            s.SetContentsOfCell("A4", "=A2+A5");
            Assert.IsTrue(s.SetContentsOfCell("A5", "82.5").SequenceEqual(new List<string>() { "A5", "A4", "A3", "A1" }));
        }

        // CHANGING CELLS
        [TestMethod]
        [TestCategory("28")]
        public void TestChangeFtoD()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "2.5");
            Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
        }

        [TestMethod]
        [TestCategory("29")]
        public void TestChangeFtoS()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "Hello");
            Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
        }

        [TestMethod]
        [TestCategory("30")]
        public void TestChangeStoF()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "Hello");
            s.SetContentsOfCell("A1", "=23");
            Assert.AreEqual(new Formula("23"), (Formula)s.GetCellContents("A1"));
            Assert.AreNotEqual(new Formula("24"), (Formula)s.GetCellContents("A1"));
        }

        // STRESS TESTS
        [TestMethod]
        [TestCategory("31")]
        public void TestStress1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1+B2");
            s.SetContentsOfCell("B1", "=C1-C2");
            s.SetContentsOfCell("B2", "=C3*C4");
            s.SetContentsOfCell("C1", "=D1*D2");
            s.SetContentsOfCell("C2", "=D3*D4");
            s.SetContentsOfCell("C3", "=D5*D6");
            s.SetContentsOfCell("C4", "=D7*D8");
            s.SetContentsOfCell("D1", "=E1");
            s.SetContentsOfCell("D2", "=E1");
            s.SetContentsOfCell("D3", "=E1");
            s.SetContentsOfCell("D4", "=E1");
            s.SetContentsOfCell("D5", "=E1");
            s.SetContentsOfCell("D6", "=E1");
            s.SetContentsOfCell("D7", "=E1");
            s.SetContentsOfCell("D8", "=E1");
            IList<String> cells = s.SetContentsOfCell("E1", "0");
            Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
        }

        // Repeated for extra weight
        [TestMethod]
        [TestCategory("32")]
        public void TestStress1a()
        {
            TestStress1();
        }
        [TestMethod]
        [TestCategory("33")]
        public void TestStress1b()
        {
            TestStress1();
        }
        [TestMethod]
        [TestCategory("34")]
        public void TestStress1c()
        {
            TestStress1();
        }

        [TestMethod]
        [TestCategory("35")]
        public void TestStress2()
        {
            Spreadsheet s = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++)
            {
                cells.Add("A" + i);
                Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, "=A" + (i + 1))));
            }
        }
        [TestMethod]
        [TestCategory("36")]
        public void TestStress2a()
        {
            TestStress2();
        }
        [TestMethod]
        [TestCategory("37")]
        public void TestStress2b()
        {
            TestStress2();
        }
        [TestMethod]
        [TestCategory("38")]
        public void TestStress2c()
        {
            TestStress2();
        }

        [TestMethod]
        [TestCategory("39")]
        public void TestStress3()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 1; i < 200; i++)
            {
                s.SetContentsOfCell("A" + i, "=A" + (i + 1));
            }
            try
            {
                s.SetContentsOfCell("A150", "=A50");
                Assert.Fail();
            }
            catch (CircularException)
            {
            }
        }

        [TestMethod]
        [TestCategory("40")]
        public void TestStress3a()
        {
            TestStress3();
        }
        [TestMethod]
        [TestCategory("41")]
        public void TestStress3b()
        {
            TestStress3();
        }
        [TestMethod]
        [TestCategory("42")]
        public void TestStress3c()
        {
            TestStress3();
        }

        [TestMethod]
        [TestCategory("43")]
        public void TestStress4()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++)
            {
                s.SetContentsOfCell("A1" + i, "=A1" + (i + 1));
            }
            LinkedList<string> firstCells = new LinkedList<string>();
            LinkedList<string> lastCells = new LinkedList<string>();
            for (int i = 0; i < 250; i++)
            {
                firstCells.AddFirst("A1" + i);
                lastCells.AddFirst("A1" + (i + 250));
            }
            Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SequenceEqual(firstCells));
            Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SequenceEqual(lastCells));
        }
        [TestMethod]
        [TestCategory("44")]
        public void TestStress4a()
        {
            TestStress4();
        }
        [TestMethod]
        [TestCategory("45")]
        public void TestStress4b()
        {
            TestStress4();
        }
        [TestMethod]
        [TestCategory("46")]
        public void TestStress4c()
        {
            TestStress4();
        }

        [TestMethod]
        [TestCategory("47")]
        public void TestStress5()
        {
            RunRandomizedTest(47, 2519);
        }

        [TestMethod]
        [TestCategory("48")]
        public void TestStress6()
        {
            RunRandomizedTest(48, 2521);
        }

        [TestMethod]
        [TestCategory("49")]
        public void TestStress7()
        {
            RunRandomizedTest(49, 2526);
        }

        [TestMethod]
        [TestCategory("50")]
        public void TestStress8()
        {
            RunRandomizedTest(50, 2521);
        }

        /// <summary>
        /// Sets random contents for a random cell 10000 times
        /// </summary>
        /// <param name="seed">Random seed</param>
        /// <param name="size">The known resulting spreadsheet size, given the seed</param>
        public void RunRandomizedTest(int seed, int size)
        {
            Spreadsheet s = new Spreadsheet();
            Random rand = new Random(seed);
            for (int i = 0; i < 10000; i++)
            {
                try
                {
                    switch (rand.Next(3))
                    {
                        case 0:
                            s.SetContentsOfCell(randomName(rand), "3.14");
                            break;
                        case 1:
                            s.SetContentsOfCell(randomName(rand), "hello");
                            break;
                        case 2:
                            s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                            break;
                    }
                }
                catch (CircularException)
                {
                }
            }
            ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(size, set.Count);
        }

        /// <summary>
        /// Generates a random cell name with a capital letter and number between 1 - 99
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        private String randomName(Random rand)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
        }

        /// <summary>
        /// Generates a random Formula
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        private String randomFormula(Random rand)
        {
            String f = randomName(rand);
            for (int i = 0; i < 10; i++)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        f += "+";
                        break;
                    case 1:
                        f += "-";
                        break;
                    case 2:
                        f += "*";
                        break;
                    case 3:
                        f += "/";
                        break;
                }
                switch (rand.Next(2))
                {
                    case 0:
                        f += 7.2;
                        break;
                    case 1:
                        f += randomName(rand);
                        break;
                }
            }
            return f;
        }

    // My AS4 Tests

    /// <summary>
    /// See title
    /// </summary>
    [TestMethod]
        public void SpreadsheetSizeTest()
        {
            // spreadsheet data
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "1.0");
            sheet.SetContentsOfCell("B1", "2.0");
            sheet.SetContentsOfCell("C1", "3.0");

            Assert.AreEqual(3.0, sheet.GetNamesOfAllNonemptyCells().ToList().Count);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod] 
        public void GetNamesOfAllNonemptyCellsTest() 
        { 
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "1.0");
            sheet.SetContentsOfCell("B1", "2.0");
            sheet.SetContentsOfCell("C1", "3.0");

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
            sheet.SetContentsOfCell("A1", "1.0");
            sheet.SetContentsOfCell("B1", "=2.0");
            sheet.SetContentsOfCell("C1", "word");
            sheet.SetContentsOfCell("D1", "=A1*2");
            sheet.SetContentsOfCell("E1", "=D1-5");

            List<string> expected = new List<string>() { "A1", "D1", "E1" };

            Assert.IsTrue(expected.SequenceEqual(sheet.SetContentsOfCell("A1", "5.0")));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetCellContentsNumberTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "1.0");

            Assert.AreEqual(1.0, sheet.GetCellContents("A1"));

            sheet.SetContentsOfCell("A1", "2.0");

            Assert.AreEqual(2.0, sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetCellContentsStringTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "words");

            Assert.AreEqual("words", sheet.GetCellContents("A1"));

            sheet.SetContentsOfCell("A1", "new words");

            Assert.AreEqual("new words", sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetCellContentsFormulaTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=2+1");

            Assert.AreEqual(new Formula("2+1"), sheet.GetCellContents("A1"));

            sheet.SetContentsOfCell("A1", "=8/4");

            Assert.AreEqual(new Formula("8/4"), sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void SettingNewCellContent()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "1.0");
            sheet.SetContentsOfCell("B1", "=2.0");
            sheet.SetContentsOfCell("C1", "word");

            sheet.SetContentsOfCell("A1", "5.0");
            sheet.SetContentsOfCell("B1", "=10.0");
            sheet.SetContentsOfCell("C1", "new word");

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
            sheet.SetContentsOfCell("A1", "1.0");
            sheet.SetContentsOfCell("B1", "=A1+3");

            Assert.IsTrue(sheet.SetContentsOfCell("A1", "2.0").SequenceEqual(new List<string>() { "A1", "B1" }));

            sheet.SetContentsOfCell("B1", "3.0");

            Assert.IsTrue(sheet.SetContentsOfCell("A1", "3.0").SequenceEqual(new List<string>() { "A1" }));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CircularExceptionTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "1.0");
            sheet.SetContentsOfCell("B1", "=A1");
            sheet.SetContentsOfCell("A1", "=B1");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void DependenciesTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "1.0");
            sheet.SetContentsOfCell("B1", "=A1+3");
            sheet.SetContentsOfCell("C1", "=B1-3");
            sheet.SetContentsOfCell("D1", "=A1-1");

            List<string> list = sheet.SetContentsOfCell("A1", "5.0").ToList();

            list.ForEach(Console.Write);

            Assert.IsTrue(list.OrderBy(x => x).SequenceEqual(new List<string>() { "A1", "B1", "C1", "D1" }.OrderBy(x => x)));

            List<string> list2 = sheet.SetContentsOfCell("B1", "3.0").ToList();

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
            sheet.SetContentsOfCell("", "1.0");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidCellNameTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("$a", "1.0");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameGetCellContentsTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "1.0");
            sheet.GetCellContents("B1");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void SetEmptyCellTest()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "3.0");
            sheet.SetContentsOfCell("B1", "5.0");
            sheet.SetContentsOfCell("C1", "7.0");
            sheet.SetContentsOfCell("A1", "");

            Assert.AreEqual(2.0, sheet.GetNamesOfAllNonemptyCells().ToList().Count);
        }
    }
}