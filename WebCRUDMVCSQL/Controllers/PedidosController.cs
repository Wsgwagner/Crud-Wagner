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
            // Busca os pedidos incluindo os dados do cliente para mostrar na tabela
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .ToListAsync();

            return View(pedidos);
        }

        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Buscamos o pedido com o Cliente e também a lista completa de Itens com seus respectivos Produtos
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
            ViewBag.ClienteId = new SelectList(_context.Clientes, "Id", "Nome");
            ViewBag.ProdutoId = new SelectList(_context.Produto, "Id", "Nome");
            return View();
        }

        // POST: Pedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int clienteId, List<ItemPedidoInput> itens)
        {
            if (itens == null || !itens.Any())
            {
                ModelState.AddModelError("", "É necessário adicionar pelo menos um produto ao pedido.");
                ViewBag.ClienteId = new SelectList(_context.Clientes, "Id", "Nome", clienteId);
                ViewBag.ProdutoId = new SelectList(_context.Produto, "Id", "Nome");
                return View();
            }

            double valorTotalPedido = 0;
            var novoPedido = new Pedidos
            {
                ClienteId = clienteId,
                DataPedido = DateTime.Now,
                Total = 0
            };

            foreach (var itemInput in itens)
            {
                var produto = await _context.Produto.FindAsync(itemInput.ProdutoId);
                if (produto != null)
                {
                    double subtotalItem = produto.Preco * itemInput.Quantidade;
                    valorTotalPedido += subtotalItem;

                    var itemPedido = new ItemPedido
                    {
                        ProdutoId = itemInput.ProdutoId,
                        Quantidade = itemInput.Quantidade,
                        PrecoUnitario = produto.Preco
                    };
                    novoPedido.Itens.Add(itemPedido);
                }
            }

            novoPedido.Total = valorTotalPedido;

            _context.Add(novoPedido);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Pedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // AJUSTADO: Carrega o pedido trazendo a lista de itens e produtos completa para o carrinho da View
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null) return NotFound();

            ViewBag.ClienteId = new SelectList(_context.Clientes, "Id", "Nome", pedido.ClienteId);
            ViewBag.ProdutoId = new SelectList(_context.Produto, "Id", "Nome");

            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // AJUSTADO: Agora aceita a lista modificada de itens vinda da tela
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
                ViewBag.ClienteId = new SelectList(_context.Clientes, "Id", "Nome", clienteId);
                ViewBag.ProdutoId = new SelectList(_context.Produto, "Id", "Nome");
                return View(pedidoBanco);
            }

            try
            {
                // 1. Atualiza o vínculo do cliente
                pedidoBanco.ClienteId = clienteId;

                // 2. Limpa os itens antigos associados para evitar duplicidade ou registros órfãos
                _context.Set<ItemPedido>().RemoveRange(pedidoBanco.Itens);
                pedidoBanco.Itens.Clear();

                // 3. Recalcula o total baseado no preço atual do banco e adiciona o novo carrinho
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
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Pedidos.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Pedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
                // Remove os itens vinculados primeiro por causa da integridade da chave estrangeira
                foreach (var item in pedido.Itens.ToList())
                {
                    _context.Set<ItemPedido>().Remove(item);
                }

                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }

    // DTO auxiliar posicionado corretamente no escopo do namespace
    public class ItemPedidoInput
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}