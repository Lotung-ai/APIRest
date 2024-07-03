using System;
using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain
{
    public class CurvePoint
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "CurveId is required.")]
        [Range(1, 255, ErrorMessage = "CurveId must be between 1 and 255.")]
        public byte? CurveId { get; set; }

        [Required(ErrorMessage = "AsOfDate is required.")]
        public DateTime? AsOfDate { get; set; }

        [Required(ErrorMessage = "Term is required.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Term must be a non-negative value.")]
        public double? Term { get; set; }

        [Required(ErrorMessage = "CurvePointValue is required.")]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "CurvePointValue must be a valid number.")]
        public double? CurvePointValue { get; set; }

        [Required(ErrorMessage = "CreationDate is required.")]
        public DateTime? CreationDate { get; set; }
    }
}
