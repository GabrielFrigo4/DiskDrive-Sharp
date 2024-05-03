using System.Management;

namespace DiskDrive_Sharp;

public struct DiskDrive(ManagementBaseObject managementBaseObject)
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

    public Dictionary<string, DiskDrivePartition> DiskDrivePartitions { get; private set; } = [];

    public readonly byte[] ReadRawData(long offset, int lenght)
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

        byte[] buffer = new byte[lenght];
        using (FileStream fs = new(Name, FileMode.Open, FileAccess.Read))
        {
            fs.Seek(offset, SeekOrigin.Begin);
            fs.Read(buffer, 0, lenght);
        }
        return buffer;
    }

    public readonly void WriteRawData(byte[] buffer, long offset, int lenght)
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

        using FileStream fs = new(Name, FileMode.Open, FileAccess.ReadWrite);
        fs.Seek(offset, SeekOrigin.Begin);
        fs.Write(buffer, 0, lenght);
    }
}
