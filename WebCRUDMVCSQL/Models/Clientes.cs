using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
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

        public string? Nome { get; set; }

        [Column("CPF")]
        [Display(Name = "CPF")]
        public string? Cpf { get; set; }

        [Column("Telefone")]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }
        [Column("DataNascimento")]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [Column("Email")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Column("Endereço")]
        [Display(Name = "Endereço")]
        public string? Endereco { get; set; }



    }
}
