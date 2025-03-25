# Minimal Apis

Minimal APIs are a lightweight framework introduced in .NET 6, designed to simplify the development of small and focused web APIs. Unlike traditional ASP.NET Core APIs, which rely on controllers and more boilerplate code, Minimal APIs allow developers to define endpoints directly in the  file, making them quicker to set up and easier to read.

<ul>
  <li>Simplicity: Minimal APIs reduce the amount of code you need to create an API. You can define routes in a single file using lambda expressions.</li>
  <li>Performance: They are designed for high performance, making them a great choice for microservices or serverless applications.</li>
  <li>Lightweight: Minimal APIs don't include features like controllers or views by default, which makes them suitable for APIs that don't need those features.</li>
  <li>Flexibility: Developers can easily add middleware, use dependency injection, and configure services, just like in traditional ASP.NET Core applications.</li>
</ul>


```csharp
var app = WebApplication.CreateBuilder(args).Build();

app.MapGet("/", () => "Hello, World!");

app.Run();
```

Minimal APIs are ideal for scenarios where you don't need the full MVC framework, such as building microservices, prototyping, or creating quick-and-simple backend services. Would you like to see more examples or a comparison with traditional APIs?
