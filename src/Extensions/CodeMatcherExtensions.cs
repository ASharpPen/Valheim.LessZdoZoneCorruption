using System.Diagnostics;
using System.Reflection.Emit;
using HarmonyLib;

namespace LessZdoCorruption.Extensions;

internal static class CodeMatcherExtensions
{
    public static CodeMatcher InsertAndAdvance(this CodeMatcher matcher, OpCode code) =>
        matcher.InsertAndAdvance(new CodeInstruction(code));

    public static CodeMatcher MatchForward(this CodeMatcher matcher, bool useEnd, params OpCode[] matches) =>
        matcher.MatchForward(
            useEnd,
            matches.Select(x => new CodeMatch(x)).ToArray());


    public static CodeMatcher GetOpcode(this CodeMatcher codeMatcher, out OpCode opcode)
    {
        opcode = codeMatcher.Opcode;
        return codeMatcher;
    }

    public static CodeMatcher GetInstruction(this CodeMatcher codeMatcher, out CodeInstruction instruction)
    {
        instruction = codeMatcher.Instruction;
        return codeMatcher;
    }

    public static CodeMatcher GetOperand(this CodeMatcher codeMatcher, out object operand)
    {
        operand = codeMatcher.Operand;
        return codeMatcher;
    }

    public static CodeMatcher Print(this CodeMatcher codeMatcher, int before, int after)
    {
#if DEBUG
        for (int i = -before; i <= after; ++i)
        {
            int currentOffset = i;
            int index = codeMatcher.Pos + currentOffset;

            if (index <= 0)
            {
                continue;
            }

            if (index >= codeMatcher.Length)
            {
                break;
            }

            try
            {
                var line = codeMatcher.InstructionAt(currentOffset);
                Log.Trace($"[{currentOffset}] " + line.ToString());
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
#endif
        return codeMatcher;
    }
}
