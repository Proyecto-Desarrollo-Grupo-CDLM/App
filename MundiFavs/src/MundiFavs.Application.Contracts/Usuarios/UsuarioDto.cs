using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace MundiFavs.Usuarios
{
    public class UsuarioPublicoDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } 
        public string Name { get; set; }
        public string Surname { get; set; }
    }
 
   
}
