using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace MundiFavs.Usuarios
{
    public class UsuarioDto: AuditedEntityDto<Guid>
    {
        public string NombreUsuario { get; set; }
        public string NombreApellido { get; set; }
        public string Contraseña { get; set; }
        public string CorreoElectronico { get; set; }
        public UsuarioRol Rol { get; set; }
        public Uri FotoPerfil { get; set; }
    }
    public enum UsuarioRol
    {
        Administrador,
        Usuario
    }
    
}
