using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHBurguer.Applications.Services;
using VHBurguer.Domains;
using VHBurguer.DTOs.CategoriaDto;
using VHBurguer.Exceptions;
using VHBurguer.Interfaces;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace VHBurguer.Tests.Services
{
    public class CategoriaServiceTests
    {
        // Fact marca um metodo como sendo um teste unitario é im atributo do xunit
        [Fact]
        public void Adicionar_DeveGerarErro_QuandoEstiverVazio()
        {
            //Cria um objeto falso (mock) do repositorio
            Mock<ICategoriaRepository> repositoryMock = new Mock<ICategoriaRepository>();

            CategoriaService service = new CategoriaService(repositoryMock.Object);

            CriarCategoriaDto categoriaDto = new CriarCategoriaDto
            {
                Nome = ""
            };

            //Define a acao que sera executada durante o teste
            Action acao = () => service.Adicionar(categoriaDto);

            //Verifica se a execucao lança uma DomainException contendo a mensagem informada para nome obrigatorio
            acao.Should().Throw<DomainException>().WithMessage("Nome é obrigatório.");

        }

        [Fact]
        public void Adicionar_DeveGerarErro_QuandoCategoriaJaExisitir()
        {
            Mock<ICategoriaRepository> repositoryMock = new Mock<ICategoriaRepository>();

            repositoryMock.Setup(c => c.NomeExiste("Lanche", It.IsAny<int?>())).Returns(true);

            CategoriaService service = new CategoriaService(repositoryMock.Object);

            CriarCategoriaDto categoriaDto = new CriarCategoriaDto
            {
                Nome = "Lanche"
            };

            Action acao = () => service.Adicionar(categoriaDto);

            //Verifica se a execucao lança uma DomainException contendo a mensagem informada para nome obrigatorio
            acao.Should().Throw<DomainException>().WithMessage("Categoria já existente.");

        }

    }
}
