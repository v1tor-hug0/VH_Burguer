using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VHBurguer.Applications.Services;
using VHBurguer.DTOs.ProdutoDto;
using VHBurguer.Exceptions;

namespace VHBurguer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _service;

        public ProdutoController(ProdutoService service)
        {
            _service = service;
        }

        // autenticação do usuário
        private int ObterUsuarioLogado()
        {
            //Busca no token/claims o valor armazenado como id do usuario
            //ClaimTypes.NameIdentifier geralmeente guarda o id do usuario no JWT
            string? idTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(string.IsNullOrEmpty(idTexto))
            {
                throw new Exception("Usuário não autenticado.");
            }

            return int.Parse(idTexto);
        }

        [HttpGet]
        public ActionResult<List<LerProdutoDto>> Listar()
        {
            List<LerProdutoDto> produtos = _service.Listar();

            //return StatusCode(200, produtos);
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public ActionResult<LerProdutoDto> ObterPorId(int id)
        {
            LerProdutoDto produto = _service.ObterPorId(id);

            if(produto == null) { 
                //return StatusCode(404);
                return NotFound();
            }

            return Ok(produto);
        }

        //GET -> api/produto/usuario
        [HttpGet("{id}/imagem")]
        public ActionResult ObterImagem(int id)
        {
            try
            {
                var imagem = _service.ObterImagem(id);
                return File(imagem, "image/jpeg");
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }

        }
}
