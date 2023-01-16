namespace FormulaEvaluator;

public class FormulaEvaluatorTester
{
    static void Main(string[] args)
    {
        Console.WriteLine("Correct Syntax:");

        try
        {
            Evaluator.Evaluate("5+5", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("5-5", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("5*5", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("5/5", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("5+5*2", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("2*5+5", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("(5+5)/2", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("20/(5+5)", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("(10/5)-1", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("10-(10/5)", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("5+5*5/5-5", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("5*5+5/5-5", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        Console.WriteLine("");
        Console.WriteLine("Incorrect Syntax:");

        try
        {
            Evaluator.Evaluate("*5", null);
        } 
        catch (ArgumentException) 
        {
            Console.WriteLine("invalid syntax");
        }

        try 
        { 
            Evaluator.Evaluate("10/0", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("10++3", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Evaluator.Evaluate("15/3+3-1/0", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }

        try
        {
            Console.WriteLine(Evaluator.Evaluate("((3+2)*4", null));
        }
        catch (ArgumentException)
        {
            Console.WriteLine("invalid syntax");
        }
    }
}