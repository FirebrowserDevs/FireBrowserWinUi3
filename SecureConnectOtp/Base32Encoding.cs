using System;

namespace SecureConnectOtp;

public class Base32Encoding
{
    public static byte[] ToBytes(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentNullException(nameof(input));
        }

        input = input.TrimEnd('=');
        var byteCount = input.Length * 5 / 8;
        var returnArray = new byte[byteCount];

        byte curByte = 0, bitsRemaining = 8;
        var arrayIndex = 0;

        foreach (var c in input)
        {
            var cValue = CharToValue(c);

            int mask;
            if (bitsRemaining > 5)
            {
                mask = cValue << (bitsRemaining - 5);
                curByte |= (byte)mask;
                bitsRemaining -= 5;
            }
            else
            {
                mask = cValue >> (5 - bitsRemaining);
                curByte |= (byte)mask;
                returnArray[arrayIndex++] = curByte;
                curByte = (byte)(cValue << (3 + bitsRemaining));
                bitsRemaining += 3;
            }
        }

        if (arrayIndex != byteCount)
        {
            returnArray[arrayIndex] = curByte;
        }

        return returnArray;
    }

    public static string ToString(byte[] input)
    {
        if (input == null || input.Length == 0)
        {
            throw new ArgumentNullException(nameof(input));
        }

        var charCount = (int)Math.Ceiling(input.Length / 5d) * 8;
        var returnArray = new char[charCount];

        byte nextChar = 0, bitsRemaining = 5;
        var arrayIndex = 0;

        foreach (byte b in input)
        {
            nextChar |= (byte)(b >> (8 - bitsRemaining));
            returnArray[arrayIndex++] = ValueToChar(nextChar);

            if (bitsRemaining < 4)
            {
                nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
                returnArray[arrayIndex++] = ValueToChar(nextChar);
                bitsRemaining += 5;
            }

            bitsRemaining -= 3;
            nextChar = (byte)((b << bitsRemaining) & 31);
        }

        if (arrayIndex != charCount)
        {
            returnArray[arrayIndex++] = ValueToChar(nextChar);

            while (arrayIndex != charCount)
            {
                returnArray[arrayIndex++] = '=';
            }
        }

        return new string(returnArray);
    }

    private static int CharToValue(char c)
    {
        int value = c;

        if ((value >= 'A' && value <= 'Z'))
        {
            return value - 'A';
        }
        if ((value >= '2' && value <= '7'))
        {
            return value - ('2' - 26);
        }
        if ((value >= 'a' && value <= 'z'))
        {
            return value - 'a';
        }

        throw new ArgumentException("Character is not a Base32 character.", nameof(c));
    }

    private static char ValueToChar(byte b)
    {
        if (b < 26)
        {
            return (char)(b + 'A');
        }
        if (b < 32)
        {
            return (char)(b + ('2' - 26));
        }

        throw new ArgumentException("Byte is not a Base32 value.", nameof(b));
    }
}