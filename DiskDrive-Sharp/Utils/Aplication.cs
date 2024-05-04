namespace DiskDrive_Sharp.Utils;

public static class AplicationDisk
{
    public static string PartitionLabel 
    { 
        get
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            return dir[..(dir.IndexOf(':') + 1)];
        }
    }

    public static string PartitionName
    {
        get
        {
            return $@"\\.\{PartitionLabel}";
        }
    }

    public static DiskDrive? Drive { get; internal set; }
    public static DiskPartition? Partition { get; internal set; }
}
