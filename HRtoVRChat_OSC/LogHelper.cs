using System.Diagnostics;
using System.Globalization;

namespace HRtoVRChat_OSC
{
    public static class LogHelper
    {
        private static string time => DateTime.Now.ToString(CultureInfo.CurrentCulture).Split(' ')[1];
        
        public static void Debug(object obj)
        {
            StackFrame frame = new StackFrame(1);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"[{time}] [{frame.GetMethod()?.DeclaringType}:{frame.GetMethod()}] (DEBUG): {obj}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Log(object obj, ConsoleColor color = ConsoleColor.White)
        {
            StackFrame frame = new StackFrame(1);
            Console.ForegroundColor = color;
            Console.WriteLine($"[{time}] [{frame.GetMethod()?.DeclaringType}] (LOG): {obj}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Warn(object obj)
        {
            StackFrame frame = new StackFrame(1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{time}] [{frame.GetMethod()?.DeclaringType}] (WARN): {obj}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Error(object obj, Exception e = null)
        {
            StackFrame frame = new StackFrame(1);
            Console.ForegroundColor = ConsoleColor.Red;
            object log = $"[{time}] [{frame.GetMethod()?.DeclaringType}:{frame.GetMethod()}] (ERROR): {obj}";
            if (e != null)
                log = $"{log} | Exception: {e}";
            Console.WriteLine(log);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}