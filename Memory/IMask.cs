using System;

namespace Memory;

public partial interface IMask
{

    bool IsMask(byte bytes);

    public static IMask[] ParseSignature(string signature)
    {
        string[] tokens = signature.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        IMask[] m = new IMask[tokens.Length];
        for (int i = 0; i < tokens.Length; i++)
            m[i] = tokens[i] == "??" ? new MaskNull() : new Mask(Convert.ToByte(tokens[i], 16));
        return m;
    }

}
partial interface IMask
{
    private struct Mask(byte p) : IMask
    {
        public byte pattern = p;

        bool IMask.IsMask(byte bytes)
            => bytes == pattern;

        public override string ToString()
            => pattern.ToString("x2");


    }
    private struct MaskNull : IMask
    {

        bool IMask.IsMask(byte bytes)
            => true;

        public override string ToString()
            => "??";
    }
}

