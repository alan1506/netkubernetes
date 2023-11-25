using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetKubernetes.Models;

public class InmuebleModel{
    [Key]
    [Required]
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string?  Direccion { get; set; }
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }
    public string? Pincture { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public Guid? UsuarioId { get; set; }
}