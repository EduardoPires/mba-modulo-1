﻿using Microsoft.EntityFrameworkCore;
using MLV.Business.Interfaces;
using MLV.Business.Models;
using MLV.Core.Repository;

namespace MLV.Infra.Data.Repository;

public class CategoriaRepository(MlvDbContext context) : ICategoriaRepository
{
    private bool _disposedValue;

    public IUnitOfWork UnitOfWork => context;

    public void Adicionar(Categoria model)
    {
        context.Categorias.Add(model);
    }

    public void Atualizar(Categoria model)
    {
        context.Categorias.Update(model);
    }

    public void Remover(Categoria model)
    {
        context.Categorias.Remove(model);
    }

    public async Task<List<Categoria>> ObterTodos()
    {
        return await context.Categorias
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Categoria> ObterPorId(Guid id)
    {
        return await context.Categorias
            .FindAsync(id);
    }

    public async Task<Categoria> ObterPorNome(string nome)
    {
        return await context.Categorias
            .FirstOrDefaultAsync(c => c.Nome.ToUpper().Equals(nome.ToUpper()));
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
