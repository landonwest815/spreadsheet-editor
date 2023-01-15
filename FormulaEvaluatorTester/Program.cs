namespace FormulaEvaluator;

public class FormulaEvaluatorTester
{
    static void Main(string[] args)
    {
        // Display the number of command line arguments.
        Console.WriteLine(Evaluator.Evaluate("(6*(3+4))", null));
    }
}