using AutoMapper;
using MundiFavs.Destinos;
using System;
using Volo.Abp.AutoMapper;



namespace MundiFavs;
 

public class MundiFavsApplicationAutoMapperProfile : Profile
{
    public MundiFavsApplicationAutoMapperProfile()
    {
        CreateMap<Destino, DestinoDto>();
        CreateMap<CreateUpdateDestinoDto, Destino>();

        CreateMap<Coordenadas, CoordenadasDto>(); 
        CreateMap<CoordenadasDto, Coordenadas>();
    }

}


