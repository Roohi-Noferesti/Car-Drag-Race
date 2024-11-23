namespace LitJson
{
    using System;
    using System.IO;

    internal class JsonStringWriter
    {
        private static char[] hex_seq = new char[4];
        public static bool MungeUnicodeForPHP = true;

        private static void IntToHex(int n, char[] hex)
        {
            for (int i = 0; i < 4; i++)
            {
                int num = n % 0x10;
                if (num < 10)
                {
                    hex[3 - i] = (char)(0x30 + num);
                }
                else
                {
                    hex[3 - i] = (char)(0x41 + (num - 10));
                }
                n = n >> 4;
            }
        }

        public static void Write(TextWriter writer, string str)
        {
            writer.Write('"');
            int length = str.Length;
            for (int i = 0; i < length; i++)
            {
                char ch = str[i];
                switch (ch)
                {
                    case '\b':
                        {
                            writer.Write(@"\b");
                            continue;
                        }
                    case '\t':
                        {
                            writer.Write(@"\t");
                            continue;
                        }
                    case '\n':
                        {
                            writer.Write(@"\n");
                            continue;
                        }
                    case '\f':
                        {
                            writer.Write(@"\f");
                            continue;
                        }
                    case '\r':
                        {
                            writer.Write(@"\r");
                            continue;
                        }
                }
                switch (ch)
                {
                    case '"':
                    case '\\':
                        writer.Write('\\');
                        writer.Write(str[i]);
                        break;

                    default:
                        if (MungeUnicodeForPHP)
                        {
                            if ((str[i] >= ' ') && (str[i] <= '~'))
                            {
                                writer.Write(str[i]);
                            }
                            else
                            {
                                IntToHex(str[i], hex_seq);
                                writer.Write(@"\u");
                                writer.Write(hex_seq);
                            }
                        }
                        else
                        {
                            writer.Write(str[i]);
                        }
                        break;
                }
            }
            writer.Write('"');
        }
    }
}
