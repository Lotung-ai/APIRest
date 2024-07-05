using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain
{
    public class RuleName
    {
        // TODO: Map columns in data table RULENAME with corresponding fields

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }

        [StringLength(1000, ErrorMessage = "Json cannot be longer than 1000 characters.")]
        public string Json { get; set; }

        [StringLength(500, ErrorMessage = "Template cannot be longer than 500 characters.")]
        public string Template { get; set; }

        [StringLength(1000, ErrorMessage = "SqlStr cannot be longer than 1000 characters.")]
        public string SqlStr { get; set; }

        [StringLength(500, ErrorMessage = "SqlPart cannot be longer than 500 characters.")]
        public string SqlPart { get; set; }
    }
}
