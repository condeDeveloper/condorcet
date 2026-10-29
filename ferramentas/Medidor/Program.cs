using Conde.Condorcet;

if (args.Length == 0)
{
    Console.WriteLine("uso: medidor <paradoxo|regras|monotonia|monotonia-todas>");
    return 1;
}

switch (args[0])
{
    case "paradoxo":
        Paradoxo();
        break;
    case "regras":
        RegrasTabela();
        break;
    case "monotonia":
        MonotoniaTabela();
        break;
    case "monotonia-todas":
        MonotoniaTodas();
        break;
    default:
        Console.WriteLine($"comando desconhecido: {args[0]}");
        return 1;
}

return 0;

static void Paradoxo()
{
    Console.WriteLine("Eleicoes sem vencedor de Condorcet");
    Console.WriteLine("eleitores  contagens  urnas         sem vencedor  com ciclo   %");
    foreach (var l in Medidas.Paradoxo([3, 4, 5, 6, 7, 8, 9, 10, 11]))
    {
        Console.WriteLine($"{l.Eleitores,-11}{l.UrnasPorContagem,-11}{l.UrnasDistinguiveis,-14}{l.SemVencedor,-14}{l.ComCiclo,-12}{l.Fracao}");
    }
}

static void RegrasTabela()
{
    foreach (var n in new[] { 7, 11 })
    {
        Console.WriteLine($"Com {n} eleitores: quando ha vencedor de Condorcet, quem a regra elege");
        Console.WriteLine("regra            discorda      %       elege o perdedor");
        foreach (var l in Medidas.Regras_(n))
        {
            Console.WriteLine($"{l.Regra,-17}{l.Discorda,-14}{l.Fracao,-8}{l.ElegeOPerdedor}");
        }

        Console.WriteLine();
    }
}

static void MonotoniaTabela()
{
    Console.WriteLine("Segundo turno: subir o vencedor na cedula e ele perder");
    Console.WriteLine("eleitores  urnas                  com falha              %");
    foreach (var l in Medidas.MonotoniaDoSegundoTurno([3, 5, 7, 9, 11, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23]))
    {
        Console.WriteLine($"{l.Eleitores,-11}{l.Urnas,-23}{l.ComFalha,-23}{l.Fracao}");
    }

    Console.WriteLine();
    var (eleitores, exemplo) = Medidas.MenorFalha(25);
    if (exemplo is null)
    {
        Console.WriteLine("nenhuma falha ate o limite varrido");
        return;
    }

    var e = exemplo.Value;
    Console.WriteLine($"A menor urna com falha tem {eleitores} eleitores:");
    Mostrar("  antes:  ", e.Antes);
    Console.WriteLine($"  vence {Nome(e.Vencedor)} sozinho");
    Console.WriteLine($"  {e.Votos} eleitores sobem {Nome(e.Vencedor)} de {Cedula(e.Tipo)} para {Cedula(e.Destino)}");
    Mostrar("  depois: ", e.Depois);
    Console.WriteLine($"  passa a vencer {string.Join(" e ", e.NovosVencedores.Select(Nome))}");
}

static void Mostrar(string rotulo, int[] quantas)
{
    var partes = new List<string>();
    for (var i = 0; i < quantas.Length; i++)
    {
        if (quantas[i] > 0)
        {
            partes.Add($"{quantas[i]}x {Cedula(i)}");
        }
    }

    Console.WriteLine(rotulo + string.Join(", ", partes));
}

static string Cedula(int tipo) => string.Concat(Perfil.Cedulas[tipo].Select(Nome));

static string Nome(int candidato) => ((char)('A' + candidato)).ToString();

static void MonotoniaTodas()
{
    Console.WriteLine("Com 17 eleitores: falha de monotonia por regra");
    Console.WriteLine("regra            urnas com falha   %");
    foreach (var l in Medidas.MonotoniaDeTodas(17))
    {
        Console.WriteLine($"{l.Regra,-17}{l.Discorda,-18}{l.Fracao}");
    }
}
