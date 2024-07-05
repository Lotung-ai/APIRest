using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain
{
    public class Trade
    {
        public int TradeId { get; set; }

        [Required(ErrorMessage = "Account is required.")]
        [StringLength(100, ErrorMessage = "Account length can't be more than 100.")]
        public string Account { get; set; }

        [Required(ErrorMessage = "AccountType is required.")]
        [StringLength(50, ErrorMessage = "AccountType length can't be more than 50.")]
        public string AccountType { get; set; }

        public double? BuyQuantity { get; set; }

        public double? SellQuantity { get; set; }

        public double? BuyPrice { get; set; }

        public double? SellPrice { get; set; }

        public DateTime? TradeDate { get; set; }

        [Required(ErrorMessage = "TradeSecurity is required.")]
        [StringLength(100, ErrorMessage = "TradeSecurity length can't be more than 100.")]
        public string TradeSecurity { get; set; }

        [Required(ErrorMessage = "TradeStatus is required.")]
        [StringLength(50, ErrorMessage = "TradeStatus length can't be more than 50.")]
        public string TradeStatus { get; set; }

        [Required(ErrorMessage = "Trader is required.")]
        [StringLength(100, ErrorMessage = "Trader length can't be more than 100.")]
        public string Trader { get; set; }

        [StringLength(100, ErrorMessage = "Benchmark length can't be more than 100.")]
        public string Benchmark { get; set; }

        [StringLength(100, ErrorMessage = "Book length can't be more than 100.")]
        public string Book { get; set; }

        [StringLength(100, ErrorMessage = "CreationName length can't be more than 100.")]
        public string CreationName { get; set; }

        public DateTime? CreationDate { get; set; }

        [StringLength(100, ErrorMessage = "RevisionName length can't be more than 100.")]
        public string RevisionName { get; set; }

        public DateTime? RevisionDate { get; set; }

        [StringLength(100, ErrorMessage = "DealName length can't be more than 100.")]
        public string DealName { get; set; }

        [StringLength(50, ErrorMessage = "DealType length can't be more than 50.")]
        public string DealType { get; set; }

        [StringLength(100, ErrorMessage = "SourceListId length can't be more than 100.")]
        public string SourceListId { get; set; }

        [StringLength(10, ErrorMessage = "Side length can't be more than 10.")]
        public string Side { get; set; }
    }
}
