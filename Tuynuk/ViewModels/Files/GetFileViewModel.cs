namespace Tuynuk.ViewModels.Files
{
    public class GetFileViewModel
    {
        public string Name { get; set; }
        public Stream Content { get; set; }
        public string ContentType { get; } = "application/octet-stream";
    }
}
