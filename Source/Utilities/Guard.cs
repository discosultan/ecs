using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ECS.Utilities
{
    static class Guard
    {
        [DebuggerHidden]
        internal static void NotNull(object variable, string name, string message = null)
        {
            if (variable == null)
                throw new ArgumentNullException(name, message ?? "Null is not a valid value.");
        }

        [DebuggerHidden]
        internal static void True(bool expression, string message = null)
        {
            if (!expression)
                throw new InvalidOperationException(message ?? "Expression must be true.");
        }

        [DebuggerHidden]
        internal static void NotContainedIn<TKey, TValue>(TKey key, Dictionary<TKey, TValue> dictionary, string message = null)
        {
            if (dictionary.ContainsKey(key))
                throw new InvalidOperationException(message ?? "Duplicate values are not allowed.");
        }
    }
}
