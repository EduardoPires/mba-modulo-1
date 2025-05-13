using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MLV.Business.Data;
public abstract class Entity
{
    [Required]
    public Guid Id { get; protected set; }
    [DisplayName("Criado Em")]
    [Required]
    public DateTime CriadoEm { get; protected set; }
    [DisplayName("Criado Por")]
    [Required]
    public string UsuarioCriacao { get; protected set; }
    [DisplayName("Atualizado Em")]
    public DateTime? AtualizadoEm { get; protected set; }
    [DisplayName("Alterado Por")]
    public string UsuarioAlteracao { get; protected set; }
}
