# O paradoxo de Condorcet

Três candidatos, cada eleitor ordena os três, e a pergunta é quem ganhou. Condorcet
propôs em 1785 a resposta mais natural que existe: ganha quem vence todos os outros no
**duelo direto**, um contra um. Às vezes essa pessoa não existe — A ganha de B, B ganha de
C e C ganha de A — e é por isso que o nome dele ficou ligado a um paradoxo e não a um
método.

Aqui não se sorteia eleição. Com três candidatos existem seis cédulas possíveis, e
**todas** as urnas de um tamanho são varridas, carregando o peso de quantas maneiras
distinguíveis cada contagem pode acontecer. Com 23 eleitores isso cobre as
789.730.223.053.602.816 urnas possíveis.

```
dotnet test testes/Condorcet.Testes/Condorcet.Testes.csproj -c Release
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- paradoxo
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- regras
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- monotonia
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- monotonia-todas
```

## O juiz

A classe `Vencedor` não conhece regra de apuração nenhuma. Ela só sabe a definição: quem
ganha de todos nos duelos é o vencedor de Condorcet; quem perde de todos é o perdedor; e
nenhum dos dois precisa existir. As cinco regras do arquivo ao lado — pluralidade, segundo
turno, Borda, Copeland e antipluralidade — são comparadas contra ele, nunca entre si.

Copeland serve de teste de sanidade: por contar duelos ganhos, ela **tem** que eleger o
vencedor de Condorcet sempre que ele existe, e a varredura confirma isso com zero
discordâncias. Uma regra que concorda com o juiz por construção é o jeito de saber que o
juiz não está quebrado.

## Os achados

### 1. Com eleitorado ímpar, não ter vencedor é exatamente ter ciclo. Com par, não

```
eleitores  contagens  urnas         sem vencedor  com ciclo   %
3          56         216           12            12          5.56
4          126        1296          720           0           55.56
5          252        7776          540           540         6.94
6          462        46656         22920         180         49.13
7          792        279936        21000         21000       7.5
8          1287       1679616       752640        13440       44.81
9          2002       10077696      785820        785820      7.8
10         3003       60466176      25190424      711900      41.66
11         4368       362797056     28956312      28956312    7.98
```

Nas linhas ímpares as duas colunas do meio são **o mesmo número**, sempre. Nas linhas
pares elas não têm nada a ver uma com a outra: com 4 eleitores, 720 urnas não têm vencedor
e **nenhuma** delas tem ciclo.

O paradoxo que todo mundo conta é o do ciclo. Com eleitorado par, a causa quase nunca é
essa: é **empate no duelo**, que trava a definição sem nenhuma circularidade. E não é
caso raro — é a **maioria das eleições**: 55,56% com quatro eleitores, 49,13% com seis.

A frase "o paradoxo de Condorcet acontece em uns 7% das eleições" é verdadeira para
eleitorado ímpar e falsa por um fator de oito para eleitorado par.

### 2. A pluralidade elege quem perde de todos, em 25 milhões de urnas

```
Com 11 eleitores: quando ha vencedor de Condorcet, quem a regra elege
regra            discorda      %       elege o perdedor
pluralidade      97624296      29.24   24925824
segundo turno    15040872      4.51    0
Borda            43308144      12.97   0
Copeland         0             0       0
antipluralidade  128784024     38.58   15040872
```

Em 29,24% das eleições com 11 eleitores que **têm** um vencedor de Condorcet, o turno
único elege outra pessoa. Pior: em 24.925.824 urnas ele elege o **perdedor** de Condorcet,
a pessoa que perderia de todas as outras num duelo direto.

O segundo turno e Borda nunca fazem isso — nem uma vez, nos dois eleitorados medidos.
Discordar do juiz é uma coisa; eleger quem perde de todo mundo é outra, e a diferença
entre as regras está aí, não na porcentagem de discordância.

### 3. O menor eleitorado em que ganhar votos faz perder a eleição tem 17 eleitores

```
  antes:  5x ACB, 4x BAC, 2x BCA, 6x CBA
  vence C sozinho
  2 eleitores sobem C de BCA para CBA
  depois: 5x ACB, 4x BAC, 8x CBA
  passa a vencer A
```

Confira à mão. **Antes:** primeiros lugares A=5, B=6, C=6; A é eliminado; os 5 votos dele
vão para C; C faz 11 contra 6 e vence. **Depois:** dois eleitores que já punham C em
segundo passam a pôr C em primeiro — ninguém trocou de lado, ninguém saiu, C só subiu.
Agora A=5, B=4, C=8; **B** é eliminado; os 4 votos de B vão para A; A faz 9 contra 8 e
vence.

C ganhou dois apoios e perdeu a eleição. Não existe nenhuma urna com 16 eleitores ou menos
onde isso aconteça: a varredura cobre todas elas, uma a uma.

### 4. A falha não existe em todo eleitorado, e é só do segundo turno

```
eleitores  urnas                  com falha              %
15         470184984576           0                      0
16         2821109907456          0                      0
17         16926659444736         562073960448           3.32
18         101559956668416        0                      0
19         609359740010496        0                      0
20         3656158440062976       49446736035840         1.35
21         21936950640377856      831885144330240        3.79
22         131621703842267136     0                      0
23         789730223053602816     28845427740180480      3.65
```

Com 17 eleitores, 3,32% das urnas podem ser derrubadas assim. Com 18 e 19, **nenhuma**.
Com 20 volta, cai para 1,35%, sobe para 3,79% no 21, some de novo no 22 e volta no 23. Não
é uma curva: é um liga-desliga que depende de como os cortes de eliminação caem em cada
tamanho.

E entre as cinco regras medidas com 17 eleitores, o segundo turno é a **única** que falha:
as outras quatro dão zero.

## O que as medidas me corrigiram

- **Eu quase publiquei que o segundo turno é monótono.** A primeira versão do teste subia
  o vencedor **uma** posição na cédula. Deu zero em todos os eleitorados, em todas as
  regras, e um zero tão limpo parece confirmação. Não era: com três candidatos, subir uma
  posição quase nunca muda quem é eliminado, então o teste não testava nada. O certo é
  subir para **qualquer** posição acima, mantida a ordem relativa dos outros. Trocando
  isso, a falha apareceu. **Zero medido com teste estreito é indistinguível de zero de
  verdade**, e essa é a armadilha mais perigosa que eu encontrei neste repositório.

- **E depois parei de varrer antes da resposta.** Corrigido o teste, varri até 9
  eleitores: zero. Até 11: zero. Até 15: zero. A primeira falha está em **17**. Se eu
  tivesse parado em qualquer lugar antes, teria escrito "não encontrei falha de monotonia"
  com a varredura completa como argumento. É o mesmo erro de outro repositório meu, em que
  cem milhões de valores seguidos não bastaram — e aqui ele apareceu de novo, no mesmo dia.

- **Eu achava que o paradoxo rondava os 7%.** É o número que se cita, e ele vale só para
  eleitorado ímpar. Com eleitorado par a resposta é a **maioria** das urnas, e por um
  motivo diferente: empate de duelo, não ciclo. Eu ia escrever uma linha só sobre "o
  paradoxo", e são dois fenômenos.

- **Eu esperava que a antipluralidade fosse a mais inofensiva.** Ela é a pior das cinco em
  discordância (38,58%) e elege o perdedor de Condorcet em 15 milhões de urnas. Olhar
  menos informação não é ser neutro.

## Estrutura

```
src/Condorcet/
  Perfil.cs      a urna: quantas cedulas de cada tipo, e os duelos ja somados
  Vencedor.cs    o juiz: vencedor e perdedor de Condorcet pela definicao, e o ciclo
  Regras.cs      pluralidade, segundo turno, Borda, Copeland e antipluralidade
  Urnas.cs       todas as urnas de um tamanho, com o peso multinomial
  Monotonia.cs   subir o vencedor na cedula e perguntar se ele ainda vence
  Medidas.cs     as tabelas deste README
testes/Condorcet.Testes/
ferramentas/Medidor/
```

72 testes.
