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
    public class Cell
    {
        private string name;
        private Object contents;
        public Cell(string variableName, double number) 
        {
            name = variableName;
            contents = number;
        }

        public Cell(string variableName, string text)
        {
            name = variableName;
            contents = text;
        }

        public Cell(string variableName, Formula expression)
        {
            name = variableName;
            contents = expression;
        }

        public object GetContents()
        {
            return contents;
        }

        public void SetContents(object value)
        {
            contents = value;
        }

        public string GetName()
        {
            return name;
        }
    }

    public class Spreadsheet : AbstractSpreadsheet
    {
        public Dictionary<string, Cell> cells;
        public DependencyGraph dependencyGraph;
        public HashSet<Cell> changedCells;

        public Spreadsheet()
        {
            cells = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
            changedCells = new HashSet<Cell>();
        }

        public override object GetCellContents(string name)
        {
            if (cells.ContainsKey(name))
                return cells[name].GetContents();
            else
                throw new InvalidNameException();
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        public override ISet<string> SetCellContents(string name, double number)
        {
            bool nameExists = NameExists(name);

            if (!nameExists)
                cells.Add(name, new Cell(name, number));
            else
            {
                PreviousContentsContainedFormula(name);
                cells[name].SetContents(number);
                changedCells.Add(cells[name]);
            }

            return DependentsSet(name);
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            bool nameExists = NameExists(name);

            if (!nameExists)
                cells.Add(name, new Cell(name, text));
            else
            {
                PreviousContentsContainedFormula(name);
                cells[name].SetContents(text);
                changedCells.Add(cells[name]);
            }

            return DependentsSet(name);
        }

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            bool nameExists = NameExists(name);

            if (!nameExists)
                cells.Add(name, new Cell(name, formula));
            else
            {
                PreviousContentsContainedFormula(name);
                cells[name].SetContents(formula);
                changedCells.Add(cells[name]);
            }

            AddDepencencies(name, formula);

            return DependentsSet(name);
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return dependencyGraph.GetDependents(name);
        }

        // HELPER METHODS

        public bool NameExists(string name)
        {
            // catches invalid names
            if (!Regex.IsMatch(name, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*") || name == null)
                    throw new InvalidNameException();

            return cells.ContainsKey(name);
        }

        public ISet<string> DependentsSet(string name)
        {
            ISet<string> dependents = new HashSet<string> { name };
            foreach (var variable in dependencyGraph.GetDependents(name))
                dependents.Add(variable);
            return dependents;
        }

        public void AddDepencencies(string name, Formula formula)
        {
            foreach (string variable in formula.GetVariables())
                dependencyGraph.AddDependency(name, variable);
        }

        public void RemoveDependencies(string name, Formula formula)
        {
            foreach (string variable in formula.GetVariables())
                dependencyGraph.RemoveDependency(name, variable);
        }

        public void PreviousContentsContainedFormula(string name)
        {
            if (cells[name].GetContents().GetType().Equals(typeof(Formula)))
                RemoveDependencies(name, (Formula)cells[name].GetContents());
        }
    }
}