using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace KeepAwake
{
    public class WindowManager
    {
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public void FillWindowTitles(CheckedListBox checkedListBox)
        {
            checkedListBox.Items.Clear();
            EnumWindows(new EnumWindowsProc((hWnd, lParam) =>
            {
                StringBuilder sb = new StringBuilder(256);
                if (IsWindowVisible(hWnd) && GetWindowText(hWnd, sb, sb.Capacity) > 0)
                {
                    checkedListBox.Items.Add(sb.ToString(), false);
                }
                return true;
            }), IntPtr.Zero);
        }

        public List<WindowState> GetWindowStates(CheckedListBox checkedListBox)
        {
            List<WindowState> windowStates = new List<WindowState>();
            foreach (var item in checkedListBox.Items)
            {
                var windowState = new WindowState
                {
                    Title = item.ToString(),
                    IsChecked = checkedListBox.CheckedItems.Contains(item)
                };
                windowStates.Add(windowState);
            }
            return windowStates;
        }

        public void LoadWindowStates(CheckedListBox checkedListBox, List<WindowState> windowStates)
        {
            checkedListBox.Items.Clear();
            foreach (var windowState in windowStates)
            {
                checkedListBox.Items.Add(windowState.Title, windowState.IsChecked);
            }
        }
    }
}
