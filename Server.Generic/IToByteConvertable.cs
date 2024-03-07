namespace Server.Generic;

public interface IToBytesConvertable
{
    public Task<Memory<byte>> ToBytesAsync();
}