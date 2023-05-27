using Newtonsoft.Json;

namespace Dsh
{
    public struct ConfigJSON
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("prefix")]
        public string Prefix { get; set; }
    }
}
