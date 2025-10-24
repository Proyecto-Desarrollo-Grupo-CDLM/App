using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace MundiFavs.Usuarios
{
    public class Usuario: AuditedAggregateRoot <Guid>
    {
        public string nombreUsuario { get; private set; }
        public string nombreApellido { get; private set; }
        public string contraseña { get; private set; }
        public string correoElectronico { get; private set; }

        public UsuarioRol rol { get; private set; }

        public Uri fotoPerfil { get; private set; }

        private Usuario() { }
        public Usuario(
            Guid id,
            string nombreUsuario,
            string nombreApellido,
            string contraseña,
            string correoElectronico,
            UsuarioRol rol,
            Uri fotoPerfil
            )
            : base(id)
        {
            this.nombreUsuario = nombreUsuario;
            this.nombreApellido = nombreApellido;
            this.contraseña = contraseña;
            this.correoElectronico = correoElectronico;
            this.rol = rol;
            this.fotoPerfil = fotoPerfil;
        }

    }

    public enum UsuarioRol
    {
        Administrador,
        Usuario
    }
}
