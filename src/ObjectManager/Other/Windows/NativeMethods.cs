﻿using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace OA.Core.Windows
{
    public delegate IntPtr WndProcHandler(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    class NativeMethods
    {
//#if WINDOWS
//        [DllImport("Kernel32")]
//        unsafe static extern int _lread(SafeFileHandle hFile, void* lpBuffer, int wBytes);
//        internal static unsafe void ReadBuffer(FileStream stream, byte[] buffer, int length)
//        {
//            fixed (byte* ptrBuffer = buffer)
//            {
//                _lread(stream.SafeFileHandle, ptrBuffer, length);
//            }
//        }
//#else
//        internal static void ReadBuffer(FileStream stream, byte[] buffer, int length)
//        {
//            stream.Read(buffer, 0, length);
//        }
//#endif

        [DllImport("Imm32.dll")]
        internal static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("Imm32.dll")]
        internal static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("user32.dll")]
        internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        internal static extern int MultiByteToWideChar(int CodePage, int dwFlags, byte[] lpMultiByteStr, int cchMultiByte, char[] lpWideCharStr, int cchWideChar);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern short GetKeyState(int keyCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr SetWindowsHookEx(int hookType, WndProcHandler callback, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetMessage(out Message lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "TranslateMessage")]
        internal static extern bool TranslateMessage(ref Message m);

        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr window, IntPtr module);

        internal static int LOWORD(IntPtr val)
        {
            return (unchecked((int)(long)val)) & 0xFFFF;
        }

        internal static int MAKELCID(int languageID, int sortID)
        {
            return ((0xFFFF & languageID) | (((0x000F) & sortID) << 16));
        }

        internal static int MAKELANGID(int primaryLang, int subLang)
        {
            return ((((ushort)(subLang)) << 10) | (ushort)(primaryLang));
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int GetLocaleInfo(int locale, int lcType, out uint lpLCData, int cchData);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetKeyboardLayout(uint idThread);

        internal static uint GetCurrentCodePage()
        {
            // Get the keyboard layout for the current thread.
            IntPtr keybdLayout = GetKeyboardLayout(0);
            // Extract the language ID from it, contained in its low-order word.
            int langID = LOWORD(keybdLayout);
            // Call the GetLocaleInfo function to retrieve the default ANSI code page associated with that language ID.
            int localeID = MAKELCID(langID, NativeConstants.SORT_DEFAULT);
            int localeConstraints = NativeConstants.LOCALE_IDEFAULTANSICODEPAGE | NativeConstants.LOCALE_RETURN_NUMBER;
            uint codePage = 0;
            GetLocaleInfo(localeID, localeConstraints, out codePage, Marshal.SizeOf(codePage));
            return codePage;
        }
    }
}
