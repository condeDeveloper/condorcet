namespace Conde.Condorcet;

/// <summary>
/// As tabelas do README. Cada número sai de um método daqui e está preso num
/// teste; só entra aqui o que eu vi o programa imprimir.
/// </summary>
public static class Medidas
{
    public readonly record struct LinhaParadoxo(
        int Eleitores,
        long UrnasPorContagem,
        long UrnasDistinguiveis,
        long SemVencedor,
        long ComCiclo,
        double Fracao);

    public readonly record struct LinhaRegra(string Regra, long Discorda, double Fracao, long ElegeOPerdedor);

    public readonly record struct LinhaMonotonia(int Eleitores, long Urnas, long ComFalha, double Fracao);

    /// <summary>
    /// Quantas eleições não têm vencedor de Condorcet. A conta é feita com o
    /// peso multinomial, então o resultado é sobre as 6ⁿ urnas distinguíveis,
    /// não sobre as contagens.
    /// </summary>
    public static List<LinhaParadoxo> Paradoxo(IEnumerable<int> eleitorado)
    {
        var linhas = new List<LinhaParadoxo>();

        foreach (var n in eleitorado)
        {
            long sem = 0;
            long ciclo = 0;
            long total = 0;

            foreach (var (urna, peso) in Urnas.Todas(n))
            {
                total += peso;
                if (Vencedor.DeCondorcet(urna) < 0)
                {
                    sem += peso;
                }

                if (Vencedor.TemCiclo(urna))
                {
                    ciclo += peso;
                }
            }

            linhas.Add(new LinhaParadoxo(
                n,
                Urnas.PorContagem(n),
                total,
                sem,
                ciclo,
                Math.Round(100.0 * sem / total, 2)));
        }

        return linhas;
    }

    /// <summary>
    /// Para cada regra: em quantas eleições com vencedor de Condorcet ela elege
    /// outra pessoa, e em quantas ela elege o perdedor de Condorcet.
    /// </summary>
    public static List<LinhaRegra> Regras_(int eleitores)
    {
        var nomes = new (string Nome, Func<Perfil, List<int>> Regra)[]
        {
            ("pluralidade", Regras.Pluralidade),
            ("segundo turno", Regras.SegundoTurno),
            ("Borda", Regras.Borda),
            ("Copeland", Regras.Copeland),
            ("antipluralidade", Regras.AntiPluralidade),
        };

        var discorda = new long[nomes.Length];
        var elegeOPerdedor = new long[nomes.Length];
        long comVencedor = 0;

        foreach (var (urna, peso) in Urnas.Todas(eleitores))
        {
            var condorcet = Vencedor.DeCondorcet(urna);
            var perdedor = Vencedor.PerdedorDeCondorcet(urna);

            if (condorcet >= 0)
            {
                comVencedor += peso;
            }

            for (var i = 0; i < nomes.Length; i++)
            {
                var eleitos = nomes[i].Regra(urna);

                if (condorcet >= 0 && !(eleitos.Count == 1 && eleitos[0] == condorcet))
                {
                    discorda[i] += peso;
                }

                if (perdedor >= 0 && eleitos.Contains(perdedor))
                {
                    elegeOPerdedor[i] += peso;
                }
            }
        }

        var linhas = new List<LinhaRegra>();
        for (var i = 0; i < nomes.Length; i++)
        {
            linhas.Add(new LinhaRegra(
                nomes[i].Nome,
                discorda[i],
                Math.Round(100.0 * discorda[i] / comVencedor, 2),
                elegeOPerdedor[i]));
        }

        return linhas;
    }

    /// <summary>
    /// Em quantas eleições subir o vencedor uma posição na cédula de um eleitor
    /// derruba esse vencedor.
    /// </summary>
    public static List<LinhaMonotonia> MonotoniaDoSegundoTurno(IEnumerable<int> eleitorado)
    {
        var linhas = new List<LinhaMonotonia>();

        foreach (var n in eleitorado)
        {
            long comFalha = 0;
            long total = 0;

            foreach (var (urna, peso) in Urnas.Todas(n))
            {
                total += peso;
                if (Monotonia.Falhas(urna, Regras.SegundoTurno) > 0)
                {
                    comFalha += peso;
                }
            }

            linhas.Add(new LinhaMonotonia(n, total, comFalha, Math.Round(100.0 * comFalha / total, 2)));
        }

        return linhas;
    }

    /// <summary>
    /// O menor eleitorado em que o segundo turno pode ser derrubado por ganhar
    /// votos, com a urna concreta que faz isso.
    ///
    /// Varrer de baixo para cima e parar na primeira é o que transforma "existe
    /// um exemplo famoso com 17 eleitores" em "não existe nenhum com menos".
    /// </summary>
    public static (int Eleitores, Monotonia.Exemplo? Exemplo) MenorFalha(int limite)
    {
        for (var n = 1; n <= limite; n++)
        {
            foreach (var (urna, _) in Urnas.Todas(n))
            {
                var falha = Monotonia.PrimeiraFalha(urna, Regras.SegundoTurno);
                if (falha is not null)
                {
                    return (n, falha);
                }
            }
        }

        return (0, null);
    }

    /// <summary>A mesma pergunta, para todas as regras, num eleitorado só.</summary>
    public static List<LinhaRegra> MonotoniaDeTodas(int eleitores)
    {
        var nomes = new (string Nome, Func<Perfil, List<int>> Regra)[]
        {
            ("pluralidade", Regras.Pluralidade),
            ("segundo turno", Regras.SegundoTurno),
            ("Borda", Regras.Borda),
            ("Copeland", Regras.Copeland),
            ("antipluralidade", Regras.AntiPluralidade),
        };

        var falhas = new long[nomes.Length];
        long total = 0;

        foreach (var (urna, peso) in Urnas.Todas(eleitores))
        {
            total += peso;
            for (var i = 0; i < nomes.Length; i++)
            {
                if (Monotonia.Falhas(urna, nomes[i].Regra) > 0)
                {
                    falhas[i] += peso;
                }
            }
        }

        var linhas = new List<LinhaRegra>();
        for (var i = 0; i < nomes.Length; i++)
        {
            linhas.Add(new LinhaRegra(nomes[i].Nome, falhas[i], Math.Round(100.0 * falhas[i] / total, 2), 0));
        }

        return linhas;
    }
}
