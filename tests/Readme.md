# Running Tests

1. To run all tests from the main solution directory.
    - `dotnet test`
2. To run only unit tests from main solution directory
    - `dotnet test --filter TestCategory!=IntegrationTest`
    - Or: `dotnet test [path/to/unit/test/project.csproj]`
3. To run only integration tests from main solution directory remove the "!".
