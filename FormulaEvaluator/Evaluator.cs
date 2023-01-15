using System.Collections;
using System.Numerics;
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
    public static class Evaluator
    {
        // Delegate method to help find the value of a variable
        public delegate int Lookup(String variable_name);

        /// <summary> This method evaluates integer arithmetic expressions written 
        /// using standard infix notation. </summary>
        /// <param name="expression"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns> An integer value of the calculated expression. </returns>
        public static int Evaluate(String expression, Lookup variableEvaluator) {
            // TODO...

            // Stacks for processing the expression in an arithmetic way
            Stack<int> values = new Stack<int>();
            Stack<String> operators = new Stack<String>();

            // Removes all whitespace from the input expression
            String no_whitespace_expression = String.Concat(expression.Where(c => !Char.IsWhiteSpace(c)));

            // Splits the expresion into an String array of tokens
            string[] substrings = Regex.Split(no_whitespace_expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            foreach (String substring in substrings) {

                Console.WriteLine(substring);

                int number;

                bool isVariable = false;

                bool topIsMultiply = false;
                bool topIsDivide = false;
                bool topIsAdd = false;
                bool topIsSubtract = false;

                if (operators.Count > 0) 
                { 
                    topIsMultiply = operators.Peek() == "*";
                    topIsDivide = operators.Peek() == "/";
                    topIsAdd = operators.Peek() == "+";
                    topIsSubtract = operators.Peek() == "-";
                }

                if (int.TryParse(substring, out number))
                {
                    if (topIsMultiply)
                    {
                        operators.Pop();
                        values.Push(number * values.Pop());
                    }
                    else if (topIsDivide)
                    {
                        operators.Pop();
                        values.Push(values.Pop() / number);
                    }
                    else
                    {
                        values.Push(number);
                    }
                }
                else if (substring == "+" || substring == "-")
                {
                    if (topIsAdd)
                    {
                        operators.Pop();
                        values.Push(values.Pop() + values.Pop());
                    }
                    else if (topIsSubtract)
                    {
                        operators.Pop();
                        values.Push(values.Pop() - values.Pop());
                    }

                    operators.Push(substring);
                }
                else if (substring == "*" || substring == "/")
                {
                    operators.Push(substring);
                }
                else if (substring == "(")
                {
                    operators.Push(substring);
                }
                else if (substring == ")")
                {
                    if (topIsAdd)
                    {
                        operators.Pop();
                        values.Push(values.Pop() + values.Pop());
                    }
                    else if (topIsSubtract)
                    {
                        operators.Pop();
                        int val1 = values.Pop();
                        int val2 = values.Pop(); 
                        values.Push(val2 - val1);
                    }

                    operators.Pop();

                    if (topIsMultiply)
                    {
                        operators.Pop();
                        values.Push(values.Pop() * values.Pop());
                    }
                    else if (topIsDivide)
                    {
                        operators.Pop();
                        int val1 = values.Pop();
                        int val2 = values.Pop();
                        values.Push(val2 / val1);
                    }
                }
                else
                {
                    //variable
                }
            }

            if (operators.Count == 0)
            {
                return values.Pop();
            }
            else
            {
                if (operators.Peek() == "+")
                {
                    operators.Pop();
                    return values.Pop() + values.Pop();
                }
                else if (operators.Peek() == "-")
                {
                    int val1 = values.Pop();
                    int val2 = values.Pop();
                    return val2 - val1;
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}