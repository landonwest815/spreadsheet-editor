// Skeleton written by Joe Zachary for CS 3500, September 2013
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
  public class Formula
  {
        string allowableTokens = "()+-*/";
        string inputFormula;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) : this(formula, s => s, s => true)
    {
            inputFormula = formula;
            IsValidSyntax(inputFormula);
            // parse through the formula
            // check that it is valid
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
    public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
    {
            inputFormula = formula;
            isValid(inputFormula);
            // normalize variables
            // check if it is syntacically valid
            // check if the variables are valid
    }

        private void IsValidSyntax(String formula)
        {
            List<String> tokens = GetTokens(formula).ToList();
            int leftpar = 0;
            int rightpar = 0;

            // Check if there is atleast 1 token
            if (tokens.Count < 1)
            {
                throw new FormulaFormatException("There is not atleast 1 token");
            }

            // Check if first token is one of the following: opening paranthesis, number, or variable
            if (!(tokens[0] == "(" || IsNumber(tokens[0]) || IsVariable(tokens[0])))
            {
                throw new FormulaFormatException("First token is not a '(', number, or variable");
            }

            //Check if last token is one of the following: number, variable, or closing paranthesis
            if (!(IsNumber(tokens.Last()) || IsVariable(tokens.Last()) || tokens.Last() == ")"))
            {
                throw new FormulaFormatException("Last token is not a number, variable, or ')'");
            }

            for (int i = 0; i < tokens.Count; i++)
            {
                // Checks if the current token is valid
                if (!IsValidToken(tokens[i]))
                {
                    throw new FormulaFormatException("Formula contains invalid token");
                }

                // Checks if the previous token was an opening paranthesis or an operator
                if (i > 0)
                {
                    if (tokens[i - 1] == "(" || IsOperator(tokens[i - 1]))
                    {
                        // If it was then the current token must be a number, variable, or opening paranthesis
                        if (!(IsNumber(tokens[i]) || IsVariable(tokens[i]) || tokens[i] == "("))
                        {
                            throw new FormulaFormatException("'(' or an operator is not followed by a number, variable, or '('");
                        }
                    }

                    // Checks if the previous token was a number, variable, or opening paranthesis
                    if (IsNumber(tokens[i - 1]) || IsVariable(tokens[i - 1]) || tokens[i - 1] == "(")
                    {
                        // If it was then the current token must be a number or an operator
                        if (!(tokens[i] == "(" || IsOperator(tokens[i])))
                        {
                            throw new FormulaFormatException("A number, variable, or '(' is not followed by a '(' or an operator");
                        }
                    }
                }

                // Updates paranthesis counters
                if (tokens[i] == "(") leftpar++;
                if (tokens[i] == ")") rightpar++;

                // Checks if rightpar is greater than leftpar
                if (rightpar > leftpar)
                {
                    throw new FormulaFormatException("Closing paranthesis exceed opening paranthesis at some point in the formula");
                }   
            }

            // Check if paranthesis counters match
            if (leftpar != rightpar) 
            {
                throw new FormulaFormatException("Unbalanced paranthesis");
            }
        }

    private bool IsValidToken(String token)
        {
            // If the token is a number
            if (IsNumber(token))
            {
                return true;
            }

            // If the token is a variable
            if (IsVariable(token))
            {
                return true;
            }

            // If the token is any of the allowable tokens
            if (allowableTokens.Contains(token))
            {
                return true;
            }

            // If none of the above cases yield true
            return false;
        } 

    private static bool IsNumber(String token)
        {
            if (Double.TryParse(token, out double num))
            { return true; }
            return false;
        }

    private static bool IsVariable(String token)
        {
            if (Regex.IsMatch(token, "[a-zA-Z]+[0-9]+"))
            { return true; }
            return false;
        }

    private bool IsOperator(String token)
        {
            if (allowableTokens.Contains(token))
            { return true; }
            return false;
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
    public object Evaluate(Func<string, double> lookup)
    {
            // Stacks for processing the expression in an arithmetic way
            Stack<double> values = new Stack<double>();
            Stack<String> operators = new Stack<String>();

            // Removes whitespace from the input expression
            String no_whitespace_expression = String.Concat(inputFormula.Where(c => !Char.IsWhiteSpace(c)));

            // Splits the expresion into an String array of tokens
            string[] tokens = Regex.Split(no_whitespace_expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            // Loops through every token one by one
            foreach (String t in tokens)
            {

                // Initial Setup:

                // Used for integer tokens
                double value;

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
                if (Double.TryParse(t, out value) || Regex.IsMatch(t, "[a-zA-Z]+[0-9]+"))
                {
                    if (Regex.IsMatch(t, "[a-zA-Z]+[0-9]+"))
                        value = lookup(t);

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
                        double val1 = values.Pop();
                        double val2 = values.Pop();
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
                        double val1 = values.Pop();
                        double val2 = values.Pop();
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
                            double val1 = values.Pop();
                            double val2 = values.Pop();
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
                    double val1 = values.Pop();
                    double val2 = values.Pop();
                    return val2 - val1;
                }
                // If anything else goes wrong...
                else
                {
                    throw new ArgumentException();
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
      return null;
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
      return null;
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
    public override bool Equals(object? obj)
    {
      return false;
    }

    /// <summary>
    ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
    /// Reports whether f1 == f2, using the notion of equality from the Equals method.
    /// 
    /// </summary>
    public static bool operator ==(Formula f1, Formula f2)
    {
      return false;
    }

    /// <summary>
    ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
    ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
    ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
    /// </summary>
    public static bool operator !=(Formula f1, Formula f2)
    {
      return false;
    }

    /// <summary>
    /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
    /// randomly-generated unequal Formulae have the same hash code should be extremely small.
    /// </summary>
    public override int GetHashCode()
    {
      return 0;
    }

    /// <summary>
    /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
    /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
    /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
    /// match one of those patterns.  There are no empty tokens, and no token contains white space.
    /// </summary>
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
        public FormulaFormatException()
        {
        }

        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
        : base(message)
    {
    }
  }

  /// <summary>
  /// Used as a possible return value of the Formula.Evaluate method.
  /// </summary>
  public struct FormulaError
  {
    /// <summary>
    /// Constructs a FormulaError containing the explanatory reason.
    /// </summary>
    /// <param name="reason"></param>
    public FormulaError(String reason)
        : this()
    {
      Reason = reason;
    }

    /// <summary>
    ///  The reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
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
