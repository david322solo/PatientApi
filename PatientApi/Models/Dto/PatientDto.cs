using PatientApi.Models.Enums;

namespace PatientApi.Models.Dto
{
    public class PatientDto
    {
        public UseTypeEnum Use { get; set; }
        public string Family { get; set; } = string.Empty;
        public List<string> Given { get; set; } = new();
        public GenderTypeEnum Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Active { get; set; }
    }
}