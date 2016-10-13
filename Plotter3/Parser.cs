using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotter3
{
    public class Parser
    {
        public static Int64[] parseLM(string path, byte signal)
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

        public static long bytesToLongTime(long b0, long b1, long b2, long tc)
        {
            long t = (b0) | (b1 << 8) | (b2 << 16) | (tc << 24);
            return t;
        }
    }


}
