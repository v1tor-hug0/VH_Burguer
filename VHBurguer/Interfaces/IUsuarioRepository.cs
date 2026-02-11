using VHBurguer.Domains;

namespace VHBurguer.Interfaces
{
    public interface IUsuarioRepository
    {
        List<Usuario> Listar();
        //Pode ser que nao venha nenhum usuario na busca entao colocamos o tipo de retorno como Usuario? para indicar que pode ser nulo
        Usuario? ObterPorID(int id);

        Usuario? ObterPorEmail(string email);

        bool EmailExiste(string email);

        void Adicionar(Usuario usuario);

        void Atualizar(Usuario usuario);

        void Remover(int id);
    }
}
