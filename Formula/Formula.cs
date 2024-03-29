﻿// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    /// <param name="data"> the input formula </param>

    public class Formula
    {
        string data;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        /// <param name="formula"> the input formula </param>
        public Formula(String formula) : this(formula, s => s, s => true)
        {
            // Checks if the provided formula contains valid syntax and truncates doubles at the same time
            data = IsValidSyntax(formula);
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        /// <param name="formula"> the input formula </param>
        /// <param name="normalize"> a delegate that normalizes variables </param>
        /// <param name="isValid"> a delegate that checks if a variable is valid in the current program context </param>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            // Checks if the provided formula contains valid syntax and truncates doubles at the same time
            data = IsValidSyntax(formula);

            // Normalizes the data
            data = normalize(data);

            // Checks for valid variables within the formula
            List<string> formulaVariables = GetVariables().ToList();
            foreach (var variable in formulaVariables)
            {
                if (!isValid(variable)) throw new FormulaFormatException("Error: Invalid Variables exist in formula");
            }
        }

        /// <summary>
        /// This helper method iterates through the tokens in a formula and makes sure they pass the requirements of a syntactically valid expression
        /// </summary>
        /// <param name="formula"> the formula being checked for syntactical errors </param>
        /// <exception cref="FormulaFormatException"> throws if any errors are found </exception>
        private string IsValidSyntax(string formula)
        {
            // Each token is added to the end of this string after being processed
            // This will help to make number checking in the equals method easy
            string checkedFormula = "";

            // List of tokens to process
            List<String> tokens = GetTokens(formula).ToList();

            // Paranthesis counters
            int leftpar = 0;
            int rightpar = 0;

            // Check if there is atleast 1 token
            if (tokens.Count < 1)
            {
                throw new FormulaFormatException("Error: There is not atleast 1 token");
            }

            // Check if first token is one of the following: OPENING PARANTHESIS, NUMBER, or VARIABLE
            if (!(tokens[0] == "(" || IsNumber(tokens[0]) || IsVariable(tokens[0])))
            {
                throw new FormulaFormatException("Error: First token is not a '(', number, or variable");
            }

            // Check if last token is one of the following: NUMBER, VARIABLE, or CLOSING PARANTHESIS
            if (!(IsNumber(tokens.Last()) || IsVariable(tokens.Last()) || tokens.Last() == ")"))
            {
                throw new FormulaFormatException("Error: Last token is not a number, variable, or ')'");
            }

            // Loops through the tokens and checks for the specified rules below
            for (int i = 0; i < tokens.Count; i++)
            {
                // Is the current token valid?
                if (!IsValidToken(tokens[i]))
                {
                    throw new FormulaFormatException("Error: Formula contains invalid token(s)");
                }

                // Ignore the first token for the following rules
                if (i > 0)
                {
                    // Was the previous token an OPENING PARANTHESIS or an OPERATOR?
                    if (tokens[i - 1] == "(" || IsOperator(tokens[i - 1]))
                    {
                        // If it was then is the current token a NUMBER, VARIABLE, or OPENING PARANTHESIS?
                        if (!(IsNumber(tokens[i]) || IsVariable(tokens[i]) || tokens[i] == "("))
                        {
                            throw new FormulaFormatException("Error: Either '(' or an operator is not followed by a number, variable, or '('");
                        }
                    }

                    // Was the previous token a NUMBER, VARIABLE, or CLOSING PARANTHESIS?
                    if (IsNumber(tokens[i - 1]) || IsVariable(tokens[i - 1]) || tokens[i - 1] == ")")
                    {
                        // If it was then is the current token a NUMBER or OPERATOR
                        if (!(tokens[i] == ")" || IsOperator(tokens[i])))
                        {
                            throw new FormulaFormatException("Error: Either a number, variable, or '(' is not followed by a '(' or operator");
                        }
                    }
                }

                // Updates paranthesis counters for each token being processed
                if (tokens[i] == "(") leftpar++;
                if (tokens[i] == ")") rightpar++;

                // Checks if rightpar counter is greater than the leftpar counter
                if (rightpar > leftpar)
                {
                    throw new FormulaFormatException("Error: There are more closing paranthesis than opening paranthesis at some point in the formula");
                }

                // If the current token is a NUMBER, normalize it before adding it to the checkedFormula string
                if (IsNumber(tokens[i]))
                {
                    checkedFormula = string.Concat(checkedFormula, Double.Parse(tokens[i]).ToString());
                }
                // If it isn't a NUMBER then simply add it on
                else
                {
                    checkedFormula = string.Concat(checkedFormula, tokens[i]);
                }
            }

            // Check if paranthesis counters match before returning checkedFormula
            if (leftpar != rightpar)
            {
                throw new FormulaFormatException("Error: There are not the same amount of opening and closing paranthesis");
            }

            return checkedFormula;
        }

        /// <summary>
        /// This helper method checks if a token is valid
        /// </summary>
        /// <param name="token"> the token being checked </param>
        /// <returns> bool value depending on if the token was valid </returns>
        private bool IsValidToken(String token)
        {
            // If the token is a VARIABLE, NUMBER, or OPERATOR then it is valid
            return (IsVariable(token) || IsNumber(token) || "()+-*/".Contains(token));
        }

        /// <summary>
        /// This helper method checks if a token is a number
        /// </summary>
        /// <param name="token"> he token being checked </param>
        /// <returns> bool value depending on whether the token was a number or not </returns>
        private static bool IsNumber(String token)
        {
            // If the token parses then it is a NUMBER
            return Double.TryParse(token, out double num);
        }

        /// <summary>
        /// This helper method checks if a token is a variable
        /// </summary>
        /// <param name="token"> the token being checked </param>
        /// <returns> bool value depending on whether the token was a variable or not </returns>
        private static bool IsVariable(String token)
        {
            // If the token matches the following regular expression then it is a VARIABLE
            return Regex.IsMatch(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*");
        }

        /// <summary>
        /// This helper method checks if a token is an operator
        /// </summary>
        /// <param name="token"> the token being checked </param>
        /// <returns> a bool value depending on whether the token was a operator or not </returns>
        private bool IsOperator(String token)
        {
            // If the token is one of the following characters then it is an OPERATOR
            return "+-*/".Contains(token);
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        /// <param name="lookup"> a delegate that checks the true value of a variable </param>
        /// <returns> an object that is either the evaluated formula as a Double or a FormulaError </returns>

        public object Evaluate(Func<string, double> lookup)
        {
            // Stacks for processing the expression in an arithmetic way
            Stack<double> values = new Stack<double>();
            Stack<String> operators = new Stack<String>();

            // Used for integer tokens
            double value = 0;

            // Splits the expresion into a List of tokens
            List<string> tokens = GetTokens(data).ToList();

            // Delegate for applying operands thorughout Evaluate
            var ApplyOperator = new Action(() =>
            {
                if (operators.Peek() == "*")
                {
                    values.Push(value * values.Pop());
                }
                else if (operators.Peek() == "/") 
                {
                    if (value == 0)
                        throw new ArgumentException();

                    values.Push(values.Pop() / value);
                }
                else if (operators.Peek() == "+")
                {
                    values.Push(values.Pop() + values.Pop());
                }
                else if (operators.Peek() == "-")
                {
                    double val1 = values.Pop();
                    double val2 = values.Pop();
                    values.Push(val2 - val1);
                }

                operators.Pop();
            });

            // Loops through every token one by one
            foreach (String t in tokens)
            {
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

                // If the current token is an integer or a variable...
                if (IsNumber(t) || IsVariable(t))
                {
                    // If it is a variable then lookup its value
                    if (IsVariable(t))
                    {
                        try
                        {
                            value = lookup(t);
                        }
                        // If the lookup is inconclusive then return a FormulaError
                        catch (Exception)
                        {
                            return new FormulaError("Error: Variable does not exist in the current context");
                        }
                    }
                    // If it's not a variable then parse the double into the value variable
                    else
                    {
                        value = double.Parse(t);
                    }

                    // If the top of the operators stack contains a '*'
                    if (topIsMultiply)
                    {
                        ApplyOperator(); // This will multiply the current value with the value on top of the stack
                    }

                    // If the top of the operators stack contains a '/'
                    else if (topIsDivide)
                    {
                        try
                        {
                            ApplyOperator(); // This will divide the top value on the stack with the current value
                        } 
                        catch (ArgumentException)
                        {
                            return new FormulaError("Error: Divison by zero encountered");
                        }
                    }

                    // If the top operator is neither '*' or '/' then push the current value onto the values stack
                    else
                    {
                        values.Push(value);
                    }
                }

                // If the current token is a '+' or '-'...
                else if (t == "+" || t == "-")
                {
                    // If the top of the operators stack contains a '+'
                    if (topIsAdd)
                    {
                        ApplyOperator(); // This will add the top two values on the values stack
                    }

                    // If the top of the operators stack contains a '-'
                    else if (topIsSubtract)
                    {
                        ApplyOperator(); // This will subtract the top two values on the value stack accordingly 
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
                        ApplyOperator(); // This will add the top two values on the values stack
                    }
                    // If the top operator is a '-'
                    else if (topIsSubtract) 
                    {
                        ApplyOperator(); // This will subtract the top two values on the value stack accordingly
                    }

                    // Pop the '('
                    operators.Pop();

                    // If the operator stack is not empty...
                    if (operators.Count != 0)
                    {
                        // If the top operator is a '*'...
                        // This cannot use the isTopMultiply variable because it is outside the for loop
                        if (operators.Peek() == "*")
                        {
                            // Pop the operators stack and multiply the top two values in the values stack
                            operators.Pop();
                            values.Push(values.Pop() * values.Pop());
                        }
                        // If the top operator is a '/'...
                        // This cannot use the isTopDivide variable because it is outside the for loop
                        else if (operators.Peek() == "/")
                        {
                            // Check if the value will be divided by zero
                            if (values.Peek() == 0)
                                return new FormulaError("Error: Divison by zero encountered");

                            // Pop the operators stack and divide the second highest value in the values stack by the top value
                            operators.Pop();
                            double val1 = values.Pop();
                            double val2 = values.Pop();
                            values.Push(val2 / val1);
                        }
                    }
                }
            }

            // If the operator stack is empty then return the remaining value
            if (operators.Count == 0)
            {
                return values.Pop();
            }
            // Special Cases
            else
            {
                // If the top operator is a '+'...
                if (operators.Peek() == "+")
                {
                    ApplyOperator(); // This will add the top two values on the value stack and then push it back onto the values stack
                    return values.Pop(); // This will return the resulting value from the line above
                }
                // If the top operator is a '-'...
                else if (operators.Peek() == "-")
                {
                    ApplyOperator(); // This will subtract the top two values on the value stack accordingly and then push it back onto the values stack
                    return values.Pop(); // This will return the resulting value from the line above
                }
                // If anything else goes wrong(It shouldn't)..
                else
                {
                    return new FormulaError("Error: Unknown");
                }
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            // List of tokens in the current formula
            List<String> tokens = GetTokens(data).ToList();

            // List for variables to be put into using a HashSet (no duplicates)
            HashSet<String> variables = new HashSet<String>();

            // Loops through tokens and adds them to the variables HashSet if they are indeed variables
            foreach (String token in tokens)
            {
                if (IsVariable(token))
                {
                    variables.Add(token); // If two 'X's are added only one will appear in the HashSet
                }
            }

            // Returns the HashSet as an IEnumerable
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return data; // The IsValidSyntax function above automatically formats the data variable for this function
        }

        /// <summary>
        ///  <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        /// <param name="obj"> the obj that the current function is being compared to </param>

        public override bool Equals(object? obj)
        {
            // Checks if obj is not a Formula or null
            if (obj == null || obj is not Formula) { return false; }

            // If it isn't then it creates a Formula from the obj
            Formula? objFormula = obj as Formula;

            // Compares the two Formulas and returns the bool value
            return this.data == objFormula.data;
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// </summary>
        /// <param name="f1"> the formula being compared to </param>
        /// <param name="f2"> the formula being compared with </param>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return f1.Equals(f2); // This simply calls the equals method
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
        ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// </summary>
        /// <param name="f1"> the formula being compared to </param>
        /// <param name="f2"> the formula being compared with </param>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1.Equals(f2)); // This also simply calls the equals method
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return data.GetHashCode(); // The IsValidSyntax function above formats formulas in a way that any equal formulas will have identically data variables
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        /// <param name="formula"> the formula the tokens are pulled from </param>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        public FormulaFormatException() { }

        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message) { }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"> The reason why this FormulaError was created. </param>
        public FormulaError(String reason) : this()
        { Reason = reason; }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason
        { get; private set; }
    }
}

// <change>
//   If you are using Extension methods to deal with common stack operations (e.g., checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
//   as long as it doesn't allow nulls!
// </change>