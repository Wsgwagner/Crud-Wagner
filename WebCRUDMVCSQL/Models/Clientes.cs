using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebCRUDMVCSQL.Models
{
    [Table("Clientes")]
    public class Clientes
    {
        [Column("Id")]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Column("Nome")]
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string? Nome { get; set; }

        [Column("CPF")]
        [Display(Name = "CPF")]
        [StringLength(14, ErrorMessage = "CPF inválido.")]
        public string? Cpf { get; set; }

        [Column("Telefone")]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [Column("DataNascimento")]
        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Column("Email")]
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            ErrorMessage = "E-mail inválido. Deve conter caracteres antes do @, o símbolo @ e um domínio válido.")]
        public string? Email { get; set; }

        [Column("Endereço")]
        [Display(Name = "Endereço")]
        public string? Endereco { get; set; }

        [Column("Ativo")]
        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        [Column("DataCadastro")]
        [Display(Name = "Data de Cadastro")]
        [DataType(DataType.DateTime)]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Column("DataDesativacao")]
        [Display(Name = "Data de Desativação")]
        [DataType(DataType.DateTime)]
        public DateTime? DataDesativacao { get; set; }
    }
}