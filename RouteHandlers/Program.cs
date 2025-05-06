var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var fixedResponse = DateTime.Now.ToString();
var dynamicResponse = () => $"Hello at {DateTime.Now}";

var variable = () => "Hello World";

app.MapGet("/fixedResponse", () => fixedResponse);
app.MapGet("/dynamicResponse", () => dynamicResponse());

app.MapPost("/hello", () => variable()); // Lambda variable
app.MapPut("/hello", Hello); // Local Function
app.MapDelete("/hello", () => "Hello World"); // Inline

string Hello()
{
    return "[Local Function] Hello World";
}

//-------------

int counter = 0;

app.MapGet("/counter", () => counter);
app.MapPost("/counter/increment", () => ++counter);
app.MapPost("/counter/reset", () => counter = 0);

//-------------

List<string> Users = new();

app.MapPost("/user/add/{name}", (string name) =>
{
    Users.Add(name);
    return $"Kullanýcý Eklendi {name}";
});

app.MapGet("/user/list", () =>
{
    return Users;
});

app.MapDelete("/user/delete/{name}", (string name) =>
{
    if (Users.Remove(name))
    {
        return $"{name} silindi.";
    }
    return $"{name} bulunamadý.";
});

//-------------

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
