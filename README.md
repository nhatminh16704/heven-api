# Heven.Api - Room Rental Platform

**Heven.Api** is a modern, scalable backend application built for a property and room rental platform (inspired by Airbnb). Developed with **.NET 8** and adhering to **Clean Architecture** principles, this API serves as the core engine for handling property listings, user bookings, host management, and secure transactions.

## 🌟 Business Features (Platform Capabilities)
- **Property Listings:** Hosts can create, update, and manage room/property listings with details, images, and pricing.
- **Search & Filter:** Guests can discover properties based on location, availability, and specific amenities.
- **Booking & Reservations:** Seamless booking engine handling availability checking, reservations, and cancellations.
- **User Roles:** Distinct boundaries and workflows for Guests, Hosts, and Administrators.
- **Reviews & Ratings:** System for guests to leave feedback on their stays.

## 🚀 Technical Features
- **Clean Architecture:** Clear separation between Domain, Application, Infrastructure, and Web layers.
- **CQRS Pattern:** Efficient data querying and command processing using MediatR.
- **Standardized API Responses:** A unified JSON response wrapper for all requests (Success, Data, Error) to easily integrate with Frontend or Mobile apps.
- **Global Exception Handling:** Advanced and centralized error management using the latest .NET 8 `IExceptionHandler`.
- **.NET 8 Minimal APIs:** High-performance, low-allocation routing.
- **Entity Framework Core:** Strong data access layer integrated with SQL Server.
- **Authentication & Authorization:** JWT-based security securing private endpoints and role-based actions.

## 🛠 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (Current) or Visual Studio Code
- SQL Server (LocalDB, Express, or Docker instance)

### Setup & Build
1. Clone the repository and navigate to the root directory.
2. Build the solution to restore dependencies:

### Database Initialization
On the first run in the `Development` environment, the application will automatically run Entity Framework migrations to construct the schema and seed initial data (e.g., default users or categories).
Ensure your SQL connection string in `src/Web/appsettings.json` (or `appsettings.Development.json`) is correct.

### Run the Application
Navigate to the Web project and start the server:

Once started, explore the API definitions and test endpoints via Swagger UI:
👉 **https://localhost:5001/api**

## 📂 Project Structure
- **src/Domain:** Enterprise business rules, entities (e.g., `Property`, `Booking`, `User`), and value objects.
- **src/Application:** Application logic, use cases (CQRS), MediatR handlers, DTOs, and interfaces.
- **src/Infrastructure:** Database contexts, repositories, migrations, identity, and third-party services integration.
- **src/Web:** Presentation layer containing the Minimal API endpoints, standard response filters, and middleware configurations.

## 🧪 Testing
The solution contains a comprehensive suite of unit, integration, and functional tests to ensure the reliability of the booking flow and data integrity.

To execute all tests:

## 📝 Code Style & Formatting
The project strictly follows coding conventions defined in the `.editorconfig` file. Ensure your IDE is configured to respect these rules before submitting any pull requests.