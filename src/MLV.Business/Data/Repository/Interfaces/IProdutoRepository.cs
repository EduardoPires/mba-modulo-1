using MLV.Business.Models;
using MLV.Core.Repository;

namespace MLV.Business.Data.Repository.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<Produto> ObterPorId(Guid id);
    Task<List<Produto>> ObterPorVendedorId(Guid vendedorId);
    Task<List<Produto>> ObterPorCategoriaId(Guid categoriaId);
    Task<List<Produto>> ObterTodos(Guid? categoriaId);
    void Adicionar(Produto model);
    void Atualizar(Produto model);
    void Remover(Produto model);
}
