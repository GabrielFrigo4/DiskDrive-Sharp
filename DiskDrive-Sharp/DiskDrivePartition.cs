using System.Management;

namespace DiskDrive_Sharp;

public class DiskDrivePartition
{
    public ulong? EndingAddress { get; private set; }
    public ulong? StartingAddress { get; private set; }
    public uint? DiskIndex { get; private set; }
    public uint? Index { get; private set; }
    public string? Label { get; private set; }
    public string? Antecedent { get; private set; }
    public string? Dependent { get; private set; }

    public DiskDrivePartition(ManagementBaseObject managementBaseObject)
    {
        EndingAddress = (ulong?)managementBaseObject[nameof(EndingAddress)];
        StartingAddress = (ulong?)managementBaseObject[nameof(StartingAddress)];
        Antecedent = (string?)managementBaseObject[nameof(Antecedent)];
        Dependent = (string?)managementBaseObject[nameof(Dependent)];

        if (Antecedent is not null)
        {
            DiskIndex = uint.Parse(Antecedent[(Antecedent.IndexOf('#') + 1)..Antecedent.IndexOf(',')]);
            Index = uint.Parse(Antecedent[(Antecedent.LastIndexOf('#') + 1)..Antecedent.LastIndexOf('\"')]);
        }

        if (Dependent is not null)
        {
            Label = Dependent[(Dependent.IndexOf('\"') + 1)..Dependent.LastIndexOf('\"')];
        }
    }
}
