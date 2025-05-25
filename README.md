# ERP Task – Clients API

This project is part of an ERP system, providing a RESTful API to manage client records using **ASP.NET Core Web API**.

## Features

- Retrieve all clients
- Retrieve a client by ID
- Create a new client
- Update an existing client
- Delete a client

## Project Structure

ERPtask/
├── Controllers/
│ └── ClientsController.cs
├── Models/
│ └── Client.cs
├── Services/
│ └── ClientService.cs 

## API Endpoints

| Method | Endpoint              | Description                  |
|--------|-----------------------|------------------------------|
| GET    | `/api/clients`        | Get all clients              |
| GET    | `/api/clients/{id}`   | Get a specific client by ID  |
| POST   | `/api/clients`        | Create a new client          |
| PUT    | `/api/clients/{id}`   | Update an existing client    |
| DELETE | `/api/clients/{id}`   | Delete a client              |

## Data Transfer Objects

### ClientCreateDto

```csharp
public class ClientCreateDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Phone { get; set; }

    [Required]
    public string Address { get; set; }
}
public class ClientResponseDto
{
    public Guid ClientId { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
}
