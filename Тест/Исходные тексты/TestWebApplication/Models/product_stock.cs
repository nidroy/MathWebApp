using System.ComponentModel.DataAnnotations.Schema;

namespace TestWebApplication.Models
{
    /// <summary>
    /// Остатки товара на складе
    /// </summary>
    public class product_stock
    {
        public int id { get; set; }

        [ForeignKey("product")]
        public int product_id { get; set; }
        public product product { get; set; }

        public int actual_quantity { get; set; }
        public int reserved_quantity { get; set; }
    }
}
