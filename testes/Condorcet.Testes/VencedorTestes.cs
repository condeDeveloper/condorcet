using Conde.Condorcet;
using Xunit;

namespace Conde.Condorcet.Testes;

public class VencedorTestes
{
    /// <summary>
    /// O ciclo clássico: um eleitor de cada uma das três cédulas cíclicas.
    /// A ganha de B, B ganha de C e C ganha de A.
    /// </summary>
    private static Perfil Ciclo() => new([1, 0, 0, 1, 1, 0]);

    [Fact]
    public void NoCicloNaoHaVencedorNemPerdedor()
    {
        var p = Ciclo();
        Assert.True(Vencedor.TemCiclo(p));
        Assert.Equal(-1, Vencedor.DeCondorcet(p));
        Assert.Equal(-1, Vencedor.PerdedorDeCondorcet(p));
    }

    [Fact]
    public void NoCicloCadaUmGanhaDeUmEPerdeDeOutro()
    {
        var p = Ciclo();
        Assert.Equal(1, p.Duelo(0, 1));
        Assert.Equal(1, p.Duelo(1, 2));
        Assert.Equal(1, p.Duelo(2, 0));
    }

    [Fact]
    public void QuandoTodosConcordamOPrimeiroVenceEOUltimoPerde()
    {
        var p = new Perfil([7, 0, 0, 0, 0, 0]);
        Assert.Equal(0, Vencedor.DeCondorcet(p));
        Assert.Equal(2, Vencedor.PerdedorDeCondorcet(p));
        Assert.False(Vencedor.TemCiclo(p));
    }

    [Fact]
    public void OVencedorGanhaDeTodosOsOutros()
    {
        foreach (var (urna, _) in Urnas.Todas(5))
        {
            var v = Vencedor.DeCondorcet(urna);
            if (v < 0)
            {
                continue;
            }

            for (var b = 0; b < 3; b++)
            {
                if (b != v)
                {
                    Assert.True(urna.Duelo(v, b) > 0);
                }
            }
        }
    }

    [Fact]
    public void OPerdedorPerdeDeTodosOsOutros()
    {
        foreach (var (urna, _) in Urnas.Todas(5))
        {
            var l = Vencedor.PerdedorDeCondorcet(urna);
            if (l < 0)
            {
                continue;
            }

            for (var b = 0; b < 3; b++)
            {
                if (b != l)
                {
                    Assert.True(urna.Duelo(l, b) < 0);
                }
            }
        }
    }

    [Fact]
    public void NuncaHaDoisVencedores()
    {
        foreach (var (urna, _) in Urnas.Todas(6))
        {
            var quantos = 0;
            for (var a = 0; a < 3; a++)
            {
                var ganhaDeTodos = true;
                for (var b = 0; b < 3; b++)
                {
                    if (a != b && urna.Duelo(a, b) <= 0)
                    {
                        ganhaDeTodos = false;
                    }
                }

                if (ganhaDeTodos)
                {
                    quantos++;
                }
            }

            Assert.True(quantos <= 1);
        }
    }

    [Fact]
    public void ComEleitoradoImparNaoTerVencedorEExatamenteTerCiclo()
    {
        foreach (var n in new[] { 3, 5, 7 })
        {
            foreach (var (urna, _) in Urnas.Todas(n))
            {
                Assert.Equal(Vencedor.DeCondorcet(urna) < 0, Vencedor.TemCiclo(urna));
            }
        }
    }

    [Fact]
    public void ComEleitoradoParIssoDeixaDeValer()
    {
        var semVencedorESemCiclo = 0;
        foreach (var (urna, _) in Urnas.Todas(4))
        {
            if (Vencedor.DeCondorcet(urna) < 0 && !Vencedor.TemCiclo(urna))
            {
                semVencedorESemCiclo++;
            }
        }

        Assert.True(semVencedorESemCiclo > 0);
    }

    [Fact]
    public void NuloNaoEAceito()
    {
        Assert.Throws<ArgumentNullException>(() => Vencedor.DeCondorcet(null!));
        Assert.Throws<ArgumentNullException>(() => Vencedor.PerdedorDeCondorcet(null!));
        Assert.Throws<ArgumentNullException>(() => Vencedor.TemCiclo(null!));
    }
}
