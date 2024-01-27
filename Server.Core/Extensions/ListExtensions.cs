namespace Server.Core.Extensions;

internal static class ListExtensions
{
    public static List<byte> ChainableSpaceWriting(this List<byte> list)
    {
        list.Add((byte)0x20);
        return list;
    }

    public static List<byte> ChainableNewLineWriting(this List<byte> list)
    {
        list.Add((byte)0x0a);
        return list;
    }

    public static List<byte> ChainableAdd(this List<byte> list, byte item)
    {
        list.Add(item);
        return list;
    }

    public static List<byte> ChainableAdd(this List<byte> list, byte[] items)
    {
        foreach (byte item in items)
        {
            list.ChainableAdd(item);
        }

        return list;
    }
}
