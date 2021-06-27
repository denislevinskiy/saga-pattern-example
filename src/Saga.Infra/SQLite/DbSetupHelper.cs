using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using Dapper;

namespace Saga.Infra.SQLite
{
    [ExcludeFromCodeCoverage]
    public static class DbSetupHelper
    {
        public static void Setup(string dbFileName = "saga-pattern.sqlite")
        {
            static void CreateEmptyTables(IDbConnection conn)
            {
                conn.Execute(
                    $@"CREATE TABLE IF NOT EXISTS [SagaState]
                    (
                        Correlation VARCHAR(40) NOT NULL,
                        TimeStamp DATETIME NOT NULL,
                        State VARCHAR(100) NOT NULL,
                        Info TEXT NULL                        
                    );");
                
                conn.Execute("DELETE FROM [SagaState];");
                
                conn.Execute(
                    $@"CREATE TABLE IF NOT EXISTS [Order]
                    (
                        Id VARCHAR(40) NOT NULL PRIMARY KEY,
                        TimeStamp DATETIME NOT NULL,
                        CustomerId VARCHAR(40) NOT NULL
                    );");

                conn.Execute(
                    $@"CREATE TABLE IF NOT EXISTS OrderDetails
                    (
                        Id VARCHAR(40) NOT NULL PRIMARY KEY,
                        TimeStamp DATETIME NOT NULL,
                        OrderId VARCHAR(40) NOT NULL,
                        CatalogItemId VARCHAR(40) NOT NULL,
                        Qty NUMERIC NOT NULL,
                        Amount NUMERIC NOT NULL
                    );");
            
                conn.Execute("DELETE FROM [Order];");
                conn.Execute("DELETE FROM [OrderDetails];");
            
                conn.Execute(
                    $@"CREATE TABLE IF NOT EXISTS Catalog
                    (
                        Id VARCHAR(40) NOT NULL PRIMARY KEY,
                        TimeStamp DATETIME NOT NULL,
                        ProductName VARCHAR(100) NOT NULL,
                        Stock INTEGER NOT NULL
                    );");
            
                conn.Execute("DELETE FROM [Catalog];");
                
                conn.Execute(
                    $@"CREATE TABLE IF NOT EXISTS Customer
                    (
                        Id VARCHAR(40) NOT NULL PRIMARY KEY,
                        TimeStamp DATETIME NOT NULL,
                        OrdersAmount NUMERIC NOT NULL
                    );");
            
                conn.Execute("DELETE FROM [Customer];");
            }

            static void SeedData(IDbConnection conn)
            {
                CreateCatalogData().ForEach(item => conn.Execute(
                    $@"INSERT INTO Catalog
                    (
                        Id, 
                        TimeStamp, 
                        ProductName, 
                        Stock
                    )
                    VALUES 
                    (
                        @Id, 
                        @TimeStamp, 
                        @ProductName, 
                        @Stock
                    );", 
                    item));

                conn.Execute(
                    $@"INSERT INTO Customer
                    (
                        Id, 
                        TimeStamp, 
                        OrdersAmount
                    )
                    VALUES 
                    (
                        @Id, 
                        @TimeStamp, 
                        @OrdersAmount
                    );",
                    CreateCustomerData());
            }
            
            SqlMapper.AddTypeHandler(new GuidTypeDapperHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
            
            using var conn = new SQLiteConnection($"Data Source={dbFileName}");
            CreateEmptyTables(conn);
            SeedData(conn);
        }

        private static object CreateCustomerData()
        {
            return new
            {
                Id = Guid.Parse("2d5fe5ae-9f9e-414a-8425-529d075ac86b"),
                TimeStamp = DateTimeOffset.Now,
                OrdersAmount = 0
            };
        }

        private static List<object> CreateCatalogData()
        {
            return new()
            {
                new
                {
                    Id = Guid.Parse("7b31d64c-6aab-4632-85bd-7bd38f09aa76"),
                    TimeStamp = DateTimeOffset.Now,
                    ProductName = "T-shirt",
                    Stock = 20
                },
                new
                {
                    Id = Guid.Parse("53c8415c-1798-4a12-be5e-92b6d613095d"),
                    TimeStamp = DateTimeOffset.Now,
                    ProductName = "Shorts",
                    Stock = 15
                },
                new
                {
                    Id = Guid.Parse("6f85ba42-918e-4c4d-92c3-e44ebfe373a8"),
                    TimeStamp = DateTimeOffset.Now,
                    ProductName = "Sneakers",
                    Stock = 12
                },
            };
        }
    }
}