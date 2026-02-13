using AutoMapper;
using MundiFavs.ApiMetrics;
using MundiFavs.Calificaciones; // Aseg�rate de tener este using
using MundiFavs.Destinos;
using MundiFavs.Calificaciones; // Aseg�rate de tener este using
using MundiFavs.Destinos;
using MundiFavs.Experiencias;
using MundiFavs.Usuarios;
using System;
using System;
using Volo.Abp.AutoMapper;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using MundiFavs.Eventos;
using MundiFavs.Notificaciones;


namespace MundiFavs;

public class MundiFavsApplicationAutoMapperProfile : Profile
{
    public MundiFavsApplicationAutoMapperProfile()
    {
     
        CreateMap<Destino, DestinoDto>();

        CreateMap<CreateUpdateDestinoDto, Destino>()
            .ForMember(dest => dest.Ubicacion,
                opt => opt.MapFrom(src => new Coordenadas(src.Latitud, src.Longitud)))
            .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(src => new Uri(src.ImageUrl)));

        CreateMap<Coordenadas, CoordenadasDto>();
        CreateMap<CoordenadasDto, Coordenadas>();

        CreateMap<Evento, EventoDto>();
        CreateMap<Notificacion, NotificacionDto>();

        CreateMap<Calificacion, CalificacionDto>();
        CreateMap<CreateUpdateCalificacionDto, Calificacion>();


        CreateMap<IdentityUser, UsuarioPublicoDto>();

        CreateMap<ApiMetric, ApiMetricDto>();
       
        CreateMap<Experiencia, ExperienciaDto>();
        CreateMap<CreateUpdateExperienciaDto, Experiencia>();

    }
}