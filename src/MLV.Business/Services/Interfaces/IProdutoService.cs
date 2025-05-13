using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using MLV.Business.Commands;
using MLV.Business.Models;

namespace MLV.Business.Services.Interfaces;

public interface IProdutoService
{
    Task<IEnumerable<Produto>> ObterProdutosPorVendedorId();
    Task<ValidationResult> Adicionar(ProdutoRequest request);
    Task<ValidationResult> Alterar(ProdutoRequestAlteracao request);
    Task<ValidationResult> Remover(Guid id);
}
