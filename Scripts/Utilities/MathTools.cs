using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class MathTools
{
    public static int RoundTo(this int value, int byValue)
    {
        return (int) (Math.Round((double) (value / byValue)) * byValue);
    }

    public static int RoundTo(this float value, int byValue)
    {
        var intvalue = (int) value;
        return intvalue.RoundTo(byValue);
    }

    public static string ToHex(this byte[] bytes, bool upperCase)
    {
        StringBuilder result = new StringBuilder(bytes.Length * 2);

        for (int i = 0; i < bytes.Length; i++)
            result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

        return result.ToString();
    }

    public static string ComputeHashForFile(string path)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(path))
            {
                var checksum = md5.ComputeHash(stream).ToHex(false);
                return checksum;
            }
        }
    }

    public static long ComputeSizeForFile(string path)
    {
        FileInfo file = new FileInfo(path);
        return file.Length;
    }
}
