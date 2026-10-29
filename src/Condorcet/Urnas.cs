namespace Conde.Condorcet;

/// <summary>
/// A enumeração de TODAS as eleições com um número dado de eleitores e três
/// candidatos.
///
/// Duas contagens diferentes:
/// <list type="bullet">
/// <item>as urnas <b>distinguíveis</b>, em que importa quem votou o quê: 6ⁿ;</item>
/// <item>as urnas <b>por contagem</b>, em que só importa quantas cédulas de
/// cada tipo caíram: as combinações com repetição, que é o que de fato
/// determina o resultado.</item>
/// </list>
///
/// As duas aparecem porque elas dão porcentagens diferentes para a mesma
/// pergunta, e escolher uma sem dizer qual é o jeito mais fácil de publicar um
/// número que ninguém consegue reproduzir.
/// </summary>
public static class Urnas
{
    public static long Distinguiveis(int eleitores)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(eleitores);
        long total = 1;
        for (var i = 0; i < eleitores; i++)
        {
            total *= 6;
        }

        return total;
    }

    public static long PorContagem(int eleitores)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(eleitores);

        // Combinacoes com repeticao: C(n + 5, 5).
        long total = 1;
        for (var i = 1; i <= 5; i++)
        {
            total = total * (eleitores + i) / i;
        }

        return total;
    }

    /// <summary>
    /// Cada urna uma vez, com o PESO multinomial que diz de quantas maneiras
    /// distinguíveis aquela contagem pode acontecer. Varrer as contagens e
    /// carregar o peso dá o mesmo resultado que varrer as 6ⁿ urnas, por uma
    /// fração do trabalho.
    /// </summary>
    public static IEnumerable<(Perfil Urna, long Peso)> Todas(int eleitores)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(eleitores);
        var quantas = new int[6];
        return Passo(0, eleitores);

        IEnumerable<(Perfil, long)> Passo(int tipo, int resto)
        {
            if (tipo == 5)
            {
                quantas[5] = resto;
                yield return (new Perfil(quantas), Multinomial(quantas));
                yield break;
            }

            for (var q = 0; q <= resto; q++)
            {
                quantas[tipo] = q;
                foreach (var saida in Passo(tipo + 1, resto - q))
                {
                    yield return saida;
                }
            }
        }
    }

    /// <summary>
    /// De quantas maneiras distinguíveis uma contagem pode acontecer. O cálculo
    /// é feito multiplicando e dividindo alternadamente para o valor nunca
    /// passar do que cabe: o fatorial de 12 já estoura o que seria necessário
    /// guardar no meio do caminho.
    /// </summary>
    public static long Multinomial(IReadOnlyList<int> quantas)
    {
        ArgumentNullException.ThrowIfNull(quantas);
        long resultado = 1;
        var usados = 0;
        foreach (var q in quantas)
        {
            for (var i = 1; i <= q; i++)
            {
                usados++;
                resultado = resultado * usados / i;
            }
        }

        return resultado;
    }
}
