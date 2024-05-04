using DiskDrive_Sharp.Utils;

namespace DiskDrive_Sharp;

public delegate void StepUpdate(StepData stepData);

public struct StepData(int completed, int total)
{
    public int Completed { get; private set; } = completed;
    public int Total { get; private set; } = total;
}

public class StepInfo(StepUpdate? update = null, int sectorSize = MemoryInt.SECTOR)
{
    public StepUpdate? Update { get; private set; } = update;
    public int SectorSize { get; private set; } = sectorSize;
}