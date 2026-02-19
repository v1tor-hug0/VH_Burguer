using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using VHBurguer.Domains;

namespace VHBurguer.Applications.Autenticacao
{
    public class GeradorTokenJwt
    {
        private readonly IConfiguration _config;

        // Recebe as configurações do appsettings.json através do construtor
        public GeradorTokenJwt(IConfiguration config)
        {
            _config = config;
        }

        
        public string GerarToken(Usuario usuario)
        {
            // Key -> Chave secreta usada para assinar o token garante que o token nao foi alterado
            var chave = _config["Jwt:Key"]!;

            // Issuer -> Emissor do token (nome da API / sistema que gerou) a API valida se o token foi gerado por ela mesma
            var issuer = _config["Jwt:Issuer"]!;

            // Audience -> Pra quem o token foi criado define ual sistema pode usar o token (ex: frontend, mobile) a API valida se o token é destinado para ela ou para outro sistema
            var audience = _config["Jwt:Audience"]!;

            // tempo de expiração do token em minutos define por quanto tempo o token é válido a API valida se o token ainda é válido ou se já expirou
            var expiraEmMinutos = int.Parse(_config["Jwt:ExpireInMinutes"]!);

            //Converte a chave para bytes 
            var keyBytes = Encoding.UTF8.GetBytes(chave);

            //Segurança exige uma chave que tenha pelo menos 32 caracteres
            if(keyBytes.Length < 32)
            {
                throw new Exception("Jwt: Key precisa ter pelo menos 32 caracteres (256 BITS)");
            }

            //Cria a chave de segurança usada para assinar o token
            var securityKey = new SymmetricSecurityKey(keyBytes);
        
            //Define o algoritmo de assinatura do token 
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Claims -> Informações do usuario que vao dentro do token, essas informações podem ser recuperadas na API para identificar quem esta logado e quais permissoes ele tem
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),

                new Claim(ClaimTypes.Name, usuario.Nome),

                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var token = new JwtSecurityToken(
                
                issuer: issuer, //Quem gerou o token
                audience: audience, // Quem pode usar o token
                claims: claims, // Informações do usuario que vao dentro do token
                expires: DateTime.UtcNow.AddMinutes(expiraEmMinutos), // Tempo de expiração do token
                signingCredentials: credentials // Assinatura do token para garantir que ele não foi alterado
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
