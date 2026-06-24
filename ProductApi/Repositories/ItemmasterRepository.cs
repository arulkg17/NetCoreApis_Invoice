using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductApi.Contracts;
using ProductApi.Data;
using ProductApi.DTOs;
using ProductApi.Entities;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ProductApi.Repositories;

public class ItemmasterRepository : IItemmasterRepository
{
    private readonly AppDbContext _dbContext;

    public ItemmasterRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<int> AddAsync(Itemmaster item)
    {
        var result = await _dbContext.Database.ExecuteSqlRawAsync(
            @"EXEC sp_Itemmaster_Insert 
                @CatCode,
                @ItemBarCode,
                @Itemcode,
                @Itemname,
                @Description,
                @Uom,
                @Rate,
                @Minimumstock,
                @Maximumstock,
                @IsActive",
            new SqlParameter("@CatCode", item.CatCode),
            new SqlParameter("@ItemBarCode", item.ItemBarCode),
            new SqlParameter("@Itemcode", item.ItemCode),
            new SqlParameter("@Itemname", item.ItemName),
            new SqlParameter("@Description", (object?)item.Description ?? DBNull.Value),
            new SqlParameter("@Uom", item.Uom),
            new SqlParameter("@Rate", (object?)item.Rate ?? DBNull.Value),
            new SqlParameter("@Minimumstock", (object?)item.MinimumStock ?? DBNull.Value),
            new SqlParameter("@Maximumstock", (object?)item.MaximumStock ?? DBNull.Value),
            new SqlParameter("@IsActive", (object?)item.IsActive ?? DBNull.Value)
        );
 
        return result; // returns affected rows
    }

    public async Task<bool> UpdateAsync(Itemmaster itemmaster)
    {
        var affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(
            @"EXEC sp_Itemmaster_Update
                @Id,
                @CatCode,
                @ItemBarCode,
                @Itemcode,
                @Itemname,
                @Description,
                @Uom,
                @Rate,
                @Minimumstock,
                @Maximumstock,
                @IsActive",
            new SqlParameter("@Id", itemmaster.Id),
            new SqlParameter("@CatCode", itemmaster.CatCode),
            new SqlParameter("@ItemBarCode", itemmaster.ItemBarCode),
            new SqlParameter("@Itemcode", itemmaster.ItemCode),
            new SqlParameter("@Itemname", itemmaster.ItemName),
            new SqlParameter("@Description", (object?)itemmaster.Description ?? DBNull.Value),
            new SqlParameter("@Uom", itemmaster.Uom),
            new SqlParameter("@Rate", (object?)itemmaster.Rate ?? DBNull.Value),
            new SqlParameter("@Minimumstock", (object?)itemmaster.MinimumStock ?? DBNull.Value),
            new SqlParameter("@Maximumstock", (object?)itemmaster.MaximumStock ?? DBNull.Value),
            new SqlParameter("@IsActive", (object?)itemmaster.IsActive ?? DBNull.Value)
        );

        return affectedRows > 0;
    }

    public async Task<Itemmaster?> GetByIdAsync(int id)
    {
        var items = await _dbContext.Itemmasters
            .FromSqlRaw("EXEC sp_Itemmaster_GetById @Id",
                new SqlParameter("@Id", id))
            .AsNoTracking()
            .ToListAsync();

        return items.FirstOrDefault();
    }
    public async Task<IEnumerable<Itemmaster>> GetAllAsync()
    {
        return await _dbContext.Itemmasters
            .FromSqlRaw("EXEC sp_Itemmaster_GetALL")
            .ToListAsync();
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(
            "EXEC sp_Itemmaster_Delete @Id",
            new SqlParameter("@Id", id));

        return affectedRows > 0;
    }
    public async Task<PagedResultDto<Itemmaster>> GetAllPagedAsync(
    string? catCode,
    string? itemName,
    string? uom,
    int pageNumber,
    int pageSize)
    {
        using (var connection = _dbContext.Database.GetDbConnection())
        { 
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "sp_Itemmaster_GetPaged";
            
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@CatCode", (object?)catCode ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@ItemName", (object?)itemName ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@Uom", (object?)uom ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
            command.Parameters.Add(new SqlParameter("@PageSize", pageSize));

            using var reader = await command.ExecuteReaderAsync();

            var items = new List<Itemmaster>();

            while (await reader.ReadAsync())
            {
                items.Add(new Itemmaster
                {
                    Id = reader.GetInt32(0),
                    CatCode = reader.GetString(1),
                    ItemBarCode = reader.GetString(2),
                    ItemCode = reader.GetString(3),
                    ItemName = reader.GetString(4),
                    Description = reader.GetString(5),
                    Uom = reader.GetString(6),
                    Rate = reader.GetDecimal(7),
                    MinimumStock = reader.GetDecimal(8),
                    MaximumStock = reader.GetDecimal(9)
                });
            }

            await reader.NextResultAsync();

            int totalRecords = 0;
            if (await reader.ReadAsync())
            {
                totalRecords = reader.GetInt32(0);
            }

            return new PagedResultDto<Itemmaster>
            {
                Data = items,
                TotalRecords = totalRecords
            };

        }

        //var catParam = new SqlParameter("@CatCode", (object?)catCode ?? DBNull.Value);
        //var itemParam = new SqlParameter("@ItemName", (object?)itemName ?? DBNull.Value);
        //var uomParam = new SqlParameter("@Uom", (object?)uom ?? DBNull.Value);
        //var pageParam = new SqlParameter("@PageNumber", pageNumber);
        //var sizeParam = new SqlParameter("@PageSize", pageSize);

        //var data = await _dbContext.Itemmasters
        //    .FromSqlRaw("EXEC sp_Itemmaster_GetPaged @CatCode, @ItemName, @Uom, @PageNumber, @PageSize",
        //        catParam, itemParam, uomParam, pageParam, sizeParam)
        //    .ToListAsync();

        //// 🔥 total count (separate SP or simple query)
        //var totalRecords = data.FirstOrDefault()?.TotalRecords ?? 0;

        //return new PagedResultDto<Itemmaster>
        //{
        //    Data = data,
        //    TotalRecords = totalRecords
        //};
    }

}
