namespace Cors
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register CORS services
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("All",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://localhost:7048/").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Use CORS policy
            app.UseCors("All"); // Use "AllowSpecificOrigin" to restrict to a specific origin

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}