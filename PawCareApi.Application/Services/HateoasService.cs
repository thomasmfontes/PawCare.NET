using PawCareApi.Application.DTOs.Clinicas;
using PawCareApi.Application.DTOs.Pets;
using PawCareApi.Application.DTOs.Prontuarios;
using PawCareApi.Application.DTOs.Tutores;
using PawCareApi.Application.Interfaces;
using PawCareApi.Domain.Common;

namespace PawCareApi.Application.Services
{
    public class HateoasService : IHateoasService
    {
        public PetResponseDto EnriquecerPet(PetResponseDto pet, string baseUrl)
        {
            var baseEndpoint = $"{baseUrl.TrimEnd('/')}/api/pets";
            pet.Links.Clear();

            pet.Links.Add(new LinkDto("self", $"{baseEndpoint}/{pet.Id}", "GET"));
            pet.Links.Add(new LinkDto("update", $"{baseEndpoint}/{pet.Id}", "PUT"));
            pet.Links.Add(new LinkDto("delete", $"{baseEndpoint}/{pet.Id}", "DELETE"));

            if (!string.IsNullOrWhiteSpace(pet.TutorCpf))
            {
                pet.Links.Add(new LinkDto("tutor", $"{baseUrl.TrimEnd('/')}/api/tutores/{pet.TutorCpf}", "GET"));
            }

            pet.Links.Add(new LinkDto("prontuarios-nosql", $"{baseUrl.TrimEnd('/')}/api/prontuarios/pet/{pet.Id}", "GET"));

            return pet;
        }

        public PageResult<PetResponseDto> EnriquecerListaPets(PageResult<PetResponseDto> pageResult, string baseUrl)
        {
            foreach (var item in pageResult.Items)
            {
                EnriquecerPet(item, baseUrl);
            }

            return pageResult;
        }

        public TutorResponseDto EnriquecerTutor(TutorResponseDto tutor, string baseUrl)
        {
            var baseEndpoint = $"{baseUrl.TrimEnd('/')}/api/tutores";
            tutor.Links.Clear();

            tutor.Links.Add(new LinkDto("self", $"{baseEndpoint}/{tutor.Cpf}", "GET"));
            tutor.Links.Add(new LinkDto("update", $"{baseEndpoint}/{tutor.Cpf}", "PUT"));
            tutor.Links.Add(new LinkDto("delete", $"{baseEndpoint}/{tutor.Cpf}", "DELETE"));
            tutor.Links.Add(new LinkDto("pets", $"{baseUrl.TrimEnd('/')}/api/pets?tutorCpf={tutor.Cpf}", "GET"));

            return tutor;
        }

        public PageResult<TutorResponseDto> EnriquecerListaTutores(PageResult<TutorResponseDto> pageResult, string baseUrl)
        {
            foreach (var item in pageResult.Items)
            {
                EnriquecerTutor(item, baseUrl);
            }

            return pageResult;
        }

        public ClinicaResponseDto EnriquecerClinica(ClinicaResponseDto clinica, string baseUrl)
        {
            var baseEndpoint = $"{baseUrl.TrimEnd('/')}/api/clinicas";
            clinica.Links.Clear();

            clinica.Links.Add(new LinkDto("self", $"{baseEndpoint}/{clinica.Id}", "GET"));
            clinica.Links.Add(new LinkDto("update", $"{baseEndpoint}/{clinica.Id}", "PUT"));
            clinica.Links.Add(new LinkDto("delete", $"{baseEndpoint}/{clinica.Id}", "DELETE"));

            return clinica;
        }

        public PageResult<ClinicaResponseDto> EnriquecerListaClinicas(PageResult<ClinicaResponseDto> pageResult, string baseUrl)
        {
            foreach (var item in pageResult.Items)
            {
                EnriquecerClinica(item, baseUrl);
            }

            return pageResult;
        }

        public ProntuarioResponseDto EnriquecerProntuario(ProntuarioResponseDto prontuario, string baseUrl)
        {
            var baseEndpoint = $"{baseUrl.TrimEnd('/')}/api/prontuarios";
            prontuario.Links.Clear();

            prontuario.Links.Add(new LinkDto("self", $"{baseEndpoint}/{prontuario.Id}", "GET"));
            prontuario.Links.Add(new LinkDto("update", $"{baseEndpoint}/{prontuario.Id}", "PUT"));
            prontuario.Links.Add(new LinkDto("delete", $"{baseEndpoint}/{prontuario.Id}", "DELETE"));
            prontuario.Links.Add(new LinkDto("pet-relacional", $"{baseUrl.TrimEnd('/')}/api/pets/{prontuario.PetId}", "GET"));

            return prontuario;
        }

        public PageResult<ProntuarioResponseDto> EnriquecerListaProntuarios(PageResult<ProntuarioResponseDto> pageResult, string baseUrl)
        {
            foreach (var item in pageResult.Items)
            {
                EnriquecerProntuario(item, baseUrl);
            }

            return pageResult;
        }
    }
}

