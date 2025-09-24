using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ParametricTool.Utils
{
    public static class WindowUtil
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        public static void Ex_Show(this Window window)
        {
            //设置窗口的父项
            //得到当前窗口的指针
            IntPtr acwnd = GetActiveWindow();
            new System.Windows.Interop.WindowInteropHelper(window).Owner = acwnd;
            //判断窗口的状态
            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = WindowState.Normal;
            }
            //窗口显示
            window.Show();
        }
    }
}
