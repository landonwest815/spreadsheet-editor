using SpreadsheetUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SS
{
    /// <summary>
    /// <para>
    ///     An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    ///     spreadsheet consists of an infinite number of named cells.
    /// </para>
    /// <para>
    ///     A string is a valid cell name if and only if:
    /// </para>
    /// <list type="number">
    ///      <item> it starts with one or more letters</item>
    ///      <item> it ends with one or more numbers (digits)</item>
    /// </list>   
    /// 
    /// <para>
    ///     For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    ///     satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    ///     regardless of IsValid.
    /// </para>
    ///
    /// <para>
    ///     Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    ///     must be normalized with the Normalize method before it is used by or saved in 
    ///     this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    ///     the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// </para>
    /// 
    /// <para>
    ///     A spreadsheet contains a cell corresponding to every possible cell name.  (This
    ///     means that a spreadsheet contains an infinite number of cells.)  In addition to 
    ///     a name, each cell has a contents and a value.  The distinction is important.
    /// </para>
    /// 
    /// <para>
    ///     The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    ///     contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    ///     of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// </para>
    /// 
    /// <para>
    ///     In a new spreadsheet, the contents of every cell is the empty string. Note: 
    ///     this is by definition (it is IMPLIED, not stored).
    /// </para>
    /// 
    /// <para>
    ///     The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    ///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
    ///     in the grid.)
    /// </para>
    /// 
    /// <list type="number">
    ///   <item>If a cell's contents is a string, its value is that string.</item>
    /// 
    ///   <item>If a cell's contents is a double, its value is that double.</item>
    /// 
    ///   <item>
    ///      If a cell's contents is a Formula, its value is either a double or a FormulaError,
    ///      as reported by the Evaluate method of the Formula class.  The value of a Formula,
    ///      of course, can depend on the values of variables.  The value of a variable is the 
    ///      value of the spreadsheet cell it names (if that cell's value is a double) or 
    ///      is undefined (otherwise).
    ///   </item>
    /// 
    /// </list>
    /// 
    /// <para>
    ///     Spreadsheets are never allowed to contain a combination of Formulas that establish
    ///     a circular dependency.  A circular dependency exists when a cell depends on itself.
    ///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    ///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    ///     dependency.
    /// </para>
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, Cell> cells; // holds all non-empty cells
        private DependencyGraph dependencyGraph; // keeps track of dependencies across cells
        string spreadsheetVersion;

        // TODO: implement all constructors
        /// <summary>
        /// Constructor that initializes the above data
        /// </summary>
        public Spreadsheet() : this(s => true, s => s, "default")
        {
            cells = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
        }
          
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
            spreadsheetVersion = version;
            Normalize = normalize;
            IsValid = isValid;
            spreadsheetVersion = version;
        }

        public Spreadsheet(string filepath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            // implement
        }

        /// <summary>
        /// This class creates cell objects that can either hold a number, text, or formula object.
        /// There are also getter and setters for various parts of the data
        /// </summary>
        private class Cell
        {
            private string name;
            private Object contents;
            private Object value;

            /// <summary>
            /// Constructor for a cell holding a number
            /// </summary>
            /// <param name="variableName"> the name of the cell being initialized </param>
            /// <param name="number"> the number being held within the cell /param>
            public Cell(string variableName, double number, double numberValue)
            {
                name = variableName;
                contents = number;
                value = numberValue;
            }

            /// <summary>
            /// Constructor for a cell holding a string of text
            /// </summary>
            /// <param name="variableName"> name of the cell being initialized </param>
            /// <param name="text"> string of text held within the cell </param>
            public Cell(string variableName, string text, string textValue)
            {
                name = variableName;
                contents = text;
                value = textValue;
            }

            /// <summary>
            /// Constructor for a cell holding a formula object
            /// </summary>
            /// <param name="variableName"> name of the cell being initialized </param>
            /// <param name="expression"> the formula expression held within the cell </param>
            public Cell(string variableName, Formula expression)
            {
                name = variableName;
                contents = expression;
            }

            /// <summary>
            /// Getter method for the cell contents
            /// </summary>
            /// <returns> contents of the cell </returns>
            public object GetContents()
            {
                return contents;
            }

            /// <summary>
            /// Setter method for the cell contents
            /// </summary>
            /// <param name="value"> the value to set the cell contents to </param>
            public void SetContents(object value)
            {
                contents = value;
            }

            /// <summary>
            /// Getter method for the name of the cell
            /// </summary>
            /// <returns> the name of the cell </returns>
            public string GetName()
            {
                return name;
            }

            public void SetValue(Object calculatedValue)
            {
                value = calculatedValue;
            }

            public Object GetValue()
            {
                return value;
            }
        }

        /// <summary>
        ///   Returns the contents (as opposed to the value) of the named cell.
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   Thrown if the name is invalid: blank/empty/""
        /// </exception>
        /// 
        /// <param name="name">The name of the spreadsheet cell to query</param>
        /// 
        /// <returns>
        ///   The return value should be either a string, a double, or a Formula.
        ///   See the class header summary 
        /// </returns>
        public override object GetCellContents(string name)
        {
            // checks to make sure the name given exists in the dictionary
            if (cells.ContainsKey(name)) return cells[name].GetContents();
            else                         throw new InvalidNameException();
        }

        /// <summary>
        ///   Returns the names of all non-empty cells.
        /// </summary>
        /// 
        /// <returns>
        ///     Returns an Enumerable that can be used to enumerate
        ///     the names of all the non-empty cells in the spreadsheet.  If 
        ///     all cells are empty then an IEnumerable with zero values will be returned.
        /// </returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        // TODO: return an IList
        /// <summary>
        ///  Set the contents of the named cell to the given number.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid: non-empty/not ""
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="number"> The new contents/value </param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            if (!NameExists(name)) // Checks if the cell exists already
                cells.Add(name, new Cell(name, number, number));
            else
            {
                PreviousContentsContainedFormula(name); // This method checks if the contents being replaced was a formula and adjusts the dependencyGraph if so
                cells[name].SetContents(number);
                cells[name].SetValue(number);
            }

            RecalculateCells(GetAllDependents(name));

            return NameWithDependents(name);
        }

        // TODO: return an IList
        /// <summary>
        /// The contents of the named cell becomes the text.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid/non-empty ""
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>       
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="text"> The new content/value of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            if (text == null) throw new ArgumentNullException(); // Error checker

            if (!NameExists(name)) // Checks if the cell exists already
                cells.Add(name, new Cell(name, text, text));
            else
            {
                PreviousContentsContainedFormula(name); // This method checks if the contents being replaced was a formula and adjusts the dependencyGraph if so
                cells[name].SetContents(text);
                cells[name].SetValue(text);
            }

            if (text == "") // "" signifies an empty cell so it is removed
            {
                cells.Remove(name);
                return NameWithDependents(name);
            }

            RecalculateCells(GetAllDependents(name));

            return NameWithDependents(name);
        }

        // TODO: return an IList
        /// <summary>
        /// Set the contents of the named cell to the formula.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid/non empty
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name</param>
        /// <param name="formula"> The content of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            if (formula.ToString() == null) throw new ArgumentNullException(); // Error checker
             
            if (!NameExists(name))
                cells.Add(name, new Cell(name, formula));
            else
            {
                PreviousContentsContainedFormula(name); // This method checks if the contents being replaced was a formula and adjusts the dependencyGraph if so
                cells[name].SetContents(formula);
            }

            AddDepencencies(name, formula); // Adds a dependency between the name of the cell being adjusted and all the varibales within the formula expression

            try // Checks for circular exception
            {
                IList<string> dependentsSet = NameWithDependents(name);
                RecalculateCells(dependentsSet);
                return dependentsSet;
            }
            catch (CircularException)
            {
                RemoveDependencies(name, formula); // Restore dependency graph
                throw new CircularException();
            }
        }

        /// <summary>
        /// This method retrieves all the direct dependents of a given cell
        /// </summary>
        /// <param name="name"> the given cell name </param>
        /// <returns> the direct dependents of a given cell name </returns>
        /// <exception cref="ArgumentNullException"> throws if the name given is null</exception>
        /// <exception cref="InvalidNameException"> throws if the name given does not exist in the dictionary </exception>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null) throw new ArgumentNullException();
            if (name == null || !cells.ContainsKey(name)) throw new InvalidNameException();

            return dependencyGraph.GetDependees(name);
        }

        // TODO: implement the method
        /// <summary>
        ///   <para>Sets the contents of the named cell to the appropriate value. </para>
        ///   <para>
        ///       First, if the content parses as a double, the contents of the named
        ///       cell becomes that double.
        ///   </para>
        ///
        ///   <para>
        ///       Otherwise, if content begins with the character '=', an attempt is made
        ///       to parse the remainder of content into a Formula.  
        ///       There are then three possible outcomes:
        ///   </para>
        ///
        ///   <list type="number">
        ///       <item>
        ///           If the remainder of content cannot be parsed into a Formula, a 
        ///           SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       </item>
        /// 
        ///       <item>
        ///           If changing the contents of the named cell to be f
        ///           would cause a circular dependency, a CircularException is thrown,
        ///           and no change is made to the spreadsheet.
        ///       </item>
        ///
        ///       <item>
        ///           Otherwise, the contents of the named cell becomes f.
        ///       </item>
        ///   </list>
        ///
        ///   <para>
        ///       Finally, if the content is a string that is not a double and does not
        ///       begin with an "=" (equal sign), save the content as a string.
        ///   </para>
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name parameter is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="SpreadsheetUtilities.FormulaFormatException"> 
        ///   If the content is "=XYZ" where XYZ is an invalid formula, throw a FormulaFormatException.
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name that is being changed</param>
        /// <param name="content"> The new content of the cell</param>
        /// 
        /// <returns>
        ///       <para>
        ///           This method returns a list consisting of the passed in cell name,
        ///           followed by the names of all other cells whose value depends, directly
        ///           or indirectly, on the named cell. The order of the list MUST BE any
        ///           order such that if cells are re-evaluated in that order, their dependencies 
        ///           are satisfied by the time they are evaluated.
        ///       </para>
        ///
        ///       <para>
        ///           For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///           list {A1, B1, C1} is returned.  If the cells are then evaluate din the order:
        ///           A1, then B1, then C1, the integrity of the Spreadsheet is maintained.
        ///       </para>
        /// </returns>
        public override IList<String> SetContentsOfCell(String name, String content)
        {
            string normalizedName = Normalize(name); // normalizes passed in cell name
            if (IsValid(normalizedName)) throw new InvalidNameException(); // checks it for validity
            IList<string> cellsToReevaluate = new List<string>();

            if (Double.TryParse(content, out double value))
                cellsToReevaluate = SetCellContents(name, value);
            else if (content[0].ToString() == "=")                                 
                cellsToReevaluate = SetCellContents(name, new Formula(content));
            else
                cellsToReevaluate = SetCellContents(name, content);

            return cellsToReevaluate;
        }

        // TODO: implement the four methods below
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// Method used to determine whether a string that consists of one or more letters
        /// followed by one or more digits is a valid variable name.
        /// </summary>
        public Func<string, bool> IsValid { get; protected set; }

        /// <summary>
        /// Method used to convert a cell name to its standard form.  For example,
        /// Normalize might convert names to upper case.
        /// </summary>
        public Func<string, string> Normalize { get; protected set; }

        /// <summary>
        /// Version information
        /// </summary>
        public string Version { get; protected set; }
          
        // TODO: implement this method
        /// <summary>
        ///   Look up the version information in the given file. If there are any problems opening, reading, 
        ///   or closing the file, the method should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        /// 
        /// <remarks>
        ///   In an ideal world, this method would be marked static as it does not rely on an existing SpreadSheet
        ///   object to work; indeed it should simply open a file, lookup the version, and return it.  Because
        ///   C# does not support this syntax, we abused the system and simply create a "regular" method to
        ///   be implemented by the base class.
        /// </remarks>
        /// 
        /// <exception cref="SpreadsheetReadWriteException"> 
        ///   Thrown if any problem occurs while reading the file or looking up the version information.
        /// </exception>
        /// 
        /// <param name="filename"> The name of the file (including path, if necessary)</param>
        /// <returns>Returns the version information of the spreadsheet saved in the named file.</returns>
        public override string GetSavedVersion(String filename)
        {
            return "do this";
        }

        // TODO: implement this method
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(String filename)
        {

        }

        // TODO: implement this method
        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell that we want the value of (will be normalized)</param>
        /// 
        /// <returns>
        ///   Returns the value (as opposed to the contents) of the named cell.  The return
        ///   value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </returns>
        public override object GetCellValue(String name)
        {
            return cells[name].GetValue();
        }

        // HELPER METHODS

        private double CellValueLookup(String name)
        {
            if (Double.TryParse(cells[name].GetValue().ToString(), out double value))
                return value;
            else
            {
                throw new ArgumentException();
            }
        }

        private void RecalculateCells(IList<string> cellsToRecalculate)
        {
            foreach (string cell in cellsToRecalculate)
            {
                Formula f = (Formula)cells[cell].GetContents();
                cells[cell].SetValue(f.Evaluate(CellValueLookup));
            }
        }

        /// <summary>
        /// This helper method checks if the name of a cell is a valid name and exists within the dictionary
        /// </summary>
        /// <param name="name"> the cell name being checked </param>
        /// <returns> bool variable depending on the outcome </returns>
        /// <exception cref="InvalidNameException"> throws if the name is not a valid cell name </exception>
        private bool NameExists(string name)
        {
            if (!Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z_0-9]*$") || name == null) throw new InvalidNameException();

            return cells.ContainsKey(name);
        }

        /// <summary>
        /// This helper method retrieves a set of dependents with a given cell name at the front
        /// </summary>
        /// <param name="name"> the cell that the dependents depend on </param>
        /// <returns> an Iset of type string with the cell name and its dependents </returns>
        private IList<string> NameWithDependents(string name)
        {
            IList<string> dependents = new List<string> { name };
            foreach (var variable in GetAllDependents(name))
                dependents.Add(variable);
            return dependents;
        }

        private IList<string> GetAllDependents(string name)
        {
            IList<string> dependents = new List<string>();
            foreach (var variable in GetCellsToRecalculate(name))
                dependents.Add(variable);
            return dependents;
        }

        /// <summary>
        /// This helper method adds dependencies from a given cell name and formula
        /// </summary>
        /// <param name="name"> the cell name to add dependencies to </param>
        /// <param name="formula"> the variables to add to the dependees of the given cell name </param>
        private void AddDepencencies(string name, Formula formula)
        {
            foreach (string variable in formula.GetVariables())
                dependencyGraph.AddDependency(name, variable);
        }

        /// <summary>
        /// This helper method removes dependencies from a given cell name and formula
        /// </summary>
        /// <param name="name"> the cell name to remove dependencies from </param>
        /// <param name="formula"> the variables to remove from the dependees of the given cell name </param>
        private void RemoveDependencies(string name, Formula formula)
        {
            foreach (string variable in formula.GetVariables())
                dependencyGraph.RemoveDependency(name, variable);
        }

        /// <summary>
        /// This helper method checks if the previous contents were a formual and adjusts dependencies
        /// </summary>
        /// <param name="name"> the name of the cell that was adjusted </param>
        private void PreviousContentsContainedFormula(string name)
        {
            if (cells[name].GetContents().GetType().Equals(typeof(Formula)))
                RemoveDependencies(name, (Formula)cells[name].GetContents());
        }
    }
}