namespace DiskDrive_Sharp;

public delegate void BlockCreate(BlockCreateData blockCreateData);

public struct BlockCreateData
    (BlockData blockData, long blockIndex, 
    long created, long total)
{
    public BlockData BlockData { get; private set; } = blockData;
    public long BlockIndex { get; private set; } = blockIndex;
    public long Created { get; private set; } = created;
    public long Total { get; private set; } = total;
}

public class BlockData
    (long dataOffset, long dataLenght,
    long sectorOffset, long bufferOffset)
{
    public long DataOffset { get; private set; } = dataOffset;
    public long DataLenght { get; private set; } = dataLenght;
    public long SectorOffset { get; private set; } = sectorOffset;
    public long BufferOffset { get; private set; } = bufferOffset;
}