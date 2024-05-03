using System.Management;

namespace DiskDrive_Sharp;

public static class DiskDriveManagement
{
    public static Dictionary<uint, DiskDrive> DiskDrives { get; private set; } = [];
    public static void Init(bool includeMainDiskDrive = false)
    {
        using ManagementObjectSearcher diskDriveSearcher = new(QueryCode.GET_DISK_DRIVE);
        foreach (var disk_drive in diskDriveSearcher.Get())
        {
            DiskDrive diskDrive = new(disk_drive);

            if (((diskDrive.Index == 0 || diskDrive.Name == @"\\.\PHYSICALDRIVE0") &&
                !includeMainDiskDrive) || diskDrive.Index is null)
            {
                continue;
            }
            DiskDrives.Add((uint)diskDrive.Index, diskDrive);
        }

        using ManagementObjectSearcher diskPartitionSearcher = new(QueryCode.GET_DISK_PARTITION);
        foreach (var disk_partition in diskPartitionSearcher.Get())
        {
            DiskDrivePartition diskDrivePartition = new(disk_partition);

            if (diskDrivePartition.DiskIndex is null)
            {
                continue;
            }
            if (diskDrivePartition.Label is null)
            {
                continue;
            }

            if (DiskDrives.TryGetValue((uint)diskDrivePartition.DiskIndex, out DiskDrive diskDrive))
            {
                diskDrive.DiskDrivePartitions.Add(diskDrivePartition.Label, diskDrivePartition);
            }
        }
    }
}

internal static class QueryCode
{
    internal const string GET_DISK_DRIVE = "SELECT * FROM Win32_DiskDrive";
    internal const string GET_DISK_PARTITION = "SELECT * FROM Win32_LogicalDiskToPartition";
}
