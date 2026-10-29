namespace Conde.Condorcet;

/// <summary>
/// O juiz do repositório: quem é o vencedor de Condorcet, pela definição, e
/// quem é o perdedor de Condorcet.
///
/// Vencedor de Condorcet é quem ganha de todos os outros no duelo direto, um a
/// um. Perdedor é quem perde de todos. Nenhum dos dois precisa existir, e a
/// pergunta de quantas vezes o vencedor não existe é o assunto do repositório.
///
/// Nada aqui depende de regra de apuração nenhuma: as regras aparecem no
/// arquivo ao lado e são comparadas contra este juiz.
/// </summary>
public static class Vencedor
{
    /// <summary>O vencedor de Condorcet, ou -1 se não houver.</summary>
    public static int DeCondorcet(Perfil p)
    {
        ArgumentNullException.ThrowIfNull(p);
        for (var a = 0; a < p.Candidatos; a++)
        {
            var ganhaDeTodos = true;
            for (var b = 0; b < p.Candidatos && ganhaDeTodos; b++)
            {
                if (a != b && p.Duelo(a, b) <= 0)
                {
                    ganhaDeTodos = false;
                }
            }

            if (ganhaDeTodos)
            {
                return a;
            }
        }

        return -1;
    }

    /// <summary>O perdedor de Condorcet, ou -1 se não houver.</summary>
    public static int PerdedorDeCondorcet(Perfil p)
    {
        ArgumentNullException.ThrowIfNull(p);
        for (var a = 0; a < p.Candidatos; a++)
        {
            var perdeDeTodos = true;
            for (var b = 0; b < p.Candidatos && perdeDeTodos; b++)
            {
                if (a != b && p.Duelo(a, b) >= 0)
                {
                    perdeDeTodos = false;
                }
            }

            if (perdeDeTodos)
            {
                return a;
            }
        }

        return -1;
    }

    /// <summary>
    /// Existe ciclo quando os duelos dão uma roda: A ganha de B, B ganha de C e
    /// C ganha de A. É a forma mais pura do paradoxo, e não é a única maneira de
    /// não haver vencedor de Condorcet: empate no duelo também impede.
    /// </summary>
    public static bool TemCiclo(Perfil p)
    {
        ArgumentNullException.ThrowIfNull(p);
        var ab = p.Duelo(0, 1);
        var bc = p.Duelo(1, 2);
        var ca = p.Duelo(2, 0);
        return (ab > 0 && bc > 0 && ca > 0) || (ab < 0 && bc < 0 && ca < 0);
    }
}
