# EasySQL - C# Wrapper

** DO NOT USE THIS CODE IN PRODUCTION MODE, THIS CODE IS ONLY FOR TEST PURPOSES! **

EasySQL is a C# Wrapper that allow you to manage MySQL easily.

  - Easy to understand.
  - Fast and structured query result.
  - Easy to connect and dispose

# Requirements to use EasySQL

  - MySQL NuGet package.

# How to install

Compile this source-code and add as reference into your project.

##### How to use

Here is a simple introduction to how to use EasySQL:

Put our using in your project typing:
`using EasySQL.SQL;`

###### Creating new connection:
```
SQL myConnection = new SQL("myUsername", "myPassword", "myDatabase", "127.0.0.1");

var conn = myConnection.CreateConnection();

if(conn)
{
    //You're connected
}
```

###### Inserting data:
```
List<string> keys = { "name", "number" };
List<object> values = { "Washington", 17 };

bool insertValues = myConnection.Insert("myTable", keys, values);

if(insertValues)
{
    //Data was inserted
}
```

###### Updating data:
```
List<string> keys = { "number" };
List<object> values = { 96 };

bool updateValues = myConnection.Update("myTable", keys, values, string.Format("name='{0}'", "Washington"));

if(updateValues)
{
    //Data was updated
}
```

###### Deleting data
```
bool deleteValues = myConnection.Delete("myTable", string.Format("name='{0}'", "Washington"));

if(deleteValues)
{
    //Data was deleted
}
```

###### Querying data
```
var Result = myConnection.Get("*", "myTable", "number=96");

if(Result.Count != 0)
{
    //has results
    foreach(var row in Result)
    {
        var nameColumn = row.GetValue(0).ToString();
        var countryColumn = row.GetValue(1).ToString();
        
        Console.WriteLine(string.Format("Name: {0} Number: {1}", nameColumn, countryColumn));
    }
}
```

###### Important
At the end of script remember to use the Dispose function to finish the connection!
`myConnection.Dispose();`

### TODO
- Make it faster
- Create best way to use conditional
- Best security with conditionals, to block possible SQL Injection


Best regards, Washington
