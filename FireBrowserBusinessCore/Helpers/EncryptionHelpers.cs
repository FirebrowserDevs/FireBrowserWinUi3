using System.Security.Cryptography;
using System.Text;

namespace FireBrowserBusinessCore.Helpers
{
    internal static class EncryptionHelpers
    {
        static byte[] s_additionalEntropy = { 9, 6, 4, 1, 5 };

        public static byte[] ProtectString(string str)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(str);
                return ProtectedData.Protect(data, s_additionalEntropy, DataProtectionScope.CurrentUser);
            }
            catch (CryptographicException)
            {
                return null;
            }
        }

        public static string UnprotectToString(byte[] data)
        {
            try
            {
                var unprotectedData = ProtectedData.Unprotect(data, s_additionalEntropy, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(unprotectedData);
            }
            catch (CryptographicException)
            {
                return null;
            }
        }
    }
}
