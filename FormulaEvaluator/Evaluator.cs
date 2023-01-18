using System.Collections;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Landon West
    /// Partner:   None
    /// Date:      13-Jan-2023
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
    ///    This library class evaluates an arithmetic expression that may contain 
    ///    numbers, variables, and operators.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// This delegate handles all variables passed into the Evaluate method
        /// </summary>
        /// <param name="variable_name"> variable from expression </param>
        /// <returns> an integer value (if no expression is thrown) </returns>
        public delegate int Lookup(String variable_name);

        /// <summary> This method evaluates integer arithmetic expressions written 
        /// using standard infix notation. </summary>
        /// <param name="expression"> arithmetic expression to be calculated</param>
        /// <param name="variableEvaluator"> delegate to help with variables </param>
        /// <returns> An integer value of the calculated expression. (If an exception is not thrown) </returns>
        /// <exception cref="ArgumentException"> throws ArgumentException if invalid syntax is detected </exception>
        public static int Evaluate(String expression, Lookup variableEvaluator) {

            // Stacks for processing the expression in an arithmetic way
            Stack<int> values = new Stack<int>();
            Stack<String> operators = new Stack<String>();

            // Removes whitespace from the input expression
            String no_whitespace_expression = String.Concat(expression.Where(c => !Char.IsWhiteSpace(c)));

            // Splits the expresion into an String array of tokens
            string[] tokens = Regex.Split(no_whitespace_expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            // Loops through every token one by one
            foreach (String t in tokens) {

                // Initial Setup:

                // Used for integer tokens
                int value;

                // Helps simplify the process below
                bool topIsMultiply = false;
                bool topIsDivide = false;
                bool topIsAdd = false;
                bool topIsSubtract = false;

                // Sets values for the above booleans when evaluating each token
                if (operators.Count > 0) 
                { 
                    topIsMultiply = operators.Peek() == "*";
                    topIsDivide = operators.Peek() == "/";
                    topIsAdd = operators.Peek() == "+";
                    topIsSubtract = operators.Peek() == "-";
                }

                // The Process:

                // If the current token is an integer or a variable...
                if (int.TryParse(t, out value) || Regex.IsMatch(t, "[a-zA-Z]+[0-9]+"))
                {
                    if (Regex.IsMatch(t, "[a-zA-Z]+[0-9]+"))
                        value = variableEvaluator(t);

                    // If the top of the operators stack contains a '*'
                    if (topIsMultiply)
                    {
                        // ERROR CHECKER
                        if (values.Count == 0)
                            throw new ArgumentException();

                        // Pop the operators stack and multiply the current token with the top of the values stack
                        operators.Pop();
                        values.Push(value * values.Pop());
                    }

                    // If the top of the operators stack contains a '/'
                    else if (topIsDivide)
                    {
                        // ERROR CHECKER
                        if (value == 0)
                            throw new ArgumentException();

                        // ERROR CHECKER
                        if (values.Count == 0)
                            throw new ArgumentException();  

                        //Pop the operators stack and divide the top of the values stack by the current token
                        operators.Pop();
                        values.Push(values.Pop() / value);
                    }
                    // Otherwise push it onto the values stack
                    else
                    {
                        values.Push(value); 
                    }
                }
                // If the current token is a '+' or '-'...
                else if (t == "+" || t == "-")
                {
                    // If the top operator is a '+'
                    if (topIsAdd)
                    {
                        // ERROR CHECKER
                        if (values.Count < 2)
                            throw new ArgumentException();
                        
                        // Pop the operators stack and add the top two values on the values stack together
                        operators.Pop();
                        values.Push(values.Pop() + values.Pop());
                    }
                    // If the top operator is a '-'
                    else if (topIsSubtract)
                    {
                        // ERROR CHECKER
                        if (values.Count < 2)
                            throw new ArgumentException();
                        
                        // Pop the operators stack and subtract the top value of the values stack from the second highest value on the values stack
                        operators.Pop();
                        int val1 = values.Pop();
                        int val2 = values.Pop();
                        values.Push(val2 - val1);
                    }
                    // Push current token onto operators stack
                    operators.Push(t);
                }
                // If the current token is a '*' or '/' or '('...
                else if (t == "*" || t == "/" || t == "(")
                {
                    // Push current token onto operators stack
                    operators.Push(t);
                }
                // If the current token is a ')'...
                else if (t == ")")
                {
                    // If the top operator is a '+'
                    if (topIsAdd)
                    {
                        // ERROR CHECKER
                        if (values.Count < 2)
                            throw new ArgumentException();

                        // Pop the operators stack and add the top two values on the values stack together
                        operators.Pop();
                        values.Push(values.Pop() + values.Pop());
                    }
                    // If the top operator is a '-'
                    else if (topIsSubtract)
                    {
                        // ERROR CHECKER
                        if (values.Count < 2)
                            throw new ArgumentException();

                        // Pop the operators stack and subtract the top value of the values stack from the second highest value on the values stack
                        operators.Pop();
                        int val1 = values.Pop();
                        int val2 = values.Pop();
                        values.Push(val2 - val1);
                    }

                    // ERROR CHECKER
                    if (operators.Count == 0)
                        throw new ArgumentException();

                    // ERROR CHECKER
                    if (operators.Peek() != "(")
                        throw new ArgumentException();

                    // Pop the '('
                    operators.Pop();

                    // If the operator stack is not empty...
                    if (operators.Count != 0)
                    {
                        // If the top operator is a '*'...
                        if (operators.Peek() == "*")
                        {
                            // Pop the operators stack and multiply the top two values in the values stack
                            operators.Pop();
                            values.Push(values.Pop() * values.Pop());
                        }
                        // If the top operator is a '/'...
                        else if (operators.Peek() == "/")
                        {
                            // ERROR CHECKER
                            if (values.Peek() == 0)
                                throw new ArgumentException();

                            // Pop the operators stack and divide the second highest value in the values stack by the top value
                            operators.Pop();
                            int val1 = values.Pop();
                            int val2 = values.Pop();
                            values.Push(val2 / val1);
                        }
                    }
                }
                // Remaining tokens to be checked should only be random whitespaces...
                else if (!String.IsNullOrEmpty(t))
                {
                    throw new ArgumentException();
                }
            }

            // Finishing Steps:

            // If the values stack is emtpy -> invalid syntax
            if (values.Count == 0)
            {
                    throw new ArgumentException();
            }

            // If the operator stack is empty -> return the remaining value
            if (operators.Count == 0)
            {
                // ERROR CHECKER
                if (values.Count > 1)
                    throw new ArgumentException();

                return values.Pop();
            }
            // Special cases
            else
            {
                // ERROR CHECKER
                if (operators.Count > 1)
                    throw new ArgumentException();

                // ERROR CHECKER
                if (values.Count > 2)
                    throw new ArgumentException();

                // If the top operator is a '+'...
                if (operators.Peek() == "+")
                {                        
                    // ERROR CHECKER
                    if (values.Count < 2)
                        throw new ArgumentException();
              
                    // Pop the operator stack and add the top two values in the values stack
                    operators.Pop();
                    return values.Pop() + values.Pop();
                }
                // If the top operator is a '-'...
                else if (operators.Peek() == "-")
                {
                    // ERROR CHECKER
                    if (values.Count < 2)
                        throw new ArgumentException();

                    // Pop the operator stack and subtract the top value in the values stack from the second highest value in the values stack
                    operators.Pop();
                    int val1 = values.Pop();
                    int val2 = values.Pop();
                    return val2 - val1;
                }
                // If anything else goes wrong...
                else
                {
                    throw new ArgumentException();
                }
            }
        }
    }
}