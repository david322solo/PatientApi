using Microsoft.AspNetCore.Mvc;
using PatientApi.Models.Dto;
using PatientApi.Services;
using PatientApi.Validation;

namespace PatientApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _service;
        private readonly PatientDtoValidator _validator;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IPatientService service, PatientDtoValidator validator, ILogger<PatientController> logger)
        {
            _service = service;
            _validator = validator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
        {
            var patients = await _service.GetAllAsync();
            _logger.LogInformation("Запрошен список пациентов");
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatient(Guid id)
        {
            var patient = await _service.GetAsync(id);
            if (patient == null)
            {
                _logger.LogWarning("Пациент с Id: {Id} не найден", id);
                return NotFound();
            }
            _logger.LogInformation("Запрошен пациент с Id: {Id}", id);
            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] PatientDto dto)
        {
            var validationResult = _validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Ошибка валидации при создании пациента: {Errors}", validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }

            var created = await _service.CreateAsync(dto);
            _logger.LogInformation("Пациент создан");
            return CreatedAtAction(nameof(GetPatient), new { id = created.BirthDate }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] PatientDto dto)
        {
            var validationResult = _validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Ошибка валидации при обновлении пациента: {Errors}", validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }

            var updated = await _service.UpdateAsync(id, dto);
            if (!updated)
            {
                _logger.LogWarning("Пациент с Id: {Id} не найден для обновления", id);
                return NotFound();
            }
            _logger.LogInformation("Пациент с Id: {Id} обновлён", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Пациент с Id: {Id} не найден для удаления", id);
                return NotFound();
            }
            _logger.LogInformation("Пациент с Id: {Id} удалён", id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> SearchByBirthDate([FromQuery(Name = "birthDate")] string birthDateFilter)
        {
            var patients = await _service.SearchByBirthDateAsync(birthDateFilter);
            _logger.LogInformation("Поиск пациентов по фильтру даты: {Filter}", birthDateFilter);
            return Ok(patients);
        }
    }
}
