using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Global Error Handler Middleware
app.UseExceptionHandler((appError) =>
{
    appError.Run(async (context) =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        if (contextFeature is not null)
        {
            context.Response.StatusCode = contextFeature.Error switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                BadRequestException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError,
            };

            //context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // await context.Response.WriteAsync("An error has been occured beybi !"); // Custom hata mesajý, tüm hatalarda bu mesajý döner.
            // await context.Response.WriteAsync(contextFeature.Error.Message); // Hatanýn türüne baðlý olarak deðiþken hata mesajlarý alýnýr.

            await context.Response.WriteAsync((new ErrorDetails
            {
                Message = contextFeature.Error.Message,
                ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                StatusCode = context.Response.StatusCode
            }).ToString());
        }
    });
});

// GET
app.MapGet("/api/error", () =>
{
    throw new Exception("An error has been occured.");
});

// GET
app.MapGet("/api/books", () =>
{
    return Book.List();
});

// GET
app.MapGet("/api/books/{id}", (int id) =>
{
    var book = Book.List().Where(x => x.Id.Equals(id)).FirstOrDefault();

    return book is not null ? Results.Ok(book) : throw new BookNotFoundException(id);

    //return Results.Ok(Book.ListWithId(id));
});

// GET
app.MapGet("/api/books/search", (string? title) =>
{
    var books = string.IsNullOrEmpty(title) ? Book.List() : Book.List().Where(x => x.Title is not null && x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();

    return books.Any() ? Results.Ok(books) : Results.NotFound();
});

// POST
app.MapPost("/api/books", (Book InsertBook) =>
{
    // Temel alan kontrolleri
    if (string.IsNullOrWhiteSpace(InsertBook.Title) || InsertBook.Price <= 0)
    {
        throw new BookBadRequestException(InsertBook);
    }
    if (InsertBook.Title.Contains("string"))
    {
        throw new BookBadRequestException(InsertBook);
    }

    InsertBook.Id = Book.List().Max(x => x.Id) + 1;
    Book.CreateBook(InsertBook);

    return Results.Created($"/api/books/{InsertBook.Id}", InsertBook); // 201
});

// PUT
app.MapPut("/api/books/{id}", (int id, Book updateBook) =>
{
    var book = Book.List().Where(x => x.Id.Equals(id)).FirstOrDefault();

    if (book is null)
    {
        return Results.NotFound(); // 404
    }

    book.Title = updateBook.Title;
    book.Price = updateBook.Price;

    return Results.Ok(book); // 200
});

// DELETE
app.MapDelete("/api/books/{id}", (int id) =>
{
    var book = Book.List().Where(x => x.Id.Equals(id)).FirstOrDefault();

    if (book is null)
    {
        return Results.NotFound(); // 404
    }

    Book.List().Remove(book);

    return Results.NoContent(); // 204
});

app.Run();

public abstract class NotFoundException : Exception
{
    protected NotFoundException(string message) : base(message)
    {
    }
}

public abstract class BadRequestException : Exception
{
    protected BadRequestException(string message) : base(message)
    {
    }
}

public sealed class BookNotFoundException : NotFoundException
{
    public BookNotFoundException(int id) : base($"The book with {id} could not be found!")
    {
    }
}

public sealed class BookBadRequestException : BadRequestException
{
    public BookBadRequestException(Book book) : base($"Bad Request")
    {
    }
}

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public string? ErrorDate { get; set; }

    public override string ToString() // ToString yazýldýðýnda ErrorDetails sýnýfýný Serialize edecektir.
    {
        return JsonSerializer.Serialize(this);
    }
}

public class Book
{
    public int Id { get; set; }
    public String? Title { get; set; }
    public Decimal Price { get; set; }

    private static List<Book> BookList = new List<Book>
    {
        new Book(){ Id=1, Title="Suç ve Ceza", Price=400 },
        new Book(){ Id=2, Title="Beyaz Zambaklar Ülkesinde", Price=300 },
        new Book(){ Id=3, Title="Hayvanlar Çiftliði", Price=230 },
    };

    public static List<Book> List()
    {
        return BookList;
    }

    public static Book ListWithId(int id)
    {
        Book book = BookList.Where(x => x.Id.Equals(id)).FirstOrDefault();

        return book is not null ? book : throw new KeyNotFoundException($"ID {id} olan kitap bulunamadý.");
    }

    public static void CreateBook(Book book)
    {
        BookList.Add(book);
    }
}