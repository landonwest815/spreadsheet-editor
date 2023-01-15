namespace FormulaEvaluator;

public class FormulaEvaluatorTester
{
    static void Main(string[] args)
    {
        if (Evaluator.Evaluate("5+5", null) == 10) Console.WriteLine("Passed!");
        else Console.WriteLine("Failed!");

        if (Evaluator.Evaluate("5-5", null) == 0) Console.WriteLine("Passed!");
        else Console.WriteLine("Failed!");

        if (Evaluator.Evaluate("5*5", null) == 25) Console.WriteLine("Passed!");
        else Console.WriteLine("Failed!");

        if (Evaluator.Evaluate("5/5", null) == 1) Console.WriteLine("Passed!");
        else Console.WriteLine("Failed!");

        if (Evaluator.Evaluate("5+5*2", null) == 15) Console.WriteLine("Passed!");
        else Console.WriteLine("Failed!");

        if (Evaluator.Evaluate("2*5+5", null) == 15) Console.WriteLine("Passed!");
        else Console.WriteLine("Failed!");

        if (Evaluator.Evaluate("(5+5)/2", null) == 5) Console.WriteLine("Passed!");
        else Console.WriteLine("Failed!");

        if (Evaluator.Evaluate("20/(5+5)", null) == 2) Console.WriteLine("Passed!");
        else Console.WriteLine("Failed!");

        if (Evaluator.Evaluate("(10/5)-1", null) == 1) Console.WriteLine("Passed!");
        else Console.WriteLine("Failed!");

        if (Evaluator.Evaluate("10-(10/5)", null) == 8) Console.WriteLine("Passed!");
        else Console.WriteLine("Failed!");

        if (Evaluator.Evaluate("5+5*5/5-5", null) == 5) Console.WriteLine("Passed!");
        else Console.WriteLine("Failed!");

        if (Evaluator.Evaluate("5*5+5/5-5", null) == 21) Console.WriteLine("Passed!");
        else Console.WriteLine("Failed!");
    }
}