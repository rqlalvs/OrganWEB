﻿using OrganWeb.Areas.Ecommerce.Models.Financeiro;
using OrganWeb.Areas.Ecommerce.Models.zRepositories;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganWeb.Areas.Ecommerce.Models.Vendas
{
    [Table("tbVenda")]
    public class Venda : VendaRepository
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Data { get; set; }

        public byte[] Contrato { get; set; }

        [Required]
        [ForeignKey("Endereco")]
        [RegularExpression(@"[0-9]{5}[\d]{3}", ErrorMessage = "Digite um CEP válido somente com números")]
        [StringLength(8, MinimumLength = 8)]
        public string CEP { get; set; }
        
        [Required]
        [Range(0, 9999)]
        public int NumEndereco { get; set; }
        
        [StringLength(30)]
        public string CompEndereco { get; set; }

        [Required]
        [ForeignKey("Pagamento")]
        public int IdPagamento { get; set; }

        [Required]
        [ForeignKey("Pedido")]
        public int IdPedido { get; set; }

        public virtual Pagamento Pagamento { get; set; }
        public virtual Pedido Pedido { get; set; }
        public virtual Endereco.Endereco Endereco { get; set; }
    }
}