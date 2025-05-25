using Microsoft.AspNetCore.Mvc;

namespace ERPtask.servcies
{
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using ERPtask.models;
    using ERPtask.Repositrories;

    public class ClientService
    {
        private readonly ClientRepository _repository;

        public ClientService(ClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Client>> GetAllClients()
        {
            return await _repository.GetAllClients();
        }

        public async Task<Client> GetClient(Guid clientId)
        {
            return await _repository.GetClientById(clientId);
        }

        public async Task<Client> CreateClient(Client client)
        {
            if (string.IsNullOrWhiteSpace(client.Email))
                throw new ArgumentException("Email is required");

            return await _repository.CreateClient(client);
        }

        public async Task<bool> UpdateClient(Client client)
        {
            var existingClient = await _repository.GetClientById(client.ClientId);
            if (existingClient == null)
                return false;

            return await _repository.UpdateClient(client);
        }

        public async Task<bool> DeleteClient(Guid clientId)
        {
            var existingClient = await _repository.GetClientById(clientId);
            if (existingClient == null)
                return false;

            return await _repository.DeleteClient(clientId);
        }
    }
}
