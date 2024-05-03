using System.Management;
using System.Xml.Linq;

namespace DiskDrive_Sharp;

public class DiskDrivePartition
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


    public DiskDrivePartition(ManagementBaseObject managementBaseObject)
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

    public byte[] ReadRawData(long offset, int lenght)
    {
        if (Size is null)
        {
            throw new Exception($"{nameof(Size)} is NULL!");
        }
        if (Label is null)
        {
            throw new Exception($"{nameof(Label)} is NULL!");
        }

        if ((long)Size < offset + lenght)
        {
            throw new Exception($"Data out of Edges; Size:{Size}, Data Position:{offset + lenght}");
        }

        byte[] buffer = new byte[lenght];
        using (FileStream fs = new(Label, FileMode.Open, FileAccess.Read))
        {
            fs.Seek(offset, SeekOrigin.Begin);
            fs.Read(buffer, 0, lenght);
        }
        return buffer;
    }

    public void WriteRawData(byte[] buffer, long offset, int lenght)
    {
        if (Size is null)
        {
            throw new Exception($"{nameof(Size)} is NULL!");
        }
        if (Label is null)
        {
            throw new Exception($"{nameof(Name)} is NULL!");
        }

        if ((long)Size < offset + lenght)
        {
            throw new Exception($"Data out of Edges; Size:{Size}, Data Position:{offset + lenght}");
        }

        using FileStream fs = new(Name, FileMode.Open, FileAccess.ReadWrite);
        fs.Seek(offset, SeekOrigin.Begin);
        fs.Write(buffer, 0, lenght);
    }
}
