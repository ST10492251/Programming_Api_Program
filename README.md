# RaceDayAPI – Race Day Management System

## 1. Project Overview

RaceDayAPI is a RESTful Web API developed using C# and ASP.NET Core. The system is designed to support the management of race-day activities, including user accounts, events, event categories, participant enrollments, race results, routes, and weather records.

The API connects to a Microsoft SQL Server database called `RaceDayDb` to store and retrieve information. Entity Framework Core is used to interact with the database, while Swagger provides an interface for testing and documenting API endpoints.

## 2. Project Objectives

The main objectives of RaceDayAPI are to:

- Provide an API for managing race events and related information.
- Support user registration, login, and logout.
- Allow users to view available events and retrieve event details.
- Manage participant enrollments for events.
- Store and retrieve race results and related records.
- Maintain data in a centralized SQL Server database.
- Provide a documented interface for testing API functionality.

## 3. Technologies Used

| Technology | Purpose |
|---|---|
| C# | Main programming language |
| ASP.NET Core Web API (.NET 8) | API development framework |
| Microsoft SQL Server | Database management |
| SQL Server Management Studio (SSMS) | Database administration |
| Entity Framework Core 8 | Database access and object-relational mapping |
| Swagger / Swashbuckle | API documentation and testing |
| Visual Studio | Development environment |
| Git and GitHub | Version control and source code management |

## 4. Database Description

The project uses a Microsoft SQL Server database named `RaceDayDb`.

The database supports the storage and management of information related to users, events, enrollments, categories, results, routes, and weather records.

The database context includes the following entities:

- **Users** – Stores user account information.
- **Events** – Stores information about race events.
- **Enrollments** – Records participants enrolled in events.
- **Categories** – Stores event category information.
- **EventCategories** – Associates events with their categories.
- **Results** – Stores race result information.
- **Routes** – Stores race route information.
- **WeatherRecords** – Stores weather information associated with race activities.

The database is accessed through Entity Framework Core using the `RaceDayDbContext` class.

### Database Entity Relationship Diagram (ERD)

The project database design is represented by the **RACEDAY DB ERD**, which illustrates the relationships between the database tables and their keys.

## 5. API Endpoints

The following endpoints describe the main API functionality. Confirm the final routes against the controllers in the project.

### Authentication

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Registers a new user |
| POST | `/api/auth/login` | Authenticates a user |
| POST | `/api/auth/logout` | Logs out the current user |

### Events

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/events` | Retrieves the available events |
| GET | `/api/events/{id}` | Retrieves a specific event |
| POST | `/api/events` | Creates a new event |

### Enrollments

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/enrollments` | Creates an event enrollment |
| GET | `/api/enrollments/events/{eventId}` | Retrieves enrollments for a specific event |

**Note:** Additional endpoints may be available for categories, results, routes, and weather records, depending on the implemented controllers.

## 6. Installation and Setup

### Prerequisites

Before running the project, ensure that you have installed:

- Visual Studio with the ASP.NET and web development workload.
- .NET 8 SDK.
- Microsoft SQL Server.
- SQL Server Management Studio (SSMS).
- Git, if cloning the project from GitHub.

### Step 1: Clone the Repository

Open a terminal and execute:

```bash
git clone YOUR_GITHUB_REPOSITORY_URL
```

Replace `YOUR_GITHUB_REPOSITORY_URL` with the actual URL of the RaceDayAPI repository.

Alternatively, download the repository as a ZIP file and extract it.

### Step 2: Open the Project

1. Open Visual Studio.
2. Select **Open a project or solution**.
3. Locate and open `RaceDayAPI.sln`.
4. Allow Visual Studio to restore the required NuGet packages.

### Step 3: Configure the Database Connection

Open `appsettings.json` and configure the connection string to match your local SQL Server instance.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS01;Database=RaceDayDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Important:** Use the correct SQL Server instance name and authentication method for your computer. Ensure that the database exists and that the Windows account running the API has permission to access it.

Do not commit database passwords, API secrets, or other sensitive credentials to GitHub.

### Step 4: Restore Dependencies

In Visual Studio, right-click the solution and select **Restore NuGet Packages**.

Alternatively, run the following command from the project directory:

```bash
dotnet restore
```

### Step 5: Build the Project

In Visual Studio, select **Build > Rebuild Solution**.

Alternatively, execute:

```bash
dotnet build
```

Resolve any compilation errors before proceeding.

### Step 6: Run the API

Press **F5** in Visual Studio to run the project with debugging enabled, or press **Ctrl + F5** to run without debugging.

The API will start using the launch settings configured in the project.

## 7. Testing the API

Swagger can be used to explore and test the available API endpoints.

1. Run the RaceDayAPI project.
2. Open the Swagger interface using the URL configured by Visual Studio.
3. Select an endpoint to expand its details.
4. Click **Try it out**.
5. Enter the required request information.
6. Click **Execute**.
7. Review the HTTP response code and response body.

For endpoints that require authentication, log in first and provide the required authentication credentials or session information.

## 8. Project Structure

The project may contain the following folders and files:

```text
RaceDayAPI/
├── Controllers/
│   ├── AuthController.cs
│   ├── EventsController.cs
│   └── EnrollmentsController.cs
├── Data/
│   └── RaceDayDbContext.cs
├── Models/
│   ├── User.cs
│   ├── Event.cs
│   ├── Enrollment.cs
│   ├── Category.cs
│   ├── EventCategory.cs
│   ├── Result.cs
│   ├── Route.cs
│   └── WeatherRecord.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json
├── Program.cs
├── RaceDayAPI.csproj
├── RaceDayAPI.sln
└── README.md
```


## 9. Error Handling and Troubleshooting

Common issues and possible solutions include:

- **Database connection errors:** Verify the SQL Server instance name, database name, connection string, and database permissions.
- **Login or access denied errors:** Ensure that the account used by the API has the necessary SQL Server login and database-user permissions.
- **Swagger does not open:** Check the configured launch URL and ensure that Swagger is enabled in the development environment.
- **Build errors:** Restore NuGet packages, inspect the error messages, and rebuild the solution.
- **HTTP 404 errors:** Confirm the controller route and endpoint URL.
- **HTTP 401 or 403 errors:** Check authentication status and authorization requirements.

## 10.YouTube link part 2
https://youtu.be/ruUMyVkAnos?si=qlZeJUxw8aO0sHe8


## 11. Conclusion

RaceDayAPI provides a foundation for managing race-day information through a RESTful API. By combining ASP.NET Core, C#, Entity Framework Core, and Microsoft SQL Server, the project supports structured data management and API-based communication. Swagger makes it easier to explore and test the API during development.

The project can be extended with additional endpoints, stronger authentication and authorization, validation, and further race-management functionality as requirements evolve.

---

**Project Name:** RaceDayAPI  
**Framework:** ASP.NET Core Web API (.NET 8)  
**Database:** Microsoft SQL Server (`RaceDayDb`)  
**Development Environment:** Visual Studio  
**Documentation and Testing:** Swagger
