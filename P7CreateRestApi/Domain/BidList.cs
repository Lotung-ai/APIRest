using System;
using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain
{
    public class BidList
    {
        public int BidListId { get; set; }
        [Required(ErrorMessage = "Account is required.")]
        [StringLength(50, ErrorMessage = "Account length can't be more than 50.")]
        public string Account { get; set; }
        [Required(ErrorMessage = "BidType is required.")]
        [StringLength(20, ErrorMessage = "BidType length can't be more than 20.")]
        public string BidType { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "BidQuantity must be a positive value.")]
        public double? BidQuantity { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "AskQuantity must be a positive value.")]
        public double? AskQuantity { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Bid must be a positive value.")]
        public double? Bid { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Ask must be a positive value.")]
        public double? Ask { get; set; }
        [StringLength(100, ErrorMessage = "Benchmark length can't be more than 100.")]
        public string Benchmark { get; set; }
        public DateTime? BidListDate { get; set; }
        [StringLength(250, ErrorMessage = "Commentary length can't be more than 250.")]
        public string Commentary { get; set; }
        [StringLength(100, ErrorMessage = "BidSecurity length can't be more than 100.")]
        public string BidSecurity { get; set; }
        [StringLength(20, ErrorMessage = "BidStatus length can't be more than 20.")]
        public string BidStatus { get; set; }
        [StringLength(50, ErrorMessage = "Trader length can't be more than 50.")]
        public string Trader { get; set; }
        [StringLength(50, ErrorMessage = "Book length can't be more than 50.")]
        public string Book { get; set; }
        [StringLength(50, ErrorMessage = "CreationName length can't be more than 50.")]
        public string CreationName { get; set; }
        public DateTime? CreationDate { get; set; }
        [StringLength(50, ErrorMessage = "RevisionName length can't be more than 50.")]
        public string RevisionName { get; set; }
        public DateTime? RevisionDate { get; set; }
        [StringLength(50, ErrorMessage = "DealName length can't be more than 50.")]
        public string DealName { get; set; }
        [StringLength(20, ErrorMessage = "DealType length can't be more than 20.")]
        public string DealType { get; set; }
        [StringLength(50, ErrorMessage = "SourceListId length can't be more than 50.")]
        public string SourceListId { get; set; }
        [StringLength(10, ErrorMessage = "Side length can't be more than 10.")]
        public string Side { get; set; }
    }
}
