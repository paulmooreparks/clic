using System.Collections.Generic;

namespace Clic;

internal static class StackExtensions {
    public static void PushAll<T>(this Stack<T> stack, IEnumerable<T> values) {
        foreach (var value in values) {
            stack.Push(value);
        }
    }
}

