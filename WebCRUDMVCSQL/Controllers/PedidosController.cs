#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCRUDMVCSQL.Models;

namespace WebCRUDMVCSQL.Controllers
{
    public class PedidosController : Controller
    {
        private readonly Contexto _context;

        public PedidosController(Contexto context)
        {
            _context = context;
        }

        // GET: Pedidos
        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            return View(pedidos);
        }

        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido == null) return NotFound();

            return View(pedido);
        }

        // GET: Pedidos/Create
        public IActionResult Create()
        {
            ViewBag.ClienteId = new SelectList(
                _context.Clientes.Where(c => c.Ativo).OrderBy(c => c.Nome),
                "Id", "Nome"
            );
            ViewBag.ProdutoId = new SelectList(
                _context.Produto.OrderBy(p => p.Nome),
                "Id", "Nome"
            );
            return View();
        }

        // POST: Pedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int clienteId, List<ItemPedidoInput> itens)
        {
            if (itens == null || !itens.Any())
            {
                ModelState.AddModelError("", "Adicione pelo menos um produto ao pedido.");
                RecarregarViewBags(clienteId);
                return View();
            }

            var novoPedido = new Pedidos
            {
                ClienteId = clienteId,
                DataPedido = DateTime.Now,
                Total = 0
            };

            double total = 0;
            foreach (var itemInput in itens)
            {
                var produto = await _context.Produto.FindAsync(itemInput.ProdutoId);
                if (produto != null)
                {
                    total += produto.Preco * itemInput.Quantidade;
                    novoPedido.Itens.Add(new ItemPedido
                    {
                        ProdutoId = itemInput.ProdutoId,
                        Quantidade = itemInput.Quantidade,
                        PrecoUnitario = produto.Preco
                    });
                }
            }

            novoPedido.Total = total;

            _context.Add(novoPedido);
            await _context.SaveChangesAsync();

            var cliente = await _context.Clientes.FindAsync(clienteId);
            TempData["Sucesso"] = $"Pedido #{novoPedido.Id} criado com sucesso para {cliente?.Nome}!";

            return RedirectToAction(nameof(Index));
        }

        // GET: Pedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound();

            ViewBag.ClienteId = new SelectList(
                _context.Clientes.Where(c => c.Ativo).OrderBy(c => c.Nome),
                "Id", "Nome", pedido.ClienteId
            );
            ViewBag.ProdutoId = new SelectList(
                _context.Produto.OrderBy(p => p.Nome),
                "Id", "Nome"
            );

            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int clienteId, List<ItemPedidoInput> itens)
        {
            if (id == 0) return NotFound();

            var pedidoBanco = await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedidoBanco == null) return NotFound();

            if (itens == null || !itens.Any())
            {
                ModelState.AddModelError("", "O pedido deve conter pelo menos um produto.");
                RecarregarViewBags(clienteId);
                return View(pedidoBanco);
            }

            try
            {
                pedidoBanco.ClienteId = clienteId;

                _context.Set<ItemPedido>().RemoveRange(pedidoBanco.Itens);
                pedidoBanco.Itens.Clear();

                double novoTotal = 0;
                foreach (var itemInput in itens)
                {
                    var produto = await _context.Produto.FindAsync(itemInput.ProdutoId);
                    if (produto != null)
                    {
                        novoTotal += produto.Preco * itemInput.Quantidade;
                        pedidoBanco.Itens.Add(new ItemPedido
                        {
                            ProdutoId = itemInput.ProdutoId,
                            Quantidade = itemInput.Quantidade,
                            PrecoUnitario = produto.Preco
                        });
                    }
                }

                pedidoBanco.Total = novoTotal;

                _context.Update(pedidoBanco);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = $"Pedido #{id} atualizado com sucesso!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Pedidos.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido != null)
            {
                _context.Set<ItemPedido>().RemoveRange(pedido.Itens);
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();

                TempData["Aviso"] = $"Pedido #{id} removido com sucesso.";
            }

            return RedirectToAction(nameof(Index));
        }

        //  Métodos privados

        private void RecarregarViewBags(int clienteIdSelecionado = 0)
        {
            ViewBag.ClienteId = new SelectList(
                _context.Clientes.Where(c => c.Ativo).OrderBy(c => c.Nome),
                "Id", "Nome", clienteIdSelecionado
            );
            ViewBag.ProdutoId = new SelectList(
                _context.Produto.OrderBy(p => p.Nome),
                "Id", "Nome"
            );
        }
    }

    // DTO auxiliar
    public class ItemPedidoInput
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}