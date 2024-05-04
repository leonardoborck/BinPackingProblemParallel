using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace BinPackingProblemParallel.Classes
{
    public class Crow
    {
        public List<List<int>> MemoriaAtual;
        public List<List<int>> MelhorMemoria;
        public double AvaliacaoDaMelhorSolucao;

        public List<Item> Itens;

        public List<Recipiente> RecipientesAtual;
        public List<Recipiente> MelhorRecipientes;

        public Crow(List<Item> itens, List<Recipiente> recipientes)
        {
            Itens = itens; RecipientesAtual = recipientes; MelhorRecipientes = recipientes;
            MemoriaAtual = new List<List<int>>();
            MelhorMemoria = new List<List<int>>();
            AvaliacaoDaMelhorSolucao = 0;
        }

        public void GeraSolucaoInicialAleatoria(int z)
        {
            List<int> recipientes = new List<int>();

            for(int i = 0; i < RecipientesAtual.Count(); i++)
            {
                recipientes.Add(0);
            }

            for(int j = 0; j < Itens.Count(); j++)
            {
                List<int> recipienteAtual = recipientes.GetRange(0, recipientes.Count());
                MemoriaAtual.Add(recipienteAtual);
            }

            for (int j = 0; j < MemoriaAtual.Count(); j++)
            {
                int qualBin = new Random().Next(MemoriaAtual[j].Count());
                MemoriaAtual[j][qualBin] = 1;
            }

            CorrigeSolucoesInviaveis();
            MelhorMemoria = MemoriaAtual;
            MelhorRecipientes = RecipientesAtual;
            AvaliacaoDaMelhorSolucao = AvaliaSolucao(MelhorMemoria, MelhorRecipientes, z);

        }

        public void CorrigeSolucoesInviaveis()
        {
            List<int> itensRemovidos = new List<int>();

            for(int k = 0;  k < RecipientesAtual.Count(); k++)
                RemoveItensDosRecipientesSobrecarregados(itensRemovidos, k);
            ReinsereItensUsandoLeftBottomPolicy(itensRemovidos);

            //remove recipiente vazio
            List<int> recipientesParaRemover = new List<int>();
            for (int k = 0; k < RecipientesAtual.Count(); k++)
            {
                if(!IndexDosItensNoRecipiente(k,MemoriaAtual).Any())
                    recipientesParaRemover.Add(k);          
            }
            //remove da lista
            foreach (var index in recipientesParaRemover.OrderByDescending(x => x))
            {
                RecipientesAtual.RemoveAt(index);
                for (int j = 0; j < MemoriaAtual.Count(); j++)
                {
                    MemoriaAtual[j].RemoveAt(index);
                }
            }

        }

        void ReinsereItensUsandoLeftBottomPolicy(List<int> itensRemovidos)
        {
            foreach (var itemRemovido in itensRemovidos)
            {
                bool conseguiuAdicionar = false;
                for (int k = 0; k < RecipientesAtual.Count(); k++)
                {
                    MemoriaAtual[itemRemovido][k] = 1;
                    if (VerificaAUtilizacaoDoRecipiente(k, MemoriaAtual, RecipientesAtual) > 1.0)
                    {
                        MemoriaAtual[itemRemovido][k] = 0;
                        continue;
                    }
                    else
                    {
                        conseguiuAdicionar = true;
                        break;
                    }
                }

                if (conseguiuAdicionar)
                    continue;

                //nao colocou em nenhum recipiente precisa criar um novo.
                RecipientesAtual.Add(RecipientesAtual[0]); //alterar isso depois para o caso de tamanhos diferentes de recipientes

                for (int j = 0; j < Itens.Count(); j++)
                {
                    if (itemRemovido == j)
                        MemoriaAtual[j].Add(1);
                    else
                        MemoriaAtual[j].Add(0);
                }
            }
        }

        public void RemoveItensDosRecipientesSobrecarregados(List<int> itensRemovidos, int indexDoRecipiente)
        {
            while(VerificaAUtilizacaoDoRecipiente(indexDoRecipiente, MemoriaAtual, RecipientesAtual) > 1.0)
            {
                List<int> indexDosItensNoRecipiente = IndexDosItensNoRecipiente(indexDoRecipiente, MemoriaAtual);

                int indexDoindexDoItemParaRemover = new Random().Next(indexDosItensNoRecipiente.Count());
                while(Existe(itensRemovidos, indexDosItensNoRecipiente[indexDoindexDoItemParaRemover]))
                {
                    indexDoindexDoItemParaRemover = new Random().Next(indexDosItensNoRecipiente.Count());
                }
                MemoriaAtual[indexDosItensNoRecipiente[indexDoindexDoItemParaRemover]][indexDoRecipiente] = 0;
                itensRemovidos.Add(indexDosItensNoRecipiente[indexDoindexDoItemParaRemover]);
            }
        }

        bool Existe(List<int> itensRemovidos, int item)
        {
            for (int i = 0; i < itensRemovidos.Count(); i++)
            {
                if (itensRemovidos[i] == item)
                    return true;
            }
            return false;
        }

        public double AvaliaSolucao(List<List<int>> solucao, List<Recipiente> recipientes, int z)
        {
            double soma = 0;
            for (int i = 0; i < recipientes.Count(); i++)
                soma += Math.Pow(VerificaAUtilizacaoDoRecipiente(i, solucao, recipientes), z);

            double numeroDeRecipientesUtilizados = recipientes.Count();

            return soma / numeroDeRecipientesUtilizados;
        }

        public double VerificaAUtilizacaoDoRecipiente(int indexDoRecipiente, List<List<int>> solucao, List<Recipiente> recipientes)
        {
            double somaItens = 0;
            for (int j = 0; j < Itens.Count(); j++)
            {
                if (solucao[j][indexDoRecipiente] == 1)
                    somaItens += Itens[j].Altura * Itens[j].Largura;
            }
            var teste = somaItens / (recipientes[indexDoRecipiente].Altura * recipientes[indexDoRecipiente].Largura);
            return teste;
        }

        List<int> IndexDosItensNoRecipiente(int indexDoRecipiente, List<List<int>> solucao)
        {
            List<int> indexDosItens = new List<int>();
            for (int j = 0; j < Itens.Count(); j++)
            {
                if (solucao[j][indexDoRecipiente] == 1)
                {
                    indexDosItens.Add(j);
                }
            }
            return indexDosItens;
        }

        public bool HouveMelhora(int z)
        {
            double solucaoAtual = AvaliaSolucao(MemoriaAtual, RecipientesAtual, z);

            if (solucaoAtual <= AvaliacaoDaMelhorSolucao)
                return false;

            MelhorMemoria = MemoriaAtual;
            MelhorRecipientes = RecipientesAtual;
            AvaliacaoDaMelhorSolucao = solucaoAtual;

            return true;
        }

        public void ItemShuffle()
        {
            int primeiroItem = new Random().Next(MemoriaAtual.Count());
            int recipienteDoPrimeiroItem = 0;

            for (int k = 0; k < MemoriaAtual[primeiroItem].Count(); k++)
            {
                if (MemoriaAtual[primeiroItem][k] == 1)
                {
                    recipienteDoPrimeiroItem = k;
                    break;
                }
            }
            int segundoItem;
            int recipienteDoSegundoItem = 0;
            do
            {
                segundoItem = new Random().Next(MemoriaAtual.Count());
                for (int k = 0; k < MemoriaAtual[segundoItem].Count(); k++)
                {
                    if (MemoriaAtual[segundoItem][k] == 1)
                    {
                        recipienteDoSegundoItem = k;
                        break;
                    }
                }
            } while (recipienteDoPrimeiroItem == recipienteDoSegundoItem);

            MemoriaAtual[primeiroItem][recipienteDoPrimeiroItem] = 0;
            MemoriaAtual[primeiroItem][recipienteDoSegundoItem] = 1;

            MemoriaAtual[segundoItem][recipienteDoSegundoItem] = 0;
            MemoriaAtual[segundoItem][recipienteDoPrimeiroItem] = 1;
        }

        public void BinDelete()
        {
            int recipienteParaRemover = new Random().Next(MemoriaAtual[0].Count());

            List<int> itensRemovidos = new List<int>();
            for (int j = 0; j < MemoriaAtual.Count(); j++)
            {
                if (MemoriaAtual[j][recipienteParaRemover] == 1)
                    itensRemovidos.Add(j);
            }

            RecipientesAtual.RemoveAt(recipienteParaRemover);
            for (int i = 0; i < MemoriaAtual.Count(); i++)
            {
                MemoriaAtual[i].RemoveAt(recipienteParaRemover);
            }

            ReinsereItensUsandoLeftBottomPolicy(itensRemovidos);
        }
    }
}
