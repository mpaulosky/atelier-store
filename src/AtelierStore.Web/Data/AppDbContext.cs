using Microsoft.EntityFrameworkCore;

namespace AtelierStore.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}
