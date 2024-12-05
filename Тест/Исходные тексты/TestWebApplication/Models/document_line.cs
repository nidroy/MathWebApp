using System.ComponentModel.DataAnnotations.Schema;

namespace TestWebApplication.Models
{
    /// <summary>
    /// Спецификации (строчки) документов
    /// </summary>
    public class document_line
    {
        public int id { get; set; }

        [ForeignKey("document_header")]
        public int document_header_id { get; set; }
        public document_header document_header { get; set; }

        [ForeignKey("product")]
        public int product_id { get; set; }
        public product product { get; set; }

        public int quantity { get; set; }
        public int reserved_quantity { get; set; }
        public decimal price { get; set; }
        public int discount { get; set; }
    }
}
