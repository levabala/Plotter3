module Parsing
    open System
    open System.IO
    
    (*let ParseRaw (path : string, chunksize: int, args : string[], progressCallBack) = 
        let signals = [for a in args -> byte a]
        let f1 a,b,c,d = (a) ||| (b << 8) ||| (d << 16) ||| (c << 24) //bytes to long time
        let f2 a,b,c = a[b] ||| (a[b+1] << 8) ||| (a[b+2] << 16) //to low time        

        let events = [
            use fs = new FileStream(path, FileMode.Open)
            use br = new BinaryReader(fs)
            let mutable pos = 0

            while pos < fs.Length do
                            let a = Math.Min(chunksize, int (fs.Length - pos))
                            let mutable buf = [||]
                            br.Read(buf,0,a)
                            pos <- pos + a
                            for i in 0..4..buf.Length 
                                if                            
        ]
        events*)
        

(*
Func<long, long, long, long, long> bytesToLongTime = new Func<long, long, long, long, long>((b0, b1, b2, tc) => 
            {
                return (b0) | (b1 << 8) | (b2 << 16) | (tc << 24);
            });

            Func<byte[], int, uint> bytesToLowTime = new Func<byte[], int, uint>((buf, i) =>
            {
                return ((uint)buf[i]) | ((uint)buf[i + 1] << 8) | ((uint)buf[i + 2] << 16);
            });*)
