﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using OrganWeb.Models;

namespace OrganWeb.Areas.Sistema.Models
{
    [Table("tbCultura")]
    public class Cultura : Repository<Cultura>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome da Cultura")]
        [StringLength(50, MinimumLength = 3)]
        public string Nome{ get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [StringLength(300, MinimumLength = 10)]
        public string Descricao { get; set; }
    }
}