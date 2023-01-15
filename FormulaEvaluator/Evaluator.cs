using System.Collections;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Landon West
    /// Partner:   None
    /// Date:      1/13/2023
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
    ///    This library class evaluates...
    /// </summary>
    public class Evaluator
    {
        // Delegate method to help find the value of a variable
        public delegate int Lookup(String variable_name);

        // Method to evaluate an expression using arithmetic 
        public static int Evaluate(String expression, Lookup variableEvaluator) {
            // TODO...

            // Stacks for processing the expression in an arithmetic way
            Stack values = new Stack();
            Stack operators = new Stack();

            // Removes all whitespace from the input expression
            String no_whitespace_expression = String.Concat(expression.Where(c => !Char.IsWhiteSpace(c)));

            // Splits the expresion into an String array of tokens
            string[] substrings = Regex.Split(no_whitespace_expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            return 0;
        }
    }
}