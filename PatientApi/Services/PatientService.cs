using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PatientApi.Data;
using PatientApi.Models;
using PatientApi.Models.Dto;

namespace PatientApi.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientService> _logger;

        public PatientService(AppDbContext context, IMapper mapper, ILogger<PatientService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PatientDto> CreateAsync(PatientDto dto)
        {
            var patient = _mapper.Map<Patient>(dto);
            patient.Id = Guid.NewGuid();
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Создан пациент с Id: {Id}", patient.Id);
            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto?> GetAsync(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                _logger.LogWarning("Пациент с Id: {Id} не найден", id);
                return null;
            }
            _logger.LogInformation("Получен пациент с Id: {Id}", id);
            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<IEnumerable<PatientDto>> GetAllAsync()
        {
            var patients = await _context.Patients.ToListAsync();
            _logger.LogInformation("Получен список всех пациентов. Количество: {Count}", patients.Count);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<IEnumerable<PatientDto>> SearchByBirthDateAsync(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter) || filter.Length < 4)
                return Enumerable.Empty<PatientDto>();

            var op = filter.Substring(0, 2);
            var value = filter.Substring(2);

            if (!DateTime.TryParse(value, out var date))
                return Enumerable.Empty<PatientDto>();

            IQueryable<Patient> query = _context.Patients;

            switch (op)
            {
                case "eq":
                    query = query.Where(p => p.BirthDate.Date == date.Date);
                    break;
                case "ne":
                    query = query.Where(p => p.BirthDate.Date != date.Date);
                    break;
                case "lt":
                    query = query.Where(p => p.BirthDate < date);
                    break;
                case "gt":
                    query = query.Where(p => p.BirthDate > date);
                    break;
                case "ge":
                    query = query.Where(p => p.BirthDate >= date);
                    break;
                case "le":
                    query = query.Where(p => p.BirthDate <= date);
                    break;
                case "sa":
                    query = query.Where(p => p.BirthDate > date);
                    break;
                case "eb":
                    query = query.Where(p => p.BirthDate < date);
                    break;
                case "ap":
                    query = query.Where(p => Math.Abs((p.BirthDate - date).TotalDays) <= 1);
                    break;
                default:
                    return Enumerable.Empty<PatientDto>();
            }

            var patients = await query.ToListAsync();
            _logger.LogInformation("Поиск пациентов по фильтру: {Filter}. Найдено: {Count}", filter, patients.Count);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<bool> UpdateAsync(Guid id, PatientDto dto)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                _logger.LogWarning("Обновление: пациент с Id: {Id} не найден", id);
                return false;
            }

            _mapper.Map(dto, patient);
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Обновлен пациент с Id: {Id}", id);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                _logger.LogWarning("Удаление: пациент с Id: {Id} не найден", id);
                return false;
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Удалён пациент с Id: {Id}", id);
            return true;
        }
    }
}