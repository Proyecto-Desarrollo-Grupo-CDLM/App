using AutoMapper;
using MundiFavs.Destinos;
using MundiFavs.Calificaciones; // Aseg�rate de tener este using
using System;
using Volo.Abp.AutoMapper;
using MundiFavs.Usuarios;
using System;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using MundiFavs.Eventos;
using MundiFavs.Notificaciones;


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

        CreateMap<Evento, EventoDto>();
        CreateMap<Notificacion, NotificacionDto>();

        CreateMap<Calificacion, CalificacionDto>();
        CreateMap<CreateUpdateCalificacionDto, Calificacion>();


        CreateMap<IdentityUser, UsuarioPublicoDto>();

    }
}