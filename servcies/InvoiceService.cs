using ERPtask.models;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ERPtask.Repositrories;
using ERPtask.Controllers;
namespace ERPtask.servcies
{
    public class InvoiceService
    {
        private readonly InvoiceRepository _invoiceRepository;
        private readonly TaxRuleRepository _taxRuleRepository;
        private readonly ClientRepository _clientRepository;

        public InvoiceService(
            InvoiceRepository invoiceRepo,
            TaxRuleRepository taxRuleRepo,
            ClientRepository clientRepo)
        {
            _invoiceRepository = invoiceRepo;
            _taxRuleRepository = taxRuleRepo;
            _clientRepository = clientRepo;
        }
     
        public async Task<List<Invoice>> GetAllInvoices()
        {
            return await _invoiceRepository.GetAllInvoices();
        }
        public async Task<Invoice> GenerateInvoice(
             Guid clientId,
             List<InvoiceItemCreateDto> itemsDto,
             string region,
             decimal discounts)
        {
            var client = await _clientRepository.GetClientById(clientId);
            if (client == null) throw new ArgumentException("Client not found");

            var items = itemsDto.Select(i => new InvoiceItem
            {
                Name = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            var subtotal = items.Sum(i => i.Quantity * i.UnitPrice);
            var taxRate = await _taxRuleRepository.GetTaxRateByRegion(region);
            var taxableAmount = subtotal - discounts;
            var tax = taxableAmount * taxRate;
            var total = taxableAmount + tax;

            var invoice = new Invoice
            {
                InvoiceId = Guid.NewGuid(),
                ClientId = clientId,
                Subtotal = subtotal,
                Tax = tax,
                Discounts = discounts,
                Total = total,
                DueDate = DateTime.UtcNow.AddDays(10),
                Status = "Draft",
                Items = items
            };

            return await _invoiceRepository.CreateInvoice(invoice);
        }

        public async Task<Invoice> GetInvoiceById(Guid id)
        {
            return await _invoiceRepository.GetInvoiceById(id);
        }

        public async Task<bool> UpdateInvoiceStatus(Guid invoiceId, string status)//('Draft', 'Sent', 'Paid', 'Overdue')
        {
            return await _invoiceRepository.UpdateInvoiceStatus(invoiceId, status);
        }

        public async Task<bool> DeleteInvoice(Guid invoiceId)
        {
            return await _invoiceRepository.DeleteInvoice(invoiceId);
        }
    }
}
