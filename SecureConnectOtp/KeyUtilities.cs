using System;

namespace SecureConnectOtp;

internal class KeyUtilities
{
    internal static void Destroy(byte[] sensitiveData)
    {
        if (sensitiveData == null)
        {
            throw new ArgumentNullException(nameof(sensitiveData));
        }
        new Random().NextBytes(sensitiveData);
    }

    /// <summary>
    /// converts a long into a big endian byte array.
    /// </summary>
    /// <remarks>
    /// RFC 4226 specifies big endian as the method for converting the counter to data to hash.
    /// </remarks>
    internal static byte[] GetBigEndianBytes(long input)
    {
        // Since .net uses little endian numbers, we need to reverse the byte order to get big endian.
        var data = BitConverter.GetBytes(input);
        Array.Reverse(data);
        return data;
    }

    /// <summary>
    /// converts an int into a big endian byte array.
    /// </summary>
    /// <remarks>
    /// RFC 4226 specifies big endian as the method for converting the counter to data to hash.
    /// </remarks>
    internal static byte[] GetBigEndianBytes(int input)
    {
        // Since .net uses little endian numbers, we need to reverse the byte order to get big endian.
        var data = BitConverter.GetBytes(input);
        Array.Reverse(data);
        return data;
    }
}