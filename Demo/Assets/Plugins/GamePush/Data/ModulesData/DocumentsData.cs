namespace GamePush.Data
{
    public class FetchDocumentInput
    {
        public string type { get; set; }
        // public string? Format { get; set; }
    }

    public enum DocumentFormat
    {
        RAW,
        TXT,
        HTML
    }

    public enum DocumentType
    {
        PLAYER_PRIVACY_POLICY
    }

    public class DocumentData
    {
        public string type { get; set; }
        public string content { get; set; }
        public DocumentFormat Format { get; set; }
    }
}