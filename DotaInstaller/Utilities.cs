﻿using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Windows;

namespace DotaInstaller
{
    public static class Utilities
    {
        public static Version BuildFromString(string pVersion)
        {
            var parts = pVersion.Split('.').ToList().Select(Int16.Parse).ToList();
            return new Version(parts[0], parts[1], parts[2]);
        }

        public static bool CheckForAdmin()
        {
            using (WindowsIdentity iden = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(iden);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

        }

        public static void RegisterException()
        {
            AppDomain.CurrentDomain.UnhandledException += (object s, UnhandledExceptionEventArgs args) =>
            {
                using (var writer = new StreamWriter($@"{Directory.GetCurrentDirectory()}\Error.txt"))
                {
                    var excp = args.ExceptionObject as Exception;
#if !DEBUG
                    writer.WriteLine(
                        $"Message: {excp.Message}{Environment.NewLine}Stack Trace: {excp.StackTrace}{Environment.NewLine}");
#else
                    throw new Exception(excp.Message);
#endif
#pragma warning disable CS0162 // Unreachable code detected
                    MessageBox.Show($"An exception has occurred!{Environment.NewLine}Exception: {excp.Message}");
#pragma warning restore CS0162 // Unreachable code detected
                }
            };
        }
    }
}