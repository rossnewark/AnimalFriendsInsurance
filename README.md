# Animal Friends Insurance - Customer Registration API

This is the **Customer Registration API** for the Animal Friends Insurance (AFI) platform. It allows customers to register for the AFI customer portal by submitting their details. The API is built using **ASP.NET Core**, and it connects to a database (SQL Server or In-Memory for testing purposes) to store customer data.

## Features

- Customer registration functionality
- Supports both SQL Server and In-Memory databases (for testing)
- Swagger API documentation for easy testing
- Logging and error handling
- API endpoints are secured (if necessary, you can add authentication/authorization)

## Prerequisites

Make sure you have the following software installed on your machine:

- **.NET SDK 6.0 or later** – You can download it from [here](https://dotnet.microsoft.com/download).
- **SQL Server** (Optional, if you prefer using SQL Server) – You can use [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or a remote server.
- **Visual Studio Code**, **Visual Studio**, or any IDE of your choice.

## Cloning the Repository

To clone the project and get it running locally, follow these steps:

### 1. Clone the repository

Open a terminal/command prompt and run the following command:

git clone https://github.com/rossnewark/AnimalFriendsInsurance.git

git clone https://github.com/rossnewark/AnimalFriendsInsurance.git

### 2. Navigate to the project directory

cd AnimalFriendsInsurance

### 3. Install the required dependencies
Make sure all necessary packages are installed by running:

dotnet restore
This will download and install all dependencies specified in the project file (.csproj).

### 4. Configure your database connection
Before running the app, configure your database connection in the appsettings.json or appsettings.Testing.json file.

If you're using SQL Server, configure the DefaultConnection string:

`"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=CustomerRegistration;User Id=afi;Password=password;TrustServerCertificate=True;MultipleActiveResultSets=true"
}`

If you're running tests or don't have SQL Server, the project will default to an In-Memory Database for testing.

### 5. Run the project
Once the database is configured, you can run the application using:

`dotnet run`

This will start the API locally. You can now access it via http://localhost:5000 or https://localhost:5001.

### 6. Access Swagger API Documentation (Optional)
The API includes Swagger for interactive API documentation. You can access it at:

`https://localhost:5001/swagger`

This will allow you to test the endpoints directly from the browser.

### Running Tests
If you'd like to run the unit tests, make sure you have xUnit and Microsoft.AspNetCore.Mvc.Testing installed. Then, you can run the tests using:

`dotnet test`

This will run the tests defined in your RegistrationApiTests class or any other test files in your project.
