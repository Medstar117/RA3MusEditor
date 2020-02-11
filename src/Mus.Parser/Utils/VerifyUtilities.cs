using System;
using System.IO;

namespace Mus.Parser.Utils
{
    public static class VerifyUtilities
    {
        public static void EnsureEqual<T>(string name, string nameOfActual, T actual, T expected)
        {
            Ensure(name, "==", (x, y) => x.Equals(y), nameOfActual, actual, expected);
        }

        public static void Ensure<T>(string name, string nameOfExpect, Func<T, T, bool> expect, string nameOfActual, T actual, T expected)
        {
            if(!expect(actual, expected))
            {
                throw new InvalidDataException($"{name}: {nameOfActual}({actual}) not {nameOfExpect} {expected}");
            }
        }
    }
}