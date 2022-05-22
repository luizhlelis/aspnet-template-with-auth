# aspnet-template-with-auth

🎬 🔐 ASP.NET Core template API with authentication and `Hexagonal Architecture` (also known as ports and adapters) ⬡⬢

## Pre requisites

- [.NET 6.0](https://dotnet.microsoft.com/en-us/download): to run, build, test the project locally;
- [dotnet-ef tool](https://docs.microsoft.com/en-us/ef/core/cli/dotnet): to create new migrations;
- [Docker](https://www.docker.com/products/docker-desktop/): to run everything inside containers.

## Running commands

To up all the app and dependencies containerized, type the following command in the [src](./src) folder:

```shell
docker-compose up --build
```

> **NOTE:** the command above will up the web application, SqlServer and will execute all the existing migrations.
> That's enough to run everything, but even though, if you want to run the app locally using containerized dependencies,
> you must try the commands below.

To run the SqlServer, type the following command in
the [src](./src) folder:

```shell
docker-compose up sql-server-database
```

To up the existent migrations in SqlServer, type the following command in
the [src](./src) folder:

```shell
docker-compose up --build migrations
```

If you want to create a new migration (after an entity model update, for example), type the following command in
the [root](./) folder:

```shell
dotnet ef migrations add <migration-name> --project src/SampleApp.Infrastructure/SampleApp.Infrastructure.csproj --startup-project src/SampleApp.Web/SampleApp.Web.csproj --context SampleAppContext --verbose
```

To run all the automated test, type the following command in the [src](./src) folder:

```shell
dotnet test
```

If you're running the app in docker, open the following link in your browser:

```shell
http://localhost/swagger/index.html
```

Otherwise, if you're running it locally, the port will be different:

```shell
https://localhost:7211/swagger/index.html
```

## Credentials

There are two pre-created users (created as seed in migrations). You can use the following credentials to login with each one them:

```json
{
  "username": "admin-user",
  "password": "StrongPassword@123"
}
```

```json
{
  "username": "customer-user",
  "password": "StrongPassword@123"
}
```

## Design and Architecture decisions

- Rich Domain Models (behaviors inside the domain entities)

- [Hexagonal architecture](https://alistair.cockburn.us/hexagonal-architecture/)

![hexagonal architecture](./assets/hexagonal.png)

> Font: [Hexagonal Architecture, there are always two sides to
> every story](https://medium.com/ssense-tech/hexagonal-architecture-there-are-always-two-sides-to-every-story-bc0780ed7d9c)

Follow bellow how the `Application` layer was concept:

```shell
\
┣ Application
┃ ┣ 📂 Commands (all commands to trigger this layer)
┃ ┣ 📂 Dtos
┃ ┣ 📂 Ports (ports from hexagonal architecture)
┃ ┣ 📂 Domain
┃    ┣ 📂 Entities (DB entities with behaviors: Rich Domain Models)
┃    ┣ 📂 Enums
┃    ┣ 📂 Handlers (complex behaviors: involves more than one entity)
┃    ┣ 📂 Validators (all validations to prevent incorrect data to travel through other layers)
┃    ┣ 📂 ValueObjects (represents a value and has no identity)
```