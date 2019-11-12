﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrganWeb.Models;
using OrganWeb.Areas.Sistema.Models.Safras;
using System.Threading.Tasks;
using OrganWeb.Areas.Sistema.Models.Administrativo;
using OrganWeb.Areas.Sistema.Models.zBanco;

namespace OrganWeb.Areas.Sistema.Models
{
    public class PlantioRepository : OrganRepository<Plantio>
    {
        private List<Plantio> Plantios;
        private List<Colheita> Colheitas;
        private List<AreaPlantio> AreaPlantios;
        public async Task<List<Plantio>> GetPlantios()
        {
            Plantios = await GetAll();
            Colheitas = await new Colheita().GetAll();
            AreaPlantios = await new AreaPlantio().GetAll();
            return Plantios.Select((p) => new Plantio
            {
                Porcentagem = ProgressoPlantio(p),
                Id = p.Id,
                Nome = p.Nome,
                Sistema = p.Sistema,
                TipoPlantio = p.TipoPlantio,
                DataInicio = p.DataInicio,
                DataColheita = p.DataColheita,
                NomeAreas = GetAreaPlantios(p, AreaPlantios)
            }).ToList();
        }

        public List<Plantio> GetPlantiosIncompletos()
        {
            Plantios.RemoveAll(p => Colheitas.Any(c => c.IdPlantio == p.Id));
            return Plantios;
        }

        public string GetAreaPlantios(Plantio plantio, List<AreaPlantio> areaPlantios)
        {
            List<string> nomes = areaPlantios.Where(x => x.Plantio.Id == plantio.Id).Select(a => a.Area.Nome).ToList();
            return string.Join(", ", nomes.Select(x => x.ToString()).ToArray());
        }

        private double ProgressoPlantio(Plantio plantio)
        {
            DateTime hoje = DateTime.Today;
            int diasAgoracomeco = 0;
            try
            {
                //agora - começo
                if (hoje > plantio.DataInicio)
                    diasAgoracomeco = hoje.Subtract(plantio.DataInicio).Days;

                //fim - começo
                int diasFimcomeco = plantio.DataColheita.Subtract(plantio.DataInicio).Days;

                int progresso = ((100 * diasAgoracomeco) / diasFimcomeco);

                if (progresso > 100)
                    return 100;

                return progresso;
            }
            catch
            {
                return 100;
            }
        }
    }
}