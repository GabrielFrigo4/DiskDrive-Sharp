using DiskDrive_Sharp.Utils;

namespace DiskDrive_Sharp;

public delegate void StepUpdate(StepData stepData);

public struct StepData(int completed, int total)
{
    public int Completed { get; private set; } = completed;
    public int Total { get; private set; } = total;
}

public class StepInfo(int memorySize = MemoryInt.SECTOR, StepUpdate? update = null)
{
    public int MemorySize { get; private set; } = memorySize;
    public StepUpdate? Update { get; private set; } = update;
}