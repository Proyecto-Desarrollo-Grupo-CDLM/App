using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MundiFavs.Destinos;

public class CreateUpdateDestinoDto
{
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Pais { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Ciudad { get; set; } = string.Empty;

    [Required]
    public int Poblacion { get; set; }

    [Required]
    public decimal Latitud { get; set; }

    [Required]
    public decimal Longitud { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;
}