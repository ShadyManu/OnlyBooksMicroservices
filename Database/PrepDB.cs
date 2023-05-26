namespace AuthSrv.Database
{
    public static class PrepDB
    {
    public static void ApplyDatabaseMigrations(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<DataContext>();
                Console.WriteLine(dbContext);
                Console.WriteLine(dbContext);

                dbContext!.Database.Migrate();
                Console.WriteLine(dbContext);
            }
        }

    }
}