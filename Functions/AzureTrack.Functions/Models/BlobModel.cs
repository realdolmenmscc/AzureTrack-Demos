namespace AzureTrack.Functions.Models
{
    public sealed class BlobModel
    {
        public byte[] Blob { get; set; }
        public string Name { get; set; }
        public AnalysisResult Analysis { get; set; }

        public void ApplyAnalysis(AnalysisResult analysis)
        {
            if (!string.IsNullOrEmpty(analysis.Brand))
            {
                Name = $"{analysis.Brand}/{Name}";
            }

            Analysis = analysis;
        }
    }
}
