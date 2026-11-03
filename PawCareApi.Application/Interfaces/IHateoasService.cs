using PawCareApi.Application.DTOs.Clinicas;
using PawCareApi.Application.DTOs.Pets;
using PawCareApi.Application.DTOs.Prontuarios;
using PawCareApi.Application.DTOs.Tutores;
using PawCareApi.Domain.Common;

namespace PawCareApi.Application.Interfaces
{
    public interface IHateoasService
    {
        PetResponseDto EnriquecerPet(PetResponseDto pet, string baseUrl);
        PageResult<PetResponseDto> EnriquecerListaPets(PageResult<PetResponseDto> pageResult, string baseUrl);
        
        TutorResponseDto EnriquecerTutor(TutorResponseDto tutor, string baseUrl);
        PageResult<TutorResponseDto> EnriquecerListaTutores(PageResult<TutorResponseDto> pageResult, string baseUrl);
        
        ClinicaResponseDto EnriquecerClinica(ClinicaResponseDto clinica, string baseUrl);
        PageResult<ClinicaResponseDto> EnriquecerListaClinicas(PageResult<ClinicaResponseDto> pageResult, string baseUrl);

        ProntuarioResponseDto EnriquecerProntuario(ProntuarioResponseDto prontuario, string baseUrl);
        PageResult<ProntuarioResponseDto> EnriquecerListaProntuarios(PageResult<ProntuarioResponseDto> pageResult, string baseUrl);
    }
}

