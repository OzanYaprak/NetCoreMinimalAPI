using IdentityProject.Abstracts;
using IdentityProject.DTOs.BookDTOs;
using IdentityProject.DTOs.CategoryDTOs;
using IdentityProject.DTOs.IdentityDTOs;
using IdentityProject.Entities;
using IdentityProject.Exceptions.BookExceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityProject.APIs
{
    public static class ApiExtensions
    {
        #region BOOK API ENDPOINTS

        public static void BookAPIs(this WebApplication app)
        {
            // GetAllBooks
            app.MapGet("/api/books", [Authorize] (IBookService bookService) =>
            {
                return bookService.Count > 0
                    ? Results.Ok(bookService.GetBooks())
                    : throw new BookNotFoundException(0);
            })
                .RequireAuthorization() // Bu endpoint'e erişim için Admin rolü gereklidir.
                .Produces<List<BookDTO>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithTags("CRUD", "GETs");

            // GetBookById
            app.MapGet("/api/books/{id}", [Authorize(Roles = "Admin,User")] (int id, IBookService bookService) =>
            {
                var book = bookService.GetBookById(id); // Kitap listesinden ID'ye göre kitap arar.
                return Results.Ok(book); // Eğer kitap bulunursa, kitap bilgilerini döndürür. // Eğer kitap bulunamazsa, hata fırlatır.

                //return Results.Ok(Book.ListWithId(id)); // Book sınıfındaki ListWithId metodunu kullanarak ID'ye göre kitap döndürür.
            })
                .Produces<Book>(StatusCodes.Status200OK) // Produces, Swagger'da bu endpoint'in başarılı durum kodunu ve başarılı durum mesajını gösterir.
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound) // Eğer kitap bulunamazsa, 404 Not Found durum kodunu döndürür.
                .WithTags("GETs"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduğunu gösterir. // Bu endpoint'i "GETs" grubuna ekler.

            // PostBook
            app.MapPost("/api/books", [Authorize(Roles = "Admin")] (BookDtoForInsertion InsertBook, IBookService bookService) =>
            {
                var book = bookService.CreateBook(InsertBook);
                return Results.Created($"/api/books/{book.Id}", InsertBook); // 201
            })
                .Produces<Book>(StatusCodes.Status201Created) // Produces, Swagger'da bu endpoint'in başarılı durum kodunu ve başarılı durum mesajını gösterir.
                .Produces<List<ValidationResult>>(StatusCodes.Status422UnprocessableEntity) // Eğer kitap doğrulama hatası varsa, 422 Unprocessable Entity durum kodunu döndürür.
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduğunu gösterir. // Bu endpoint'i "CRUD" grubuna ekler.

            // PutBook
            app.MapPut("/api/books/{id}", [Authorize(Roles = "Admin")] (int id, BookDtoForUpdate updateBook, IBookService bookService) =>
            {
                Book book = bookService.UpdateBook(id, updateBook);
                return Results.Ok(book); // 200
            })
                .Produces<Book>(StatusCodes.Status200OK)
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .Produces<ErrorDetails>(StatusCodes.Status422UnprocessableEntity)
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduğunu gösterir. // Bu endpoint'i "CRUD" grubuna ekler.

            // DeleteBook
            app.MapDelete("/api/books/{id}", [Authorize(Roles = "Admin")] (int id, IBookService bookService) =>
            {
                bookService.DeleteBook(id); // Kitabı kitap listesinden siler.
                return Results.NoContent(); // 204
            })
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduğunu gösterir. // Bu endpoint'i "CRUD" grubuna ekler.

            // SearchBooks
            app.MapGet("/api/books/search", [Authorize(Roles = "Admin,User")] (string? title, IBookService bookService) =>
            {
                var books = string.IsNullOrEmpty(title) ? bookService.GetBooks() : bookService.GetBooks().Where(x => x.Title is not null && x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList(); // Eğer title boş ise, tüm kitapları döndürür. // Eğer title dolu ise, kitap listesinden title'a göre arama yapar. // StringComparison.OrdinalIgnoreCase, büyük/küçük harf duyarsız arama yapar.

                return books.Any() ? Results.Ok(books) : Results.NotFound(); // Eğer kitap listesi boş değilse, kitap listesini döndürür. // Eğer kitap listesi boşsa, 404 Not Found durum kodunu döndürür.
            })
                .Produces<List<Book>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .WithTags("GETs");
        }

        #endregion BOOK API ENDPOINTS

        #region CATEGORY API ENDPOINTS

        public static void CategoryAPIs(this WebApplication app)
        {
            // GetCategoryById
            app.MapGet("/api/categories/{id}", (int id, ICategoryService categoryService) =>
            {
                var book = categoryService.GetCategoryById(id);
                return Results.Ok(book);
            })
                .Produces<Book>(StatusCodes.Status200OK)
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
                .WithTags("CATEGORY", "GETs");

            // PostCategory
            app.MapPost("/api/categories", (CategoryDTOForInsertion InsertCategory, ICategoryService categoryService) =>
            {
                var category = categoryService.CreateCategory(InsertCategory);
                return Results.Created($"/api/categories/{category.CategoryId}", InsertCategory);
            })
                .Produces<Book>(StatusCodes.Status201Created)
                .Produces<List<ValidationResult>>(StatusCodes.Status422UnprocessableEntity)
                .WithTags("CATEGORY", "CRUD");

            // PutCategory
            app.MapPut("/api/categories/{id}", (int id, CategoryDTOForUpdate updateCategory, ICategoryService categoryService) =>
            {
                Category category = categoryService.UpdateCategory(id, updateCategory);
                return Results.Ok(category);
            })
                .Produces<Book>(StatusCodes.Status200OK)
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .Produces<ErrorDetails>(StatusCodes.Status422UnprocessableEntity)
                .WithTags("CATEGORY", "CRUD");

            // DeleteCategory
            app.MapDelete("/api/categories/{id}", (int id, ICategoryService categoryService) =>
            {
                categoryService.DeleteCategory(id);
                return Results.NoContent();
            })
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .WithTags("CATEGORY", "CRUD");

            // SearchCategory
            app.MapGet("/api/categories/search", (string? categoryName, ICategoryService categoryService) =>
            {
                var categories = string.IsNullOrEmpty(categoryName) ? categoryService.GetCategories() : categoryService.GetCategories().Where(x => x.CategoryName is not null && x.CategoryName.Contains(categoryName, StringComparison.OrdinalIgnoreCase)).ToList();

                return categories.Any() ? Results.Ok(categories) : Results.NotFound();
            })
                .Produces<List<Book>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .WithTags("CATEGORY", "GETs");
        }

        #endregion CATEGORY API ENDPOINTS

        #region AUTHENTICATION API ENDPOINTS

        public static void AuthenticationAPIs(this WebApplication app)
        {
            app.MapPost("/api/registeruser", async (UserDTOForRegistration userDTO, IAuthService authService) =>
            {
                var result = await authService.RegisterUserAsync(userDTO);
                return result.Succeeded
                    ? Results.Ok(new { Message = "User registered successfully.", result })
                    : Results.BadRequest(new { Message = "User registration failed.", Errors = result.Errors.Select(e => e.Description) });
            })
                .Produces<IdentityResult>(StatusCodes.Status200OK)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .WithTags("Authentication For User");

            app.MapPost("/api/registeradmin", async (AdminDTOForRegistration adminDTO, IAuthService authService) =>
            {
                var result = await authService.RegisterAdminAsync(adminDTO);
                return result.Succeeded
                    ? Results.Ok(new { Message = "Admin registered successfully.", result })
                    : Results.BadRequest(new { Message = "Admin registration failed.", Errors = result.Errors.Select(e => e.Description) });
            })
                .Produces<IdentityResult>(StatusCodes.Status200OK)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .WithTags("Authentication For Admin");

            app.MapPost("/api/login", async (UserDTOForAuthentication userDTO, IAuthService authService) =>
            {
                if (await authService.ValidateUserCredentialsAsync(userDTO))
                {
                    return Results.Ok(new
                    {
                        Message = "Login successful.",
                        Token = await authService.CreateJwtTokenAsync(true)
                    });
                }
                return Results.Unauthorized(); // 401
            })
                .Produces(StatusCodes.Status200OK)
                .Produces<ErrorDetails>(StatusCodes.Status401Unauthorized)
                .WithTags("API Login");

            app.MapPost("/api/refresh-token", async (TokenDTO tokenDTO, IAuthService authService) =>
            {
                var tokenDTOToReturn = await authService.CreateRefreshTokenAsync(tokenDTO);
                return tokenDTOToReturn != null ? Results.Ok(tokenDTOToReturn) : Results.BadRequest(new { Message = "Invalid refresh token." });
            })
               .Produces(StatusCodes.Status200OK)
               .WithTags("Refresh Token");
        }

        #endregion AUTHENTICATION API ENDPOINTS
    }
}

