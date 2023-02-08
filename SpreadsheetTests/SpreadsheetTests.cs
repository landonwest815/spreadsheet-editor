using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 3.0);
            sheet.SetCellContents("A2", 4.0);
            sheet.SetCellContents("A3", new Formula("A1*A2"));
            sheet.SetCellContents("A4", 4);
            sheet.SetCellContents("A5", 2);

            Console.WriteLine(sheet.GetNamesOfAllNonemptyCells());
            Console.WriteLine(sheet.GetCellContents("A1"));

            sheet.SetCellContents("A1", 10.0);

            Console.WriteLine(sheet.GetCellContents("A1"));

            List<string> list = sheet.SetCellContents("A1", 5.0).ToList();

            list.ForEach(x => Console.WriteLine(x));

            List<string> list2 = sheet.dependencyGraph.GetDependents("A1").ToList();
            list2.ForEach(x=> Console.WriteLine(x));

            List<string> list3 = sheet.dependencyGraph.GetDependees("A1").ToList();
            list3.ForEach(x => Console.WriteLine(x));

        }
    }
}