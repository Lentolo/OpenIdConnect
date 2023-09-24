using Microsoft.EntityFrameworkCore;

namespace AuthenticationClient;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    { }
}