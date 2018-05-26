using System;
using System.Windows;
using System.Windows.Forms;

namespace DotaInstaller
{
    public static class WinFormsCompatibility
    {
        /// <summary>
        ///     Gets a handle of the given <paramref name="window"/> and wraps it into <see cref="IWin32Window"/>,
        ///     so it can be consumed by WinForms code, such as <see cref="FolderBrowserDialog"/>.
        /// </summary>
        /// <param name="window">
        ///     The WPF window whose handle to get.
        /// </param>
        /// <returns>
        ///     The handle of <paramref name="window"/> is returned as <see cref="IWin32Window.Handle"/>.
        /// </returns>
        public static IWin32Window GetIWin32Window(this Window window)
        {
            return new Win32Window(new System.Windows.Interop.WindowInteropHelper(window).Handle);
        }

        /// <summary>
        ///     Implementation detail of <see cref="GetIWin32Window"/>.
        /// </summary>
        private class Win32Window : IWin32Window
        { // NOTE: This is System.Windows.Forms.IWin32Window, not System.Windows.Interop.IWin32Window!
            public Win32Window(IntPtr handle)
            {
                Handle = handle; // C# 6 "read-only" automatic property.
            }

            public IntPtr Handle { get; }
        }
    }
}