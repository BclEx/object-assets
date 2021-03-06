using OA.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace OA.Ultima.Core
{
    [SuppressUnmanagedCodeSecurity]
    static class ConsoleManager
    {
        const string Kernel32_DllName = "kernel32.dll";
        static readonly Stack<ConsoleColor> _consoleColors = new Stack<ConsoleColor>();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        public static void PushColor(ConsoleColor color)
        {
            try
            {
                _consoleColors.Push(Console.ForegroundColor);
                Console.ForegroundColor = color;
            }
            catch { }
        }

        public static ConsoleColor PopColor()
        {
            try
            {
                Console.ForegroundColor = _consoleColors.Pop();
            }
            catch { }
            return Console.ForegroundColor;
        }

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        public static void Show()
        {
            if (!HasConsole)
            {
#if DEBUG
                Utils.Info("Console started in Debug mode");
                AllocConsole();
                InvalidateOutAndError();
#else
                Utils.Info("Cannot show console when project is built in Release mode.");
#endif
            }
        }

        public static void Hide()
        {
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
        }

        public static void Toggle()
        {
            if (HasConsole) Hide();
            else Show();
        }

        private static void InvalidateOutAndError()
        {
            var type = typeof(Console);
            var output = type.GetField("_out", BindingFlags.Static | BindingFlags.NonPublic);
            var error = type.GetField("_error", BindingFlags.Static | BindingFlags.NonPublic);
            var initializeStdOutError = type.GetMethod("InitializeStdOutError", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert(output != null);
            Debug.Assert(error != null);
            Debug.Assert(initializeStdOutError != null);
            output.SetValue(null, null);
            error.SetValue(null, null);
            initializeStdOutError.Invoke(null, new object[] { true });
        }

        static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
}