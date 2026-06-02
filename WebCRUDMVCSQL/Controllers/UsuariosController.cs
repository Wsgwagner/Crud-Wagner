using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCRUDMVCSQL.Models;

namespace WebCRUDMVCSQL.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly Contexto _context;

        public UsuariosController(Contexto context)
        {
            _context = context;
        }

        // GET: Usuarios/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Usuarios/Login
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(string email, string senha)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                ViewBag.Erro = "Por favor, preencha o E-mail e a Senha.";
                return View();
            }

            // 1. Buscamos o usuário no banco APENAS pelo e-mail
            var usuario = await _context.Set<Usuarios>()
                .FirstOrDefaultAsync(u => u.Email == email);

            // 2. Se o usuário existir, usamos o BCrypt para verificar se a senha bate com o Hash
            if (usuario != null)
            {                
                bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.Senha);

                if (senhaValida)
                {
                    return RedirectToAction("Index", "Produtos");
                }
            }

            // Se o usuário não existir OU a senha não bater, exibe o erro genérico (por segurança)
            ViewBag.Erro = "E-mail ou senha inválidos.";
            return View();
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Cpf,Telefone,DataNascimento,Email,Endereco,Senha")] Usuarios Usuarios)
        {
            if (ModelState.IsValid)
            {
                var emailExiste = await _context.Set<Usuarios>().AnyAsync(u => u.Email == Usuarios.Email);
                if (emailExiste)
                {
                    ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
                    return View(Usuarios);
                }

                if (!string.IsNullOrWhiteSpace(Usuarios.Nome))
                {
                    String nomeFormatado = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Usuarios.Nome.ToLower());
                    Usuarios.Nome = nomeFormatado;
                }

                // Transforma a senha limpa (ex: "123456") em um hash seguro de 60 caracteres
                Usuarios.Senha = BCrypt.Net.BCrypt.HashPassword(Usuarios.Senha);

                _context.Add(Usuarios);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Login));
            }

            return View(Usuarios);
        }
        // GET: Usuarios/Sair
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sair()
        {
            return RedirectToAction(nameof(Login));
        }
    }
}