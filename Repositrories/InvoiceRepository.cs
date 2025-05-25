using ERPtask.models;
using System.Data.SqlClient;

namespace ERPtask.Repositrories
{
    public class InvoiceRepository
    {
        private readonly string _connectionString;

        public InvoiceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Invoice> CreateInvoice(Invoice invoice)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert main invoice
                        var cmd = new SqlCommand(
                            @"INSERT INTO Invoices 
                            (InvoiceId, ClientId, Subtotal, Tax, Discounts, Total, DueDate, Status)
                            VALUES (@Id, @ClientId, @Subtotal, @Tax, @Discounts, @Total, @DueDate, @Status)",
                            connection, transaction);

                        cmd.Parameters.AddWithValue("@Id", invoice.InvoiceId);
                        cmd.Parameters.AddWithValue("@ClientId", invoice.ClientId);
                        cmd.Parameters.AddWithValue("@Subtotal", invoice.Subtotal);
                        cmd.Parameters.AddWithValue("@Tax", invoice.Tax);
                        cmd.Parameters.AddWithValue("@Discounts", invoice.Discounts);
                        cmd.Parameters.AddWithValue("@Total", invoice.Total);
                        cmd.Parameters.AddWithValue("@DueDate", invoice.DueDate);
                        cmd.Parameters.AddWithValue("@Status", invoice.Status);
                        await cmd.ExecuteNonQueryAsync();

                        // Insert invoice items
                        foreach (var item in invoice.Items)
                        {
                            var itemCmd = new SqlCommand(
                                @"INSERT INTO InvoiceItems 
                            (ItemId, InvoiceId, Name, Quantity, UnitPrice)
                            VALUES (@ItemId, @InvoiceId, @Name, @Quantity, @UnitPrice)",
                                connection, transaction);

                            itemCmd.Parameters.AddWithValue("@ItemId", Guid.NewGuid());
                            itemCmd.Parameters.AddWithValue("@InvoiceId", invoice.InvoiceId);
                            itemCmd.Parameters.AddWithValue("@Name", item.Name);
                            itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                            itemCmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                            await itemCmd.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                        return invoice;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<Invoice> GetInvoiceById(Guid invoiceId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var invoice = new Invoice();

                // Get main invoice
                var cmd = new SqlCommand(
                    "SELECT * FROM Invoices WHERE InvoiceId = @InvoiceId",
                    connection);
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        invoice = new Invoice
                        {
                            InvoiceId = reader.GetGuid(0),
                            ClientId = reader.GetGuid(1),
                            Subtotal = reader.GetDecimal(2),
                            Tax = reader.GetDecimal(3),
                            Discounts = reader.GetDecimal(4),
                            Total = reader.GetDecimal(5),
                            DueDate = reader.GetDateTime(6),
                            Status = reader.GetString(7),
                            Items = new List<InvoiceItem>()
                        };
                    }
                    else return null;
                }

                // Get invoice items
                var itemsCmd = new SqlCommand(
                    "SELECT * FROM InvoiceItems WHERE InvoiceId = @InvoiceId",
                    connection);
                itemsCmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

                using (var itemsReader = await itemsCmd.ExecuteReaderAsync())
                {
                    while (await itemsReader.ReadAsync())
                    {
                        invoice.Items.Add(new InvoiceItem
                        {
                            ItemId = itemsReader.GetGuid(0),
                            InvoiceId = itemsReader.GetGuid(1),
                            Name = itemsReader.GetString(2),
                            Quantity = itemsReader.GetInt32(3),
                            UnitPrice = itemsReader.GetDecimal(4)
                        });
                    }
                }

                return invoice;
            }
        }

        public async Task<bool> UpdateInvoiceStatus(Guid invoiceId, string status)//('Draft', 'Sent', 'Paid', 'Overdue')
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(
                    "UPDATE Invoices SET Status = @Status WHERE InvoiceId = @InvoiceId",
                    connection);

                cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);
                cmd.Parameters.AddWithValue("@Status", status);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> DeleteInvoice(Guid invoiceId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete items first
                        var itemsCmd = new SqlCommand(
                            "DELETE FROM InvoiceItems WHERE InvoiceId = @InvoiceId",
                            connection, transaction);
                        itemsCmd.Parameters.AddWithValue("@InvoiceId", invoiceId);
                        await itemsCmd.ExecuteNonQueryAsync();

                        // Delete invoice
                        var invoiceCmd = new SqlCommand(
                            "DELETE FROM Invoices WHERE InvoiceId = @InvoiceId",
                            connection, transaction);
                        invoiceCmd.Parameters.AddWithValue("@InvoiceId", invoiceId);
                        var result = await invoiceCmd.ExecuteNonQueryAsync();

                        transaction.Commit();
                        return result > 0;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public async Task<List<Invoice>> GetAllInvoices()
        {
            var invoices = new Dictionary<Guid, Invoice>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(
                    @"SELECT i.*, ii.ItemId, ii.Name, ii.Quantity, ii.UnitPrice 
                    FROM Invoices i
                    LEFT JOIN InvoiceItems ii ON i.InvoiceId = ii.InvoiceId
                    ORDER BY i.InvoiceId", connection);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var invoiceId = reader.GetGuid(0);

                        if (!invoices.ContainsKey(invoiceId))
                        {
                            invoices[invoiceId] = new Invoice
                            {
                                InvoiceId = invoiceId,
                                ClientId = reader.GetGuid(1),
                                Subtotal = reader.GetDecimal(2),
                                Tax = reader.GetDecimal(3),
                                Discounts = reader.GetDecimal(4),
                                Total = reader.GetDecimal(5),
                                DueDate = reader.GetDateTime(6),
                                Status = reader.GetString(7),
                                Items = new List<InvoiceItem>()
                            };
                        }

                        if (!reader.IsDBNull(8)) 
                        {
                            var item = new InvoiceItem
                            {
                                ItemId = reader.GetGuid(8),
                                InvoiceId = invoiceId,
                                Name = reader.GetString(9),
                                Quantity = reader.GetInt32(10),
                                UnitPrice = reader.GetDecimal(11)
                            };
                            invoices[invoiceId].Items.Add(item);
                        }
                    }
                }
            }
            return invoices.Values.ToList();
        }
    }
    public class TaxRuleRepository
    {
        private readonly string _connectionString;

        public TaxRuleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<decimal> GetTaxRateByRegion(string region)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(
                    "SELECT TaxRate FROM TaxRules WHERE Region = @Region",
                    connection);
                cmd.Parameters.AddWithValue("@Region", region);

                var result = await cmd.ExecuteScalarAsync();
                return result != null ? Convert.ToDecimal(result) : 0m;
            }
        }
        public async Task CreateTaxRule(TaxRule rule)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(
                    @"INSERT INTO TaxRules (Region, TaxRate)
                VALUES (@Region, @TaxRate)", connection);

                cmd.Parameters.AddWithValue("@Region", rule.Region);
                cmd.Parameters.AddWithValue("@TaxRate", rule.TaxRate);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<TaxRule>> GetAllTaxRules()
        {
            var rules = new List<TaxRule>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM TaxRules", connection);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        rules.Add(new TaxRule
                        {
                            Region = reader.GetString(0),
                            TaxRate = reader.GetDecimal(1)
                        });
                    }
                }
            }
            return rules;
        }
    }
}
