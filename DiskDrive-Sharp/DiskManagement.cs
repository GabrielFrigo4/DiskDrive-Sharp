using DiskDrive_Sharp.Utils;
using System.Management;

namespace DiskDrive_Sharp;

public static class DiskManagement
{
    public static Dictionary<uint, DiskDrive> DiskDrives { get; private set; } = [];
    public static void Init(bool includeMainDiskDrive = false)
    {
        using ManagementObjectSearcher diskDriveSearcher = new(QueryCode.GET_DISK_DRIVE);
        foreach (var obj_disk_drive in diskDriveSearcher.Get())
        {
            DiskDrive diskDrive = new(obj_disk_drive);

            if (((diskDrive.Index == 0 || diskDrive.Name == @"\\.\PHYSICALDRIVE0") &&
                !includeMainDiskDrive) || diskDrive.Index is null)
            {
                continue;
            }
            DiskDrives.Add((uint)diskDrive.Index, diskDrive);
        }

        using ManagementObjectSearcher diskPartitionSearcher = new(QueryCode.GET_DISK_PARTITION);
        foreach (var obj_disk_partition in diskPartitionSearcher.Get())
        {
            DiskPartition diskPartition = new(obj_disk_partition);

            if (diskPartition.DiskIndex is null)
            {
                continue;
            }
            if (diskPartition.Label is null)
            {
                continue;
            }

            if (DiskDrives.TryGetValue((uint)diskPartition.DiskIndex, out DiskDrive? diskDrive))
            {
                diskPartition.DiskDrive = diskDrive;
                diskDrive.DiskPartitions.Add(diskPartition.Label, diskPartition);
                if(AplicationDisk.PartitionLabel == diskPartition.Label)
                {
                    AplicationDisk.Partition = diskPartition;
                    AplicationDisk.Drive = diskDrive;
                }
            }
        }
    }
}

internal static class QueryCode
{
    internal const string GET_DISK_DRIVE = "SELECT * FROM Win32_DiskDrive";
    internal const string GET_DISK_PARTITION = "SELECT * FROM Win32_LogicalDiskToPartition";
}
