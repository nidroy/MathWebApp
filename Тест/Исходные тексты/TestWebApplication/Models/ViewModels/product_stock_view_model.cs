namespace TestWebApplication.Models.ViewModels
{
    public class product_stock_view_model
    {
        public product_stock product_stock { get; set; }
        public IEnumerable<product> products { get; set; }
    }
}
