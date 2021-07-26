// ***********************************
// ** KopiCloud DetectNET v1.0      **
// ***********************************
// ** Created: July 26, 2021        **
// ** Guillermo Musumeci            **
// ***********************************
// ** Updated: July 26, 2021        **
// ** Guillermo Musumeci            **
// ***********************************

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace DetectNET
{
    class Program
    {
        #region "Variables"
        private static int NETFrameworkCounter = 0;
        private static int NETCoreRuntimesCounter = 0;
        private static int NETCoreSDKsCounter = 0;
        #endregion

        #region "Main"
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(AppInfo.AppFullName);
            Console.ResetColor();
            Console.WriteLine("");

            Get1To45VersionFromRegistry();
            Get45PlusFromRegistry();
            if (NETFrameworkCounter == 0) Console.WriteLine("NO Versions of .NET Framework Installed");

            GetCoreRuntimes();
            if (NETCoreRuntimesCounter == 0) Console.WriteLine("NO Versions of .NET Core Runtime Installed");

            GetCoreSDKs();
            if (NETCoreSDKsCounter == 0) Console.WriteLine("NO Versions of .NET Core SDK Installed");
        }
        #endregion

        #region "Rewrite the .NET Version"
        private static void WriteVersion(string version, string spLevel = "")
        {
            version = version.Trim();
            if (string.IsNullOrEmpty(version))
                return;

            string spLevelString = "";
            if (!string.IsNullOrEmpty(spLevel))
                spLevelString = " SP" + spLevel;

            Console.WriteLine($".NET Framework {version}{spLevelString}");
        }
        #endregion

        #region "Get NET 45 or Later Versions From Registry"
        private static void Get45PlusFromRegistry()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

            using (RegistryKey ndpKey = Registry.LocalMachine.OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    NETFrameworkCounter++;
                    Console.WriteLine($".NET Framework {CheckFor45PlusVersion((int)ndpKey.GetValue("Release"))}");
                }
            }

            // Checking the version using >= enables forward compatibility.
            string CheckFor45PlusVersion(int releaseKey)
            {
                if (releaseKey >= 528040)
                    return "4.8";
                if (releaseKey >= 461808)
                    return "4.7.2";
                if (releaseKey >= 461308)
                    return "4.7.1";
                if (releaseKey >= 460798)
                    return "4.7";
                if (releaseKey >= 394802)
                    return "4.6.2";
                if (releaseKey >= 394254)
                    return "4.6.1";
                if (releaseKey >= 393295)
                    return "4.6";
                if (releaseKey >= 379893)
                    return "4.5.2";
                if (releaseKey >= 378675)
                    return "4.5.1";
                if (releaseKey >= 378389)
                    return "4.5";
                // This code should never execute. A non-null release key should mean
                // that 4.5 or later is installed.
                return "No 4.5 or later version detected";
            }
        }
        #endregion

        #region "Get NET 1 To 45 Versions From Registry"
        private static void Get1To45VersionFromRegistry()
        {
            // Opens the registry key for the .NET Framework entry.
            using (RegistryKey ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    // Skip .NET Framework 4.5 version information.
                    if (versionKeyName == "v4")
                    {
                        continue;
                    }

                    if (versionKeyName.StartsWith("v"))
                    {
                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                        // Get the .NET Framework version value.
                        string name = (string)versionKey.GetValue("Version", "");
                        // Get the service pack (SP) number.
                        string sp = versionKey.GetValue("SP", "").ToString();

                        // Get the installation flag, or an empty string if there is none.
                        NETFrameworkCounter++;
                        string install = versionKey.GetValue("Install", "").ToString();
                        if (string.IsNullOrEmpty(install)) // No install info; it must be in a child subkey.
                            WriteVersion(name);
                        else
                        {
                            if (!(string.IsNullOrEmpty(sp)) && install == "1")
                            {
                                WriteVersion(name, sp);
                            }
                        }
                        if (!string.IsNullOrEmpty(name))
                        {
                            continue;
                        }
                        foreach (string subKeyName in versionKey.GetSubKeyNames())
                        {
                            RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                            name = (string)subKey.GetValue("Version", "");
                            if (!string.IsNullOrEmpty(name))
                                sp = subKey.GetValue("SP", "").ToString();

                            install = subKey.GetValue("Install", "").ToString();
                            if (string.IsNullOrEmpty(install)) //No install info; it must be later.
                                WriteVersion(name);
                            else
                            {
                                if (!(string.IsNullOrEmpty(sp)) && install == "1")
                                {
                                    WriteVersion(name, sp);
                                }
                                else if (install == "1")
                                {
                                    WriteVersion(name);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region "Get .NET Core Runtimes"
        static void GetCoreRuntimes()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = "--list-runtimes";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                string standard_output;
                while ((standard_output = process.StandardOutput.ReadLine()) != null)
                {
                    if (standard_output.Contains("Microsoft.NETCore.App"))
                    {
                        NETCoreRuntimesCounter++;
                        Console.WriteLine(RemovePath(standard_output.Replace("Microsoft.NETCore.App", ".NET Core Runtime")));
                    }
                }
                process.WaitForExit();
            }
            catch
            {
            }
        }
        #endregion

        #region "Get .NET Core SDKs"
        static void GetCoreSDKs()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = "--list-sdks";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                string standard_output;
                while ((standard_output = process.StandardOutput.ReadLine()) != null)
                {
                    NETCoreSDKsCounter++;
                    Console.WriteLine(".NET Core SDK " + RemovePath(standard_output));
                }
                process.WaitForExit();
            }
            catch
            {
            }
        }
        #endregion

        #region "Remove Path From .NET Core Version"
        static string RemovePath(string CoreVersion)
        {
            string Result = "";
            int indexStart = CoreVersion.IndexOf('[');
            if (indexStart > 0)
                Result = CoreVersion.Substring(0, indexStart);
            else
                Result = CoreVersion;
            return Result;
        }
        #endregion
    }
}
