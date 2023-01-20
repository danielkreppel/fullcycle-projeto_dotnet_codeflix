# Dotnet project from Fullcycle practice course

### Dotnet core 6
### DDD approach (Domain-Driven Design) and Clean Architecture
1. Domain (Enterprise Business Rules)
2. Use Cases (Application Business Rules)
3. Infrastructure (Interface Adapters: Intermediate the communication from internal layers to the external layers and vice-versa, adapting data accordingly)
4. Tests
5. API

![image](https://user-images.githubusercontent.com/31414164/190870678-e2733f30-9d77-4079-8d03-cf3e0c9cb0ed.png)


### Tests layer using TDD process (Test Driven Development) with these packages (Unit Tests, Integration Tests and End-To-End Tests):
1. XUnit
2. Bogus
3. FluentAssertions
4. Moq
5. Microsoft.EntityFrameworkCore.InMemory (In-memory database provider for EF to be used for testing purposes)
6. Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory for in-memory web host for tests with Api client)

###  Application Layer using these packages:
1. FluentValidation
2. MediatR (Decouple the external layers from the internal layers)

### Docker
1. 1 Container for the API
2. 1 Container for the Test DataBase (MySql)
3. 1 Container for the Development DataBase (MySql)
4. Run "docker network create -d bridge service_catalog" (So the API container finds the DB Container. Setup in docker-compose.yml)
5. Few usefull Docker commands:
- docker-compose up
- docker ps
6. To check the containers running in Visual Studio: View -> Other Windows -> Containers

### Persistence
1. MySql Database
2. Entity Framework Core (https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
3. Migrations in API project. Example to add new migrations: 
- First setup the environment for the app settings running: $env:ASPNETCORE_ENVIRONMENT='Migrations' (to use appsettings.Migrations.json)
- Then run: dotnet ef migrations add \<name\> -s .\FC.Codeflix.Catalog.Api\ -p .\FC.Codeflix.Catalog.Infra.Data.EF\ -v
