using System.Management;

namespace DiskDrive_Sharp;

public class DiskPartition: IDiskData
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

    public string GetName()
    {
        return Name ?? throw new Exception($"{nameof(Name)} is NULL!");
    }

    public ulong GetSize()
    {
        return Size ?? throw new Exception($"{nameof(Size)} is NULL!");
    }

    public uint GetIndex()
    {
        return Index ?? throw new Exception($"{nameof(Index)} is NULL!");
    }

    public void ReadRawData(long offset, int lenght, out Span<byte> buffer)
    {
        if (Size is null)
        {
            throw new Exception($"{nameof(Size)} is NULL!");
        }
        if (Name is null)
        {
            throw new Exception($"{nameof(Name)} is NULL!");
        }

        if ((long)Size < offset + lenght)
        {
            throw new Exception($"Data out of Edges; Size:{Size}, Data Position:{offset + lenght}");
        }

        using FileStream fs = new(Name, FileMode.Open, FileAccess.Read);
        buffer = new byte[lenght];
        fs.Seek(offset, SeekOrigin.Begin);
        fs.Read(buffer);
    }

    public void ReadRawData(long offset, int lenght, out byte[] buffer)
    {
        ReadRawData(offset, lenght, out Span<byte> spanBuffer);
        buffer = spanBuffer.ToArray();
    }

    public void WriteRawData(long offset, ReadOnlySpan<byte> buffer)
    {
        if (Size is null)
        {
            throw new Exception($"{nameof(Size)} is NULL!");
        }
        if (Name is null)
        {
            throw new Exception($"{nameof(Name)} is NULL!");
        }

        if ((long)Size < offset + buffer.Length)
        {
            throw new Exception($"Data out of Edges; Size:{Size}, Data Position:{offset + buffer.Length}");
        }

        using FileStream fs = new(Name, FileMode.Open, FileAccess.ReadWrite);
        fs.Seek(offset, SeekOrigin.Begin);
        fs.Write(buffer);
    }

    public void WriteRawData(long offset, byte[] buffer)
    {
        ReadOnlySpan<byte> spanBuffer = buffer;
        WriteRawData(offset, spanBuffer);
    }

    public void ReadInSteps(long offset, int lenght, out Span<byte> buffer, StepInfo stepInfo)
    {
        int total = lenght / stepInfo.MemorySize;

        buffer = new byte[lenght];
        for (int completed = 1; completed <= total; completed++)
        {
            int segmentSize = stepInfo.MemorySize;
            int pre_completed = completed - 1;
            if (completed == total)
            {
                segmentSize = lenght - pre_completed * stepInfo.MemorySize;
            }

            ReadRawData(offset + pre_completed * stepInfo.MemorySize, segmentSize, out Span<byte> spanBuffer);
            spanBuffer.CopyTo(buffer.Slice(pre_completed * stepInfo.MemorySize, segmentSize));

            StepData stepData = new(completed, total);
            stepInfo.Update?.Invoke(stepData);
        }
    }

    public void ReadInSteps(long offset, int lenght, out byte[] buffer, StepInfo stepInfo)
    {
        ReadInSteps(offset, lenght, out Span<byte> spanBuffer, stepInfo);
        buffer = spanBuffer.ToArray();
    }

    public void WriteInSteps(long offset, ReadOnlySpan<byte> buffer, StepInfo stepInfo)
    {
        int total = buffer.Length / stepInfo.MemorySize;

        for (int completed = 1; completed <= total; completed++)
        {
            int segmentSize = stepInfo.MemorySize;
            int pre_completed = completed - 1;
            if (completed == total)
            {
                segmentSize = buffer.Length - pre_completed * stepInfo.MemorySize;
            }

            ReadOnlySpan<byte> bufferSlice = buffer.Slice(pre_completed * stepInfo.MemorySize, segmentSize);
            WriteRawData(offset + pre_completed * stepInfo.MemorySize, bufferSlice);

            StepData stepData = new(completed, total);
            stepInfo.Update?.Invoke(stepData);
        }
    }

    public void WriteInSteps(long offset, byte[] buffer, StepInfo stepInfo)
    {
        ReadOnlySpan<byte> spanBuffer = buffer;
        WriteInSteps(offset, spanBuffer, stepInfo);
    }
}
