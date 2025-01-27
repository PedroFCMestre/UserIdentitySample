using Projects;

var builder = DistributedApplication.CreateBuilder(args);



var postgres = builder.AddPostgres("postgres")
                        .WithContainerName("users")
                        .WithPgAdmin().WithLifetime(ContainerLifetime.Persistent);

var db = postgres.AddDatabase("users");

var migrationService = builder.AddProject<Migrations>("migrations")
                                .WithReference(db)
                                .WaitFor(db);

var apiService = builder.AddProject<UserIdentitySample_ApiService>("apiservice")
                            .WithReference(db)
                            .WaitForCompletion(migrationService);

builder.AddProject<UserIdentitySample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
