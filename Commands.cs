using Cliffer;

using System.CommandLine;
using System.CommandLine.Invocation;

namespace Clic;

[RootCommand("clic, the CLI Stack Calculator")]
internal class RootCommand {
    public async Task<int> Execute(Command command, IServiceProvider serviceProvider, InvocationContext context) {
        return await command.Repl(serviceProvider, context, new ClicReplContext());
    }
}

[Command("load", "Load stack and variables from persistent storage")]
internal class LoadCommand {
    public int Execute(Stack<double> stack, Dictionary<string,double> variables, PersistenceService persistenceService) {
        var newStack = persistenceService.LoadStack();
        stack.Clear();
        stack.PushAll(newStack);

        var newVariables = persistenceService.LoadVariables();
        variables.Clear();

        foreach (var kvp in newVariables) {
            variables[kvp.Key] = kvp.Value;
        }

        return Result.Success;
    }
}

[Command("save", "Save stack and variables to persistent storage")]
internal class SaveCommand {
    public int Execute(Stack<double> stack, Dictionary<string,double> variables, PersistenceService persistenceService) {
        persistenceService.SaveStack(stack);
        persistenceService.SaveVariables(variables);
        return Result.Success;
    }
}

[Command("push", "Push one or more numbers onto the stack")]
[Argument(typeof(IEnumerable<string>), "args", "Numbers or constants to push onto the stack", Cliffer.ArgumentArity.OneOrMore)]
internal class PushCommand {
    public int Execute(IEnumerable<string> args, Stack<double> stack) {
        if (args.ProcessArgs(stack.Push, (arg) =>  Console.Error.WriteLine($"Error: Invalid number or constant '{arg}'") )) {
            return Result.Success;
        }
        
        return Result.Error;
    }
}

[Command("pop", "Pop a number from the stack")]
[Argument(typeof(string), "variable", "An optional variable name to store the popped value", arity: Cliffer.ArgumentArity.ZeroOrOne)]
internal class PopCommand {
    public int Execute(string variable, Stack<double> stack) {
        if (!string.IsNullOrEmpty(variable) && !IsValidVariableName(variable)) {
            return Result.Error;
        }

        if (stack.TryPop(out var value)) {
            if (!string.IsNullOrEmpty(variable)) { // We already checked validity above
                var variables = Utility.GetService<Dictionary<string, double>>()!;
                variables[variable] = value;
            }

            Console.WriteLine(value);
            return Result.Success;
        }

        Console.Error.WriteLine("Error: Stack empty");
        return Result.Error;
    }

    internal bool IsValidVariableName(string variable) {
        if (string.IsNullOrEmpty(variable)) {
            Console.Error.WriteLine("Error: Variable name cannot be empty");
            return false;
        }

        if (char.IsDigit(variable[0])) {
            Console.Error.WriteLine("Error: Variable name cannot start with a digit");
            return false;
        }

        if (variable.Any(c => !char.IsLetterOrDigit(c))) {
            Console.Error.WriteLine("Error: Variable name must be alphanumeric");
            return false;
        }

        if (Constants.TryGetConstant(variable, out var _)) {
            Console.Error.WriteLine("Error: Variable name cannot be a constant name");
            return false;
        }

        return true;
    }
}

[Command("swap", "Swap the top two numbers on the stack")]
internal class SwapCommand {
    public int Execute(Stack<double> stack) {
        if (stack.Count > 1) {
            var a = stack.Pop();
            var b = stack.Pop();
            stack.Push(a);
            stack.Push(b);
            return Result.Success;
        }

        Console.Error.WriteLine("Error: Not enough items on stack");
        return Result.Error;
    }
}

[Command("dup", "Duplicate the top number on the stack")]
internal class DupCommand {
    public int Execute(Stack<double> stack) {
        if (stack.Any()) {
            var value = stack.Peek();
            stack.Push(value);
            return Result.Success;
        }

        Console.Error.WriteLine("Error: Stack is empty");
        return Result.Error;
    }
}

[Command("clear", "Clear the stack and/or variables")]
[Option(typeof(bool), "--stack", "Clear the stack", aliases: ["-s"])]
[Option(typeof(bool), "--vars", "Clear the list of variables", aliases: ["-v"])]
internal class ClearCommand {
    public int Execute(
        [OptionParam("--stack")] bool clearStack,
        [OptionParam("--vars")] bool clearVars,
        Stack<double> stackStorage, 
        Dictionary<string, double> variables
        ) {

        if (!clearStack && !clearVars) {
            Console.Error.WriteLine("Error: No action specified");
            return Result.Error;
        }

        if (clearStack) {
            stackStorage.Clear();
        }

        if (clearVars) {
            variables.Clear();
        }

        return Result.Success;
    }
}

[Command("del", "Delete a variable")]
[Argument(typeof(string), "variable", "The variable to delete")]
internal class DelCommand {
    public int Execute(string variable, Dictionary<string,double> variables) {
        if (string.IsNullOrEmpty(variable)) {
            Console.Error.WriteLine("Error: Variable name cannot be empty");
            return Result.Error;
        }
        
        if (variables.Remove(variable)) {
            return Result.Success;
        }

        Console.Error.WriteLine($"Error: Variable '{variable}' not found");
        return Result.Error;
    }

}

[Command("stack", "List all items on the stack, from top to bottom")]
internal class StackCommand {
    public int Execute(Stack<double> stack) {
        foreach (var item in stack) {
            Console.WriteLine(item);
        }

        return Result.Success;
    }
}

[Command("vars", "List all variables")]
internal class VarsCommand {
    public int Execute(Dictionary<string,double> variables) {
        foreach (var item in variables) {
            Console.WriteLine($"{item.Key} = {item.Value}");
        }

        return Result.Success;
    }
}

internal abstract class BinaryStackOperation {
    protected abstract double Operation(double a, double b);

    public int Execute(IEnumerable<string> args, Stack<double> stack) {
        if (args.Count() > 2) {
            Console.Error.WriteLine("Error: Too many arguments");
            return Result.Error;
        }

        if (args.ProcessArgs(stack.Push, (arg) => Console.Error.WriteLine($"Error: Invalid number or constant '{arg}'"))) {
            if (stack.Count > 1) {
                var term2 = stack.Pop();
                var term1 = stack.Pop();
                var result = Operation(term1, term2);
                stack.Push(result);
                Console.WriteLine(result);
                return Result.Success;
            }

            Console.Error.WriteLine("Error: Not enough items on stack");
            return Result.Error;
        }

        return Result.Error;
    }
}

internal abstract class UnaryStackOperation {
    protected abstract double Operation(double value);

    public int Execute(Stack<double> stack) {
        if (stack.Count > 1) {
            var term1 = stack.Pop();
            var result = Operation(term1);
            stack.Push(result);
            Console.WriteLine(result);
            return Result.Success;
        }

        Console.Error.WriteLine("Error: Stack empty");
        return Result.Error;
    }
}

internal abstract class PushOperation {
    public int Execute(Stack<double> stack) {
        stack.Push(Constant);
        Console.WriteLine(Constant);
        return Result.Success;
    }

    internal abstract double Constant { get; }
}

[Command("+", "Add the top two numbers on the stack")]
[Argument(typeof(IEnumerable<string>), "args", "Numbers to add", Cliffer.ArgumentArity.ZeroOrMore)]
internal class AddCommand : BinaryStackOperation {
    protected override double Operation(double a, double b) => a + b;
}

[Command("-", "Subtract the top number on the stack from the second number on the stack")]
[Argument(typeof(IEnumerable<string>), "args", "Numbers to subtract", Cliffer.ArgumentArity.ZeroOrMore)]
internal class SubtractCommand : BinaryStackOperation {
    protected override double Operation(double a, double b) => a - b;
}

[Command("*", "Multiply the top two numbers on the stack")]
[Argument(typeof(IEnumerable<string>), "args", "Numbers to multiply", Cliffer.ArgumentArity.ZeroOrMore)]
internal class MultiplyCommand : BinaryStackOperation {
    protected override double Operation(double a, double b) => a * b;
}

[Command("/", "Divide the second number on the stack by the top number on the stack")]
[Argument(typeof(IEnumerable<string>), "args", "Numbers to divide", Cliffer.ArgumentArity.ZeroOrMore)]
internal class DivideCommand : BinaryStackOperation {
    protected override double Operation(double a, double b) => a / b;
}

[Command("mod", "Calculate the result of the second number on the stack modulo the top number on the stack")]
[Argument(typeof(IEnumerable<string>), "args", "Numbers for which to calculate modulus", Cliffer.ArgumentArity.ZeroOrMore)]
internal class ModulusCommand : BinaryStackOperation {
    protected override double Operation(double a, double b) => a % b;
}

[Command("pow", "Raise the second number on the stack to the power of the top number on the stack")]
[Argument(typeof(IEnumerable<string>), "args", "Numbers for which to calculate power", Cliffer.ArgumentArity.ZeroOrMore)]
internal class PowerCommand : BinaryStackOperation {
    protected override double Operation(double a, double b) => Math.Pow(a, b);
}

[Command("sqrt", "Calculate the square root of the top number on the stack")]
internal class SquareRootCommand : UnaryStackOperation {
    protected override double Operation(double value) => Math.Sqrt(value);
}

[Command("log", "Calculate the natural logarithm of the top number on the stack")]
internal class LogarithmCommand : UnaryStackOperation {
    protected override double Operation(double value) => Math.Log(value);
}

[Command("abs", "Replace the top number on the stack with its absolute arg")]
internal class AbsoluteValueCommand : UnaryStackOperation {
    protected override double Operation(double value) => Math.Abs(value);
}

[Command("sin", "Calculate the sine of the top number on the stack")]
internal class SineCommand : UnaryStackOperation {
    protected override double Operation(double value) => Math.Sin(value);
}

[Command("cos", "Calculate the cosine of the top number on the stack")]
internal class CosineCommand : UnaryStackOperation {
    protected override double Operation(double value) => Math.Cos(value);
}

[Command("tan", "Calculate the tangent of the top number on the stack")]
internal class TangentCommand : UnaryStackOperation {
    protected override double Operation(double value) => Math.Tan(value);
}

internal static class Macros {
    [Macro("cube", "Cube the top number on the stack")]
    private static string cube => "pow 3";

    [Macro("neg", "Negate the top item on the stack")]
    private static string negate => "* -1";

    [Macro("peek", "See the top item on the stack")]
    private static string peek => "dup;pop";

    [Macro("rec", "Calculate the reciprocal of the top item on the stack")]
    private static string rec => "push 1;swap;/";

    [Macro("square", "Square the top number on the stack")]
    private static string square => "pow 2";

    [Macro("print", "Print the value of a variable or constant")]
    private static string print => "push {{[arg]::0}};pop";
}

