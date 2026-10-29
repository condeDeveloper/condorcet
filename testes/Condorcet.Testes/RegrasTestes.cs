using Conde.Condorcet;
using Xunit;

namespace Conde.Condorcet.Testes;

public class RegrasTestes
{
    [Fact]
    public void QuandoTodosConcordamTodasAsRegrasElegemOMesmo()
    {
        var p = new Perfil([9, 0, 0, 0, 0, 0]);
        Assert.Equal([0], Regras.Pluralidade(p));
        Assert.Equal([0], Regras.Borda(p));
        Assert.Equal([0], Regras.Copeland(p));
        Assert.Equal([0], Regras.SegundoTurno(p));

        // A antipluralidade e a excecao, e ela nao esta errada: contando so
        // ultimos lugares, A e B tem zero cada um e sao indistinguiveis. A
        // regra que olha menos informacao empata onde as outras decidem.
        Assert.Equal([0, 1], Regras.AntiPluralidade(p));
    }

    [Fact]
    public void NoCicloCopelandEmpataOsTres()
    {
        var p = new Perfil([1, 0, 0, 1, 1, 0]);
        Assert.Equal([0, 1, 2], Regras.Copeland(p));
        Assert.Equal([0, 1, 2], Regras.Borda(p));
    }

    [Fact]
    public void TodaRegraDevolvePeloMenosUmNome()
    {
        foreach (var (urna, _) in Urnas.Todas(5))
        {
            Assert.NotEmpty(Regras.Pluralidade(urna));
            Assert.NotEmpty(Regras.Borda(urna));
            Assert.NotEmpty(Regras.Copeland(urna));
            Assert.NotEmpty(Regras.SegundoTurno(urna));
            Assert.NotEmpty(Regras.AntiPluralidade(urna));
        }
    }

    [Fact]
    public void TodaRegraDevolveNomesValidosESemRepetir()
    {
        foreach (var (urna, _) in Urnas.Todas(4))
        {
            foreach (var eleitos in new[]
            {
                Regras.Pluralidade(urna),
                Regras.Borda(urna),
                Regras.Copeland(urna),
                Regras.SegundoTurno(urna),
                Regras.AntiPluralidade(urna),
            })
            {
                Assert.All(eleitos, c => Assert.InRange(c, 0, 2));
                Assert.Equal(eleitos.Count, eleitos.Distinct().Count());
                Assert.Equal(eleitos.Order().ToList(), eleitos);
            }
        }
    }

    [Fact]
    public void CopelandSempreElegeOVencedorDeCondorcetQuandoEleExiste()
    {
        // Copeland conta duelos ganhos, entao quem ganha de todos tem a nota
        // maxima: esta igualdade e o teste de sanidade do juiz.
        foreach (var n in new[] { 4, 5, 6 })
        {
            foreach (var (urna, _) in Urnas.Todas(n))
            {
                var condorcet = Vencedor.DeCondorcet(urna);
                if (condorcet >= 0)
                {
                    Assert.Equal([condorcet], Regras.Copeland(urna));
                }
            }
        }
    }

    [Fact]
    public void BordaNuncaElegeOPerdedorDeCondorcet()
    {
        foreach (var n in new[] { 4, 5, 6, 7 })
        {
            foreach (var (urna, _) in Urnas.Todas(n))
            {
                var perdedor = Vencedor.PerdedorDeCondorcet(urna);
                if (perdedor >= 0)
                {
                    Assert.DoesNotContain(perdedor, Regras.Borda(urna));
                }
            }
        }
    }

    [Fact]
    public void OSegundoTurnoTambemNuncaElegeOPerdedorDeCondorcet()
    {
        foreach (var n in new[] { 5, 6, 7 })
        {
            foreach (var (urna, _) in Urnas.Todas(n))
            {
                var perdedor = Vencedor.PerdedorDeCondorcet(urna);
                if (perdedor >= 0)
                {
                    Assert.DoesNotContain(perdedor, Regras.SegundoTurno(urna));
                }
            }
        }
    }

    [Fact]
    public void APluralidadeElegeOPerdedorDeCondorcetEmAlgumaUrna()
    {
        var achou = false;
        foreach (var (urna, _) in Urnas.Todas(7))
        {
            var perdedor = Vencedor.PerdedorDeCondorcet(urna);
            if (perdedor >= 0 && Regras.Pluralidade(urna).Contains(perdedor))
            {
                achou = true;
                break;
            }
        }

        Assert.True(achou);
    }

    [Fact]
    public void NuloNaoEAceito()
    {
        Assert.Throws<ArgumentNullException>(() => Regras.Pluralidade(null!));
        Assert.Throws<ArgumentNullException>(() => Regras.Borda(null!));
        Assert.Throws<ArgumentNullException>(() => Regras.Copeland(null!));
        Assert.Throws<ArgumentNullException>(() => Regras.SegundoTurno(null!));
        Assert.Throws<ArgumentNullException>(() => Regras.AntiPluralidade(null!));
    }
}
