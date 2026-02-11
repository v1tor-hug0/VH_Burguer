using VHBurguer.Domains;
using VHBurguer.DTOs;
using VHBurguer.Exceptions;
using VHBurguer.Interfaces;

namespace VHBurguer.Applications.Services
{
    public class UsuarioService
    {
        //_repository é o canal para acessar os dados do banco de dados, ou seja, é o canal para acessar os métodos do repository
        private readonly IUsuarioRepository _repository;

        //Injeção de dependência do repository, ou seja, o repository é injetado no service para que o service possa acessar os métodos do repository
        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;

        }

        //Pq private ?
        //pq o metodo nao e regrea de negocio e nao faz sentido existir fora do service, ou seja, ele é um metodo auxiliar para o service e nao faz sentido existir fora do service

        private static LerUsuarioDto LerDto(Usuario usuario)
        {
            LerUsuarioDto lerUsuario = new LerUsuarioDto
            {
                UsuarioID = usuario.UsuarioID,
                Nome = usuario.Nome,
                Email = usuario.Email,
                StatusUsuario = usuario.StatusUsuario ?? true
            };
            return lerUsuario;
        }

        public List<LerUsuarioDto> Listar()
        {
            List<Usuario> usuario = _repository.Listar();

            List<LerUsuarioDto> usuarioDto = usuario.Select(usuarioBanco => LerDto(usuarioBanco)).ToList();  //Select que perorre cada Usuario e LerDto(usuario) //
                                                                                                             //ToList() para converter em uma lista
            return usuarioDto;
        }

        private static void ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new DomainException("Email invalido. ");
            }

        }

        private static byte[] HashSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))  //Garante que a senha nao esta vazia
            {
                throw new DomainException("A senha é obrigatória.");
            }

            using (var sha256 = System.Security.Cryptography.SHA256.Create())  // importar o namespace System.Security.Cryptography para usar o SHA256
            {
                return sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha)); //importar o namespace System.Text 
            }
        }

        public LerUsuarioDto ObterPorId(int id)
        {
            Usuario? usuario = _repository.ObterPorID(id);
            if (usuario == null)
            {
                throw new DomainException("Usuario não encontrado. ");
            }
            return LerDto(usuario); //Se existe usuario, converte para DTO e retorna 
        }

        public LerUsuarioDto ObterPorEmail(string email)
        {
            Usuario? usuario = _repository.ObterPorEmail(email);
            if (usuario == null)
            {
                throw new DomainException("Usuario não encontrado. ");
            }
            return LerDto(usuario); //Se existe usuario, converte para DTO e retorna 
        }
    }
}
