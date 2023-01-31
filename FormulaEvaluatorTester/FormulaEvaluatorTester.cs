using System.Collections;

namespace FormulaEvaluator;

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
///    This tester class passes in expressions -- that involve numbers, variables, and operators -- to be evaluated.
/// </summary>
public class EvaluateTester
    {
    /// <summary> This is the delegate used to determine a variable's value in the evaluate method </summary>
    /// <param name="v"> variable from expression </param>
    /// <returns> the integer value tied to the variable (if the variable exists) </returns>
    /// <exception cref="ArgumentException"> throws ArgumentException if invalid syntax is detected </exception>
    public static int VariableLookup(string v)
        {
            if (v == "A1")
                return 1;
            else if (v == "B1")
                return 2;
            else if (v == "C1")
                return 3;
            else if (v == "A2")
                return 2;
            else if (v == "B2")
                return 4;
            else if (v == "C2")
                return 6;
            else if (v == "A3")
                return 3;
            else if (v == "B3")
                return 6;
            else if (v == "C3")
                return 9;
            else
                throw new ArgumentException();
        }

    /// <summary> This class contains all the written expressions to be tested </summary>
    public class Tests
    {
        /// <summary> This is the main method for the tests to run in </summary>
        /// <param name="args"> necessary parameter for main method to function </param>
        static void Main(string[] args)
        {
            // ADDITION TESTS:

                Console.WriteLine("Failed Addition Tests:");

                // 0+0  ZEROS
                try
                {
                    int val = Evaluator.Evaluate("0+0", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax1");
                }

                // 5+5  SIMPLE ADDITION
                try
                {
                    int val = Evaluator.Evaluate("5+5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax2");
                }

                // 5+5-2  ADDITION & SUBTRACTION AFTER
                try
                {
                    Evaluator.Evaluate("5+5-2", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax3");
                }

                // 10-5+5  ADDITION & SUBTRACTION BEFORE
                try
                {
                    Evaluator.Evaluate("10-5+5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax4");
                }

                // 5+5*2  ADDITION & MULTIPLICATION AFTER
                try
                {
                    Evaluator.Evaluate("5+5*2", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax5");
                }

                // 2*5+5  ADDITION & MULTIPLICATION BEFORE
                try
                {
                    Evaluator.Evaluate("2*5+5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax6");
                }

                // 5+5/2  ADDITION & DIVISION AFTER
                try
                {
                    Evaluator.Evaluate("5+5/2", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax7");
                }

                // 5/1+5  ADDITION & DIVISION BEFORE
                try
                {
                    Evaluator.Evaluate("5/1+5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax8");
                }

                // 5+5+A1  ADDITION & VARIABLE AFTER
                try
                {
                    Evaluator.Evaluate("5+5+A1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax9");
                }

                // A2+5+5+A1  ADDITION & VARIABLE BEFORE/AFTER
                try
                {
                    Evaluator.Evaluate("A2+5+5+A1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax10");
                }

                // A2+A1  VARIABLE ADDITION
                try
                {
                    Evaluator.Evaluate("A2+A1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax11");
                }

            // SUBTRACTION TESTS:

                Console.WriteLine("Failed Subtraction Tests:");

                // 0-0  ZEROS
                try
                {
                    int val = Evaluator.Evaluate("0-0", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 5-5  SIMPLE ADDITION
                try
                {
                    int val = Evaluator.Evaluate("5-5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 5-5+2  SUBTRACTION & ADDITION AFTER
                try
                {
                    Evaluator.Evaluate("5-5+2", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 10+5-5  SUBTRACTION & ADDITION BEFORE
                try
                {
                    Evaluator.Evaluate("10+5-5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 20-5*2  SUBTRACTION & MULTIPLICATION AFTER
                try
                {
                    Evaluator.Evaluate("20-5*2", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 2*10-5  SUBTRACTION & MULTIPLICATION BEFORE
                try
                {
                    Evaluator.Evaluate("2*20-5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 15-10/2  SUBTRACTION & DIVISION AFTER
                try
                {
                    Evaluator.Evaluate("15-10/2", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 10/5-1  SUBTRACTION & DIVISION BEFORE
                try
                {
                    Evaluator.Evaluate("10/5-1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 5-5+A1  SUBTRACTION & VARIABLE AFTER
                try
                {
                    Evaluator.Evaluate("5-5+A1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // A2+5-5+A1  SUBTRACTION & VARIABLE BEFORE/AFTER
                try
                {
                    Evaluator.Evaluate("A2+5-5+A1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // A2-A1  VARIABLE SUBTRACTION
                try
                {
                    Evaluator.Evaluate("A2-A1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

            // MULTIPLICATION TESTS:

                Console.WriteLine("Failed Multiplication Tests:");

                // 0*0  ZEROS
                try
                {
                    int val = Evaluator.Evaluate("0*0", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 5*5  SIMPLE MULTIPLICATION
                try
                {
                    int val = Evaluator.Evaluate("5*5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 5*5+2  MULTIPLICATION & ADDITION AFTER
                try
                {
                    Evaluator.Evaluate("5*5+2", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 10+5*5  MULTIPLICATION & ADDITION BEFORE
                try
                {
                    Evaluator.Evaluate("10+5*5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 2*10/2  MULTIPLICATION & DIVISION AFTER
                try
                {
                    Evaluator.Evaluate("2*10/2", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 10/5*1  MULTIPLICATION & DIVISION BEFORE
                try
                {
                    Evaluator.Evaluate("10/5*1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 5*5+A1  MULTIPLICATION & VARIABLE AFTER
                try
                {
                    Evaluator.Evaluate("5*5+A1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // A2+5*5+A1  MULTIPLICATION & VARIABLE BEFORE/AFTER
                try
                {
                    Evaluator.Evaluate("A2+5*5+A1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // A2*A1  VARIABLE MULTIPLICATION
                try
                {
                    Evaluator.Evaluate("A2*A1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

            // DIVISION TESTS:

                Console.WriteLine("Failed Division Tests:");

                // 5/5  SIMPLE DIVISION
                try
                {
                    int val = Evaluator.Evaluate("5/5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 5/5+2  DIVISION & ADDITION AFTER
                try
                {
                    Evaluator.Evaluate("5/5+2", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 10+5/5  DIVISION & ADDITION BEFORE
                try
                {
                    Evaluator.Evaluate("10+5/5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // 5/5+A1  DIVISION & VARIABLE AFTER
                try
                {
                    Evaluator.Evaluate("5/5+A1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // A2+5/5+A1  DIVISION & VARIABLE BEFORE/AFTER
                try
                {
                    Evaluator.Evaluate("A2+5/5+A1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // A2*A1  VARIABLE DIVISION
                try
                {
                    Evaluator.Evaluate("A2/A1", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

            // PARANTHESIS TESTS:

                Console.WriteLine("Failed Paranthesis Tests:");

                // (5+5)  SIMPLE PARANTHESIS
                try
                {
                    Evaluator.Evaluate("(5+5)", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // ((5+5))  DOUBLE PARANTHESIS
                try
                {
                    Evaluator.Evaluate("((5+5))", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // ((5+5)*5)  PARANTHESIS WITH ADDITION AND MULTIPLICATION AFTER
                try
                {
                    Evaluator.Evaluate("((5+5)*5)", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

                // (5*(5+5))  PARANTHESIS WITH ADDITION AND MULTIPLICATION BEFORE
                try
                {
                    Evaluator.Evaluate("(5*(5+5))", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("invalid syntax");
                }

            // ARGUMENTEXCEPTION TESTS:

                Console.WriteLine("THERE SHOULD BE 11 ERRORS BELOW:");

                // 5/0  DIVIDE BY ZERO
                try
                {
                    int val = Evaluator.Evaluate("5/0", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("error #1");
                }

                // 5+  UNBALANCED ADDITION
                try
                {
                    int val = Evaluator.Evaluate("5+", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("error #2");
                }

                // +5  UNBALANCED ADDITION pt.2
                try
                {
                    int val = Evaluator.Evaluate("+5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("error #3");
                }

                // 5-  UNBALANCED SUBTRACTION
                try
                {
                    int val = Evaluator.Evaluate("5-", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("error #4");
                }

                // -5  UNBALANCED SUBTRACTION pt.2
                try
                {
                    int val = Evaluator.Evaluate("-5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("error #5");
                }

                // 5*  UNBALANCED MULTIPLICATION
                try
                {
                    int val = Evaluator.Evaluate("5*", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("error #6");
                }

                // *5  UNBALANCED MULTIPLICATION pt.2
                try
                {
                    int val = Evaluator.Evaluate("*5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("error #7");
                }

                // 5/  UNBALANCED DIVISION
                try
                {
                    int val = Evaluator.Evaluate("5/", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("error #8");
                }

                // /5  UNBALANCED ADDITION pt.2
                try
                {
                    int val = Evaluator.Evaluate("/5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("error #9");
                }

                // (5  UNBALANCED PARANTHESIS
                try
                {
                    int val = Evaluator.Evaluate("(5", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("error #10");
                }

                // 5)  UNBALANCED PARANTHESIS pt.2
                try
                {
                    int val = Evaluator.Evaluate("5)", VariableLookup);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("error #11");
                }
        }
    }
}