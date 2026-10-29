using Conde.Condorcet;
using Xunit;

namespace Conde.Condorcet.Testes;

/// <summary>
/// Os números do README presos aqui. Só entra o que eu vi o medidor imprimir.
/// </summary>
public class MedidasTestes
{
    [Theory]
    [InlineData(3, 56, 216, 12, 12, 5.56)]
    [InlineData(4, 126, 1296, 720, 0, 55.56)]
    [InlineData(5, 252, 7776, 540, 540, 6.94)]
    [InlineData(6, 462, 46656, 22920, 180, 49.13)]
    [InlineData(7, 792, 279936, 21000, 21000, 7.5)]
    [InlineData(8, 1287, 1679616, 752640, 13440, 44.81)]
    [InlineData(9, 2002, 10077696, 785820, 785820, 7.8)]
    [InlineData(10, 3003, 60466176, 25190424, 711900, 41.66)]
    [InlineData(11, 4368, 362797056, 28956312, 28956312, 7.98)]
    public void ATabelaDoParadoxo(int n, long contagens, long urnas, long sem, long ciclo, double fracao)
    {
        var l = Medidas.Paradoxo([n])[0];
        Assert.Equal(contagens, l.UrnasPorContagem);
        Assert.Equal(urnas, l.UrnasDistinguiveis);
        Assert.Equal(sem, l.SemVencedor);
        Assert.Equal(ciclo, l.ComCiclo);
        Assert.Equal(fracao, l.Fracao);
    }

    [Fact]
    public void ComEleitoradoImparSemVencedorEOMesmoQueComCiclo()
    {
        foreach (var l in Medidas.Paradoxo([3, 5, 7, 9, 11]))
        {
            Assert.Equal(l.SemVencedor, l.ComCiclo);
        }
    }

    [Fact]
    public void ComEleitoradoParAMaioriaDasEleicoesNaoTemVencedor()
    {
        foreach (var l in Medidas.Paradoxo([4, 6]))
        {
            Assert.True(l.Fracao > 45.0);
            Assert.True(l.ComCiclo < l.SemVencedor / 100);
        }
    }

    [Fact]
    public void AContagemDeUrnasBateComAsDuasFormulas()
    {
        foreach (var n in new[] { 3, 5, 7, 9 })
        {
            Assert.Equal(Urnas.Distinguiveis(n), Medidas.Paradoxo([n])[0].UrnasDistinguiveis);
            Assert.Equal(Urnas.PorContagem(n), Urnas.Todas(n).Count());
        }
    }

    [Theory]
    [InlineData("pluralidade", 68040, 26.28, 11760)]
    [InlineData("segundo turno", 21000, 8.11, 0)]
    [InlineData("Borda", 32550, 12.57, 0)]
    [InlineData("Copeland", 0, 0.0, 0)]
    [InlineData("antipluralidade", 120504, 46.54, 21000)]
    public void ATabelaDasRegrasComSeteEleitores(string regra, long discorda, double fracao, long perdedor)
    {
        var l = Medidas.Regras_(7).Single(x => x.Regra == regra);
        Assert.Equal(discorda, l.Discorda);
        Assert.Equal(fracao, l.Fracao);
        Assert.Equal(perdedor, l.ElegeOPerdedor);
    }

    [Theory]
    [InlineData("pluralidade", 97624296, 29.24, 24925824)]
    [InlineData("segundo turno", 15040872, 4.51, 0)]
    [InlineData("Borda", 43308144, 12.97, 0)]
    [InlineData("Copeland", 0, 0.0, 0)]
    [InlineData("antipluralidade", 128784024, 38.58, 15040872)]
    public void ATabelaDasRegrasComOnzeEleitores(string regra, long discorda, double fracao, long perdedor)
    {
        var l = Medidas.Regras_(11).Single(x => x.Regra == regra);
        Assert.Equal(discorda, l.Discorda);
        Assert.Equal(fracao, l.Fracao);
        Assert.Equal(perdedor, l.ElegeOPerdedor);
    }

    [Fact]
    public void CopelandNuncaDiscordaDoJuiz()
    {
        foreach (var n in new[] { 7, 11 })
        {
            Assert.Equal(0, Medidas.Regras_(n).Single(x => x.Regra == "Copeland").Discorda);
        }
    }

    [Theory]
    [InlineData(3, 0, 0.0)]
    [InlineData(9, 0, 0.0)]
    [InlineData(15, 0, 0.0)]
    [InlineData(16, 0, 0.0)]
    [InlineData(17, 562073960448, 3.32)]
    [InlineData(18, 0, 0.0)]
    [InlineData(19, 0, 0.0)]
    [InlineData(20, 49446736035840, 1.35)]
    [InlineData(21, 831885144330240, 3.79)]
    [InlineData(22, 0, 0.0)]
    [InlineData(23, 28845427740180480, 3.65)]
    public void ATabelaDaMonotonia(int n, long comFalha, double fracao)
    {
        var l = Medidas.MonotoniaDoSegundoTurno([n])[0];
        Assert.Equal(comFalha, l.ComFalha);
        Assert.Equal(fracao, l.Fracao);
    }

    [Fact]
    public void AMenorFalhaTemDezesseteEleitores()
    {
        var (eleitores, exemplo) = Medidas.MenorFalha(20);
        Assert.Equal(17, eleitores);
        Assert.NotNull(exemplo);
        Assert.Equal(17, exemplo!.Value.Antes.Sum());
        Assert.Equal(2, exemplo.Value.Votos);
        Assert.DoesNotContain(exemplo.Value.Vencedor, exemplo.Value.NovosVencedores);
    }

    [Fact]
    public void SoOSegundoTurnoFalhaEmMonotonia()
    {
        var linhas = Medidas.MonotoniaDeTodas(17);
        Assert.Equal(562073960448, linhas.Single(l => l.Regra == "segundo turno").Discorda);
        Assert.Equal(3.32, linhas.Single(l => l.Regra == "segundo turno").Fracao);
        Assert.Equal(4, linhas.Count(l => l.Discorda == 0));
    }
}
