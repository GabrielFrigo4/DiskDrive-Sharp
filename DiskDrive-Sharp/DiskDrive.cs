using DiskDrive_Sharp.Utils;
using System.Management;

namespace DiskDrive_Sharp;

public struct DiskDrive(ManagementBaseObject managementBaseObject): IDiskData
{
    public ushort? Availability { get; private set; } = (ushort?)managementBaseObject[nameof(Availability)];
    public uint? BytesPerSector { get; private set; } = (uint?)managementBaseObject[nameof(BytesPerSector)];
    public ushort[]? Capabilities { get; private set; } = (ushort[]?)managementBaseObject[nameof(Capabilities)];
    public string[]? CapabilityDescriptions { get; private set; } = (string[]?)managementBaseObject[nameof(CapabilityDescriptions)];
    public string? Caption { get; private set; } = (string?)managementBaseObject[nameof(Caption)];
    public string? CompressionMethod { get; private set; } = (string?)managementBaseObject[nameof(CompressionMethod)];
    public uint? ConfigManagerErrorCode { get; private set; } = (uint?)managementBaseObject[nameof(ConfigManagerErrorCode)];
    public bool? ConfigManagerUserConfig { get; private set; } = (bool?)managementBaseObject[nameof(ConfigManagerUserConfig)];
    public string? CreationClassName { get; private set; } = (string?)managementBaseObject[nameof(CreationClassName)];
    public ulong? DefaultBlockSize { get; private set; } = (ulong?)managementBaseObject[nameof(DefaultBlockSize)];
    public string? Description { get; private set; } = (string?)managementBaseObject[nameof(Description)];
    public string? DeviceID { get; private set; } = (string?)managementBaseObject[nameof(DeviceID)];
    public bool? ErrorCleared { get; private set; } = (bool?)managementBaseObject[nameof(ErrorCleared)];
    public string? ErrorDescription { get; private set; } = (string?)managementBaseObject[nameof(ErrorDescription)];
    public string? ErrorMethodology { get; private set; } = (string?)managementBaseObject[nameof(ErrorMethodology)];
    public string? FirmwareRevision { get; private set; } = (string?)managementBaseObject[nameof(FirmwareRevision)];
    public uint? Index { get; private set; } = (uint?)managementBaseObject[nameof(Index)];
    public DateTime? InstallDate { get; private set; } = (DateTime?)managementBaseObject[nameof(InstallDate)];
    public string? InterfaceType { get; private set; } = (string?)managementBaseObject[nameof(InterfaceType)];
    public uint? LastErrorCode { get; private set; } = (uint?)managementBaseObject[nameof(LastErrorCode)];
    public string? Manufacturer { get; private set; } = (string?)managementBaseObject[nameof(Manufacturer)];
    public ulong? MaxBlockSize { get; private set; } = (ulong?)managementBaseObject[nameof(MaxBlockSize)];
    public ulong? MaxMediaSize { get; private set; } = (ulong?)managementBaseObject[nameof(MaxMediaSize)];
    public bool? MediaLoaded { get; private set; } = (bool?)managementBaseObject[nameof(MediaLoaded)];
    public string? MediaType { get; private set; } = (string?)managementBaseObject[nameof(MediaType)];
    public ulong? MinBlockSize { get; private set; } = (ulong?)managementBaseObject[nameof(MinBlockSize)];
    public string? Model { get; private set; } = (string?)managementBaseObject[nameof(Model)];
    public string? Name { get; private set; } = (string?)managementBaseObject[nameof(Name)];
    public bool? NeedsCleaning { get; private set; } = (bool?)managementBaseObject[nameof(NeedsCleaning)];
    public uint? NumberOfMediaSupported { get; private set; } = (uint?)managementBaseObject[nameof(NumberOfMediaSupported)];
    public uint? Partitions { get; private set; } = (uint?)managementBaseObject[nameof(Partitions)];
    public string? PNPDeviceID { get; private set; } = (string?)managementBaseObject[nameof(PNPDeviceID)];
    public ushort[]? PowerManagementCapabilities { get; private set; } = (ushort[]?)managementBaseObject[nameof(PowerManagementCapabilities)];
    public bool? PowerManagementSupported { get; private set; } = (bool?)managementBaseObject[nameof(PowerManagementSupported)];
    public uint? SCSIBus { get; private set; } = (uint?)managementBaseObject[nameof(SCSIBus)];
    public ushort? SCSILogicalUnit { get; private set; } = (ushort?)managementBaseObject[nameof(SCSILogicalUnit)];
    public ushort? SCSIPort { get; private set; } = (ushort?)managementBaseObject[nameof(SCSIPort)];
    public ushort? SCSITargetId { get; private set; } = (ushort?)managementBaseObject[nameof(SCSITargetId)];
    public uint? SectorsPerTrack { get; private set; } = (uint?)managementBaseObject[nameof(SectorsPerTrack)];
    public string? SerialNumber { get; private set; } = (string?)managementBaseObject[nameof(SerialNumber)];
    public uint? Signature { get; private set; } = (uint?)managementBaseObject[nameof(Signature)];
    public ulong? Size { get; private set; } = (ulong?)managementBaseObject[nameof(Size)];
    public string? Status { get; private set; } = (string?)managementBaseObject[nameof(Status)];
    public ushort? StatusInfo { get; private set; } = (ushort?)managementBaseObject[nameof(StatusInfo)];
    public string? SystemCreationClassName { get; private set; } = (string?)managementBaseObject[nameof(SystemCreationClassName)];
    public string? SystemName { get; private set; } = (string?)managementBaseObject[nameof(SystemName)];
    public ulong? TotalCylinders { get; private set; } = (ulong?)managementBaseObject[nameof(TotalCylinders)];
    public uint? TotalHeads { get; private set; } = (uint?)managementBaseObject[nameof(TotalHeads)];
    public ulong? TotalSectors { get; private set; } = (ulong?)managementBaseObject[nameof(TotalSectors)];
    public ulong? TotalTracks { get; private set; } = (ulong?)managementBaseObject[nameof(TotalTracks)];
    public uint? TracksPerCylinder { get; private set; } = (uint?)managementBaseObject[nameof(TracksPerCylinder)];

    public Dictionary<string, DiskPartition> DiskDrivePartitions { get; private set; } = [];

    public readonly string GetName()
    {
        return Name ?? throw new Exception($"{nameof(Name)} is NULL!");
    }

    public readonly ulong GetSize()
    {
        return Size ?? throw new Exception($"{nameof(Size)} is NULL!");
    }

    public readonly uint GetIndex()
    {
        return Index ?? throw new Exception($"{nameof(Index)} is NULL!");
    }

    public readonly void ReadRawData(long offset, int lenght, out Span<byte> buffer)
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

    public readonly void ReadRawData(long offset, int lenght, out byte[] buffer)
    {
        ReadRawData(offset, lenght, out Span<byte> spanBuffer);
        buffer = spanBuffer.ToArray();
    }

    public readonly void WriteRawData(long offset, ReadOnlySpan<byte> buffer)
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
        if (buffer.Length % MemoryInt.SECTOR != 0)
        {
            throw new Exception($"buffer.Length must be divisible by MemoryInt.SECTOR!");
        }

        using FileStream fs = new(Name, FileMode.Open, FileAccess.ReadWrite);
        fs.Seek(offset, SeekOrigin.Begin);
        fs.Write(buffer);
    }

    public readonly void WriteRawData(long offset, byte[] buffer)
    {
        ReadOnlySpan<byte> spanBuffer = buffer;
        WriteRawData(offset, spanBuffer);
    }

    public readonly void ReadInSteps(long offset, int lenght, out Span<byte> buffer, StepInfo stepInfo)
    {
        int total = (lenght + stepInfo.SectorSize - 1) / stepInfo.SectorSize;

        buffer = new byte[lenght];
        for (int completed = 1; completed <= total; completed++)
        {
            int segmentSize = stepInfo.SectorSize;
            int pre_completed = completed - 1;
            if (completed == total)
            {
                segmentSize = lenght - pre_completed * stepInfo.SectorSize;
            }

            ReadRawData(offset + pre_completed * stepInfo.SectorSize, segmentSize, out Span<byte> spanBuffer);
            spanBuffer.CopyTo(buffer.Slice(pre_completed * stepInfo.SectorSize, segmentSize));

            StepData stepData = new(completed, total);
            stepInfo.Update?.Invoke(stepData);
        }
    }

    public readonly void ReadInSteps(long offset, int lenght, out byte[] buffer, StepInfo stepInfo)
    {
        ReadInSteps(offset, lenght, out Span<byte> spanBuffer, stepInfo);
        buffer = spanBuffer.ToArray();
    }

    public readonly void WriteInSteps(long offset, ReadOnlySpan<byte> buffer, StepInfo stepInfo)
    {
        if (buffer.Length % MemoryInt.SECTOR != 0)
        {
            throw new Exception($"buffer.Length must be divisible by MemoryInt.SECTOR!");
        }

        int total = (buffer.Length + stepInfo.SectorSize - 1) / stepInfo.SectorSize;

        for (int completed = 1; completed <= total; completed++)
        {
            int segmentSize = stepInfo.SectorSize;
            int pre_completed = completed - 1;
            if (completed == total)
            {
                segmentSize = buffer.Length - pre_completed * stepInfo.SectorSize;
            }

            ReadOnlySpan<byte> bufferSlice = buffer.Slice(pre_completed * stepInfo.SectorSize, segmentSize);
            WriteRawData(offset + pre_completed * stepInfo.SectorSize, bufferSlice);

            StepData stepData = new(completed, total);
            stepInfo.Update?.Invoke(stepData);
        }
    }

    public readonly void WriteInSteps(long offset, byte[] buffer, StepInfo stepInfo)
    {
        ReadOnlySpan<byte> spanBuffer = buffer;
        WriteInSteps(offset, spanBuffer, stepInfo);
    }
}
