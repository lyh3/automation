// Copyright (C) 2012 McAfee, Inc. All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Automation.Base
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Copy a stream to a destination stream
        /// </summary>
        /// <param name="src">The source stream</param>
        /// <param name="dest">The destination stream</param>
        /// <param name="progressCallback">(Optional) Called every 64k copied with the total copied and when the copy is finished</param>
        public static void CopyTo(this Stream src, Stream dest, Action<long> progressCallback = null)
        {
            long copied = 0;
            int size = (src.CanSeek) ? Math.Min((int)(src.Length - src.Position), 65535) : 65535; // Max 64k buffer size
            byte[] buffer = new byte[size];
            int n;
            do
            {
                n = src.Read(buffer, 0, buffer.Length);

                if (n > 0)
                {
                    dest.Write(buffer, 0, n);

                    if (progressCallback != null)
                    {
                        copied += n;
                        progressCallback(copied);
                    }
                }
            } while (n != 0);
        }

        /// <summary>
        /// Copy a memory stream to another stream
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void CopyTo(this MemoryStream src, Stream dest)
        {
            dest.Write(src.GetBuffer(), (int)src.Position, (int)(src.Length - src.Position));
        }

        /// <summary>
        /// Copy a stream into a memory stream
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void CopyTo(this Stream src, MemoryStream dest)
        {
            if (src.CanSeek)
            {
                int pos = (int)dest.Position;
                int length = (int)(src.Length - src.Position) + pos;
                dest.SetLength(length);

                while (pos < length)
                    pos += src.Read(dest.GetBuffer(), pos, length - pos);
            }
            else
                src.CopyTo((Stream)dest);
        }
        public static MemoryStream ReadBytes(this string binaryFilePath, int offset, int length)
        {
            try
            {
                if (File.Exists(binaryFilePath))
                {
                    using (Stream source = File.OpenRead(binaryFilePath))
                    {
                        return source.ReadBytesFromStram(offset, length);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public static MemoryStream ReadBytesFromStram(this Stream source, int offset, int length)
        {
            const int CHUNK = 2048;
            var dest = new MemoryStream();
            byte[] buffer = new byte[CHUNK];
            int pos = offset;
            int bytesReaded = 0;
            if (source.CanSeek)
            {
                try
                {
                    while (bytesReaded < length)
                    {
                        var remainder = length - bytesReaded;
                        var readcount = Math.Min(remainder, CHUNK);
                        source.Seek((long)pos, SeekOrigin.Begin);
                        var r = source.Read(buffer, 0, readcount);
                        if (r > 0)
                        {
                            dest.Write(buffer, 0, (int)r);
                            pos += (int)r;
                            bytesReaded += r;
                        }
                    }
                }
                catch(Exception ex)
                {
                }

            }
            return dest;
        }
    }
}
