using AVMLabs.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AVMLabs.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Test> Tests => Set<Test>();
        public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
        public DbSet<WorkOrderItem> WorkOrderItems => Set<WorkOrderItem>();
        public DbSet<Invoice> Invoices => Set<Invoice>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkOrder>()
                .HasOne(w => w.Client)
                .WithMany(c => c.WorkOrders)
                .HasForeignKey(w => w.ClientId);

            modelBuilder.Entity<WorkOrderItem>()
                .HasOne(i => i.WorkOrder)
                .WithMany(w => w.Items)
                .HasForeignKey(i => i.WOId);

            modelBuilder.Entity<WorkOrderItem>()
                .HasOne(i => i.Test)
                .WithMany()
                .HasForeignKey(i => i.TestId);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Client)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.ClientId);

            // ---- Seed data: 3 clients, 5 tests, 3 work orders ----
            modelBuilder.Entity<Client>().HasData(
                new Client { ClientId = 1, ClientName = "Al Noor Hospital", ContactPerson = "Ahmed Khalid", Phone = "+971500000001", Email = "ahmed@alnoor.ae", City = "Dubai", Country = "UAE", IsActive = true },
                new Client { ClientId = 2, ClientName = "Apollo Diagnostics", ContactPerson = "Priya Menon", Phone = "+919840000002", Email = "priya@apollo.in", City = "Chennai", Country = "India", IsActive = true },
                new Client { ClientId = 3, ClientName = "Gulf Care Clinic", ContactPerson = "Sara Ali", Phone = "+96550000003", Email = "sara@gulfcare.qa", City = "Doha", Country = "Qatar", IsActive = true }
            );

            modelBuilder.Entity<Test>().HasData(
                new Test { TestId = 1, TestCode = "CBC001", TestName = "Complete Blood Count", SampleType = "Blood", Rate = 15.00m, IsActive = true },
                new Test { TestId = 2, TestCode = "LFT001", TestName = "Liver Function Test", SampleType = "Blood", Rate = 25.00m, IsActive = true },
                new Test { TestId = 3, TestCode = "KFT001", TestName = "Kidney Function Test", SampleType = "Blood", Rate = 25.00m, IsActive = true },
                new Test { TestId = 4, TestCode = "URN001", TestName = "Urine Routine", SampleType = "Urine", Rate = 10.00m, IsActive = true },
                new Test { TestId = 5, TestCode = "THY001", TestName = "Thyroid Profile", SampleType = "Blood", Rate = 30.00m, IsActive = true }
            );

            var seedDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<WorkOrder>().HasData(
                new WorkOrder { WOId = 1, ClientId = 1, WODate = seedDate, Status = "Completed", TotalAmount = 40.00m },
                new WorkOrder { WOId = 2, ClientId = 2, WODate = seedDate.AddDays(4), Status = "Pending", TotalAmount = 35.00m },
                new WorkOrder { WOId = 3, ClientId = 3, WODate = seedDate.AddDays(9), Status = "Completed", TotalAmount = 30.00m }
            );

            modelBuilder.Entity<WorkOrderItem>().HasData(
                new WorkOrderItem { WOItemId = 1, WOId = 1, TestId = 1, Quantity = 1, Rate = 15.00m },
                new WorkOrderItem { WOItemId = 2, WOId = 1, TestId = 2, Quantity = 1, Rate = 25.00m },
                new WorkOrderItem { WOItemId = 3, WOId = 2, TestId = 3, Quantity = 1, Rate = 25.00m },
                new WorkOrderItem { WOItemId = 4, WOId = 2, TestId = 4, Quantity = 1, Rate = 10.00m },
                new WorkOrderItem { WOItemId = 5, WOId = 3, TestId = 5, Quantity = 1, Rate = 30.00m }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
