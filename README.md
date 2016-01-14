# Reliak.ProcessArgumentBuilderUtil
Cross-plattform utility library for building process arguments
--
### Example (Posix-shell)
```
var escapeHandler = new PosixShellArgumentEscapeHandler();
var builder = new ProcessArgumentBuilder(escapeHandler);
builder.AddOption("--someflag")
       .AddNamedArgument("-f", "some filename")
       .AddNamedArgument("--password", "secret", isSensitiveArgument: true)
       .AddArgument("-StartsWithDash.txt");

var args = builder.Build();
var argsSafe = builder.BuildSafe(sensitiveArgumentPlaceholder: "*****");

Console.WriteLine(args);
Console.WriteLine(argsSafe);
```
The output for above example would be:
```
--someflag -f "some filename" --password secret "-StartsWithDash.txt"
--someflag -f "some filename" --password ***** "-StartsWithDash.txt"
```

### Example (Windows-shell):
```
var escapeHandler = new WindowsArgumentEscapeHandler();
var builder = new ProcessArgumentBuilder(escapeHandler, defaultArgumentKeyValueSeparator: ":");
builder.AddOption("/someflag")
       .AddNamedArgument("/f", "some filename")
       .AddNamedArgument("/c", "connectionstring", isSensitiveArgument: true);

var args = builder.Build();
var argsSafe = builder.BuildSafe(sensitiveArgumentPlaceholder: "*****");

Console.WriteLine(args);
Console.WriteLine(argsSafe);
```
The output for above example would be:
```
/someflag /f:"some filename" /c:connectionstring
/someflag /f:"some filename" /c:*****
```