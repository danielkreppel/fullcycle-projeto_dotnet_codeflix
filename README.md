# Dotnet project from Fullcycle practice course

### Dotnet core 6
### DDD approach (Domain-Driven Design) and Clean Architecture
1. Domain (Enterprise Business Rules)
2. Use Cases (Application Business Rules)
3. Infrasctucture (Interface Adapters: Intermediate the communication from internal layers to the external layers and vice-versa, adapting data accordingly)
4. Tests

![image](https://user-images.githubusercontent.com/31414164/190870678-e2733f30-9d77-4079-8d03-cf3e0c9cb0ed.png)


### Unit Tests layer using TDD process (Test Driven Development) with these packages:
1. XUnit
2. Bogus
3. FluentAssertions
4. Moq
5. Microsoft.EntityFrameworkCore.InMemory (In-memory database provider for EF to be used for testing purposes)

###  Application Layer using these packages:
1. FluentValidation
2. MediatR (Decouple the external layers from the internal layers)
