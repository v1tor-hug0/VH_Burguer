using VHBurguer.Contexts;
using VHBurguer.Interfaces;
using VHBurguer.Domains;

namespace VHBurguer.Interfaces.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly VH_BurguerContext _context;

        public UsuarioRepository(VH_BurguerContext context)
        {
            _context = context;
        }

        public List<Usuario> Listar()
        {
            return _context.Usuario.ToList();
        }

        public Usuario? ObterPorID(int id)
        {
            return _context.Usuario.Find(id);
        }

        public Usuario? ObterPorEmail(string email)
        {
            return _context.Usuario.FirstOrDefault(Usuario => Usuario.Email == email);
        }

        public bool EmailExiste(string email)
        {
            return _context.Usuario.Any(Usuario => Usuario.Email == email);
        }

        public void Adicionar(Usuario usuario)
        {
            _context.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        public void Atualizar(Usuario usuario)
        {
            Usuario? usuarioBanco = _context.Usuario.FirstOrDefault(usuarioAux => usuarioAux.UsuarioID == usuario.UsuarioID);
            if (usuarioBanco == null)
            {
                return;
            }

            usuarioBanco.Nome = usuario.Nome;
            usuarioBanco.Email = usuario.Email;
            usuarioBanco.Senha = usuario.Senha;
            _context.SaveChanges();
        }

        public void Remover(int id)
        {
            Usuario? usuario = _context.Usuario.FirstOrDefault(usuarioAux => usuarioAux.UsuarioID == id);
            if (usuario == null)
            {
                return;
            }
            _context.Usuario.Remove(usuario);
            _context.SaveChanges();

        }
    }
}
