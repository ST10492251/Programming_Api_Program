# Programming_Api_Program.
# RaceDay API

## Project Overview

**RaceDay API** is a planned C# ASP.NET Core Web API project designed to manage and store race-day information using a SQL Server database.

The project will demonstrate how a Web API can communicate with a relational database to create, retrieve, update, and delete information. The API will provide RESTful endpoints that can later be used by client applications to interact with the RaceDay database.

The project will also include an Entity Relationship Diagram (ERD) to represent the planned database structure, including entities, attributes, primary keys, foreign keys, and relationships.

**Project Status:** In Development / Not Yet Built

---

## Project Objectives

The main objectives of the RaceDay project are to:

* Design a structured relational database for race-day information.
* Develop a C# ASP.NET Core Web API.
* Connect the API to a SQL Server database.
* Implement CRUD operations.
* Create RESTful API endpoints.
* Establish relationships between database tables.
* Maintain data integrity using primary and foreign keys.
* Document the planned API endpoints.
* Test the API once development is complete.

---

## Planned Technologies

The following technologies are planned for the project:

* C#
* ASP.NET Core Web API
* SQL Server
* SQL Server Management Studio (SSMS)
* Entity Framework Core
* Swagger / OpenAPI
* Visual Studio / Visual Studio Code
* GitHub

---

## Planned Database

The RaceDay system will use a SQL Server relational database to store and manage race-day information.

The database will be designed to organise information into related tables. These tables will use primary and foreign keys to establish relationships and maintain data integrity.

The database design will be developed before the API is fully implemented.

### Planned Database Features

* Relational database structure
* Primary keys
* Foreign keys
* Table relationships
* Data validation
* Referential integrity
* CRUD operations
* Structured data storage

---

## RaceDay ERD

The Entity Relationship Diagram (ERD) will serve as the blueprint for the RaceDay database.

The ERD will show:

* Entities and tables
* Attributes and columns
* Primary keys
* Foreign keys
* Relationships
* Cardinality

The ERD will be used to guide the creation of the SQL Server database and ensure that the database structure is logically organised.

---

## Planned API Endpoints

The RaceDay API will contain RESTful endpoints for interacting with the database.

The planned endpoints will use standard HTTP methods such as:

| HTTP Method | Purpose                 |
| ----------- | ----------------------- |
| GET         | Retrieve records        |
| POST        | Create new records      |
| PUT         | Update existing records |
| DELETE      | Delete records          |

Example endpoint structure:

```text
GET     /api/[controller]
GET     /api/[controller]/{id}
POST    /api/[controller]
PUT     /api/[controller]/{id}
DELETE  /api/[controller]/{id}
```

These endpoints are currently planned and may change during development.

---

## API Endpoint Documentation

API endpoint documentation will be developed as the project progresses.

The documentation will describe:

* Endpoint URLs
* HTTP methods
* Request parameters
* Request bodies
* Response formats
* HTTP status codes
* Example requests
* Example responses

Swagger/OpenAPI may be used to provide an interactive way to view and test the completed API.

---

## Planned CRUD Functionality

The completed system is expected to support the four main CRUD operations.

### Create

Users will be able to add new race-day records to the database.

### Read

Users will be able to retrieve existing race-day information.

### Update

Users will be able to modify existing records.

### Delete

Users will be able to remove records when required.

---

## Planned Project Structure

The project structure will be developed during implementation.

A possible structure is:

```text
RaceDay/
│
├── Controllers/
│
├── Models/
│
├── Data/
│
├── Migrations/
│
├── appsettings.json
│
├── Program.cs
│
└── RaceDay.csproj
```

The final structure may change depending on the implementation of the project.

---

## Current Development Status

The RaceDay API is currently in the planning and design stage.

### Completed

* [x] Initial project concept
* [x] Database planning
* [x] ERD design
* [x] API planning
* [x] Endpoint planning

### To Be Completed

* [ ] Create SQL Server database
* [ ] Create C# Web API
* [ ] Connect API to SQL Server
* [ ] Implement CRUD operations
* [ ] Test API endpoints
* [ ] Complete API documentation

---

## Future Development

The next stages of development will include:

1. Finalising the database design.
2. Creating the SQL Server database.
3. Creating the C# ASP.NET Core Web API.
4. Creating the required models and controllers.
5. Connecting the API to SQL Server.
6. Implementing CRUD functionality.
7. Testing the API endpoints.
8. Completing the API documentation.
9. Updating the README as the project develops.

---

## Project Purpose

The RaceDay project is being developed for educational purposes to demonstrate knowledge and practical skills in:

* C# programming
* ASP.NET Core Web API development
* SQL Server
* Relational database design
* Entity Relationship Diagrams
* RESTful API development
* CRUD operations
* Database integration
* API documentation
* GitHub version control

---

## youtube video link 
https://youtu.be/Nj7h6yU0tc0
---

## Project Status

**Status: In Development**

The database, API, and endpoint functionality are currently being designed and will be implemented during the development stage.
