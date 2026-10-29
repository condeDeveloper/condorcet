using Conde.Condorcet;
using Xunit;

namespace Conde.Condorcet.Testes;

public class MonotoniaTestes
{
    /// <summary>
    /// A menor urna em que o segundo turno pode ser derrubado por ganhar votos.
    /// Ela saiu da varredura, não de um livro, e está conferida à mão no README.
    /// </summary>
    private static Perfil MenorFalha() => Montar(("ACB", 5), ("BAC", 4), ("BCA", 2), ("CBA", 6));

    private static Perfil Montar(params (string Cedula, int Quantos)[] partes)
    {
        var quantas = new int[6];
        foreach (var (cedula, quantos) in partes)
        {
            var ordem = cedula.Select(c => c - 'A').ToArray();
            quantas[Monotonia.Indice(ordem)] += quantos;
        }

        return new Perfil(quantas);
    }

    [Fact]
    public void NaMenorFalhaOVencedorVenceSozinho()
    {
        var p = MenorFalha();
        Assert.Equal(17, p.Eleitores);
        Assert.Equal([2], Regras.SegundoTurno(p));
    }

    [Fact]
    public void SubirODoisEleitoresDerrubaOVencedor()
    {
        var antes = MenorFalha();
        var depois = Montar(("ACB", 5), ("BAC", 4), ("CBA", 8));
        Assert.Equal(17, depois.Eleitores);
        Assert.Equal([2], Regras.SegundoTurno(antes));
        Assert.DoesNotContain(2, Regras.SegundoTurno(depois));
        Assert.Equal([0], Regras.SegundoTurno(depois));
    }

    [Fact]
    public void OsDoisEleitoresRealmenteSubiramOVencedor()
    {
        // De BCA para CBA: C sai do segundo lugar para o primeiro, e a ordem
        // relativa dos outros dois nao muda.
        var antes = Monotonia.Indice([1, 2, 0]);
        var depois = Monotonia.Indice([2, 1, 0]);
        Assert.Contains(depois, Monotonia.Subindo(antes, 2));
    }

    [Fact]
    public void AMenorFalhaEDetectada()
    {
        Assert.True(Monotonia.Falhas(MenorFalha(), Regras.SegundoTurno) > 0);
        Assert.NotNull(Monotonia.PrimeiraFalha(MenorFalha(), Regras.SegundoTurno));
    }

    [Fact]
    public void SubirUmCandidatoSoProduzCedulasValidas()
    {
        for (var tipo = 0; tipo < 6; tipo++)
        {
            for (var c = 0; c < 3; c++)
            {
                foreach (var destino in Monotonia.Subindo(tipo, c))
                {
                    Assert.InRange(destino, 0, 5);
                    var antes = Array.IndexOf(Perfil.Cedulas[tipo], c);
                    var depois = Array.IndexOf(Perfil.Cedulas[destino], c);
                    Assert.True(depois < antes);
                }
            }
        }
    }

    [Fact]
    public void QuemJaEstaEmPrimeiroNaoTemParaOndeSubir()
    {
        for (var tipo = 0; tipo < 6; tipo++)
        {
            var primeiro = Perfil.Cedulas[tipo][0];
            Assert.Empty(Monotonia.Subindo(tipo, primeiro));
        }
    }

    [Fact]
    public void SubirMantemAOrdemRelativaDosOutros()
    {
        for (var tipo = 0; tipo < 6; tipo++)
        {
            for (var c = 0; c < 3; c++)
            {
                foreach (var destino in Monotonia.Subindo(tipo, c))
                {
                    var antes = Perfil.Cedulas[tipo].Where(x => x != c).ToArray();
                    var depois = Perfil.Cedulas[destino].Where(x => x != c).ToArray();
                    Assert.Equal(antes, depois);
                }
            }
        }
    }

    [Fact]
    public void NaoExisteFalhaComDezesseisEleitoresOuMenos()
    {
        for (var n = 1; n <= 16; n++)
        {
            foreach (var (urna, _) in Urnas.Todas(n))
            {
                Assert.Equal(0, Monotonia.Falhas(urna, Regras.SegundoTurno));
            }
        }
    }

    [Fact]
    public void AsOutrasRegrasNaoFalhamComDezesseteEleitores()
    {
        var linhas = Medidas.MonotoniaDeTodas(17);
        foreach (var l in linhas.Where(l => l.Regra != "segundo turno"))
        {
            Assert.Equal(0, l.Discorda);
        }
    }

    [Fact]
    public void CedulaDesconhecidaNaoEAceita()
    {
        Assert.Throws<ArgumentException>(() => Monotonia.Indice([0, 0, 0]));
        Assert.Throws<ArgumentNullException>(() => Monotonia.Indice(null!));
        Assert.Throws<ArgumentNullException>(() => Monotonia.Falhas(null!, Regras.Borda));
        Assert.Throws<ArgumentNullException>(() => Monotonia.Falhas(new Perfil([1, 0, 0, 0, 0, 0]), null!));
    }
}
