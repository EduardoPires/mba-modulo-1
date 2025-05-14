using FluentValidation;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace MLV.Business.Commands;

public class CategoriaRequest : RequestBase
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Nome { get; set; }
    [DisplayName("Descrição")]
    public string Descricao { get; set; }

    public CategoriaRequest()
    {
        Id = Guid.NewGuid();
    }

    public override bool IsValid()
    {
        ValidationResult = new CategoriaValidation().Validate(this);

        return ValidationResult.IsValid;
    }

    private sealed class CategoriaValidation : AbstractValidator<CategoriaRequest>
    {
        public CategoriaValidation()
        {
            RuleFor(m => m.Id)
                .NotEmpty()
                .WithMessage("O campo {PropertyName} precisa ser fornecido");
            RuleFor(m => m.Nome)
                .NotEmpty()
                .WithMessage("O campo {PropertyName} precisa ser fornecido")
                .MaximumLength(100)
                .WithMessage("O campo {PropertyName} não pode exceder {MaxLength} caracteres");
            RuleFor(m => m.Descricao)
                .MaximumLength(100)
                .WithMessage("O campo {PropertyName} não pode exceder {MaxLength} caracteres");
        }
    }
}
