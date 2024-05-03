namespace DiskDrive_Sharp;

public interface IDiskData
{
    public string GetName();
    public ulong GetSize();
    public uint GetIndex();

    public void ReadRawData(long offset, int lenght, out Span<byte> buffer);
    public void ReadRawData(long offset, int lenght, out byte[] buffer);
    public void WriteRawData(long offset, int lenght, ReadOnlySpan<byte> buffer);
    public void WriteRawData(long offset, int lenght, byte[] buffer);

    public void ReadInSteps(long offset, int lenght, out Span<byte> buffer, StepInfo stepInfo);
    public void ReadInSteps(long offset, int lenght, out byte[] buffer, StepInfo stepInfo);
    public void WriteInSteps(long offset, int lenght, ReadOnlySpan<byte> buffer, StepInfo stepInfo);
    public void WriteInSteps(long offset, int lenght, byte[] buffer, StepInfo stepInfo);
}
