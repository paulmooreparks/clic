using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clic;

internal static class Constants {
    static readonly Dictionary<string, double> _constants = new() {
        { "e", Math.E },
        { "pi", Math.PI },
        { "π", Math.PI },
        { "tau", Math.Tau },
        { "τ", Math.Tau }
    };

    static Constants() { }

    internal static bool TryGetConstant(string name, out double value) {
        return _constants.TryGetValue(name, out value);
    }
}
