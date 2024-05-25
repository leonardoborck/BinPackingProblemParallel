using BinPackingProblemParallel.Classes;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        string[] classes = { "CLASS01" };//, "CLASS02", "CLASS03", "CLASS04", "CLASS05", "CLASS06", "CLASS07", "CLASS08", "CLASS09", "CLASS10"};
        string[] numeroDeItens = { "020" };//, "040", "060", "080", "100" };
        string[] instancias = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" };

        string[] classesAleatorio = { "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "C10" };
        string[] instanciasAleatorio = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50" };

        List<BaseDeDados> basesDeDados = new List<BaseDeDados>();

        bool tamanhoAleatorio = true;
        if (!tamanhoAleatorio)
            for (int classe = 0; classe < classes.Count(); classe++)
            {
                for (int item = 0; item < numeroDeItens.Count(); item++)
                {
                    for (int instancia = 0; instancia < instancias.Count(); instancia++)
                    {
                        var stream = new StreamReader($"..\\..\\..\\base\\{classes[classe]}_{numeroDeItens[item]}_{instancias[instancia]}.json");
                        var baseDeDados = JsonConvert.DeserializeObject<BaseDeDados>(stream.ReadToEnd());
                        baseDeDados.Nome = $"{classes[classe]}_{numeroDeItens[item]}_{instancias[instancia]}";

                        var ItensQuePrecisamSerRepetidos = baseDeDados.Itens.Where(x => x.Demanda > 1).ToList();
                        foreach (var itemParaRepetir in ItensQuePrecisamSerRepetidos)
                        {
                            for (int i = 0; i < itemParaRepetir.Demanda - 1; i++)
                                baseDeDados.Itens.Add(itemParaRepetir);
                        }
                        basesDeDados.Add(baseDeDados);
                    }
                }
            }
        else
        {
            for (int classe = 0; classe < classesAleatorio.Count(); classe++)
            {
                for (int instancia = 0; instancia < instanciasAleatorio.Count(); instancia++)
                {
                    var stream = new StreamReader($"..\\..\\..\\base\\MB\\MB_{classesAleatorio[classe]}_{instanciasAleatorio[instancia]}.json");
                    var baseDeDados = JsonConvert.DeserializeObject<BaseDeDados>(stream.ReadToEnd());
                    baseDeDados.Nome = $"MB_{classesAleatorio[classe]}_{instanciasAleatorio[instancia]}";

                    var ItensQuePrecisamSerRepetidos = baseDeDados.Itens.Where(x => x.Demanda > 1).ToList();
                    foreach (var itemParaRepetir in ItensQuePrecisamSerRepetidos)
                    {
                        for (int i = 0; i < itemParaRepetir.Demanda - 1; i++)
                            baseDeDados.Itens.Add(itemParaRepetir);
                    }
                    basesDeDados.Add(baseDeDados);
                }
            }
        }

        const int z = 2;
        const int flightSize = 5;
        const int flocksize = 35;
        const int maxIteration = 80;
        const int repeticao = 30;

        ConcurrentBag<string> concurrentBag = new ConcurrentBag<string>();

        for (int i = 0; i < repeticao; i++)
        {
            Parallel.ForEach(basesDeDados, baseDeDados =>
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                List<Item> itens = baseDeDados.Itens.GetRange(0, baseDeDados.Itens.Count);
                var recipientes = baseDeDados.Recipientes.GetRange(0, baseDeDados.Recipientes.Count);

                CrowSearch crowSearch = new CrowSearch(baseDeDados.Itens, baseDeDados.Recipientes);
                crowSearch.InicializaCorvos(flocksize, z, ehTamanhoAleatorio: tamanhoAleatorio);
                Crow corvo = crowSearch.BuscaDoCorvo(flightSize, z, maxIteration);
                watch.Stop();

                concurrentBag.Add($"{baseDeDados.Nome},{watch.ElapsedMilliseconds},{corvo.AvaliacaoDaMelhorSolucao},{corvo.MelhorRecipientes.Count},{flocksize},{flightSize},{maxIteration}");
            });
        }

        var arquivoDeTeste = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\teste_AleatorioNovo.csv";//"{classes[0]}.csv";
        File.Delete(arquivoDeTeste);
        try
        {
            using (StreamWriter w = File.AppendText(arquivoDeTeste))
            {
                w.WriteLine($"Instancia,Tempo(ms),Utilizacao,Recipientes,TamanhoDoBando,TamanhoDoVoo,IteracoesMaximas");
                foreach (string texto in concurrentBag)
                {
                    w.WriteLine(texto);
                }
            }
        }
        catch { }
    }
}