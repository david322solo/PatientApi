using AutoMapper;
using PatientApi.Models;
using PatientApi.Models.Dto;

namespace PatientApi.Mapping
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<PatientDto, Patient>();
            CreateMap<Patient, PatientDto>();
        }
    }
}