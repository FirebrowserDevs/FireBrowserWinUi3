using System;

namespace SecureConnectOtp;
public class Hotp : Otp
{
    private readonly int _hotpSize;

    /// <summary>
    /// Gets the number of diigts that the returning HOTP should have.
    /// </summary>
    public int HotpSize => _hotpSize;

    /// <summary>
    /// Create a HOTP instance
    /// </summary>
    /// <param name="secretKey">The secret key to use in HOTP calculations</param>
    /// <param name="mode">The hash mode to use</param>
    /// <param name="hotpSize">The number of digits that the returning HOTP should have.  The default is 6.</param>
    public Hotp(byte[] secretKey, OtpHashMode mode = OtpHashMode.Sha1, int hotpSize = 6)
        : base(secretKey, mode)
    {
        VerifyParameters(hotpSize);
        _hotpSize = hotpSize;
    }

    /// <summary>
    /// Create a HOTP instance
    /// </summary>
    /// <param name="key">The key to use in HOTP calculations</param>
    /// <param name="mode">The hash mode to use</param>
    /// <param name="hotpSize">The number of digits that the returning HOTP should have.  The default is 6.</param>
    public Hotp(IKeyProvider key, OtpHashMode mode = OtpHashMode.Sha1, int hotpSize = 6)
        : base(key, mode)
    {
        VerifyParameters(hotpSize);

        _hotpSize = hotpSize;
    }

    private static void VerifyParameters(int hotpSize)
    {
        if (hotpSize < 6)
        {
            throw new ArgumentOutOfRangeException(nameof(hotpSize));
        }
        if (hotpSize > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(hotpSize));
        }
    }

    /// <summary>
    /// Takes a counter and then computes a HOTP value
    /// </summary>
    /// <param name="timestamp">The timestamp to use for the HOTP calculation</param>
    /// <param name="counter"></param>
    /// <returns>a HOTP value</returns>
    public string ComputeHOTP(long counter) => Compute(counter, _hashMode);

    /// <summary>
    /// Verify a value that has been provided with the calculated value
    /// </summary>
    /// <param name="hotp">the trial HOTP value</param>
    /// <param name="counter">The counter value to verify</param>
    /// <returns>True if there is a match.</returns>
    public bool VerifyHotp(string hotp, long counter) => hotp == ComputeHOTP(counter);

    /// <summary>
    /// Takes a time step and computes a HOTP code
    /// </summary>
    /// <param name="counter">counter</param>
    /// <param name="mode">The hash mode to use</param>
    /// <returns>HOTP calculated code</returns>
    protected override string Compute(long counter, OtpHashMode mode)
    {
        var data = KeyUtilities.GetBigEndianBytes(counter);
        var otp = CalculateOtp(data, mode);
        return Digits(otp, _hotpSize);
    }
}