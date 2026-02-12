using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VHBurguer.Applications.Services;
using VHBurguer.DTOs;
using VHBurguer.Exceptions;

namespace VHBurguer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuarioController(UsuarioService service)
        {
            _service = service;
        }

        [HttpGet] //GET => Lista informaçoes
        public ActionResult<List<LerUsuarioDto>> Listar()
        {
            List<LerUsuarioDto> usuario = _service.Listar();
            return Ok(usuario);
        }

        [HttpGet("{id}")]
        public ActionResult<LerUsuarioDto> ObterPorId(int id)
        {
            LerUsuarioDto usuario = _service.ObterPorId(id);
            if (usuario == null)
            {
                return NotFound(); // Retorna 404 Not Found se o usuário não for encontrado
            }
            return Ok(usuario);

        }

        [HttpGet("email/{email}")]
        public ActionResult<LerUsuarioDto> ObterPorEmail(string email)
        {
            LerUsuarioDto usuario = _service.ObterPorEmail(email);
            if (usuario == null)
            {
                return NotFound(); // Retorna 404 Not Found se o usuário não for encontrado
            }
            return Ok(usuario);
        }

        [HttpPost]
        public ActionResult<LerUsuarioDto> Adicionar(CriarUsuarioDto UsuarioDto)
        {
            try
            {
                LerUsuarioDto usuarioCriado = _service.Adicionar(UsuarioDto);
                return StatusCode(201, usuarioCriado); // Retorna 201 Created com o usuário criado
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message); // Retorna 400 Bad Request com a mensagem de erro

            }
        }

        [HttpPut("{id}")]
        public ActionResult<LerUsuarioDto> Atualizar(int id, CriarUsuarioDto UsuarioDto)
        {
            try
            {
                LerUsuarioDto usuarioAtualizado = _service.Atualizar(id, UsuarioDto);
                return Ok(usuarioAtualizado); // Retorna 200 OK com o usuário atualizado
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message); // Retorna 400 Bad Request com a mensagem de erro
            }

        }


        //Remove os dados no banco mas so vai inativar o usuario, ativando a trigger de inativaçao do usuario, para nao perder os dados relacionados a ele, como pedidos e etc
        [HttpDelete("{id}")]
        public ActionResult Remover(int id)
        {
            try
            {
                _service.Remover(id);
                return NoContent(); // Retorna 204 No Content para indicar que a exclusão foi bem-sucedida
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message); // Retorna 400 Bad Request com a mensagem de erro
            }
        }
    }
}
