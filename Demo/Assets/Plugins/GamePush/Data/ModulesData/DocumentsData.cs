namespace GamePush.Data
{
    public class FetchDocumentInput
    {
        public string Type { get; set; }
        public DocumentFormat? Format { get; set; }
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
        public string Type { get; set; }
        public string Content { get; set; }
        public DocumentFormat Format { get; set; }
    }
}