using System;
using System.Reflection;
using System.Reflection.Emit;
using MelonLoader;

namespace MQOD
{
    public static class ILGeneratorEx
    {
        public static void EmitLog(this ILGenerator il, OpCode opCode)
        {
            MelonLogger.Msg($"{il.ILOffset:X} {opCode}");
            il.Emit(opCode);
        }
        
        public static void EmitLog(this ILGenerator il, OpCode opCode, Label label)
        {
            MelonLogger.Msg($"{il.ILOffset:X} {opCode}");
            il.Emit(opCode,label);
        }
        
        
        public static void EmitLog(this ILGenerator il, OpCode opCode, long obj)
        {
            MelonLogger.Msg($"{il.ILOffset:X} {opCode}");
            il.Emit(opCode,obj);
        }
        public static void EmitLog(this ILGenerator il, OpCode opCode, int obj)
        {
            MelonLogger.Msg($"{il.ILOffset:X} {opCode}");
            il.Emit(opCode,obj);
        }
        
        public static void EmitLogCall(this ILGenerator il, OpCode opCode, MethodInfo methodInfo, Type[] opts)
        {
            MelonLogger.Msg($"{il.ILOffset:X} {opCode}");
            il.EmitCall(opCode,methodInfo, opts);
        }
    }
}