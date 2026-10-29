namespace Conde.Condorcet;

/// <summary>
/// A pergunta mais desconfortável que se pode fazer a uma regra de apuração:
/// <b>subir na preferência de um eleitor pode fazer o candidato perder?</b>
///
/// Numa regra razoável a resposta é não, por definição de razoável. Aqui isso
/// não é citado: é contado.
///
/// O teste é conservador. Parte-se de uma urna em que alguém vence sozinho;
/// movem-se <c>k</c> votos de um tipo de cédula para a cédula idêntica com o
/// vencedor em alguma posição ACIMA, sem mexer na ordem relativa dos outros; e
/// pergunta-se se ele continua entre os vencedores. Só conta como falha se ele
/// sair completamente, inclusive de empates.
/// </summary>
public static class Monotonia
{
    /// <summary>
    /// Quantas maneiras existem de derrubar o vencedor subindo-o na cédula.
    /// Zero quando a urna se comporta.
    /// </summary>
    public static int Falhas(Perfil p, Func<Perfil, List<int>> regra)
    {
        ArgumentNullException.ThrowIfNull(p);
        ArgumentNullException.ThrowIfNull(regra);

        var antes = regra(p);
        if (antes.Count != 1)
        {
            return 0;
        }

        var vencedor = antes[0];
        var falhas = 0;

        for (var tipo = 0; tipo < Perfil.Cedulas.Length; tipo++)
        {
            if (p.Quantas[tipo] == 0)
            {
                continue;
            }

            foreach (var destino in Subindo(tipo, vencedor))
            {
                for (var k = 1; k <= p.Quantas[tipo]; k++)
                {
                    var novas = p.Quantas.ToArray();
                    novas[tipo] -= k;
                    novas[destino] += k;

                    if (!regra(new Perfil(novas)).Contains(vencedor))
                    {
                        falhas++;
                    }
                }
            }
        }

        return falhas;
    }

    /// <summary>
    /// Um exemplo concreto de falha, para o README não ficar só com
    /// porcentagem: a urna, o vencedor, quantos votos subiram e o que saiu.
    /// </summary>
    public readonly record struct Exemplo(
        int[] Antes,
        int Vencedor,
        int Tipo,
        int Destino,
        int Votos,
        int[] Depois,
        List<int> NovosVencedores);

    public static Exemplo? PrimeiraFalha(Perfil p, Func<Perfil, List<int>> regra)
    {
        ArgumentNullException.ThrowIfNull(p);
        ArgumentNullException.ThrowIfNull(regra);

        var antes = regra(p);
        if (antes.Count != 1)
        {
            return null;
        }

        var vencedor = antes[0];

        for (var tipo = 0; tipo < Perfil.Cedulas.Length; tipo++)
        {
            if (p.Quantas[tipo] == 0)
            {
                continue;
            }

            foreach (var destino in Subindo(tipo, vencedor))
            {
                for (var k = 1; k <= p.Quantas[tipo]; k++)
                {
                    var novas = p.Quantas.ToArray();
                    novas[tipo] -= k;
                    novas[destino] += k;

                    var depois = regra(new Perfil(novas));
                    if (!depois.Contains(vencedor))
                    {
                        return new Exemplo(p.Quantas.ToArray(), vencedor, tipo, destino, k, novas, depois);
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Os índices das cédulas iguais a esta, mas com o candidato em alguma
    /// posição acima, mantida a ordem relativa dos outros.
    ///
    /// Subir SÓ UMA posição não basta: com três candidatos isso quase nunca
    /// muda quem é eliminado no primeiro turno, e a medida dá zero por falta de
    /// teste, não por falta de falha.
    /// </summary>
    public static List<int> Subindo(int tipo, int candidato)
    {
        var ordem = Perfil.Cedulas[tipo];
        var onde = Array.IndexOf(ordem, candidato);
        var destinos = new List<int>();

        for (var alvo = onde - 1; alvo >= 0; alvo--)
        {
            var nova = new List<int>(ordem);
            nova.RemoveAt(onde);
            nova.Insert(alvo, candidato);
            destinos.Add(Indice([.. nova]));
        }

        return destinos;
    }

    public static int Indice(int[] ordem)
    {
        ArgumentNullException.ThrowIfNull(ordem);
        for (var i = 0; i < Perfil.Cedulas.Length; i++)
        {
            var c = Perfil.Cedulas[i];
            if (c[0] == ordem[0] && c[1] == ordem[1] && c[2] == ordem[2])
            {
                return i;
            }
        }

        throw new ArgumentException("cedula desconhecida", nameof(ordem));
    }
}
