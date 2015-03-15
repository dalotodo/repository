# Repository Pattern Framework

This project aims to be a framework that implements the Repository pattern over different storage implementations, allowing  consumers of the framework to be technology independent, and therefore making code more flexible and SOLID compliant.

In short, a Repository service pattern is a standarized CRUD Service implementation, thus implementing its basic operations: Create, Read (Query), Update and Delete items from and into the repository.

## What's in the box

At this time the Repository project includes In-Memory (Collection based) & Entity Framework (EF) implementations, and a Repository Decorator that allows any Repository to be constructed as a stack of chained Repository services. This approach, when used with DI allows adding service layers into the Repository for data validation, sanitization, logging or event notifications with little or no impact on the consumer clients.

Take a look at the Wiki to find out more about the project and its implementations.
