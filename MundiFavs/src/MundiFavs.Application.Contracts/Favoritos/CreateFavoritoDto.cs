using System;
using System.ComponentModel.DataAnnotations;

namespace MundiFavs.Favoritos
{
    public class CreateFavoritoDto
    {
        [Required]
        public Guid DestinoId { get; set; }
    }
}