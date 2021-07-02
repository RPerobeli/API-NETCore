﻿using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.WebApp.Api
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class LivroController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public LivroController(IRepository<Livro> repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult ListarLivros()
        {
            IQueryable<LivroApi> lista = _repo.All.Select(l => l.ToApi());
            return Ok(lista);
        }
        [HttpGet("{id}")]
        public IActionResult Recuperar(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToApi());
        }
        [HttpGet("{id}/capa")]
        public IActionResult ImagemCapa(int id)
        {
            byte[] img = _repo.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        [HttpPost]
        public IActionResult Incluir([FromBody] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                _repo.Incluir(livro);
                var uri = Url.Action("Recuperar", new { id = livro.Id });
                return Created(uri, livro);
            }
            return BadRequest();
        }
        [HttpPut]
        public IActionResult Alterar(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    livro.ImagemCapa = _repo.All.Where(l => l.Id == livro.Id).Select(l => l.ImagemCapa).FirstOrDefault();
                }
                _repo.Alterar(livro);

                return Ok();
            }
            return BadRequest();
        }
        [HttpDelete("{id}")]
        public IActionResult Remover(int id)
        {
            var model = _repo.Find(id);
            if(model == null)
            {
                return NotFound();
            }
            _repo.Excluir();
            return NoContent();
        }
    }
}
