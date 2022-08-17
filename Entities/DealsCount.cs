using Newtonsoft.Json;

namespace LesegaisParserTestJob.Entities;

public sealed class DealsCount
{
    [JsonProperty("number")]
    public int Number { get; set; }

    [JsonProperty("overallBuyerVolume")]
    public double OverallBuyerVolume { get; set; }

    [JsonProperty("overallSellerVolume")]
    public double OverallSellerVolume { get; set; }

    [JsonProperty("size")]
    public int Size { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }

    public override string ToString() => JsonConvert.SerializeObject(this);
}