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
    public class ClientesController : Controller
    {
        private readonly Contexto _context;

        public ClientesController(Contexto context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes
                .OrderByDescending(c => c.Ativo)
                .ThenBy(c => c.Nome)
                .ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Cpf,Telefone,DataNascimento,Email,Endereco")] Clientes cliente)
        {
            ValidarEmail(cliente.Email);
            await ValidarCpfDuplicado(cliente.Cpf, idIgnorar: null);

            if (ModelState.IsValid)
            {
                cliente.Nome = FormatarNome(cliente.Nome);
                cliente.Ativo = true;
                cliente.DataCadastro = DateTime.Now;

                _context.Add(cliente);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = $"Cliente {cliente.Nome} cadastrado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Cpf,Telefone,DataNascimento,Email,Endereco,Ativo,DataCadastro,DataDesativacao")] Clientes cliente)
        {
            if (id != cliente.Id)
                return NotFound();

            ValidarEmail(cliente.Email);
            await ValidarCpfDuplicado(cliente.Cpf, idIgnorar: cliente.Id);

            if (ModelState.IsValid)
            {
                try
                {
                    cliente.Nome = FormatarNome(cliente.Nome);
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();

                    TempData["Sucesso"] = $"Cliente {cliente.Nome} atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }

        // POST: Clientes/AlterarStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarStatus(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound();

            cliente.Ativo = !cliente.Ativo;
            cliente.DataDesativacao = cliente.Ativo ? null : DateTime.Now;

            _context.Update(cliente);
            await _context.SaveChangesAsync();

            TempData[cliente.Ativo ? "Sucesso" : "Aviso"] =
                $"Cliente {cliente.Nome} {(cliente.Ativo ? "reativado" : "desativado")} com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        // ── Métodos privados ────────────────────────────────────────────

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        private async Task ValidarCpfDuplicado(string cpf, int? idIgnorar)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return;

            var cpfLimpo = cpf.Replace(".", "").Replace("-", "").Trim();

            var existe = await _context.Clientes
                .AnyAsync(c =>
                    c.Cpf.Replace(".", "").Replace("-", "").Trim() == cpfLimpo &&
                    (idIgnorar == null || c.Id != idIgnorar));

            if (existe)
                ModelState.AddModelError("Cpf", "Já existe um cliente cadastrado com este CPF.");
        }

        private void ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                !email.Contains("@") ||
                email.IndexOf("@") == 0 ||
                email.IndexOf("@") == email.Length - 1)
            {
                ModelState.AddModelError("Email",
                    "E-mail inválido. Informe um endereço completo (ex: nome@dominio.com).");
            }
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