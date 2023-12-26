using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace FireBrowserBusinessCore.Helpers
{
    internal static class EncryptionHelpers
    {
        // Your additional entropy
        static byte[] s_additionalEntropy = { 9, 6, 4, 1, 5 };

        public static byte[] ProtectString(string str)
        {
            try
            {
                string callingAppName = GetCallingAppName();
                if (IsAllowedApp(callingAppName))
                {
                    var data = Encoding.UTF8.GetBytes(str);
                    return ProtectedData.Protect(data, s_additionalEntropy, DataProtectionScope.CurrentUser);
                }
                else
                {
                    throw new UnauthorizedAccessException($"Access denied for the calling application '{callingAppName}'.");
                }
            }
            catch (CryptographicException)
            {
                return null;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static string GetCallingAppName()
        {
            return Process.GetCurrentProcess().ProcessName;
        }

        private static bool IsAllowedApp(string appName)
        {
            return appName == nameof(FireBrowserWinUi3) || appName == ("FireVault");
        }

        public static string UnprotectToString(byte[] data)
        {
            try
            {
                string callingAppName = GetCallingAppName();
                if (IsAllowedApp(callingAppName))
                {
                    var unprotectedData = ProtectedData.Unprotect(data, s_additionalEntropy, DataProtectionScope.CurrentUser);
                    return Encoding.UTF8.GetString(unprotectedData);
                }
                else
                {
                    throw new UnauthorizedAccessException($"Access denied for the calling application '{callingAppName}'.");
                }
            }
            catch (CryptographicException)
            {
                return null;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}