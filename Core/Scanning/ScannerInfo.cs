namespace DocumentArchiever.Core.Scanning
{
    public class ScannerInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ScannerType Type { get; set; }
        public bool SupportsDuplex { get; set; }
        public bool SupportsFeeder { get; set; }
        public object NativeDevice { get; set; }

        public override string ToString()
        {
            return $"[{Type}] {Name}";
        }
    }

    public enum ScannerType
    {
        Twain,
        Wia
    }
}