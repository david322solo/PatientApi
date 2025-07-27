using PatientApi.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PatientApi.Models
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }

        public UseTypeEnum Use { get; set; } = UseTypeEnum.official;

        [Required]
        public string Family { get; set; }

        public List<string> Given { get; set; } = new();

        public GenderTypeEnum Gender { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public bool Active { get; set; }
    }
}