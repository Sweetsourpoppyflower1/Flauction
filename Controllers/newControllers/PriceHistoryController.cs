using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace Flauction.Controllers.newControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceHistoryController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<PriceHistoryController> _logger;

        public PriceHistoryController(IConfiguration configuration, ILogger<PriceHistoryController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        [HttpGet("plant/{plantId}")]
        public async Task<IActionResult> GetPriceHistory(int plantId)
        {
            try
            {
                _logger.LogInformation($"GetPriceHistory called for plantId: {plantId}");
                
                var result = new
                {
                    currentSupplierHistory = await GetSupplierPriceHistory(plantId),
                    allSuppliersHistory = await GetAllSuppliersPriceHistory(plantId),
                    currentSupplierAverage = await GetSupplierAveragePrice(plantId),
                    allSuppliersAverage = await GetAllSuppliersAveragePrice(plantId)
                };

                _logger.LogInformation($"GetPriceHistory returning data for plantId: {plantId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetPriceHistory: {ex.Message} | StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        private async Task<List<dynamic>> GetSupplierPriceHistory(int plantId)
        {
            var history = new List<dynamic>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string priceHistorySql = @"
                        SELECT TOP 10
                            pph.pricehistory_id,
                            pph.plant_id,
                            pph.old_min_price,
                            pph.old_start_price,
                            pph.new_min_price,
                            pph.new_start_price,
                            pph.changed_at,
                            pph.changed_by
                        FROM PlantPriceHistory pph
                        WHERE pph.plant_id = @plantId
                        ORDER BY pph.changed_at DESC";

                    using (SqlCommand cmd = new SqlCommand(priceHistorySql, conn))
                    {
                        cmd.Parameters.AddWithValue("@plantId", plantId);
                        await conn.OpenAsync();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                try
                                {
                                    history.Add(new
                                    {
                                        pricehistory_id = (int)reader["pricehistory_id"],
                                        plant_id = (int)reader["plant_id"],
                                        date = ((DateTime)reader["changed_at"]).ToString("dd MMMM yyyy HH:mm:ss"),
                                        changed_at = (DateTime)reader["changed_at"],
                                        old_min_price = Convert.ToDecimal(reader["old_min_price"]),
                                        old_start_price = Convert.ToDecimal(reader["old_start_price"]),
                                        new_min_price = Convert.ToDecimal(reader["new_min_price"]),
                                        new_start_price = Convert.ToDecimal(reader["new_start_price"]),
                                        changed_by = reader["changed_by"]?.ToString() ?? "Unknown"
                                    });
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning($"Error reading supplier price history row: {ex.Message}");
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetSupplierPriceHistory: {ex.Message}");
            }

            return history;
        }

        private async Task<List<dynamic>> GetAllSuppliersPriceHistory(int plantId)
        {
            var history = new List<dynamic>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string sql = @"
                        SELECT TOP 10
                            pph.plant_id,
                            COALESCE(p.productname, 'Unknown') as product_name,
                            pph.new_start_price as price,
                            pph.changed_at as date,
                            pph.changed_by,
                            COALESCE(s.name, 'Unknown Supplier') as supplier_name
                        FROM PlantPriceHistory pph
                        LEFT JOIN Plant p ON pph.plant_id = p.plant_id
                        LEFT JOIN Supplier s ON p.supplier_id = s.Id
                        WHERE pph.plant_id = @plantId
                        ORDER BY pph.changed_at DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@plantId", plantId);
                        await conn.OpenAsync();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                try
                                {
                                    var supplierName = reader["supplier_name"].ToString();
                                    var productName = reader["product_name"].ToString();
                                    var price = Convert.ToDecimal(reader["price"]);
                                    
                                    _logger.LogInformation($"Price history row - Product: {productName}, Supplier: {supplierName}, Price: {price}");
                                    
                                    history.Add(new
                                    {
                                        productName = productName,
                                        supplierName = supplierName,
                                        date = ((DateTime)reader["date"]).ToString("dd MMMM yyyy HH:mm:ss"),
                                        price = price
                                    });
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning($"Error reading all suppliers price history row: {ex.Message}");
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAllSuppliersPriceHistory: {ex.Message}");
            }

            return history;
        }

        private async Task<decimal> GetSupplierAveragePrice(int plantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string sql = @"
                        SELECT AVG(CAST(new_start_price AS DECIMAL(18,2))) as avg_price
                        FROM PlantPriceHistory
                        WHERE plant_id = @plantId AND new_start_price > 0";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@plantId", plantId);
                        await conn.OpenAsync();
                        var result = await cmd.ExecuteScalarAsync();
                        return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetSupplierAveragePrice: {ex.Message}");
                return 0m;
            }
        }

        private async Task<decimal> GetAllSuppliersAveragePrice(int plantId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string sql = @"
                        SELECT AVG(CAST(new_start_price AS DECIMAL(18,2))) as avg_price
                        FROM PlantPriceHistory
                        WHERE plant_id = @plantId AND new_start_price > 0";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@plantId", plantId);
                        await conn.OpenAsync();
                        var result = await cmd.ExecuteScalarAsync();
                        return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAllSuppliersAveragePrice: {ex.Message}");
                return 0m;
            }
        }
    }
}