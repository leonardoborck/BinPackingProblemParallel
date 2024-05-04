using Newtonsoft.Json;

namespace BinPackingProblemParallel.Classes
{
    public class BaseDeDados
    {
        [JsonProperty("Items")]
        public List<Item> Itens;

        [JsonProperty("Objects")]
        public List<Recipiente> Recipientes;

        [JsonIgnore()]
        public string Nome;
    }
}
