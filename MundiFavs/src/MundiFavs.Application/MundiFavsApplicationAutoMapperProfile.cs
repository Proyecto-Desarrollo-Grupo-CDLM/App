using AutoMapper;
using MundiFavs.Destinos;

namespace MundiFavs;

public class MundiFavsApplicationAutoMapperProfile : Profile
{
    public MundiFavsApplicationAutoMapperProfile()
    {
        CreateMap<Destino, DestinoDto>();
        CreateMap<CreateUpdateDestinoDto, Destino>();
    }
}
