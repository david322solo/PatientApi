using PatientApi.Models.Dto;

namespace PatientApi.Services
{
    public interface IPatientService
    {
        Task<PatientDto> CreateAsync(PatientDto dto);
        Task<PatientDto?> GetAsync(Guid id);
        Task<IEnumerable<PatientDto>> GetAllAsync();
        Task<bool> UpdateAsync(Guid id, PatientDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<PatientDto>> SearchByBirthDateAsync(string filter);
    }
}