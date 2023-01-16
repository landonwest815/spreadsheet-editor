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

            // Loops through every token one by one
            foreach (String substring in substrings) {

                int number; // Used for integer tokens

                bool isVariable = false; // Will implement later

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

                // If the current token is an integer...
                if (int.TryParse(substring, out number))
                {
                    // If the top of the operators stack contains a '*'
                    if (topIsMultiply)
                    {
                        if (values.Count == 0) 
                            throw new ArgumentException();
                        
                        operators.Pop(); // Pop it
                        values.Push(number * values.Pop()); // Multiply current token with top value
                    }

                    // If the top of the operators stack contains a '/'
                    else if (topIsDivide)
                    {
                        if (number == 0)
                            throw new ArgumentException();
                        
                        operators.Pop(); // Pop it
                        values.Push(values.Pop() / number); // Divide top value by the current token
                    }
                    // Otherwise push it onto values stack
                    else
                    {
                        values.Push(number); 
                    }
                }
                // If the current token is a '+' or '-'...
                else if (substring == "+" || substring == "-")
                {
                    // If the top operator is a '+'
                    if (topIsAdd)
                    {
                        if (values.Count < 2)
                            throw new ArgumentException();
                        
                        operators.Pop(); // Pop it
                        values.Push(values.Pop() + values.Pop()); // Add the top two values
                    }
                    // If the top operator is a '-'
                    else if (topIsSubtract)
                    {
                        if (values.Count < 2)
                            throw new ArgumentException();
                        
                        operators.Pop(); // Pop it
                        int val1 = values.Pop();
                        int val2 = values.Pop();
                        values.Push(val2 - val1); // Subract the top value from the second highest value
                    }

                    operators.Push(substring); // Push current token onto operators stack
                }
                // If the current token is a '*' or '/'...
                else if (substring == "*" || substring == "/")
                {
                    operators.Push(substring); // Push current token onto operators stack
                }
                // If the current token is a '('...
                else if (substring == "(")
                {
                    operators.Push(substring); // Push current token onto operators stack
                }
                // If the current token is a ')'...
                else if (substring == ")")
                {
                    // If the top operator is a '+'...
                    if (topIsAdd)
                    {
                        if (values.Count < 2)
                            throw new ArgumentException();
                       
                        operators.Pop(); // Pop it
                        values.Push(values.Pop() + values.Pop()); // Add the two top values
                    }
                    // If the top operator is a '-'
                    else if (topIsSubtract)
                    {
                        if (values.Count < 2)
                            throw new ArgumentException();

                        operators.Pop(); // Pop it
                        int val1 = values.Pop();
                        int val2 = values.Pop(); 
                        values.Push(val2 - val1); // Subtract the top value from the second highest value
                    }
                    
                    if (operators.Peek() != "(")
                        throw new ArgumentException();

                    operators.Pop(); // Pop the '('

                    if (operators.Count != 0)
                    {
                        // If the top operator is a '*'...
                        if (operators.Peek() == "*")
                        {
                            operators.Pop(); // Pop it
                            values.Push(values.Pop() * values.Pop()); // Multiply the top two values
                        }
                        // If the top operator is a '/'...
                        else if (operators.Peek() == "/")
                        {
                            if (values.Peek() == 0)
                                throw new ArgumentException();

                            operators.Pop(); // Pop it
                            int val1 = values.Pop();
                            int val2 = values.Pop();
                            values.Push(val2 / val1); // Divide the second highest value by the top value
                        }
                    }
                }
                else
                {
                    // Variable handling to be implemented later
                }
            }
            // If the operator stack is empty -> return the remaining value
            if (operators.Count == 0)
            {
                if (values.Count > 1)
                    throw new ArgumentException();

                return values.Pop();
            }
            // Special cases
            else
            {
                if (operators.Count > 1)
                    throw new ArgumentException();

                if (values.Count > 2)
                    throw new ArgumentException();

                // If the top operator is a '+'...
                if (operators.Peek() == "+")
                {
                    operators.Pop(); // Pop it
                    return values.Pop() + values.Pop(); // Add the two remaining values
                }
                // If the top operator is a '-'...
                else if (operators.Peek() == "-")
                {
                    int val1 = values.Pop();
                    int val2 = values.Pop();
                    return val2 - val1; // Subtract the top value from the second highest value
                }
                // If something goes wrong...
                else
                {
                    throw new ArgumentException();
                }
            }
        }
    }
}