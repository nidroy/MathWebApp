namespace TestWebApplication.Models.ViewModels
{
    public class document_header_view_model
    {
        public document_header document_header { get; set; }
        public IEnumerable<counterparty> counterparties { get; set; }
        public IEnumerable<string> document_types { get; set; }
        public IEnumerable<string> document_statuses { get; set; }
    }
}
