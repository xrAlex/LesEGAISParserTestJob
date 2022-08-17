using Newtonsoft.Json;

namespace LesegaisParserTestJob.Entities;

public sealed class SearchReportWoodDeal<T>
{
    [JsonProperty("searchReportWoodDeal")]
    public T SearchReport { get; set; }
}

public sealed class DealsCountData
{
    [JsonProperty("data")]
    public SearchReportWoodDeal<DealsCount> Data { get; set; }
}


public sealed class DealData
{
    [JsonProperty("data")]
    public SearchReportWoodDeal<DealsContent> Data { get; set; }
}


