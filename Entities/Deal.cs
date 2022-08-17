using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LesegaisParserTestJob.Entities;

public sealed class DealsContent
{
    [JsonProperty("content")]
    public List<Deal> Content { get; set; }
}

public sealed class Deal
{
    [JsonProperty("buyerInn")]
    public string BuyerInn { get; set; }

    [JsonProperty("buyerName")]
    public string BuyerName { get; set; }

    [JsonProperty("dealDate")]
    public DateTime? DealDate { get; set; }

    [JsonProperty("dealNumber")]
    public string DealNumber { get; set; }

    [JsonProperty("sellerInn")]
    public string SellerInn { get; set; }

    [JsonProperty("sellerName")]
    public string SellerName { get; set; }

    [JsonProperty("woodVolumeBuyer")]
    public double WoodVolumeBuyer { get; set; }

    [JsonProperty("woodVolumeSeller")]
    public double WoodVolumeSeller { get; set; }

    public override string ToString() => JsonConvert.SerializeObject(this);
}
