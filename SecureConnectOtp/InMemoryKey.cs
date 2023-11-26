using System;
using System.Security.Cryptography;

namespace SecureConnectOtp;

public class InMemoryKey : IKeyProvider
{
    private readonly object _stateSync = new object();
    private readonly byte[] _keyData;
    private readonly int _keyLength;

    /// <summary>
    /// Creates an instance of a key.
    /// </summary>
    /// <param name="key">Plaintext key data</param>
    public InMemoryKey(byte[] key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (key.Length <= 0)
        {
            throw new ArgumentException("The key must not be empty");
        }

        _keyLength = key.Length;
        var paddedKeyLength = (int)Math.Ceiling((decimal)key.Length / (decimal)16) * 16;
        _keyData = new byte[paddedKeyLength];
        Array.Copy(key, _keyData, key.Length);
    }

    /// <summary>
    /// Gets a copy of the plaintext key
    /// </summary>
    /// <remarks>
    /// This is internal rather than protected so that the tests can use this method
    /// </remarks>
    /// <returns>Plaintext Key</returns>
    internal byte[] GetCopyOfKey()
    {
        var plainKey = new byte[_keyLength];
        lock (_stateSync)
        {
            Array.Copy(_keyData, plainKey, _keyLength);
        }
        return plainKey;
    }

    /// <summary>
    /// Uses the key to get an HMAC using the specified algorithm and data
    /// </summary>
    /// <param name="mode">The HMAC algorithm to use</param>
    /// <param name="data">The data used to compute the HMAC</param>
    /// <returns>HMAC of the key and data</returns>
    public byte[] ComputeHmac(OtpHashMode mode, byte[] data)
    {
        byte[] hashedValue;
        using (var hmac = CreateHmacHash(mode))
        {
            var key = GetCopyOfKey();
            try
            {
                hmac.Key = key;
                hashedValue = hmac.ComputeHash(data);
            }
            finally
            {
                KeyUtilities.Destroy(key);
            }
        }

        return hashedValue;
    }

    /// <summary>
    /// Create an HMAC object for the specified algorithm
    /// </summary>
    private static HMAC CreateHmacHash(OtpHashMode otpHashMode)
    {
        HMAC hmacAlgorithm;
        switch (otpHashMode)
        {
            case OtpHashMode.Sha256:
                hmacAlgorithm = new HMACSHA256();
                break;
            case OtpHashMode.Sha512:
                hmacAlgorithm = new HMACSHA512();
                break;
            // case OtpHashMode.Sha1:
            default:
                hmacAlgorithm = new HMACSHA1();
                break;
        }
        return hmacAlgorithm;
    }
}