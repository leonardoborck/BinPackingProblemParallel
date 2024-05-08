using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinPackingProblemParallel.Classes
{
    public class Recipiente
    {
        [JsonProperty("Height")]
        public int Altura;
        [JsonProperty("Length")]
        public int Largura;
        [JsonProperty("Cost")]
        public int Custo;
        [JsonProperty("Reference")]
        public int CodigoDoRecipiente;

        public Recipiente(int altura, int largura, int custo, int codigoDoRecipiente)
        {
            Altura = altura; Largura = largura; Custo = custo; CodigoDoRecipiente = codigoDoRecipiente;
        }

        public Recipiente Clone() => (Recipiente)this.MemberwiseClone();
    }
}
