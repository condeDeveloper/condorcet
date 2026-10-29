namespace Conde.Condorcet;

/// <summary>
/// Um perfil de votação: a lista de cédulas, cada uma uma ordem completa dos
/// candidatos, da preferida à última, sem empate.
///
/// Com três candidatos existem seis cédulas possíveis, e o perfil guarda
/// quantas de cada tipo foram depositadas. Duas urnas com as mesmas contagens
/// são a mesma eleição, independentemente de quem votou o quê.
/// </summary>
public sealed class Perfil
{
    private readonly int[] _quantas;
    private readonly int[,] _duelos;

    public static readonly int[][] Cedulas =
    [
        [0, 1, 2],
        [0, 2, 1],
        [1, 0, 2],
        [1, 2, 0],
        [2, 0, 1],
        [2, 1, 0],
    ];

    public int Candidatos => 3;

    public int Eleitores { get; }

    public IReadOnlyList<int> Quantas => _quantas;

    public Perfil(int[] quantasDeCada)
    {
        ArgumentNullException.ThrowIfNull(quantasDeCada);
        if (quantasDeCada.Length != Cedulas.Length)
        {
            throw new ArgumentException("sao seis cedulas possiveis com tres candidatos");
        }

        if (Array.Exists(quantasDeCada, q => q < 0))
        {
            throw new ArgumentOutOfRangeException(nameof(quantasDeCada), "contagem negativa");
        }

        _quantas = (int[])quantasDeCada.Clone();
        Eleitores = 0;
        foreach (var q in _quantas)
        {
            Eleitores += q;
        }

        _duelos = new int[3, 3];
        for (var c = 0; c < Cedulas.Length; c++)
        {
            var ordem = Cedulas[c];
            for (var i = 0; i < 3; i++)
            {
                for (var j = i + 1; j < 3; j++)
                {
                    _duelos[ordem[i], ordem[j]] += _quantas[c];
                }
            }
        }
    }

    /// <summary>Quantos eleitores põem <paramref name="a"/> na frente de <paramref name="b"/>.</summary>
    public int Preferem(int a, int b) => _duelos[a, b];

    /// <summary>
    /// O resultado do duelo entre dois candidatos: 1 se <paramref name="a"/>
    /// ganha, -1 se perde, 0 se empata.
    /// </summary>
    public int Duelo(int a, int b) => _duelos[a, b].CompareTo(_duelos[b, a]);

    /// <summary>Quantos eleitores puseram o candidato em cada posição.</summary>
    public int ContaNaPosicao(int candidato, int posicao)
    {
        var soma = 0;
        for (var c = 0; c < Cedulas.Length; c++)
        {
            if (Cedulas[c][posicao] == candidato)
            {
                soma += _quantas[c];
            }
        }

        return soma;
    }

    public override string ToString() => string.Join(",", _quantas);
}
