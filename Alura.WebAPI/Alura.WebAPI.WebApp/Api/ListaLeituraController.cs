using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.WebAPI.WebApp.Api
{
    [Authorize]
    [ApiController]
    [Route("/api/[Controller]")]
    public class ListaLeituraController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public ListaLeituraController(ListaLeitura.Persistencia.IRepository<Livro> repo)
        {
            _repo = repo;
        }

        [HttpGet("{tipo}")]
        public IActionResult Recuperar(TipoListaLeitura tipo)
        {
            var lista = CriaLista(tipo);
            return Ok(lista);
        }
        [HttpGet]
        public IActionResult ListarLivros()
        {
            Lista paraler = CriaLista(TipoListaLeitura.ParaLer);
            Lista lendo = CriaLista(TipoListaLeitura.Lendo);
            Lista lido = CriaLista(TipoListaLeitura.Lidos);
            var colecao = new List<Lista> { paraler, lendo, lido };
            return Ok(colecao);
        }

        private Lista CriaLista(TipoListaLeitura tipo)
        {
            return new Lista
            {
                Tipo = tipo.ParaString(),
                Livros = _repo.All.Where(l => l.Lista == tipo).Select(l => l.ToApi()).ToList()
            };
        }
    }
}
