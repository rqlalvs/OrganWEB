﻿using OrganWeb.Models.Usuario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OrganWeb.Areas.Ecommerce.Models
{
    [Table("tbAnuncio")]
    public class Anuncio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 5)]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [StringLength(100, MinimumLength = 10)]
        public string Desc { get; set; }

        [Required]
        public bool Status { get; set; }

        [Required]
        public byte[] Foto { get; set; }

        [Range(0.00, 100.00)]
        public decimal Desconto { get; set; }

        //TODO: ver se vai ter IdProduto no anúncio
        public int IdProduto { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public string IdUsuario { get; set; }

        public virtual ApplicationUser Usuario { get; set; }
    }
}