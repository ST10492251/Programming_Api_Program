# RaceDayAPI – Race Day Management System

## 1. Project Overview

RaceDay is a web-based Race Day Management System developed using C# and ASP.NET Core. The solution consists of two main applications:

- **RaceDayAPI (Part 2):** A RESTful Web API that handles application data, authentication, business logic, and database operations.
- **RaceDayMVC (Part 3):** An ASP.NET Core MVC application that provides a user-friendly interface and communicates with RaceDayAPI.

The system supports the management of race-day activities, including user accounts, events, event categories, participant enrolments, race results, routes, and weather records.

The applications connect to a Microsoft SQL Server database called `RaceDayDb`. Entity Framework Core is used to interact with the database, while Swagger provides an interface for documenting and testing API endpoints.

## 2. Project Objectives

The main objectives of RaceDay are to:

- Develop a RESTful API for managing race events and related information.
- Provide user registration, login, and logout functionality.
- Support different user roles, including Organisers and Participants.
- Allow users to browse available race events and retrieve event details.
- Enable participants to enrol in race events.
- Allow organisers to manage events and event categories.
- Provide functionality for viewing participant enrolments.
- Store and retrieve race results.
- Allow participants to view their race results and finishing positions.
- Maintain application data in a centralized SQL Server database.
- Provide an MVC interface that communicates with the Web API.
- Support API documentation and testing through Swagger.

## 3. Technologies Used

| Technology | Purpose |
|---|---|
| C# | Main programming language |
| ASP.NET Core Web API (.NET 8) | Backend API development |
| ASP.NET Core MVC (.NET 8) | Frontend web application |
| Microsoft SQL Server | Database management |
| SQL Server Management Studio (SSMS) | Database administration |
| Entity Framework Core 8 | Database access and object-relational mapping |
| Swagger / Swashbuckle | API documentation and testing |
| HTML5 | Web page structure |
| CSS3 | Web page styling |
| Bootstrap | Responsive user interface |
| JavaScript | Client-side functionality |
| Visual Studio | Development environment |
| Git and GitHub | Version control and source code management |

## 4. System Architecture

The RaceDay solution follows a client-server architecture.

### RaceDayAPI

The API acts as the backend of the application. It processes HTTP requests, applies business logic, handles authentication, and communicates with the SQL Server database.

### RaceDayMVC

The MVC application acts as the frontend. It sends HTTP requests to RaceDayAPI using `HttpClient` and displays the returned information through Razor views.

### Database

Microsoft SQL Server stores the application's information. Entity Framework Core provides communication between the API and the database.

The general communication process is:

1. The user interacts with the MVC application.
2. The MVC controller sends an HTTP request to RaceDayAPI.
3. The API processes the request.
4. Entity Framework Core communicates with SQL Server when database operations are required.
5. The API returns an HTTP response.
6. The MVC application displays the returned information to the user.

## 5. Database Description

The project uses a Microsoft SQL Server database named `RaceDayDb`.

The database supports the storage and management of information related to users, events, enrolments, categories, results, routes, and weather records.

The database context is managed through the `RaceDayDbContext` class.

The main database entities include:

- **Users** – Stores user account information, including names, email addresses, passwords, and roles.
- **Events** – Stores race event information, including event names, dates, locations, descriptions, and statuses.
- **Enrollments** – Records participants enrolled in events.
- **Categories** – Stores event category information.
- **EventCategories** – Associates events with their available categories and entry information.
- **Results** – Stores race result information, including finishing times, positions, and result statuses.
- **Routes** – Stores race route information.
- **WeatherRecords** – Stores weather information associated with race activities.

### Database Entity Relationship Diagram (ERD)

The database design is represented by the **RACEDAY DB ERD**.

The ERD illustrates the relationships between the database tables, primary keys, and foreign keys.

## 6. RaceDayAPI – Part 2

RaceDayAPI is the backend Web API responsible for processing requests and managing application data.

### 6.1 Authentication

The API provides authentication functionality through the following endpoints:

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Registers a new user |
| POST | `/api/auth/login` | Authenticates a user |
| POST | `/api/auth/logout` | Logs out the current user |

Passwords are hashed before being stored in the database.

The application uses role-based authorization to distinguish between Organisers and Participants.

### 6.2 Events

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/events` | Retrieves available events |
| GET | `/api/events/{id}` | Retrieves a specific event |
| GET | `/api/events/published` | Retrieves published events |
| POST | `/api/events` | Creates a new event |
| PUT | `/api/events/{id}` | Updates an existing event |
| DELETE | `/api/events/{id}` | Deletes an event |

### 6.3 Enrollments

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/enrollments` | Creates a participant enrolment |
| GET | `/api/enrollments/mine` | Retrieves the current participant's enrolments |
| GET | `/api/enrollments/events/{eventId}` | Retrieves enrolments for a specific event |

### 6.4 Categories

The Categories controller provides functionality for managing event categories, including retrieving and managing category information.

### 6.5 Results

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/results/mine` | Retrieves the current participant's results |
| POST | `/api/results` | Saves or updates a participant's race result |

The results functionality allows organisers to record finishing times, finishing positions, and result statuses.

Participants can retrieve their own results through the API.

**Note:** The endpoint tables describe the main implemented functionality. Refer to the Swagger interface for the complete list of available routes.

## 7. RaceDayMVC – Part 3

RaceDayMVC provides the frontend interface for interacting with the RaceDay system.

The MVC application communicates with RaceDayAPI using `HttpClient`. It uses controllers, models, and Razor views to process and display application information.

### 7.1 Account Management

Users can:

- Register an account.
- Log in using their credentials.
- Access the dashboard associated with their role.
- Log out of the application.

### 7.2 Organiser Functionality

Organisers have access to an organiser dashboard.

The dashboard provides access to:

- **Manage Events:** Create and manage race events.
- **Manage Categories:** Manage event categories.
- **View Enrolments:** View participants registered for events.
- **Manage Results:** Record and update participant race results.

### 7.3 Participant Functionality

Participants have access to a participant dashboard.

The dashboard provides access to:

- **Browse Events:** View published race events and their details.
- **My Enrolments:** View personal event enrolments.
- **My Results:** View race performance, finishing times, and positions.

### 7.4 Authentication and Session Management

The MVC application uses session-based authentication to maintain the user's login information.

The session stores information such as:

- User ID
- Email address
- User role
- API authentication cookie

The API authentication cookie is forwarded with relevant HTTP requests to access protected API endpoints.

Organisers and Participants are directed to different dashboards according to their assigned roles.

## 8. Installation and Setup

### 8.1 Prerequisites

Before running the project, ensure that the following software is installed:

- Visual Studio with the ASP.NET and web development workload.
- .NET 8 SDK.
- Microsoft SQL Server.
- SQL Server Management Studio (SSMS).
- Git, if cloning the project from GitHub.

### 8.2 Clone the Repository

Open a terminal and execute:

```bash
git clone YOUR_GITHUB_REPOSITORY_URL
```

Replace `YOUR_GITHUB_REPOSITORY_URL` with the actual URL of the GitHub repository.

Alternatively, download the repository as a ZIP file and extract it.

### 8.3 Open the Solution

1. Open Visual Studio.
2. Select **Open a project or solution**.
3. Locate the RaceDay solution file.
4. Open the `.sln` file.
5. Allow Visual Studio to restore the required NuGet packages.

### 8.4 Configure the Database Connection

Open the `appsettings.json` file inside the RaceDayAPI project.

Configure the connection string to match your local SQL Server instance.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=RaceDayDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace `YOUR_SERVER_NAME` with the SQL Server instance name configured on your computer.

Ensure that the database exists and that the account running the API has permission to access it.

**Important:** Do not commit database passwords, API secrets, or other sensitive credentials to GitHub.

### 8.5 Configure the MVC API URL

Open `appsettings.json` inside the RaceDayMVC project.

Configure the API base address:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7089/"
  }
}
```

Ensure that the URL matches the address at which RaceDayAPI is running.

### 8.6 Restore Dependencies

In Visual Studio:

1. Right-click the solution.
2. Select **Restore NuGet Packages**.

Alternatively, execute the following command:

```bash
dotnet restore
```

### 8.7 Build the Solution

In Visual Studio:

1. Select **Build**.
2. Select **Rebuild Solution**.
3. Resolve any compilation errors before running the application.

Alternatively, execute:

```bash
dotnet build
```

### 8.8 Run the Applications

Both projects should be configured to start in Visual Studio.

1. Right-click the solution.
2. Select **Configure Startup Projects**.
3. Select **Multiple startup projects**.
4. Set RaceDayAPI and RaceDayMVC to **Start**.
5. Click **Apply** and **OK**.
6. Press **F5** to run both applications.

The API Swagger interface can be accessed at:

`https://localhost:7089/swagger/index.html`

The MVC application runs at its configured local development URL.

## 9. Testing the Application

### 9.1 API Testing

Swagger is used to explore and test the available API endpoints.

1. Run RaceDayAPI.
2. Open the Swagger interface.
3. Select an endpoint.
4. Click **Try it out**.
5. Enter the required request information.
6. Click **Execute**.
7. Review the HTTP response code and response body.

Endpoints requiring authentication must be accessed using an authenticated session.

### 9.2 MVC Testing

The MVC application can be tested by performing the following actions:

1. Register a new account or log in with an existing account.
2. Verify that the correct dashboard is displayed.
3. Log in as an Organiser and test event and category management.
4. View participant enrolments.
5. Record a participant's race result.
6. Log in as a Participant.
7. Browse published events.
8. Enrol in an event.
9. View personal enrolments.
10. View available race results.
11. Test the logout functionality.

## 10. Project Structure

The solution contains two main projects.

### RaceDayAPI

```text
RaceDayAPI/
├── Controllers/
│   ├── AuthController.cs
│   ├── EventsController.cs
│   ├── EnrollmentsController.cs
│   └── ResultsController.cs
├── Data/
│   └── RaceDayDbContext.cs
├── Controllers/
│   └── Models/
├── Properties/
│   └── launchSettings.json
├── appsettings.json
├── Program.cs
└── RaceDayAPI.csproj
```

### RaceDayMVC

```text
RaceDayMVC/
├── Controllers/
│   ├── AccountController.cs
│   ├── HomeController.cs
│   ├── OrganiserController.cs
│   ├── ParticipantController.cs
│   ├── EventsController.cs
│   ├── CategoriesController.cs
│   ├── EnrollmentsController.cs
│   └── ResultsController.cs
├── Models/
├── Views/
│   ├── Account/
│   ├── Home/
│   ├── Organiser/
│   ├── Participant/
│   ├── Events/
│   ├── Categories/
│   ├── Enrollments/
│   └── Results/
├── wwwroot/
├── Properties/
├── appsettings.json
├── Program.cs
└── RaceDayMVC.csproj
```

*Note: The folder structures above illustrate the main project organisation. Refer to the actual solution for all files and folders.*

## 11. Error Handling and Troubleshooting

Common issues and possible solutions include:

- **Database connection errors:** Verify the SQL Server instance name, database name, connection string, and database permissions.
- **Login or access denied errors:** Ensure that the account has the necessary SQL Server login and database-user permissions.
- **Swagger does not open:** Check the configured launch URL and ensure Swagger is enabled in the development environment.
- **Build errors:** Restore NuGet packages, inspect the error messages, and rebuild the solution.
- **HTTP 404 errors:** Confirm that the controller route and endpoint URL are correct.
- **HTTP 401 errors:** Check whether the user is authenticated.
- **HTTP 403 errors:** Verify the user's role and access permissions.
- **MVC cannot connect to the API:** Ensure RaceDayAPI is running and that the MVC API base URL is correct.

## 12. YouTube Demonstrations

### Part 2 – RaceDayAPI

The following video demonstrates the RaceDayAPI implementation.

[Watch the RaceDayAPI Part 2 Demonstration] (https://youtu.be/ruUMyVkAnos?si=qlZeJUxw8aO0sHe8).

### Part 3 – RaceDayMVC

The following video demonstrates the RaceDayMVC implementation.
[Watch the RaceDayMVC Part 3 Demonstration] ().

## 13. Conclusion

The RaceDay project demonstrates the development of a web-based Race Day Management System using C#, ASP.NET Core Web API, ASP.NET Core MVC, Entity Framework Core, and Microsoft SQL Server.

Part 2 focuses on developing a RESTful Web API that manages application data, authentication, and database operations.

Part 3 extends the system by introducing an MVC application that communicates with the API and provides separate functionality for Organisers and Participants.

Together, the two applications support race event management, category management, participant enrolments, and race result tracking.

The project provides practical experience in API development, database integration, authentication, authorization, MVC architecture, HTTP communication, and version control using GitHub.

---

**Project Name:** RaceDayAPI – Race Day Management System  
**Backend:** RaceDayAPI  
**Frontend:** RaceDayMVC  
**Framework:** ASP.NET Core Web API and ASP.NET Core MVC (.NET 8)  
**Programming Language:** C#  
**Database:** Microsoft SQL Server (`RaceDayDb`)  
**ORM:** Entity Framework Core 8  
**Development Environment:** Visual Studio  
**API Documentation and Testing:** Swagger  
**Version Control:** Git and GitHub
