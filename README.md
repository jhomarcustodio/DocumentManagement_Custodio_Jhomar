# Simple Document Management System (SDMS)

## Overview

This is a simple document management system that supports CRUD operations on documents.

This project does not implement authentication or authorization, meaning all endpoints are accessible anonymously.

## Prerequisites

Before running the project, make sure you have the following installed:

- [.NET SDK 10 or higher](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- PostgreSQL

## Setup

This guide covers how to run the project on Windows. Running the project on macOS or Linux may require additional steps that are not covered in this document.

The project uses PostgreSQL as its relational database. You can either install PostgreSQL locally or run it inside a Docker container. Instructions for setting up PostgreSQL are not included in this document, as there are plenty of guides available online.

**Database Credentials**

The application uses the following default PostgreSQL credentials:

- **Username:** `postgres`
- **Password:** `postgres`

Make sure your PostgreSQL instance is configured accordingly.

## Running the Application

You can run the API project using the .NET CLI.

**Option 1: From the API project directory**

Navigate to the `SDMS.Api` folder and execute:

```bash
dotnet run
```

**Option 2: From the project root directory**

Execute the following command:

```bash
dotnet run --project ./SDMS.Api/SDMS.Api.csproj
```

Once running, the API will be accessible at:

```text
http://localhost:5000
```

The application automatically creates the database and its tables when it starts.

## Running the Tests

The project includes automated tests that can be executed using the .NET CLI.

From the project root directory, run:

```bash
dotnet test
```

**Note:** The `SDMS.Test` project uses `Testcontainers.PostgreSql` to spin up a PostgreSQL database inside a Docker container during test execution. Make sure Docker Desktop is running before executing the tests.

## API Endpoints

The API exposes the following endpoints:

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/documents` | Upload a new document |
| GET | `/api/documents` | Retrieve a list of documents |
| GET | `/api/documents/{id}` | Retrieve a document using its unique ID |
| GET | `/api/documents/{id}/download` | Download a document using its unique ID |
| DELETE | `/api/documents/{id}` | Delete a document using its unique ID |

## Project Structure and Implementation

The solution consists of two projects:

- **SDMS.Api** - Contains the Web API implementation.
- **SDMS.Test** - Contains the automated tests.

### SDMS.Api

The API project follows the default folder structure provided by the ASP.NET Core Web API template.

I intentionally avoided implementing more specific architectural patterns because this project only contains one controller. The goal was to keep the implementation simple without introducing unnecessary abstractions.

Here are some of the decisions I made regarding the project structure:

- **Services/** - Contains application services, including `FileHelperService`, which handles file-related operations.

- **Database/** - Contains database-related configurations, including `DocumentDbContext.cs`.

- **Extensions/** - Contains useful extension methods. I extracted some methods from `Program.cs` into this folder to keep the entry point clean and organized.

- **DTO/** - Contains the data transfer objects (DTOs) sent to clients. This separates external API contracts from internal entity definitions.

- **Entities/** - Contains the entity definitions that Entity Framework Core uses to generate database tables through migrations.

  Additional entity configurations, such as primary keys and property constraints, are defined directly in `Database/DocumentDbContext.cs` rather than in separate entity configuration files, for simplicity.

I also decided not to implement the Repository Pattern to keep data access straightforward.

File uploads are stored locally in the API project's `Uploads/` directory, which is automatically generated when needed. This was another deliberate simplification.

### API Design and Data Access

The controller uses the `[Route]` attribute to define a common route for its endpoints, while individual actions define additional route segments and parameters where necessary.

The controller directly handles data access using Entity Framework Core. A `DbContext` instance is injected through its constructor and used by the endpoint methods.

One important design consideration is the separation between entities and DTOs.

Although `DocumentDto.cs` currently exposes all the document entity's properties, we can limit the properties returned to clients or introduce additional properties to our entities without necessarily breaking the API contract.

### Endpoint Implementation

#### Get All Documents

The collection endpoint retrieves all documents from the database, projects the resulting entities into DTOs, and returns the collection to the client.

If the database contains no entries, the endpoint returns an empty collection.

#### Get Document by ID

The endpoint attempts to retrieve a single document using the provided route parameter.

If no matching document is found, it returns `404 Not Found`. Otherwise, it returns the document's metadata.

#### Download Document

This endpoint follows a similar approach to the Get Document by ID endpoint, but performs an additional check to ensure that the physical file exists in the `Uploads/` directory.

If the document or its associated file cannot be found, the endpoint returns `404 Not Found`. Otherwise, it returns the file for download.

#### Upload Document

The upload endpoint performs the following operations:

1. Validates the uploaded file using the validation methods provided by `FileHelperService`.
2. Returns `400 Bad Request` if the file fails validation.
3. Checks the database for duplicate filenames.
4. Returns `409 Conflict` if a duplicate filename is found.
5. Saves the file to the local `Uploads/` directory using a uniquely generated filename provided by `FileHelperService`.
6. Persists the document's metadata in the database using `DbContext`.
7. Returns `201 Created`, including the URL of the newly created resource in the `Location` header.

#### Delete Document

The delete endpoint first checks whether a document with the provided ID exists in the database.

If no matching document is found, it returns `404 Not Found`.

Otherwise, it performs the following operations:

1. Deletes the associated file from local storage using the `StoredFilename` property, which contains the generated filename.
2. Removes the document's metadata from the database.
3. Returns `204 No Content`.

### FileHelperService

File-related operations are handled by `FileHelperService`.

The service provides methods for:

- Validating uploaded files.
- Saving files to local storage using uniquely generated filenames.
- Retrieving stored files.
- Deleting stored files.

It also contains private utility methods for determining file paths and filenames, as well as retrieving the application's uploads directory.

### Configuration

Normally, I would include application settings files in `.gitignore`. However, the current `appsettings` files only contain default configuration values for connecting to the database, so I considered it safe to include them in the repository.

## AI-Assisted Development

I used AI tools and online resources to assist with my development process:

- **ChatGPT and Google Search** - Used together to research topics I had difficulty remembering or was unfamiliar with, particularly automated testing. ChatGPT was also used to help format this README.
- **GitHub Copilot** - Used its IntelliSense and autocomplete features to speed up development.

I did not use AI to generate code for this project.

## Limitations

The project has the following limitations:

- **File Type Support:** The API only accepts `application/pdf` files for simplicity.
- **Error Handling:** Basic error handling has been implemented. No custom exception-handling middleware is used.
- **Test Coverage:** The existing tests are not exhaustive. Some scenarios and edge cases may not be covered.