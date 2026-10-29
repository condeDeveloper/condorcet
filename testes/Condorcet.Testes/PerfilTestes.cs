using Conde.Condorcet;
using Xunit;

namespace Conde.Condorcet.Testes;

public class PerfilTestes
{
    [Fact]
    public void SaoSeisCedulasComTresCandidatos()
    {
        Assert.Equal(6, Perfil.Cedulas.Length);
        var textos = Perfil.Cedulas.Select(c => string.Join("", c)).ToList();
        Assert.Equal(6, textos.Distinct().Count());
    }

    [Fact]
    public void TodaCedulaOrdenaTodosOsCandidatos()
    {
        foreach (var c in Perfil.Cedulas)
        {
            Assert.Equal([0, 1, 2], c.Order().ToArray());
        }
    }

    [Fact]
    public void OTotalDeEleitoresESomaDasContagens()
    {
        Assert.Equal(10, new Perfil([1, 2, 3, 4, 0, 0]).Eleitores);
        Assert.Equal(0, new Perfil([0, 0, 0, 0, 0, 0]).Eleitores);
    }

    [Fact]
    public void OsDuelosSomamOTotalDeEleitores()
    {
        var p = new Perfil([3, 1, 4, 1, 5, 2]);
        for (var a = 0; a < 3; a++)
        {
            for (var b = a + 1; b < 3; b++)
            {
                Assert.Equal(p.Eleitores, p.Preferem(a, b) + p.Preferem(b, a));
            }
        }
    }

    [Fact]
    public void ODueloEAComparacaoDasDuasContagens()
    {
        var p = new Perfil([5, 0, 0, 0, 0, 2]);
        Assert.Equal(1, p.Duelo(0, 1));
        Assert.Equal(-1, p.Duelo(1, 0));
        Assert.Equal(0, new Perfil([1, 0, 0, 0, 0, 1]).Duelo(0, 2));
    }

    [Fact]
    public void CadaEleitorOcupaUmaPosicaoDeCadaCandidato()
    {
        var p = new Perfil([2, 3, 1, 4, 0, 5]);
        for (var c = 0; c < 3; c++)
        {
            var soma = 0;
            for (var pos = 0; pos < 3; pos++)
            {
                soma += p.ContaNaPosicao(c, pos);
            }

            Assert.Equal(p.Eleitores, soma);
        }
    }

    [Fact]
    public void CadaPosicaoERecebidaPorTodosOsEleitores()
    {
        var p = new Perfil([2, 3, 1, 4, 0, 5]);
        for (var pos = 0; pos < 3; pos++)
        {
            var soma = 0;
            for (var c = 0; c < 3; c++)
            {
                soma += p.ContaNaPosicao(c, pos);
            }

            Assert.Equal(p.Eleitores, soma);
        }
    }

    [Fact]
    public void ContagemNegativaOuTamanhoErradoNaoEAceito()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Perfil([-1, 0, 0, 0, 0, 0]));
        Assert.Throws<ArgumentException>(() => new Perfil([1, 2, 3]));
        Assert.Throws<ArgumentNullException>(() => new Perfil(null!));
    }
}
