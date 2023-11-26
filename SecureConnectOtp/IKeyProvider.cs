namespace SecureConnectOtp;

public interface IKeyProvider
{
    byte[] ComputeHmac(OtpHashMode mode, byte[] data);
}