using DiskDrive_Sharp.Utils;
using System.Runtime.InteropServices;

namespace DiskDrive_Sharp;

public abstract class DiskAbstract : IDiskData
{
    public abstract string GetName();
    public abstract ulong GetSize();
    public abstract uint GetIndex();
    public abstract uint GetBytesPerSector();

    #region Block of Data
    private BlockData GetBlockData(long offset, long start, long end, long sectorIndex)
    {
        long dataOffset = start % GetBytesPerSector();
        long dataLenght = Math.Min(end - start, GetBytesPerSector() - dataOffset);
        long sectionOffset = sectorIndex * GetBytesPerSector();
        long bufferOffset = start - offset;

        BlockData data = new(dataOffset, dataLenght, sectionOffset, bufferOffset);
        return data;
    }

    public void GetBlockDataLoop(long offset, long length, BlockCreate blockCreate)
    {
        long start = offset;
        long end = start + length;
        long startSector = start / GetBytesPerSector();
        long endSector = (end - 1) / GetBytesPerSector();
        for (long sector_index = startSector; sector_index <= endSector; sector_index++)
        {
            BlockData data = GetBlockData(offset, start, end, sector_index);
            long blockIndex = sector_index - startSector;
            long created = sector_index - startSector + 1;
            long total = endSector - startSector + 1;

            BlockCreateData blockCreateData = new(data, blockIndex, created, total);
            blockCreate.Invoke(blockCreateData);
            start += data.DataLenght;
        }
    }

    public List<BlockData> GetBlockDataList(long offset, long length)
    {
        List<BlockData> blocks = [];
        void BlockCreateFunction(BlockCreateData blockCreateData)
        {
            blocks.Add(blockCreateData.BlockData);
        }

        GetBlockDataLoop(offset, length, BlockCreateFunction);
        return [.. blocks];
    }
    #endregion

    #region ReadRawData
    public void ReadRawData(long offset, int length, out Span<byte> buffer)
    {
        if ((long)GetSize() < offset + length)
        {
            throw new Exception($"Data out of Edges; Size:{GetSize()}, Data Position:{offset + length}");
        }
        if (offset % GetBytesPerSector() != 0)
        {
            throw new Exception($"offset must be a multiple of {GetBytesPerSector()}; offset:{offset}");
        }
        if (length % GetBytesPerSector() != 0)
        {
            throw new Exception($"length must be a multiple of {GetBytesPerSector()}; length:{length}");
        }

        using FileStream fs = new(GetName(), FileMode.Open, FileAccess.Read, FileShare.Read);
        buffer = new byte[length];
        fs.Seek(offset, SeekOrigin.Begin);
        fs.Read(buffer);
    }

    public void ReadRawData(long offset, int length, out byte[] buffer)
    {
        ReadRawData(offset, length, out Span<byte> spanBuffer);
        buffer = spanBuffer.ToArray();
    }
    #endregion

    #region WriteRawData
    public void WriteRawData(long offset, ReadOnlySpan<byte> buffer, bool lockVolume = true)
    {
        if ((long)GetSize() < offset + buffer.Length)
        {
            throw new Exception($"Data out of Edges; Size:{GetSize()}, Data Position:{offset + buffer.Length}");
        }
        if (offset % GetBytesPerSector() != 0)
        {
            throw new Exception($"offset must be a multiple of {GetBytesPerSector()}; offset:{offset}");
        }
        if (buffer.Length % GetBytesPerSector() != 0)
        {
            throw new Exception($"length must be a multiple of {GetBytesPerSector()}; length:{buffer.Length}");
        }

        using FileStream fs = new(GetName(), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        if (lockVolume)
        {
            DeviceDisk.DeviceIoControl(fs.SafeFileHandle, DeviceDisk.FSCTL_LOCK_VOLUME);
        }
        fs.Seek(offset, SeekOrigin.Begin);
        fs.Write(buffer);
        if (lockVolume)
        {
            DeviceDisk.DeviceIoControl(fs.SafeFileHandle, DeviceDisk.FSCTL_UNLOCK_VOLUME);
        }
    }

    public void WriteRawData(long offset, byte[] buffer, bool lockVolume = true)
    {
        ReadOnlySpan<byte> readOnlySpanBuffer = buffer;
        WriteRawData(offset, readOnlySpanBuffer, lockVolume);
    }
    #endregion

    #region ReadInBlocks
    private void ReadBlock(BlockData block, ref Span<byte> buffer)
    {
        ReadRawData(block.SectorOffset, (int)GetBytesPerSector(), out Span<byte> spanBuffer);
        Span<byte> spanData = spanBuffer.Slice((int)block.DataOffset, (int)block.DataLenght);
        Span<byte> bufferSlice = buffer[(int)block.BufferOffset..];
        spanData.CopyTo(bufferSlice);
    }

    private void ReadBlock(BlockData block, ref byte[] buffer)
    {
        Span<byte> spanBuffer = buffer;
        ReadBlock(block, ref spanBuffer);
        buffer = spanBuffer.ToArray();
    }

    public void ReadInBlocks(long offset, int length, out Span<byte> buffer, BlockCreate? blockCreate = null)
    {
        byte[] arrayBuffer = new byte[length];
        void BlockCreateFunction(BlockCreateData blockCreateData)
        {
            ReadBlock(blockCreateData.BlockData, ref arrayBuffer);
            blockCreate?.Invoke(blockCreateData);
        }

        GetBlockDataLoop(offset, length, BlockCreateFunction);
        buffer = arrayBuffer;
    }

    public void ReadInBlocks(long offset, int length, out byte[] buffer, BlockCreate? blockCreate = null)
    {
        ReadInBlocks(offset, length, out Span<byte> spanBuffer, blockCreate);
        buffer = spanBuffer.ToArray();
    }
    #endregion

    #region WriteInBlock
    private void WriteBlock(BlockData block, ReadOnlySpan<byte> buffer, bool lockVolume = true)
    {
        ReadOnlySpan<byte> readOnlySpanBuffer = buffer.Slice((int)block.BufferOffset, (int)block.DataLenght);
        if (block.DataLenght != GetBytesPerSector())
        {
            ReadRawData(block.SectorOffset, (int)GetBytesPerSector(), out Span<byte> snapBuffer);
            Span<byte> snapBufferData = snapBuffer[(int)block.DataOffset..];
            readOnlySpanBuffer.CopyTo(snapBufferData);
            WriteRawData(block.SectorOffset, snapBuffer, lockVolume);
        }
        else
        {
            WriteRawData(block.SectorOffset, readOnlySpanBuffer, lockVolume);
        }
    }

    private void WriteBlock(BlockData block, byte[] buffer, bool lockVolume = true)
    {
        ReadOnlySpan<byte> readOnlySpanBuffer = buffer;
        WriteBlock(block, readOnlySpanBuffer, lockVolume);
    }

    public void WriteInBlocks(long offset, ReadOnlySpan<byte> buffer, bool lockVolume = true, BlockCreate? blockCreate = null)
    {
        byte[] arrayBuffer = buffer.ToArray();
        void BlockCreateFunction(BlockCreateData blockCreateData)
        {
            WriteBlock(blockCreateData.BlockData, arrayBuffer, lockVolume);
            blockCreate?.Invoke(blockCreateData);
        }

        GetBlockDataLoop(offset, buffer.Length, BlockCreateFunction);
    }

    public void WriteInBlocks(long offset, byte[] buffer, bool lockVolume = true, BlockCreate? blockCreate = null)
    {
        ReadOnlySpan<byte> spanBuffer = buffer;
        WriteInBlocks(offset, spanBuffer, lockVolume, blockCreate);
    }
    #endregion

    #region Read and Write Struct
    public unsafe void ReadStruct<T>(long offset, out T data) where T : unmanaged
    {
        ReadInBlocks(offset, sizeof(T), out Span<byte> spanBuffer);
        fixed (void* ptr = spanBuffer)
        {
            data = Marshal.PtrToStructure<T>((nint)ptr);
        }
    }

    public unsafe void WriteStruct<T>(long offset, T data) where T : unmanaged
    {
        void* ptr = &data;
        ReadOnlySpan<byte> readOnlySpanBuffer = new(ptr, sizeof(T));
        WriteInBlocks(offset, readOnlySpanBuffer);
    }
    #endregion

    #region Read and Write Byte
    public void ReadByte(long offset, out byte data)
    {
        ReadStruct(offset, out data);
    }

    public void WriteByte(long offset, byte data)
    {
        WriteStruct(offset, data);
    }
    #endregion

    #region Read and Write Word
    public void ReadWord(long offset, out short data)
    {
        ReadStruct(offset, out data);
    }

    public void WriteWord(long offset, short data)
    {
        WriteStruct(offset, data);
    }
    #endregion

    #region Read and Write DWord
    public void ReadDWord(long offset, out int data)
    {
        ReadStruct(offset, out data);
    }

    public void WriteDWord(long offset, int data)
    {
        WriteStruct(offset, data);
    }
    #endregion

    #region Read and Write QWord
    public void ReadQWord(long offset, out long data)
    {
        ReadStruct(offset, out data);
    }

    public void WriteQWord(long offset, long data)
    {
        WriteStruct(offset, data);
    }
    #endregion

    #region Read and Write OWord
    public void ReadOWord(long offset, out Int128 data)
    {
        ReadStruct(offset, out data);
    }

    public void WriteOWord(long offset, Int128 data)
    {
        WriteStruct(offset, data);
    }
    #endregion

    #region Read and Write Char
    public void ReadChar(long offset, out char data)
    {
        ReadStruct(offset, out data);
    }

    public void WriteChar(long offset, char data)
    {
        WriteStruct(offset, data);
    }
    #endregion

    #region Read and Write Structs
    public unsafe void ReadStructs<T>(long offset, int count, out Span<T> data) where T : unmanaged
    {
        ReadInBlocks(offset, sizeof(T) * count, out Span<byte> spanBuffer);
        fixed (void* ptr = spanBuffer)
        {
            data = new(ptr, count);
        }
    }

    public unsafe void WriteStructs<T>(long offset, ReadOnlySpan<T> data) where T : unmanaged
    {
        fixed(void* ptr = data)
        {
            ReadOnlySpan<byte> readOnlySpanBuffer = new(ptr, sizeof(T) * data.Length);
            WriteInBlocks(offset, readOnlySpanBuffer);
        }
    }
    #endregion

    #region Read and Write Bytes
    public void ReadBytes(long offset, int count, out byte[] data)
    {
        ReadStructs(offset, count, out Span<byte> spanData);
        data = spanData.ToArray();
    }

    public void WriteBytes(long offset, byte[] data)
    {
        ReadOnlySpan<byte> readOnlySpanData = data;
        WriteStructs(offset, readOnlySpanData);
    }
    #endregion

    #region Read and Write Words
    public void ReadWords(long offset, int count, out short[] data)
    {
        ReadStructs(offset, count, out Span<short> spanData);
        data = spanData.ToArray();
    }

    public void WriteWords(long offset, short[] data)
    {
        ReadOnlySpan<short> readOnlySpanData = data;
        WriteStructs(offset, readOnlySpanData);
    }
    #endregion

    #region Read and Write DWords
    public void ReadDWords(long offset, int count, out int[] data)
    {
        ReadStructs(offset, count, out Span<int> spanData);
        data = spanData.ToArray();
    }

    public void WriteDWords(long offset, int[] data)
    {
        ReadOnlySpan<int> readOnlySpanData = data;
        WriteStructs(offset, readOnlySpanData);
    }
    #endregion

    #region Read and Write QWords
    public void ReadQWords(long offset, int count, out long[] data)
    {
        ReadStructs(offset, count, out Span<long> spanData);
        data = spanData.ToArray();
    }

    public void WriteQWords(long offset, long[] data)
    {
        ReadOnlySpan<long> readOnlySpanData = data;
        WriteStructs(offset, readOnlySpanData);
    }
    #endregion

    #region Read and Write OWords
    public void ReadOWords(long offset, int count, out Int128[] data)
    {
        ReadStructs(offset, count, out Span<Int128> spanData);
        data = spanData.ToArray();
    }

    public void WriteOWords(long offset, Int128[] data)
    {
        ReadOnlySpan<Int128> readOnlySpanData = data;
        WriteStructs(offset, readOnlySpanData);
    }
    #endregion

    #region Read and Write String
    public void ReadString(long offset, int count, out string data)
    {
        ReadStructs(offset, count, out Span<char> spanData);
        data = new(spanData.ToArray());
    }

    public void WriteString(long offset, string data)
    {
        ReadOnlySpan<char> readOnlySpanData = data;
        WriteStructs(offset, readOnlySpanData);
    }
    #endregion
}
