#include <iostream>

#include "Crow.h"
#include "CrowSearch.h"

vector<Crow> PopulacaoDeCorvos;
vector<Item> ItensCSA;
vector<Recipiente> RecipientesCSA;

void CarregaVariaveis(vector<Item> itens, vector<Recipiente> recipientes) {
	ItensCSA.clear();
	RecipientesCSA.clear();
	for (int i = 0; i < itens.size(); i++)
		ItensCSA.push_back(itens[i]);
	for (int i = 0; i < recipientes.size(); i++)
		RecipientesCSA.push_back(recipientes[i]);
}

void InicializaCorvos(int tamanhoDaPopulacao, int z, int numeroDeArvores) {
	PopulacaoDeCorvos.clear();
	for (int i = 0; i < tamanhoDaPopulacao; i++) {
		Crow novoCorvo(ItensCSA, RecipientesCSA);
		novoCorvo.GeraSolucaoInicialComArvores(z, numeroDeArvores);
		PopulacaoDeCorvos.push_back(novoCorvo);
	}
}

void InicializaCorvos(int tamanhoDaPopulacao, int z) {
	PopulacaoDeCorvos.clear();
	for (int i = 0; i < tamanhoDaPopulacao; i++) {
		Crow novoCorvo(ItensCSA, RecipientesCSA);
		novoCorvo.GeraSolucaoInicialAleatoria(z);
		PopulacaoDeCorvos.push_back(novoCorvo);
	}
}

double Sigmoid(double x) {
	return 1 / (1 + exp(-x));
}

Crow CrowSearch(int flightSize, double AP, int z, int maxIteration) {
	for (int i = 0; i < PopulacaoDeCorvos.size(); i++) {
		for (int iteration = 0; iteration < maxIteration; iteration++) {

			double ri = (rand() % 11) / 10.0;
			double rj = (rand() % 11) / 10.0;

			int caraAleatorio = rand() % PopulacaoDeCorvos.size();

			if (rj >= AP) {
				for (int j = 0; j < PopulacaoDeCorvos[i].MemoriaAtual.size(); j++) { //para cada item
					int recipienteDoItemAtual = 0;
					for (int k = 0; k < PopulacaoDeCorvos[i].MemoriaAtual[j].size(); k++) { //recipiente que está o item
						if (PopulacaoDeCorvos[i].MemoriaAtual[j][k] == 1) {
							recipienteDoItemAtual = k;
							break;
						}
					}
					int recipienteDoMelhorItem = 0;
					for (int k = 0; k < PopulacaoDeCorvos[caraAleatorio].MelhorMemoria[j].size(); k++) { //recipiente que está o item
						if (PopulacaoDeCorvos[caraAleatorio].MelhorMemoria[j][k] == 1) {
							recipienteDoMelhorItem = k;
							break;
						}
					}
					int novaPosicao = PopulacaoDeCorvos[i].MemoriaAtual[j][recipienteDoItemAtual] + flightSize * ri * (abs(PopulacaoDeCorvos[caraAleatorio].MelhorMemoria[j][recipienteDoMelhorItem] - PopulacaoDeCorvos[i].MemoriaAtual[j][recipienteDoItemAtual]));
					PopulacaoDeCorvos[i].MemoriaAtual[j][recipienteDoItemAtual] = 0;
					PopulacaoDeCorvos[i].MemoriaAtual[j][novaPosicao] = 1;
				}

			}

			else { // rand
				for (int j = 0; j < PopulacaoDeCorvos[i].MemoriaAtual.size(); j++) { //para cada item
					double numRand = rand() % 11 / 10.0;

					if (numRand >= 0.5) {
						int recipienteParaAtualizar = rand() % PopulacaoDeCorvos[i].MemoriaAtual[j].size();

						for (int k = 0; k < PopulacaoDeCorvos[i].MemoriaAtual[j].size(); k++)
							PopulacaoDeCorvos[i].MemoriaAtual[j][k] = k == recipienteParaAtualizar ? 1 : 0;
					}
				}
			}

			PopulacaoDeCorvos[i].CorrigeSolucoesInviaveis(PopulacaoDeCorvos[i].MemoriaAtual, PopulacaoDeCorvos[i].RecipientesAtual);
			PopulacaoDeCorvos[i].HouveMelhora(z);

				BuscaLocal(PopulacaoDeCorvos[i]);
				PopulacaoDeCorvos[i].CorrigeSolucoesInviaveis(PopulacaoDeCorvos[i].MemoriaAtual, PopulacaoDeCorvos[i].RecipientesAtual);
				PopulacaoDeCorvos[i].HouveMelhora(z);
		}
	}

	double melhor = 0;
	int index = 0;
	for (int i = 0; i < PopulacaoDeCorvos.size(); i++) {
		if (PopulacaoDeCorvos[i].AvaliacaoDaMelhorSolucao > melhor) {
			melhor = PopulacaoDeCorvos[i].AvaliacaoDaMelhorSolucao;
			index = i;
		}
	}
	//ImprimePopulacaoDeCorvos(PopulacaoDeCorvos[index].MemoriaAtual, PopulacaoDeCorvos[index].AvaliacaoDaMelhorSolucao);
	return PopulacaoDeCorvos[index];
}

void BuscaLocal(Crow &corvoAtual) {
	int operacao = rand() % 1;
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

void ImprimePopulacaoDeCorvos(vector<vector<int>> solucao, double avaliacao)
{
	cout << "Melhor Solucao:" << endl;
	cout << "Porcentagem de Utilizacao: " << avaliacao << endl;
	for (int j = 0; j < solucao.size(); j++) {
		for (int k = 0; k < solucao[j].size(); k++) {
			cout << solucao[j][k] << "\t";
		}
		cout << endl;
	}
	/*else {
		cout << "Corvo Seguido:" << endl;
		for (int j = 0; j < solucao2.size(); j++) {
			for (int k = 0; k < solucao2[j].size(); k++) {
				cout << solucao2[j][k] << "\t";
			}
			cout << endl;
		}
		cout << "Corvo Atual:" << endl;
		for (int j = 0; j < solucao.size(); j++) {
			for (int k = 0; k < solucao[j].size(); k++) {
				cout << solucao[j][k] << "\t";
			}
			cout << endl;
		}
	}*/
}


