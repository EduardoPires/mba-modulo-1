using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace MLV.Business.Commands;

public class ProdutoRequestAlteracao : RequestBase
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public Guid CategoriaId { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public int Estoque { get; set; }
    public IFormFile Imagem { get; set; }

    [JsonIgnore]
    public string CaminhoImagem { get; set; }
    [JsonIgnore]
    public string WebRootPath { get; set; }

    public override bool IsValid()
    {
        ValidationResult = new ProdutoValidation().Validate(this);

        return ValidationResult.IsValid;
    }

    private sealed class ProdutoValidation : AbstractValidator<ProdutoRequestAlteracao>
    {
        public ProdutoValidation()
        {
            RuleFor(m => m.CategoriaId)
                .NotEmpty()
                .WithMessage("O campo {PropertyName} precisa ser fornecido");
            RuleFor(m => m.Nome)
                .NotEmpty()
                .WithMessage("O campo {PropertyName} precisa ser fornecido")
                .MaximumLength(100)
                .WithMessage("O campo {PropertyName} não pode exceder {MaxLength} caracteres");
            RuleFor(m => m.Descricao)
                .MaximumLength(2048)
                .WithMessage("O campo {PropertyName} não pode exceder {MaxLength} caracteres");
            RuleFor(m => m.Valor)
                .GreaterThan(0)
                .WithMessage("O campo {PropertyName} precisa ser fornecido e deve ser maior do que zero")
                .PrecisionScale(10, 2, false)
                .WithMessage("O campo {PropertyName} não pode exceder os {ExpectedScale} dígitos de casas decimais.");
            RuleFor(m => m.Estoque)
                .GreaterThan(0)
                .WithMessage("O campo {PropertyName} precisa ser fornecido e deve ser maior do que zero");
        }
    }
}
