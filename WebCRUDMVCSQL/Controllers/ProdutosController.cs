#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCRUDMVCSQL.Models;

namespace WebCRUDMVCSQL.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly Contexto _context;

        public ProdutosController(Contexto context)
        {
            _context = context;
        }

        // GET: Produtos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Produto
                .OrderBy(p => p.Nome)
                .ToListAsync());
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var produto = await _context.Produto
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null)
                return NotFound();

            return View(produto);
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Peso,Preco")] Produto produto)
        {
            await ValidarNomeDuplicado(produto.Nome, idIgnorar: null);

            if (ModelState.IsValid)
            {
                produto.Nome = FormatarNome(produto.Nome);

                _context.Add(produto);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = $"Produto {produto.Nome} cadastrado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }

        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var produto = await _context.Produto.FindAsync(id);

            if (produto == null)
                return NotFound();

            return View(produto);
        }

        // POST: Produtos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Peso,Preco")] Produto produto)
        {
            if (id != produto.Id)
                return NotFound();

            await ValidarNomeDuplicado(produto.Nome, idIgnorar: produto.Id);

            if (ModelState.IsValid)
            {
                try
                {
                    produto.Nome = FormatarNome(produto.Nome);
                    _context.Update(produto);
                    await _context.SaveChangesAsync();

                    TempData["Sucesso"] = $"Produto {produto.Nome} atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }

        // POST: Produtos/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produto.FindAsync(id);

            if (produto == null)
                return NotFound();

            var possuiPedidos = await _context.Set<ItemPedido>()
                .AnyAsync(i => i.ProdutoId == id);

            if (possuiPedidos)
            {
                TempData["Erro"] = $"O produto \"{produto.Nome}\" não pode ser excluído pois está vinculado a um ou mais pedidos.";
                return RedirectToAction(nameof(Index));
            }

            _context.Produto.Remove(produto);
            await _context.SaveChangesAsync();

            TempData["Aviso"] = $"Produto {produto.Nome} removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // ── Métodos privados ────────────────────────────────────────────

        private bool ProdutoExists(int id)
        {
            return _context.Produto.Any(e => e.Id == id);
        }

        private async Task ValidarNomeDuplicado(string nome, int? idIgnorar)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return;

            var nomeLimpo = nome.Trim().ToUpper();

            var existe = await _context.Produto
                .AnyAsync(p =>
                    p.Nome.Trim().ToUpper() == nomeLimpo &&
                    (idIgnorar == null || p.Id != idIgnorar));

            if (existe)
                ModelState.AddModelError("Nome", "Já existe um produto cadastrado com este nome.");
        }

        private string FormatarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return nome;

            return System.Globalization.CultureInfo
                .CurrentCulture.TextInfo
                .ToTitleCase(nome.ToUpper());
        }
    }
}