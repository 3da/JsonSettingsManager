using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace JsonSettingsManager
{
    public static class StreamUtils
    {
        public static byte[][] LoadLargeBytesFromStream(Stream stream)
        {
            var result = new List<byte[]>();

            while (true)
            {
                var buffer = new byte[0X7FFFFFC7];

                var count = stream.Read(buffer, 0, buffer.Length);

                if (count > 0)
                {
                    if (count != buffer.Length)
                        Array.Resize(ref buffer, count);

                    result.Add(buffer);
                }
                else
                    break;
            }

            return result.ToArray();
        }
    }
}
