using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinPackingProblemParallel.Classes
{
    public class Item
    {
        [JsonProperty("Height")]
        public int Altura;
        [JsonProperty("Length")]
        public int Largura;
        [JsonProperty("Demand")]
        public int Demanda;
        [JsonProperty("Value")]
        public int Area;
        [JsonProperty("Reference")]
        public int CodigoDoItem;

        public Item(int altura, int largura, int demanda, int area, int codigoDoItem)
        {
            Altura = altura; Largura = largura; Demanda = demanda; Area = area; CodigoDoItem = codigoDoItem;
        }

        public Item Clone() => (Item)this.MemberwiseClone();
    }
}
