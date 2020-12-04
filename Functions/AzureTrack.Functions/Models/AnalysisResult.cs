namespace AzureTrack.Functions.Models
{
    public sealed class AnalysisResult
    {
        public bool IsRejectedContent { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
    }
}
