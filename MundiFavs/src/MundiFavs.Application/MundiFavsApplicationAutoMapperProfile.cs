using AutoMapper;
using MundiFavs.Calificaciones;
using MundiFavs.Destinos;
using System;
using Volo.Abp.AutoMapper;
using MundiFavs.Usuarios;


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

        CreateMap<Calificacion, CalificacionDto>();
        CreateMap<CreateUpdateCalificacionDto, Calificacion>();

   
    }

}


