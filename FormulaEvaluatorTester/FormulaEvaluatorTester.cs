
namespace FormulaEvaluator;

public class VariableTester
{
    public static int OtherLookup(string v)
    {
        if (v == "A2")
            return 2;
        else if (v == "B2")
            return 4;
        else if (v == "C2")
            return 6;
        else
            throw new ArgumentException();
    }


    public class FormulaEvaluatorTester
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Correct Syntax:");

            try
            {
                int val = Evaluator.Evaluate("5+5", null);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("invalid syntax 1");
            }

            try
            {
                Evaluator.Evaluate("5-5", null);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("invalid syntax 2");
            }

            try
            {
                Evaluator.Evaluate("5*5", null);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("invalid syntax 3");
            }

            try
            {
                Evaluator.Evaluate("5/5", null);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("invalid syntax 4");
            }

            try
            {
                Evaluator.Evaluate("5+5*2", null);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("invalid syntax 5");
            }

            try
            {
                Evaluator.Evaluate("2*5+5", null);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("invalid syntax 6");
            }

            try
            {
                Evaluator.Evaluate("(5+5)/2", null);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("invalid syntax 7");
            }

            try
            {
                Evaluator.Evaluate("20/(5+5)", null);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("invalid syntax 8");
            }

            try
            {
                Evaluator.Evaluate("(10/5)-1", null);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("invalid syntax 9");
            }

            try
            {
                Evaluator.Evaluate("10-(10/5)", null);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("invalid syntax 10");
            }

            try
            {
                Evaluator.Evaluate("5+5*5/5-5", null);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("invalid syntax 11");
            }

            try
            {
                Evaluator.Evaluate("5*5+5/5-5", null);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("invalid syntax 12");
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
}