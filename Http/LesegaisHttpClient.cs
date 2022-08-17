using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using LesegaisParserTestJob.Entities;
using Newtonsoft.Json;

namespace LesegaisParserTestJob.Http;

public class LesegaisHttpClient
{
    private static readonly Uri ResourceUri = new("https://www.lesegais.ru/open-area/graphql");
    private const string UserAgent = "Mozilla / 5.0 (Windows NT 6.1; Win64; x64; rv: 47.0) Gecko / 20100101 Firefox / 47.0";
    private readonly HttpClient _client;

    public LesegaisHttpClient()
    {
        _client = new HttpClient();
        _client.BaseAddress = ResourceUri;
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
    }

    private T GetResponse<T>(string request)
    {
        var content = new StringContent(request, Encoding.UTF8, "application/json");

        var response = _client.PostAsync(ResourceUri, content).Result;
        response.EnsureSuccessStatusCode();

        var responseObject = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
        return responseObject;
    }

    public List<Deal> PostDeal(int size, int number, string filter = null, string orders = null)
    {
        var requestData = JsonConvert.SerializeObject(new
        {
            query = "query SearchReportWoodDeal($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) " +
                    "{\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) " +
                    "{\n    content {\n      sellerName\n      sellerInn\n      buyerName\n      buyerInn\n" +
                    "      woodVolumeBuyer\n      woodVolumeSeller\n      dealDate\n      dealNumber\n      __typename\n    }\n" +
                    "    __typename\n  }\n}\n",
            variables = new
            {
                size,
                number,
                filter,
                orders
            },
            operationName = "SearchReportWoodDeal"
        });

        var response = GetResponse<DealData>(requestData);

        return response.Data.SearchReport.Content;
    }

    public DealsCount PostDealsCount(int size, int number, string filter = null)
    {
        var requestData = JsonConvert.SerializeObject(new
        {
            query =
                "query SearchReportWoodDealCount($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) " +
                "{\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) " +
                "{\n    total\n    number\n    size\n    overallBuyerVolume\n    overallSellerVolume\n    __typename\n  }\n}\n",
            variables = new
            {
                size,
                number,
                filter
            },
            operationName = "SearchReportWoodDealCount"
        });

        var response = GetResponse<DealsCountData>(requestData);
        return response.Data.SearchReport;
    }
}