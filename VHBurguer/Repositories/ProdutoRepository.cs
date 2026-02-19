using Microsoft.EntityFrameworkCore;
using VHBurguer.Contexts;
using VHBurguer.Domains;
using VHBurguer.Interfaces;

namespace VHBurguer.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly VH_BurguerContext _context;

        public ProdutoRepository(VH_BurguerContext context)
            {
                _context = context;
        }

        public List<Produto> Listar()
        {
            List<Produto> produtos = _context.Produto.Include(produto => produto.Categoria)
                .Include(produto => produto.Usuario).ToList(); //busca produtos e para cada produto, busca as categorias relacionadas a ele
            return produtos;
        }

        public Produto ObterPorId(int id)
        {
            Produto? produto = _context.Produto.Include(produtoDb => produtoDb.Categoria)
                .Include(produtoDb => produtoDb.Usuario)
                //Procura no banco com o aux produtoDb onde o ID do produto for igual ao id passado por parâmetro no metodo ObeterPorId
                .FirstOrDefault(produtoDb => produtoDb.ProdutoID == id);
            return produto;
        }

        public bool NomeExiste(string nome, int? ProdutoIdAtual = null)
        {
            var produtoConsultado = _context.Produto.AsQueryable(); // AsQueryable -> Monta a consulta passo a passo, nao executa a consulta no banco ainda
            
            if(ProdutoIdAtual.HasValue)
            {
                produtoConsultado = produtoConsultado.Where(p => p.ProdutoID != ProdutoIdAtual.Value);
            }
            return produtoConsultado.Any(produto => produto.Nome == nome);
        }

        public byte[] ObterImagem(int id)
        {
            var produto = _context.Produto.Where(p => p.ProdutoID == id).Select(p => p.Imagem).FirstOrDefault();
            return produto;
        }

        public void Adicionar(Produto produto, List<int> CategoriaIds)
        {
            List<Categoria> categorias = _context.Categoria.Where(categoria => CategoriaIds.Contains(categoria.CategoriaID)).ToList();// Contains -> Retornar true se houver registro

            produto.Categoria = categorias;
            _context.Produto.Add(produto);
            _context.SaveChanges();
        }

        public void Atualizar(Produto produto, List<int> CategoriaIds)
        {
            Produto? produtoBanco = _context.Produto.Include(produto => produto.Categoria).FirstOrDefault(produto => produto.ProdutoID == produto.ProdutoID);

            if (produtoBanco == null)
                return;

            produtoBanco.Nome = produto.Nome;
            produtoBanco.Preco = produto.Preco;
            produtoBanco.Descricao = produto.Descricao;

            if (produto.Imagem != null && produto.Imagem.Length > 0 )
                produtoBanco.Imagem = produto.Imagem;

            if (produto.StatusProduto.HasValue)
                produtoBanco.StatusProduto = produto.StatusProduto;


            //Busca todas as categorias no banco com o id igual das categorias que vieram da requisição/front
            var categorias = _context.Categoria.Where(categoria => CategoriaIds.Contains(categoria.CategoriaID)).ToList();

            //Clear() -> Remove as ligações atuais entre o produto e as categorias
            //ele não apaga a categoria do banco so remove o vinculo com a tabela produtoCategoria
            produtoBanco.Categoria.Clear();

            foreach(var categoria in categorias)
            {
                produtoBanco.Categoria.Add(categoria);
            }

            _context.SaveChanges();
        }


        public void Remover(int id)
        {
            Produto produto = _context.Produto.FirstOrDefault(produto => produto.ProdutoID == id);

            if (produto == null)
                return;

            _context.Produto.Remove(produto);
            _context.SaveChanges();
        }
    }
}
