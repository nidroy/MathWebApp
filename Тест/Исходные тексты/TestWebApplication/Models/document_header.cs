using System.ComponentModel.DataAnnotations.Schema;

namespace TestWebApplication.Models
{
    /// <summary>
    /// Шапки документов
    /// </summary>
    public class document_header
    {
        public int id { get; set; }
        public string document_number { get; set; }

        [ForeignKey("counterparty")]
        public int counterparty_id { get; set; }
        public counterparty counterparty { get; set; }

        private DateTime _date;
        public DateTime document_date
        {
            get => _date;
            set => _date = DateTime.SpecifyKind(value, DateTimeKind.Utc); // Установка UTC
        }
        public decimal document_amount { get; set; }

        public string document_type { get; set; } // Приход, Резерв, Расход
        public string document_status { get; set; } // Черновик, Оприходовано, Зарезервировано, Списано
    }
}
