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
            MelonLogger.Msg($"IL_{il.ILOffset:X4} {opCode}");
            il.Emit(opCode);
        }
        
        public static void EmitLog(this ILGenerator il, OpCode opCode, Label label)
        {
            MelonLogger.Msg($"IL_{il.ILOffset:X4} {opCode}");
            il.Emit(opCode,label);
        }
        
        public static void EmitLog(this ILGenerator il, OpCode opCode, long obj)
        {
            MelonLogger.Msg($"IL_{il.ILOffset:X4} {opCode}");
            il.Emit(opCode,obj);
        }
        public static void EmitLog(this ILGenerator il, OpCode opCode, int obj)
        {
            MelonLogger.Msg($"IL_{il.ILOffset:X4} {opCode}");
            il.Emit(opCode,obj);
        }
        
        public static void EmitLogCall(this ILGenerator il, OpCode opCode, MethodInfo methodInfo, Type[] opts)
        {
            MelonLogger.Msg($"IL_{il.ILOffset:X4} {opCode}");
            il.EmitCall(opCode,methodInfo, opts);
        }
    }
}