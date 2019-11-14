﻿using System;
using System.Collections.Generic;
using System.Linq;
using PagedList;
using System.Web;
using OrganWeb.Areas.Sistema.Models;
using System.Web.Mvc;
using OrganWeb.Models;
using OrganWeb.Areas.Sistema.Models.Armazenamento;
using OrganWeb.Areas.Sistema.Models.ViewModels;
using System.Net;
using PagedList.EntityFramework;
using OrganWeb.Areas.Sistema.Models.ViewsBanco.Estoque;
using System.Threading.Tasks;
using OrganWeb.Areas.Sistema.Models.ViewsBanco.Pessoa;
using OrganWeb.Areas.Sistema.Models.API;

namespace OrganWeb.Areas.Sistema.Controllers
{
    public class EstoqueController : Controller
    {
        //TODO: categorias máquina
        private Insumo insumo = new Insumo();
        private Estoque estoque = new Estoque();
        private Categoria categoria = new Categoria();
        private VwItems vwItems = new VwItems();
        private ListarUnidades listarUnidades = new ListarUnidades();
        private UnidadeCadastro unidadeCadastro = new UnidadeCadastro();
        private VwFornecedor vwFornecedor = new VwFornecedor();
        private VwHistorico vwHistorico = new VwHistorico();
        private ViewEstoque viewestoque = new ViewEstoque();
        private ListarUnidades unmd = new ListarUnidades();
        private UnidadeCadastro uncd = new UnidadeCadastro();

        //https://stackoverflow.com/questions/25125329/using-a-pagedlist-with-a-viewmodel-asp-net-mvc

        [HttpGet]
        public async Task<ViewResult> Index(string filtros, string textoPesquisa, int? page)
        {
            int pagina = page ?? 1;
            var listaFiltros = new List<string>
            {
                "Insumo",
                "Máquina",
                "Produto",
                "Semente"
            };
            ViewBag.filtros = new SelectList(listaFiltros);

            if (textoPesquisa != null)
            {
                page = 1;
            }
            else
            {
                textoPesquisa = filtros;
            }

            var itens = await vwItems.GetAll();

            viewestoque = new ViewEstoque()
            {
                VwItems = await vwItems.GetPagedAll(pagina),
                VwHistoricos = await vwHistorico.GetAll(),
                Fornecedors = await vwFornecedor.GetAll()
            };

            if (!string.IsNullOrEmpty(textoPesquisa) && !string.IsNullOrEmpty(filtros))
            {
                viewestoque.VwItems = itens.Where(s => s.Tipo == filtros && (s.Item.IndexOf(textoPesquisa, StringComparison.OrdinalIgnoreCase) >= 0)).ToPagedList(pagina, 5);
            }

            else if (!string.IsNullOrEmpty(textoPesquisa))
            {
                viewestoque.VwItems = itens.Where(s => s.Item.IndexOf(textoPesquisa, StringComparison.OrdinalIgnoreCase) >= 0 || s.Categoria.IndexOf(textoPesquisa, StringComparison.OrdinalIgnoreCase) >= 0).ToPagedList(pagina, 5);
            }

            else if (!string.IsNullOrEmpty(filtros))
            {
                viewestoque.VwItems = itens.Where(x => x.Tipo == filtros).ToPagedList(pagina, 5);
            }

            return View(viewestoque);
        }

        public async Task<ActionResult> Create()
        {
            var responseModel = await unmd.GetListarUnidades();
            insumo = new Insumo()
            {
                Estoque = new Estoque
                {
                    Unidades = responseModel.UnidadeCadastros,
                    Fornecedores = await new Fornecedor().GetAll()
                },
                Categorias = await categoria.GetAll()
            };
            return View(insumo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Insumo insumo)
        {
            if (ModelState.IsValid)
            {
                estoque.Add(insumo.Estoque);
                await estoque.Save();

                insumo.IdEstoque = estoque.Id;
                insumo.Estoque = null;
                estoque = null;
                insumo.Add(insumo);
                await insumo.Save();

                return RedirectToAction("Index");
            }
            var ums = await listarUnidades.GetListarUnidades();
            insumo.Categorias = await categoria.GetAll();
            return View(insumo);
        }//TODO: máscara nos campos

        public async Task<ActionResult> Detalhes(int? id, string tipo)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            switch (tipo)
            {
                case "Máquina":
                    return RedirectToAction("Detalhes", "Maquina", new { id });
                case "Produto":
                    return RedirectToAction("Detalhes", "Produto", new { id });
                case "Semente":
                    return RedirectToAction("Detalhes", "Semente", new { id });
            }
            insumo = await insumo.GetByID(id);
            if (insumo == null)
            {
                return HttpNotFound();
            }
            return View(insumo);
        }

        public async Task<ActionResult> Editar(int? id, string tipo)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            switch (tipo)
            {
                case "Máquina":
                    return RedirectToAction("Editar", "Maquina", new { id });
                case "Produto":
                    return RedirectToAction("Editar", "Produto", new { id });
                case "Semente":
                    return RedirectToAction("Editar", "Semente", new { id });
            }
            insumo = await insumo.GetByID(id);
            if (insumo == null)
            {
                return HttpNotFound();
            }
            insumo.Categorias = await categoria.GetAll();
            var responseModel = await unmd.GetListarUnidades();
            insumo.Estoque.Unidades = responseModel.UnidadeCadastros;
            insumo.Estoque.Fornecedores = await new Fornecedor().GetAll();
            return View(insumo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(Insumo insumo)
        {
            if (ModelState.IsValid)
            {
                var responseModel = await unmd.GetListarUnidades();
                insumo.Estoque.Unidades = responseModel.UnidadeCadastros;
                insumo.Update(insumo);
                await insumo.Save();
                if (await unmd.GetByID(insumo.Estoque.UM) == null)
                {
                    insumo.Estoque.Unidades = responseModel.UnidadeCadastros;
                    uncd = new UnidadeCadastro()
                    {
                        Id = insumo.Estoque.UM,
                        Desc = insumo.Estoque.Unidades.Where(x => x.Id == insumo.Estoque.UM).Select(x => x.Desc).FirstOrDefault().ToString()
                    };
                    unmd.Add(uncd);
                    await unmd.Save();
                }
                estoque.Update(insumo.Estoque);
                await estoque.Save();
                return RedirectToAction("Index");
            }
            insumo.Categorias = await categoria.GetAll();
            insumo.Estoque.Fornecedores = await new Fornecedor().GetAll();
            return View(insumo);
        }

        //todo: tentar excluir insumo
    }
}