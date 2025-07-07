namespace RelationsProject.APIs
{
    internal static class ApiExtensionsHelpers
    {
        public static void BookAPIs(this WebApplication app)
        {
            app.GetAllBooks();
            app.GetBookById();
            app.PostBook();
            app.PutBook();
            app.DeleteBook();
            app.SearchBooks();
        }
    }
}