﻿using ExtendCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static ExtendCSharp.Services.HookService;

namespace ExtendCSharp.Services
{
    static class HookService
    {
        public delegate int HookProcInterno(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProcInterno lpfn, IntPtr hInstance, IntPtr threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
    }



    public interface Hook
    {
        void Enable();
        bool Disable();

    }
    public class HookMouse : Hook, IService
    {
        public delegate void HookProcMouse(int nCode, MyMouseEvent wParam, MSLLHOOKSTRUCT lParam, ref bool Suppress);
        public event HookProcMouse EventDispatcher = null;
        private int Hook = 0;
        private HookProcInterno HPI;

        private int Proc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            bool Suppress = false;
            MSLLHOOKSTRUCT mouseLowLevelHook = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam,    typeof(MSLLHOOKSTRUCT));

            if (EventDispatcher != null)
                EventDispatcher(nCode, new MyMouseEvent((MouseMessagesInternal)wParam), mouseLowLevelHook, ref Suppress);

            return Suppress ? 1 : CallNextHookEx(Hook, nCode, wParam, lParam);
        }

        public void Enable()
        {
            if (Hook == 0)
            {
                HPI = new HookProcInterno(Proc);
                Hook = SetWindowsHookEx((int)HookTypes.WH_MOUSE_LL, HPI, (IntPtr)0, IntPtr.Zero);
            }
        }
        public void Enable(IntPtr ThreadProcessID)
        {
            if (Hook == 0)
            {
                HPI = new HookProcInterno(Proc);
                Hook = SetWindowsHookEx((int)HookTypes.WH_MOUSE, HPI, (IntPtr)0, ThreadProcessID);
            }
        }

        public bool Disable()
        {
            return HookService.UnhookWindowsHookEx(Hook);
        }
    }
    public class HookKeyboard : Hook, IService
    {
        public delegate void HookProcKeyboard(int nCode, MyKeyboardEvent wParam, Keys key, ref bool Suppress);
        public event HookProcKeyboard EventDispatcher = null;
        private int Hook = 0;
        private HookProcInterno HPI;

        private int Proc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //Log.Log.AddLog("nCode: " + nCode + " - wParam" + wParam + " - lParam" + lParam+" - marshal"+ Marshal.ReadInt32(lParam));
          
            bool Suppress = false;
            if (EventDispatcher != null)
                EventDispatcher(nCode, new MyKeyboardEvent((HookKeyStatusInternal)wParam),(Keys)Marshal.ReadInt32(lParam), ref Suppress);

            return Suppress ? 1 : CallNextHookEx(Hook, nCode, wParam, lParam);
        }

        public void Enable()
        {
            if (Hook == 0)
            {
                HPI = new HookProcInterno(Proc);
                Hook = SetWindowsHookEx((int)HookTypes.WH_KEYBOARD_LL, HPI, (IntPtr)0, IntPtr.Zero);
            }
        }
        public void Enable(IntPtr ThreadProcessID)
        {
            if (Hook == 0)
            {
                HPI = new HookProcInterno(Proc);
                Hook = SetWindowsHookEx((int)HookTypes.WH_KEYBOARD, HPI, (IntPtr)0, ThreadProcessID);
            }
        }
        public bool Disable()
        {
            return HookService.UnhookWindowsHookEx(Hook);
        }
    }



    public class HookManager : IService
    {
        public List<Hook> Hooks;
        public HookManager()
        {
            Hooks = new List<Hook>();
        }
        public HookManager(params Hook[] Hooks)
        {
            this.Hooks = new List<Hook>();
            if (Hooks != null)
                this.Hooks.AddRange(Hooks);
        }


        public void EnableAll()
        {
            Hooks.ForEach(a => a.Enable());
        }
        public void DisableAll()
        {
            Hooks.ForEach(a => a.Disable());
        }
    }


    public enum HookTypes
    {
        WH_MOUSE_LL = 14,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE=7,
        WH_KEYBOARD=2,
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X; 
        public int Y;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {

        public POINT Point;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public IntPtr DWExtraInfo;
    }
}
