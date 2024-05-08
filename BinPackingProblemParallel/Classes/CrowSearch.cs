using static System.Net.Mime.MediaTypeNames;

namespace BinPackingProblemParallel.Classes
{
    public class CrowSearch
    {
        public List<Crow> PopulacaoDeCorvos;
        public List<Item> Itens;
        public List<Recipiente> Recipientes;

        public void InicializaCorvos(int tamanhoDaPopulacao, int z)
        {
            PopulacaoDeCorvos.Clear();
            for (int i = 0; i < tamanhoDaPopulacao; i++)
            {
                List<Item> itens = Itens.GetRange(0, Itens.Count);
                List<Recipiente> recipientes = Recipientes.GetRange(0, Recipientes.Count);
                Crow novoCorvo = new Crow(itens, recipientes);
                novoCorvo.GeraSolucaoInicialAleatoria(z);
                PopulacaoDeCorvos.Add(novoCorvo);
            }
        }

        public CrowSearch(List<Item> itens, List<Recipiente> recipientes)
        {
            Itens = itens; Recipientes = recipientes;
            PopulacaoDeCorvos = new List<Crow>();
        }

        double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public Crow BuscaDoCorvo(int flightSize, int z, int maxIteration)
        {
            Parallel.ForEach(PopulacaoDeCorvos, corvo =>
            {
                for (int iteration = 0; iteration < maxIteration; iteration++)
                {
                    double ri = (new Random().Next(11)) / 10.0;
                    int caraAleatorio = new Random().Next(PopulacaoDeCorvos.Count);

                    for (int j = 0; j < corvo.MemoriaAtual.Count; j++)
                    { //para cada item
                        int teste = 0;
                        for (int indiceDoRecipienteAtual = 0; indiceDoRecipienteAtual < corvo.MemoriaAtual[j].Count; indiceDoRecipienteAtual++)
                        { //recipiente que está o item
                            if (corvo.MemoriaAtual[j][indiceDoRecipienteAtual] == 1)
                            {
                                teste = indiceDoRecipienteAtual;
                                break;
                            }
                        }

                        lock (PopulacaoDeCorvos[caraAleatorio])
                        {
                            int recipienteDoMelhorItem = 0;
                            for (int indiceDoMelhorRecipiente = 0; indiceDoMelhorRecipiente < PopulacaoDeCorvos[caraAleatorio].MelhorMemoria[j].Count; indiceDoMelhorRecipiente++)
                            { //recipiente que está o item
                                if (PopulacaoDeCorvos[caraAleatorio].MelhorMemoria[j][indiceDoMelhorRecipiente] == 1)
                                {
                                    recipienteDoMelhorItem = indiceDoMelhorRecipiente;
                                    break;
                                }
                            }
                            var novaPosicao = Convert.ToInt32(corvo.MemoriaAtual[j][teste] + flightSize * ri * (PopulacaoDeCorvos[caraAleatorio].MelhorMemoria[j][recipienteDoMelhorItem] - corvo.MemoriaAtual[j][teste]));

                            corvo.MemoriaAtual[j][teste] = 0;
                            if (novaPosicao < 0 || novaPosicao >= corvo.MemoriaAtual[j].Count)
                            {
                                if (novaPosicao >= corvo.MemoriaAtual[j].Count)
                                    corvo.MemoriaAtual[j][corvo.MemoriaAtual[j].Count - 1] = 1;
                                if (novaPosicao < 0)
                                    corvo.MemoriaAtual[j][0] = 1;
                                continue;
                            }
                            corvo.MemoriaAtual[j][novaPosicao] = 1;
                        };
                    }

                    corvo.CorrigeSolucoesInviaveis();
                    corvo.HouveMelhora(z);

                    //BuscaLocal(corvo);
                    //corvo.CorrigeSolucoesInviaveis();
                    //corvo.HouveMelhora(z);
                }
            });

            //for (int i = 0; i < PopulacaoDeCorvos.Count; i++)
            //{
            //    for (int iteration = 0; iteration < maxIteration; iteration++)
            //    {

            //        double ri = (new Random().Next(11)) / 10.0;
            //        int caraAleatorio = new Random().Next(PopulacaoDeCorvos.Count);

            //        for (int j = 0; j < PopulacaoDeCorvos[i].MemoriaAtual.Count; j++)
            //        { //para cada item
            //            int recipienteDoItemAtual = 0;
            //            for (int k = 0; k < PopulacaoDeCorvos[i].MemoriaAtual[j].Count; k++)
            //            { //recipiente que está o item
            //                if (PopulacaoDeCorvos[i].MemoriaAtual[j][k] == 1)
            //                {
            //                    recipienteDoItemAtual = k;
            //                    break;
            //                }
            //            }
            //            int recipienteDoMelhorItem = 0;
            //            for (int k = 0; k < PopulacaoDeCorvos[caraAleatorio].MelhorMemoria[j].Count; k++)
            //            { //recipiente que está o item
            //                if (PopulacaoDeCorvos[caraAleatorio].MelhorMemoria[j][k] == 1)
            //                {
            //                    recipienteDoMelhorItem = k;
            //                    break;
            //                }
            //            }
            //            var novaPosicao = Convert.ToInt32(PopulacaoDeCorvos[i].MemoriaAtual[j][recipienteDoItemAtual] + flightSize * ri * Math.Abs(PopulacaoDeCorvos[caraAleatorio].MelhorMemoria[j][recipienteDoMelhorItem] - PopulacaoDeCorvos[i].MemoriaAtual[j][recipienteDoItemAtual]));
            //            PopulacaoDeCorvos[i].MemoriaAtual[j][recipienteDoItemAtual] = 0;
            //            PopulacaoDeCorvos[i].MemoriaAtual[j][novaPosicao] = 1;
            //        }

            //        PopulacaoDeCorvos[i].CorrigeSolucoesInviaveis();
            //        PopulacaoDeCorvos[i].HouveMelhora(z);

            //        //BuscaLocal(PopulacaoDeCorvos[i]);
            //        //PopulacaoDeCorvos[i].CorrigeSolucoesInviaveis();
            //        //PopulacaoDeCorvos[i].HouveMelhora(z);

            //    }
            //}

            double melhor = 0;
            double menor = PopulacaoDeCorvos[0].RecipientesAtual.Count;
            int index = 0;
            for (int i = 0; i < PopulacaoDeCorvos.Count; i++)
            {
                if (PopulacaoDeCorvos[i].AvaliacaoDaMelhorSolucao > melhor)
                {
                    melhor = PopulacaoDeCorvos[i].AvaliacaoDaMelhorSolucao;
                    index = i;
                }
            }

            return PopulacaoDeCorvos[index];

        }

        void BuscaLocal(Crow corvoAtual)
        {
            int operacao = new Random().Next(2);
            switch (operacao)
            {
                case 0:
                    corvoAtual.ItemShuffle();
                    break;
                case 1:
                    corvoAtual.BinDelete();
                    break;
                default:
                    break;
            }
        }
    }
}
