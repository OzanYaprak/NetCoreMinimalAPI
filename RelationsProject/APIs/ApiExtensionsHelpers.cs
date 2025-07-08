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

        public static void CategoryAPIs(this WebApplication app)
        {
            app.GetAllCategories();
            app.GetCategoryById();
            app.PostCategory();
            app.PutCategory();
            app.DeleteCategory();
            app.SearchCategory();
        }
    }
}