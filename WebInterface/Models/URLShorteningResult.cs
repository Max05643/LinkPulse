namespace WebInterface.Models
{
    public class URLShorteningResult
    {
        public bool Success { get; set; }
        public string? ShortenedUrl { get; set; }
        public int? TimeToExpireInSec { get; set; }
    }
}
