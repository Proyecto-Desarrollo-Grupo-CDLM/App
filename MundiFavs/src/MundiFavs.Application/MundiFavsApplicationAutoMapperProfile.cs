using AutoMapper;
using MundiFavs.ApiMetrics;
using MundiFavs.Calificaciones; // Aseg�rate de tener este using
using MundiFavs.Destinos;
using MundiFavs.Usuarios;
using System;
using System;
using Volo.Abp.AutoMapper;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;


namespace MundiFavs;

public class MundiFavsApplicationAutoMapperProfile : Profile
{
    public MundiFavsApplicationAutoMapperProfile()
    {
        // --- DESTINOS (Esto se mantiene igual) ---
        CreateMap<Destino, DestinoDto>();

        CreateMap<CreateUpdateDestinoDto, Destino>()
            .ForMember(dest => dest.Ubicacion,
                opt => opt.MapFrom(src => new Coordenadas(src.Latitud, src.Longitud)))
            .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(src => new Uri(src.ImageUrl)));

        CreateMap<Coordenadas, CoordenadasDto>();
        CreateMap<CoordenadasDto, Coordenadas>();

        // --- CALIFICACIONES (EL CAMBIO DEL PASO 2) ---

        // Como ahora en el Paso 1 le pusimos "Puntuacion" al DTO,
        // ya coincide con la Entidad. No hace falta configurar nada extra.

        CreateMap<Calificacion, CalificacionDto>();
        CreateMap<CreateUpdateCalificacionDto, Calificacion>();


        CreateMap<IdentityUser, UsuarioPublicoDto>();

        CreateMap<ApiMetric, ApiMetricDto>();
    }
}