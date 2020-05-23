# litedb-async

## Building

To build the solution

`dotnet build`

To run the unit tests

`dotnet test`

To run a single specific unit test

`dotnet test --filter DisplayName=LiteDB.Async.Test.SimpleDatabaseTest.TestCanUpsertAndGetList`

To build for nuget
`dotnet pack --configuration Release`

## VS Code config

When checking out for the first time copy the `.vscode/launch.json.default` and `.vscode/tasks.json.default` to `.vscode/launch.json` and `.vscode/tasks.json`.

## How to use