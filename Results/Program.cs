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

app.MapGet("/api/books", () =>
{
    return Book.List();
});

app.MapGet("/api/books/{id}", (int id) =>
{
    var book = Book.List().Where(x => x.Id.Equals(id)).FirstOrDefault();

    return book is not null ? Results.Ok(book) : Results.NotFound();

    //return Results.Ok(Book.ListWithId(id));
});

app.MapPost("/api/books", (Book InsertBook) =>
{
    InsertBook.Id = Book.List().Max(x => x.Id) + 1;

    Book.CreateBook(InsertBook);

    return Results.Created($"/api/books/{InsertBook.Id}", InsertBook); // 201
});

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

app.MapGet("/api/books/search", (string? title) =>
{
    var books = string.IsNullOrEmpty(title) ? Book.List() : Book.List().Where(x => x.Title is not null && x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();

    return books.Any() ? Results.Ok(books) : Results.NotFound();
});

app.Run();


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