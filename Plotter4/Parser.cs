using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Plotter4
{
    class Parser
    {
        public static long[] parseLM(string path, byte signal)
        {
            byte[] buf = File.ReadAllBytes(path);

            int time_code = 0;
            List<long> events = new List<long>();

            for (int i = 0; i < buf.Length; i += 4)
            {
                if (buf[i + 3] == signal) events.Add(bytesToLongTime(buf[i], buf[i + 1], buf[i + 2], time_code));
                else if (buf[i + 3] == 0xf4) time_code++;
            }
            return events.ToArray();
        }

        public static Dictionary<byte, long[]> parseLM2(string path, byte[] signals, Action<long> progress = null)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            long len = fs.Length;
            int pos = 0;
            int buf_size = 1000000;
            byte[] buf;

            int time_code = 0;
            //List<long> events = new List<long>();
            Dictionary<byte, List<long>> events = new Dictionary<byte, List<long>>();
            foreach (byte s in signals) events[s] = new List<long>();
            while (pos < len)
            {
                int bytes_to_read = Math.Min(buf_size, (int)(len - pos));
                buf = new byte[bytes_to_read];

                int bytes_read = br.Read(buf, 0, bytes_to_read);

                for (int i = 0; i < buf.Length; i += 4)
                {
                    uint lo = bytesToLowTime(buf, i);
                    byte signal = buf[i + 3];
                    if (events.ContainsKey(signal))
                        events[signal].Add(bytesToLongTime(buf[i], buf[i + 1], buf[i + 2], time_code));
                    else if (buf[i + 3] == 0xf4) time_code++;
                }
                pos += bytes_read;
                if (progress != null) progress(pos);
            }
            Dictionary<byte, long[]> eventsArr = new Dictionary<byte, long[]>();
            foreach (KeyValuePair<byte, List<long>> pair in events)
                eventsArr[pair.Key] = pair.Value.ToArray();

            br.Close();
            fs.Close();

            return eventsArr;
        }

        public static long bytesToLongTime(long b0, long b1, long b2, long tc)
        {
            long t = (b0) | (b1 << 8) | (b2 << 16) | (tc << 24);
            return t;
        }

        public static uint bytesToLowTime(byte[] buf, int i)
        {
            uint t = ((uint)buf[i]) | ((uint)buf[i + 1] << 8) | ((uint)buf[i + 2] << 16);
            return t;
        }
    }
}
