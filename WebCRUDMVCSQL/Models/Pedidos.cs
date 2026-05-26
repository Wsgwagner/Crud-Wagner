using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebCRUDMVCSQL.Models
{
    [Table("Pedidos")]
    public class Pedidos
    {
        [Key]
        [Column("Id")]
        [Display(Name = "Número do Pedido")]
        public int Id { get; set; }

        [Required]
        [Column("ClienteId")]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Clientes? Cliente { get; set; }

        [Required]
        [Column("DataPedido")]
        [Display(Name = "Data do Pedido")]
        public DateTime DataPedido { get; set; }

        [Required]
        [Column("Total")]
        [Display(Name = "Total (R$)")]
        public double Total { get; set; }

        public virtual ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
    }

    [Table("ItensPedido")]
    public class ItemPedido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [ForeignKey("PedidoId")]
        public virtual Pedidos? Pedidos { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public virtual Produto? Produto { get; set; }

        [Required]
        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        [Required]
        [Display(Name = "Preço Unitário")]
        public double PrecoUnitario { get; set; }
    }
}