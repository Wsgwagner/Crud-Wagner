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
                    return RedirectToAction("Index", "Home");
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
        public async Task<IActionResult> Create([Bind("Id,Nome,Cpf,Telefone,DataNascimento,Email,Endereco,Senha")] Usuarios usuarios)
        {
            // 1. Validação manual do e-mail (camada extra de segurança)
            if (string.IsNullOrWhiteSpace(usuarios.Email) ||
                !usuarios.Email.Contains("@") ||
                usuarios.Email.IndexOf("@") == 0)
            {
                ModelState.AddModelError("Email", "E-mail inválido. Informe caracteres antes do @.");
            }

            // 2. Verificações de Duplicidade (Banco de Dados)
            if (!string.IsNullOrWhiteSpace(usuarios.Email))
            {
                bool emailExiste = await _context.Set<Usuarios>().AnyAsync(u => u.Email == usuarios.Email);
                if (emailExiste)
                {
                    ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
                }
            }

            if (!string.IsNullOrWhiteSpace(usuarios.Cpf))
            {
                bool cpfExiste = await _context.Set<Usuarios>().AnyAsync(u => u.Cpf == usuarios.Cpf);
                if (cpfExiste)
                {
                    ModelState.AddModelError("Cpf", "Este CPF já está cadastrado.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(usuarios);
            }

            // 4. Formatação do Nome (Title Case)
            if (!string.IsNullOrWhiteSpace(usuarios.Nome))
            {
                string nomeFormatado = System.Globalization.CultureInfo.CurrentCulture.TextInfo
                    .ToTitleCase(usuarios.Nome.ToUpper()); 
                usuarios.Nome = nomeFormatado;
            }

            // 5. Criptografia da Senha
            usuarios.Senha = BCrypt.Net.BCrypt.HashPassword(usuarios.Senha);

            // 6. Salvar no Banco
            _context.Add(usuarios);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Login));
        }


        //  Usuarios/Sair
        public IActionResult Sair()
        {
            return RedirectToAction(nameof(Login));
        }
    }
}