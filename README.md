# litedb-async

This library allows the use of asynchronous programming techniques with the LiteDb library. It is intended for Xamarin and WPF applications that require or would benefit a light weight NoSQL database but also don't want to open an manage lots of threads or block the UI while database operations are occuring.

We manage the thread for you.

Available on nuget.
<https://www.nuget.org/packages/LiteDB.Async/>

## How to use

### Installation

Include the nuget package in your project in the usual way
```
Install-Package LiteDB.Async
```
or
```
dotnet add package LiteDB.Async
```

### Collections

Open a LiteDbAsync instance by calling the constructor in the standard way.

```
var db = new LiteDatabaseAsync("Filename=mydatabase.db;Connection=shared;Password=hunter2");
```

Collections are the equivalent of tables
```
var collection = _db.GetCollection<SimplePerson>();
```
This will give us an instance of the collection we can read and write.

We can upsert just like in the regular LiteDb
```
var collection = _db.GetCollection<SimplePerson>();
            var person = new SimplePerson()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Smith"
            };

            var upsertResult = await collection.UpsertAsync(person);
```

### Queries

When we want to read from the database we should use a query.

To read all SimplePerson's in the database
```
var collection = _db.GetCollection<SimplePerson>();
var listResult = await collection.Query().ToListAsync();
```

You can use Linq 
```
var theSmiths = await collection.Query().Where(x => x.Lastname == "Smith").ToListAsync();
```

### Supported API

Almost all functions from LiteDb have an async replacement

From Collections

Query, CountAsync, LongCountAsync, ExistsAsync, MinAsync, MaxAsync, DeleteAsync, DeleteManyAsync, FindAsync, FindByIdAsync, FindOneAsync, FindAllAsync, Include, EnsureIndexAsync, InsertAsync, UpdateAsync, UpdateManyAsync, UpsertAsync

From Query

Include, Where, OrderBy, OrderByDescending, GroupBy, Select, Limit, Skip, Offset, ForUpdate, ToDocumentsAsync, ToEnumerableAsync, ToListAsync, ToArrayAsync, FirstAsync, FirstOrDefaultAsync, SingleAsync, SingleOrDefaultAsync, CountAsync, LongCountAsync, ExistsAsync

From Database

UtcDate, CheckpointSize, UserVersion, Timeout, Collation, LimitSize, BeginTransAsync, CommitAsync, RollbackAsync, PragmaAsync, GetCollectionNamesAsync, CollectionExistsAsync, DropCollectionAsync, RenameCollectionAsync, CheckpointAsync, RebuildAsync

From Storage


### How does it work?
The constructor LiteDatabaseAsync opens and wraps a LiteDB instant. It also starts a background thread which all actions are performed in. 

When a function that causes an evaluation is called it sends a message to the background thread, where the required action is performed by the LiteDb instance. The result is then return to the calling thread when the function in the background completes. If the function in the background thread causes an exception it is caught and a LiteAsyncException is raised. The original exception is preserved as the InnerException.

Each class or interface in the LiteDb library has an equivalent in the LiteDb.Async library. Each function that returns a task is named with the async suffix. You can then call LiteDb.ASync methods using similar syntax to calling async functions in EF Core.


## Building

To build the solution

`dotnet build`

To run the unit tests

`dotnet test`

To run a single specific unit test

`dotnet test --filter DisplayName=LiteDB.Async.Test.SimpleDatabaseTest.TestCanUpsertAndGetList`

To build for nuget
`dotnet pack --configuration Release`

Don't forget to bump the version number in the litedbasync.csproj project file

It will build a nupkg file.

Next log in to nuget.org and upload the nupkg file which was just built.

Done the package will be available to the world within a few minutes.

### Command line package build instructions

https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli

### Web Upload instructions

https://docs.microsoft.com/en-us/nuget/nuget-org/publish-a-package

## VS Code config

When checking out for the first time copy the `.vscode/launch.json.default` and `.vscode/tasks.json.default` to `.vscode/launch.json` and `.vscode/tasks.json`.

## Example usage
The repo https://github.com/mlockett42/mvvm-cross-litedb-async gives an example of how to use this library in Xamarin Forms and MVVM.
