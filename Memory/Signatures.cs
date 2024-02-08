using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Memory
{
    public class Signatures
    {
        public Signatures(string sig)
        {
            sig = sig.Replace(" ", "");
            if (sig.Length % 2 == 0)
            {
                sig = sig.ToUpper();
                byte a = 0;
                bool n1 = true;
                bool Valid = true;
                foreach (char ch in sig)
                {
                    if (n1)
                    {
                        n1 = false;
                        if (ch == MSistem.chS)
                        {
                            a = 1;
                            continue;
                        }
                        else if (MSistem.Test(ch))
                        {
                            a = 2;
                            continue;
                        }
                        else 
                        {
                            Valid = false;
                            break; 
                        }
                    }
                    else
                    {
                        n1 = true;
                        if (a == 1) { if (ch == MSistem.chS) { continue; } }
                        else { if (MSistem.Test(ch)) { continue; } }
                        Valid = false;
                        break;
                    }
                }
                if (Valid)
                {
                    txt = sig;
                    List<(byte, bool)> values = new List<(byte, bool)>();
                    for (int i = 0; i < sig.Length; i += 2 )
                    {
                        if (sig[i] == MSistem.chS)
                        {
                            values.Add((0, false)); 
                        }
                        else
                        {
                            values.Add(((byte)(MSistem.CharToHex(sig[i], true) + MSistem.CharToHex(sig[(i + 1)], false)), true)); 
                        }
                    }
                    this.sig = values.ToArray();
                }
            }
        }
        public (byte, bool)[] sig { get; private set; } = null;
        public string txt = "No_Signatures";
        public bool Test(byte[] a)
        {
            bool test = true;
            int i = 0;
            for (; i < sig.Length; i++)
            {
                if (sig[i].Item2)
                {
                    if (sig[i].Item1 == a[i]) { continue; }
                    test = false;
                    break;
                }
            }
            return test;
        }
        public bool Test(byte a, long id)
        {
            if (sig[id].Item2) { return a == sig[id].Item1; }
            return true;
        }

        public override string ToString()
        {
            return txt;
        }
    }
    public static class MSistem
    {
        public static char[] c16 = "0123456789ABCDEF".ToArray();
        public static char chS = '?';
        public static byte CharToHex(char ch, bool l)
        {
            double a = 0;
            for (int i = 0;i < c16.Length ;i++)
            {
                if (c16[i] == ch)
                {
                    if (l) { a = 16d * (double)i; }
                    else { a = i; }
                }
            }
            return (byte)a;
        }
        public static bool Test(char ch)
        {
            for (int i = 0; i < c16.Length;i++)
            {
                if (ch == c16[i])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
