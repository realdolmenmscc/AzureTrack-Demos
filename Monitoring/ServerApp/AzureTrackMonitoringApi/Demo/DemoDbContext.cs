using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTrackMonitoringApi.Demo
{
  public class DemoDbContext : DbContext
  {
    public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
    {
    }

    public DbSet<Record> Records { get; set; }
  }
}