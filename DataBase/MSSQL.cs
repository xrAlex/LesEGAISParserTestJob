using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LesegaisParserTestJob.Entities;

namespace LesegaisParserTestJob.DataBase;

public sealed class MSSQL
{
    private readonly SqlConnection _connection;

    private static readonly DateTime MinDate = new (1754, 1, 1);
    private static readonly DateTime MaxDate = new (9999, 12, 31);

    public void Connection() => _connection.Open();

    public void Disconnect() => _connection.Close();

    public MSSQL(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
    }

    private static DataTable GetDealDataTable()
    {
        var tbl = new DataTable();
        tbl.Columns.Add(new DataColumn("dealNumber", typeof(string)));
        tbl.Columns.Add(new DataColumn("dealDate", typeof(DateTime)));
        tbl.Columns.Add(new DataColumn("buyerInn", typeof(string)));
        tbl.Columns.Add(new DataColumn("buyerName", typeof(string)));
        tbl.Columns.Add(new DataColumn("sellerInn", typeof(string)));
        tbl.Columns.Add(new DataColumn("sellerName", typeof(string)));
        tbl.Columns.Add(new DataColumn("woodVolumeBuyer", typeof(double)));
        tbl.Columns.Add(new DataColumn("woodVolumeSeller", typeof(double)));

        return tbl;
    }

    public void Bulk(List<Deal> deals)
    {
        var tbl = GetDealDataTable();

        foreach (var deal in deals)
        {
            if (GetCurrentDealFromDb(deal) > 0) continue;

            var dataRow = tbl.NewRow();
            var isDealDateIncorrect = deal.DealDate == null || deal.DealDate < MinDate || deal.DealDate > MaxDate;

            dataRow["dealNumber"] = deal.DealNumber;
            dataRow["dealDate"] = isDealDateIncorrect ? DBNull.Value : deal.DealDate;
            dataRow["buyerInn"] = string.IsNullOrEmpty(deal.BuyerInn) ? DBNull.Value : deal.BuyerInn;
            dataRow["buyerName"] = string.IsNullOrEmpty(deal.BuyerName) ? DBNull.Value : deal.BuyerName;
            dataRow["sellerInn"] = string.IsNullOrEmpty(deal.BuyerInn) ? DBNull.Value : deal.SellerInn;
            dataRow["sellerName"] = string.IsNullOrEmpty(deal.SellerName) ? DBNull.Value : deal.SellerName;
            dataRow["woodVolumeBuyer"] = deal.WoodVolumeBuyer;
            dataRow["woodVolumeSeller"] = deal.WoodVolumeSeller;
            tbl.Rows.Add(dataRow);
        }

        using var bulk = new SqlBulkCopy(_connection);

        bulk.DestinationTableName = "Deals";

        bulk.ColumnMappings.Add("dealNumber", "dealNumber");
        bulk.ColumnMappings.Add("dealDate", "dealDate");
        bulk.ColumnMappings.Add("buyerInn", "buyerInn");
        bulk.ColumnMappings.Add("buyerName", "buyerName");
        bulk.ColumnMappings.Add("sellerInn", "sellerInn");
        bulk.ColumnMappings.Add("sellerName", "sellerName");
        bulk.ColumnMappings.Add("woodVolumeBuyer", "woodVolumeBuyer");
        bulk.ColumnMappings.Add("woodVolumeSeller", "woodVolumeSeller");

        bulk.WriteToServer(tbl);
    }

    private int GetCurrentDealFromDb(Deal deal)
    {
        const string queryString = @"SELECT COUNT(*) FROM Deals WHERE dealNumber=@dealNumber " + 
                                   "AND dealDate=@dealDate " +
                                   "AND buyerInn=@buyerInn " +
                                   "AND buyerName=@buyerName " +
                                   "AND sellerInn=@sellerInn " +
                                   "AND sellerName=@sellerName " +
                                   "AND woodVolumeBuyer=@woodVolumeBuyer " +
                                   "AND woodVolumeSeller=@woodVolumeSeller";
        
        using var command = new SqlCommand(queryString, _connection);
        var isDealDateIncorrect = deal.DealDate == null || deal.DealDate < MinDate || deal.DealDate > MaxDate;

        command.Parameters
            .AddWithValue("buyerInn", string.IsNullOrEmpty(deal.BuyerInn) ? DBNull.Value : deal.BuyerInn);

        command.Parameters
            .AddWithValue("buyerName", string.IsNullOrEmpty(deal.BuyerName) ? DBNull.Value : deal.BuyerName);

        command.Parameters
            .AddWithValue("dealDate", isDealDateIncorrect ? DBNull.Value : deal.DealDate);

        command.Parameters
            .AddWithValue("sellerInn", string.IsNullOrEmpty(deal.SellerInn) ? DBNull.Value : deal.SellerInn);

        command.Parameters
            .AddWithValue("sellerName", string.IsNullOrEmpty(deal.SellerName) ? DBNull.Value : deal.SellerName);

        command.Parameters.AddWithValue("dealNumber", deal.DealNumber);
        command.Parameters.AddWithValue("woodVolumeBuyer", deal.WoodVolumeBuyer);
        command.Parameters.AddWithValue("woodVolumeSeller", deal.WoodVolumeSeller);

        return (int)command.ExecuteScalar();
    }

    public int GetDealsCount()
    {
        using var command = new SqlCommand();
        command.Connection = _connection;
        command.CommandText = "SELECT COUNT(*) FROM Deals";
        return (int)command.ExecuteScalar();
    }
}