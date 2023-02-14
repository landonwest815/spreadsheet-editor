using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SS
{
    // comment
    /// <summary>
    /// <para>
    ///     An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    ///     spreadsheet consists of an infinite number of named cells.
    /// </para>
    /// <para>
    ///     A string is a valid cell name if and only if:
    /// </para>
    /// <list type="number">
    ///      <item> its first character is an underscore or a letter</item>
    ///      <item> its remaining characters (if any) are underscores and/or letters and/or digits</item>
    /// </list>   
    /// <para>
    ///     Note that this is the same as the definition of valid variable from the Formula class assignment.
    /// </para>
    /// 
    /// <para>
    ///     For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    ///     "25", "2x", and "&amp;" are not.  Cell names are case sensitive, so "x" and "X" are
    ///     different cell names.
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

        /// <summary>
        /// Constructor that initializes the above data
        /// </summary>
        public Spreadsheet()
        {
            cells = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
        }

        /// <summary>
        /// This class creates cell objects that can either hold a number, text, or formula object.
        /// There are also getter and setters for various parts of the data
        /// </summary>
        private class Cell
        {
            private string name;
            private Object contents;

            /// <summary>
            /// Constructor for a cell holding a number
            /// </summary>
            /// <param name="variableName"> the name of the cell being initialized </param>
            /// <param name="number"> the number being held within the cell /param>
            public Cell(string variableName, double number)
            {
                name = variableName;
                contents = number;
            }

            /// <summary>
            /// Constructor for a cell holding a string of text
            /// </summary>
            /// <param name="variableName"> name of the cell being initialized </param>
            /// <param name="text"> string of text held within the cell </param>
            public Cell(string variableName, string text)
            {
                name = variableName;
                contents = text;
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
        }

        /// <summary>
        /// This method retrieves the contents of cell based on the name input given
        /// </summary>
        /// <param name="name"> the name of cell </param>
        /// <returns> the contents of the given cell name </returns>
        /// <exception cref="InvalidNameException"> throws if the name given does not exist in the current context </exception>
        public override object GetCellContents(string name)
        {
            // checks to make sure the name given exists in the dictionary
            if (cells.ContainsKey(name)) return cells[name].GetContents();
            else                         throw new InvalidNameException();
        }

        /// <summary>
        /// This method retrieves the names of all cells for which they are nonempty
        /// </summary>
        /// <returns> IEnumerable of all nonempty cells </returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        /// <summary>
        /// This method sets the contents of a cell given a name and a number value
        /// </summary>
        /// <param name="name"> the name of the cell to adjust </param>
        /// <param name="number"> the number value to adjust the cell with </param>
        /// <returns> an ISet of type string that contains the name of the cell that was adjusted and all the cells that depend on the adjusted cell </returns>
        public override ISet<string> SetCellContents(string name, double number)
        {
            if (!NameExists(name)) // Checks if the cell exists already
                cells.Add(name, new Cell(name, number));
            else
            {
                PreviousContentsContainedFormula(name); // This method checks if the contents being replaced was a formula and adjusts the dependencyGraph if so
                cells[name].SetContents(number);
            }

            return NameWithDependents(name);
        }

        /// <summary>
        /// This method sets the contents of a cell given a name and a string of text
        /// </summary>
        /// <param name="name"> the name of the cell to adjust </param>
        /// <param name="text"> the string of text to adjust the given cell with</param>
        /// <returns> an ISet of type string that contains the name of the cell that was adjusted and all the cells that depend on the adjusted cell </returns>
        /// <exception cref="ArgumentNullException"> throws if the text input is an empty string </exception>
        public override ISet<string> SetCellContents(string name, string text)
        {
            if (text == null) throw new ArgumentNullException(); // Error checker

            if (!NameExists(name)) // Checks if the cell exists already
                cells.Add(name, new Cell(name, text));
            else
            {
                PreviousContentsContainedFormula(name); // This method checks if the contents being replaced was a formula and adjusts the dependencyGraph if so
                cells[name].SetContents(text);
            }

            if (text == "") // "" signifies an empty cell so it is removed
            {
                cells.Remove(name);
                return new HashSet<string>();
            }

            return NameWithDependents(name);
        }

        /// <summary>
        /// This method sets the contents of a cell given a name and a formula object
        /// </summary>
        /// <param name="name"> the name of the cell to adjust </param>
        /// <param name="formula"> the formula expression to adjust the given cell with </param>
        /// <returns> an ISet of type string that contains the name of the cell that was adjusted and all the cells that depend on the adjusted cell </returns>
        /// <exception cref="ArgumentNullException"> throws if the formula expression is empty </exception>
        public override ISet<string> SetCellContents(string name, Formula formula)
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
                ISet<string> dependentsSet = NameWithDependents(name);
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

        // HELPER METHODS

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
        private ISet<string> NameWithDependents(string name)
        {
            ISet<string> dependents = new HashSet<string> { name };
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