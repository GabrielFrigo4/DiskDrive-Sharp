namespace DiskDrive_Sharp;

public interface IDiskData
{
    public string GetName();
    public ulong GetSize();
    public uint GetIndex();
    public uint GetBytesPerSector();

    public void GetBlockDataLoop(long offset, long length, BlockCreate blockCreate);
    public List<BlockData> GetBlockDataList(long offset, long length);

    public void ReadRawData(long offset, int length, out Span<byte> buffer);
    public void ReadRawData(long offset, int length, out byte[] buffer);
    public void WriteRawData(long offset, ReadOnlySpan<byte> buffer, bool lockVolume = true);
    public void WriteRawData(long offset, byte[] buffer, bool lockVolume = true);

    public void ReadInBlocks(long offset, int length, out Span<byte> buffer, BlockCreate? blockCreate = null);
    public void ReadInBlocks(long offset, int length, out byte[] buffer, BlockCreate? blockCreate = null);
    public void WriteInBlocks(long offset, ReadOnlySpan<byte> buffer, bool lockVolume = true, BlockCreate? blockCreate = null);
    public void WriteInBlocks(long offset, byte[] buffer, bool lockVolume = true, BlockCreate? blockCreate = null);

    public unsafe void ReadStruct<T>(long offset, out T data) where T : unmanaged;
    public unsafe void WriteStruct<T>(long offset, T data) where T : unmanaged;

    public void ReadByte(long offset, out byte data);
    public void WriteByte(long offset, byte data);

    public void ReadWord(long offset, out short data);
    public void WriteWord(long offset, short data);

    public void ReadDWord(long offset, out int data);
    public void WriteDWord(long offset, int data);

    public void ReadQWord(long offset, out long data);
    public void WriteQWord(long offset, long data);

    public void ReadOWord(long offset, out Int128 data);
    public void WriteOWord(long offset, Int128 data);

    public void ReadChar(long offset, out char data);
    public void WriteChar(long offset, char data);

    public unsafe void ReadStructs<T>(long offset, int count, out Span<T> data) where T : unmanaged;
    public unsafe void WriteStructs<T>(long offset, ReadOnlySpan<T> data) where T : unmanaged;

    public void ReadBytes(long offset, int count, out byte[] data);
    public void WriteBytes(long offset, byte[] data);

    public void ReadWords(long offset, int count, out short[] data);
    public void WriteWords(long offset, short[] data);

    public void ReadDWords(long offset, int count, out int[] data);
    public void WriteDWords(long offset, int[] data);

    public void ReadQWords(long offset, int count, out long[] data);
    public void WriteQWords(long offset, long[] data);

    public void ReadOWords(long offset, int count, out Int128[] data);
    public void WriteOWords(long offset, Int128[] data);

    public void ReadString(long offset, int count, out string data);
    public void WriteString(long offset, string data);
}
