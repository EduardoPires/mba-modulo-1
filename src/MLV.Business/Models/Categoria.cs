using MLV.Business.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MLV.Business.Models;

public class Categoria : Entity
{
    [Required]
    public string Nome { get; private set; }
    [DisplayName("Descrição")]
    public string Descricao { get; private set; }

    public Categoria(Guid id, string nome, string descricao, string usuarioCriacao)
    {
        Id = id;
        Nome = nome.Trim();
        Descricao = descricao?.Trim();
        UsuarioCriacao = usuarioCriacao;
    }

    public void Atualizar(string nome, string descricao, string usuarioLogado)
    {
        Nome = nome.Trim();
        Descricao = descricao?.Trim();
        UsuarioAlteracao = usuarioLogado;
    }
}
