﻿using OrganWeb.Areas.Ecommerce.Models.Vendas;
using OrganWeb.Areas.Sistema.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrganWeb.Areas.Sistema.Controllers
{
    [Authorize]
    public class VendasController : Controller
    {
        private Pedido pedido = new Pedido();
        private PedidoAnuncio pedidoAnuncio = new PedidoAnuncio();
        private Venda venda = new Venda();

        public async Task<ActionResult> Index(int? pagep, int? pagev)
        {//TODO: filtro de pedido e venda por status, pagedlist
            int pagepedido = pagep ?? 1;
            int pagevenda = pagev ?? 1;
            var select = new ViewVendas
            {
                Pedidos = await pedidoAnuncio.GetPedidosAnunciante(pagepedido),
                Vendas = await venda.GetVendasDoAnunciante(pagevenda)
            };
            return View();
        }

        public async Task<ActionResult> AceitarPedido(int? idPedido)
        {
            if (idPedido == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pedido = await pedido.GetByID(idPedido);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        [HttpPost, ActionName("AceitarPedido")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CriarVenda(Pedido pedido)
        {
            pedido.Status = 2;
            pedido.Update(pedido);
            await pedido.Save();

            //TODO: criar venda aqui

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> RecusarPedido(int? idPedido)
        {
            if (idPedido == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pedido = await pedido.GetByID(idPedido);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RecusarPedido(Pedido pedido)
        {
            pedido.Status = 9;
            pedido.Update(pedido);
            await pedido.Save();
            return View();
        }
    }
}