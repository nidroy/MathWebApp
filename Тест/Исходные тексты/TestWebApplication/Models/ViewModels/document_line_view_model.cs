namespace TestWebApplication.Models.ViewModels
{
    public class document_line_view_model
    {
        public document_line document_line { get; set; }
        public IEnumerable<document_header> document_headers { get; set; }
        public IEnumerable<product> products { get; set; }
    }
}
