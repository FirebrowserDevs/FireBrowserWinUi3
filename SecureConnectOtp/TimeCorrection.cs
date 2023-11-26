using System;

namespace SecureConnectOtp;

public class TimeCorrection
{
    public static readonly TimeCorrection UncorrectedInstance = new TimeCorrection();

    /// <summary>
    /// Constructor used solely for the UncorrectedInstance static field to provide an instance without
    /// a correction factor.
    /// </summary>
    private TimeCorrection() => CorrectionFactor = TimeSpan.FromSeconds(0);

    /// <summary>
    /// Creates a corrected time object by providing the known correct current UTC time.
    /// The current system UTC time will be used as the reference
    /// </summary>
    /// <remarks>
    /// This overload assumes UTC.  If a base and reference time other than UTC are required
    /// then use the other overload.
    /// </remarks>
    /// <param name="correctUtc">The current correct UTC time</param>
    public TimeCorrection(DateTime correctUtc) => CorrectionFactor = DateTime.UtcNow - correctUtc;

    /// <summary>
    /// Creates a corrected time object by providing the known correct current time and the current reference
    /// time that needs correction
    /// </summary>
    /// <param name="correctTime">The current correct time</param>
    /// <param name="referenceTime">The current reference time (time that will have the correction
    /// factor applied in subsequent calls)</param>
    public TimeCorrection(DateTime correctTime, DateTime referenceTime) =>
        CorrectionFactor = referenceTime - correctTime;

    /// <summary>
    /// Applies the correction factor to the reference time and returns a corrected time
    /// </summary>
    /// <param name="referenceTime">The reference time</param>
    /// <returns>The reference time with the correction factor applied</returns>
    public DateTime GetCorrectedTime(DateTime referenceTime) => referenceTime - CorrectionFactor;

    /// <summary>
    /// Applies the correction factor to the current system UTC time and returns a corrected time
    /// </summary>
    public DateTime CorrectedUtcNow => GetCorrectedTime(DateTime.UtcNow);

    /// <summary>
    /// The timespan that is used to calculate a corrected time
    /// </summary>
    public TimeSpan CorrectionFactor { get; }
}