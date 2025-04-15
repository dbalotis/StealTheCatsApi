# Steal All The Cats

This is a REST API project that uses the [Cats as a Service (CaaS) API](https://thecatapi.com/) to fetch and store cat images with their metadata and their associated tags.

---

## Features
- Fetch and store cat images and their data asynchronously.
- Retrieve a cat by its ID.
- Retrieve cats in a paginated response.
- Filter cats by tag.
- Swagger API documentation at `/swagger`.
- Hangfire Dashboard at `/hangfire`
---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/ssms/download-sql-server-management-studio-ssms) or any SQL management client to be able to create a database.

---

## Setup Instructions

1. **Download the Project**:
   - Clone the project from Git or download and extract the project zip file.

2. **Build the solution - DO NOT RUN**
    - Build the solution using Visual studio 2022, or any equivalent IDE.
    - Alternatively you can build it via command line. Run cmd to open the command line. Navigate to the solution folder and run the following command.
    ```
    dotnet build
    ```
    This will download all the necessary nugets and compile the project.

3. **Configure the project**
    - In appsettings.json if the Database instance you want to use is not on the same computer as the project code (localhost), change the server name to point to the Database server for both connection strings. So basically replace localhost with your server.
    - If you want different database names than the default ones, for the project or Hangfire, edit them as well.


4. **Set Up the Databases**:
    - In Visual studio 2022 menu go to `Tools -> Nuget Package Manager -> Package manager console` and run the following command.
    ```
    Update-Database
    ```
    Alternatively, if you have setup globally the entity framework on your machine, in the command line, navigate in the project `StealTheCatsApi` project folder (where the migrations folder is) and run the following command.
    ```
    dotnet ef database update
    ```
    This will create the database of the project.

    Now you need to use SQL Server Management Studio (SSMS) and create on the same SQL server instance, the Hangfire database. If you haven't changed it earlier the name should be `HangfireDb`
    If you prefer to do it by running an SQL script, open a query window on master database, and run the following.
    ```
    CREATE DATABASE HangfireDb
    ```

6.  **Run the application**
    - The first time you run the HangfireDB will be filled up automatically with all its tables.
    - Run the application locally either with the http or https options. Although there is a Dockerfile in order to work, you need to setup a `docker-compose yaml` file to setup a docker instance of the SQL Server and create via a script the Hangfire DB.
    - Enjoy the app!

---

## Using the application

1. **Navigate to swagger and hangfire**
    - Swagger is used as an interactive documentation that allows you to send requests to the endpoints.
    - Hangfire dashboard is used to see and manage the various background jobs that are triggered and fetching cats.
    - On your browser navigate to swagger and to hangfire using the following links.
    **https**
    [https://localhost:7021/swagger](https://localhost:7021/swagger)
    [https://localhost:7021/hangfire](https://localhost:7021/hangfire)
    
    **http**
    [http://localhost:5282/swagger](http://localhost:5282/swagger)
    [http://localhost:5282/hangfire](http://localhost:5282/hangfire)

2. **Use the Fetch endpoint at lease once**
    - This will start a background job that gets cats and stores them in our database.
    - The result will be a number which is the job Id. 

3. **Check when it finished**
    - On swagger go to the Jobs section and run the endpoint using the job id as a parameter. You can run it regularly to check the job status. 
    - Once it says `Succeeded` you will be able to use the rest of the endpoints to retrieve cat data and images.
