namespace Conde.Condorcet;

/// <summary>
/// As regras de apuração que se usa de verdade. Cada uma devolve o CONJUNTO de
/// vencedores: quando há empate, saem dois ou três nomes, e é assim que tem que
/// ser — desempatar por moeda esconderia o empate das contagens.
/// </summary>
public static class Regras
{
    /// <summary>Vence quem tem mais primeiros lugares. É o turno único.</summary>
    public static List<int> Pluralidade(Perfil p)
    {
        ArgumentNullException.ThrowIfNull(p);
        var pontos = new int[p.Candidatos];
        for (var c = 0; c < p.Candidatos; c++)
        {
            pontos[c] = p.ContaNaPosicao(c, 0);
        }

        return MaioresDe(pontos);
    }

    /// <summary>Vence quem tem menos últimos lugares.</summary>
    public static List<int> AntiPluralidade(Perfil p)
    {
        ArgumentNullException.ThrowIfNull(p);
        var pontos = new int[p.Candidatos];
        for (var c = 0; c < p.Candidatos; c++)
        {
            pontos[c] = -p.ContaNaPosicao(c, p.Candidatos - 1);
        }

        return MaioresDe(pontos);
    }

    /// <summary>Dois pontos por primeiro lugar, um por segundo, zero por último.</summary>
    public static List<int> Borda(Perfil p)
    {
        ArgumentNullException.ThrowIfNull(p);
        var pontos = new int[p.Candidatos];
        for (var c = 0; c < p.Candidatos; c++)
        {
            for (var pos = 0; pos < p.Candidatos; pos++)
            {
                pontos[c] += (p.Candidatos - 1 - pos) * p.ContaNaPosicao(c, pos);
            }
        }

        return MaioresDe(pontos);
    }

    /// <summary>Um ponto por duelo ganho, meio por empatado (contado em dobro).</summary>
    public static List<int> Copeland(Perfil p)
    {
        ArgumentNullException.ThrowIfNull(p);
        var pontos = new int[p.Candidatos];
        for (var a = 0; a < p.Candidatos; a++)
        {
            for (var b = 0; b < p.Candidatos; b++)
            {
                if (a == b)
                {
                    continue;
                }

                var duelo = p.Duelo(a, b);
                pontos[a] += duelo > 0 ? 2 : duelo == 0 ? 1 : 0;
            }
        }

        return MaioresDe(pontos);
    }

    /// <summary>
    /// O segundo turno: elimina quem tem menos primeiros lugares e reconta. Com
    /// três candidatos isso é uma eliminação só, e o duelo final decide.
    ///
    /// Quando há empate em quem eliminar, todos os caminhos são seguidos e o
    /// resultado é a união — de novo para não esconder o empate.
    /// </summary>
    public static List<int> SegundoTurno(Perfil p)
    {
        ArgumentNullException.ThrowIfNull(p);
        var primeiros = new int[p.Candidatos];
        for (var c = 0; c < p.Candidatos; c++)
        {
            primeiros[c] = p.ContaNaPosicao(c, 0);
        }

        var piores = MenoresDe(primeiros);
        var vencedores = new SortedSet<int>();

        foreach (var eliminado in piores)
        {
            var restantes = new List<int>();
            for (var c = 0; c < p.Candidatos; c++)
            {
                if (c != eliminado)
                {
                    restantes.Add(c);
                }
            }

            if (restantes.Count == 1)
            {
                vencedores.Add(restantes[0]);
                continue;
            }

            var duelo = p.Duelo(restantes[0], restantes[1]);
            if (duelo >= 0)
            {
                vencedores.Add(restantes[0]);
            }

            if (duelo <= 0)
            {
                vencedores.Add(restantes[1]);
            }
        }

        return [.. vencedores];
    }

    private static List<int> MaioresDe(int[] pontos)
    {
        var melhor = int.MinValue;
        foreach (var v in pontos)
        {
            melhor = Math.Max(melhor, v);
        }

        var vencedores = new List<int>();
        for (var c = 0; c < pontos.Length; c++)
        {
            if (pontos[c] == melhor)
            {
                vencedores.Add(c);
            }
        }

        return vencedores;
    }

    private static List<int> MenoresDe(int[] pontos)
    {
        var pior = int.MaxValue;
        foreach (var v in pontos)
        {
            pior = Math.Min(pior, v);
        }

        var piores = new List<int>();
        for (var c = 0; c < pontos.Length; c++)
        {
            if (pontos[c] == pior)
            {
                piores.Add(c);
            }
        }

        return piores;
    }
}
