using Microsoft.EntityFrameworkCore;
using MLV.Business.Data.Repository.Interfaces;
using MLV.Business.Models;
using MLV.Core.Repository;

namespace MLV.Business.Data.Repository;

public class ProdutoRepository(MlvDbContext context) : IProdutoRepository
{
    private bool _disposedValue;

    public IUnitOfWork UnitOfWork => context;

    public void Adicionar(Produto model)
    {
        context.Produtos.Add(model);
    }

    public void Atualizar(Produto model)
    {
        context.Produtos.Update(model);
    }

    public void Remover(Produto model)
    {
        context.Produtos.Remove(model);
    }

    public async Task<List<Produto>> ObterTodos(Guid? categoriaId)
    {
        return await context.Produtos
            .Where(a => !categoriaId.HasValue || a.CategoriaId == categoriaId.Value)
            .Include(a => a.Categoria)
            .Include(a => a.Vendedor)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Produto> ObterPorId(Guid id)
    {
        return await context.Produtos
            .Include(a => a.Categoria)
            .Include(a => a.Vendedor)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Produto>> ObterPorVendedorId(Guid vendedorId)
    {
        return await context.Produtos
            .Where(p => p.VendedorId == vendedorId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Produto>> ObterPorCategoriaId(Guid categoriaId)
    {
        return await context.Produtos
            .Where(p => p.CategoriaId == categoriaId)
            .AsNoTracking()
            .ToListAsync();
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                context.Dispose();
            }

            _disposedValue = true;
        }
    }
}
