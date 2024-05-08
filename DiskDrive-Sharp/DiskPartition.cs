using System.Management;

namespace DiskDrive_Sharp;

public class DiskPartition : DiskAbstract
{
    public ulong? EndingAddress { get; private set; }
    public ulong? StartingAddress { get; private set; }
    public string? Antecedent { get; private set; }
    public string? Dependent { get; private set; }

    public ulong? Size { get; private set; }
    public uint? DiskIndex { get; private set; }
    public uint? Index { get; private set; }
    public string? Label { get; private set; }
    public string? Name { get; private set; }
    public uint? BytesPerSector
    {
        get
        {
            return DiskDrive?.BytesPerSector;
        }
    }

    public DiskDrive? DiskDrive { get; internal set; }


    public DiskPartition(ManagementBaseObject managementBaseObject)
    {
        EndingAddress = (ulong?)managementBaseObject[nameof(EndingAddress)];
        StartingAddress = (ulong?)managementBaseObject[nameof(StartingAddress)];
        Antecedent = (string?)managementBaseObject[nameof(Antecedent)];
        Dependent = (string?)managementBaseObject[nameof(Dependent)];

        if (StartingAddress is not null && EndingAddress is not null)
        {
            Size = EndingAddress - StartingAddress;
        }

        if (Antecedent is not null)
        {
            DiskIndex = uint.Parse(Antecedent[(Antecedent.IndexOf('#') + 1)..Antecedent.IndexOf(',')]);
            Index = uint.Parse(Antecedent[(Antecedent.LastIndexOf('#') + 1)..Antecedent.LastIndexOf('\"')]);
        }

        if (Dependent is not null)
        {
            Label = Dependent[(Dependent.IndexOf('\"') + 1)..Dependent.LastIndexOf('\"')];
        }

        if(Label is not null)
        {
            Name = $@"\\.\{Label}";
        }
    }

    public override string GetName()
    {
        return Name ?? throw new Exception($"{nameof(Name)} is NULL!");
    }

    public override ulong GetSize()
    {
        return Size ?? throw new Exception($"{nameof(Size)} is NULL!");
    }

    public override uint GetIndex()
    {
        return Index ?? throw new Exception($"{nameof(Index)} is NULL!");
    }

    public override uint GetBytesPerSector()
    {
        return BytesPerSector ?? throw new Exception($"{nameof(BytesPerSector)} is NULL!");
    }
}
