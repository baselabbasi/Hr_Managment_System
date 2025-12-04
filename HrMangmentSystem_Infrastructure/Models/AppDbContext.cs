using HrMangmentSystem_Application.Interfaces.Repository;
using HrMangmentSystem_Domain.Common;
using HrMangmentSystem_Domain.Constants;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Domain.Entities.Recruitment;
using HrMangmentSystem_Domain.Entities.Requests;
using HrMangmentSystem_Domain.Entities.Roles;
using HrMangmentSystem_Domain.Enum.Employee;
using HrMangmentSystem_Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace HrMangmentSystem_Infrastructure.Models
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly ICurrentUser _currentUser;
        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenant currentTenant, ICurrentUser currentUser) : base(options)
        {
            _currentTenant = currentTenant;
            _currentUser = currentUser;
        }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<EmployeeRole> EmployeeRoles { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<JobPosition> JobPositions { get; set; }

        public DbSet<JobApplication> JobApplications { get; set; }

        public DbSet<DocumentCv> DocumentCvs { get; set; }

        public DbSet<DocumentEmployeeInfo> DocumentEmployeeInfo { get; set; }

        public DbSet<GenericRequest> GenericRequests { get; set; }

        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        public DbSet<EmployeeDataChange> EmployeeDataChanges { get; set; }

        public DbSet<RequestHistory> RequestHistories { get; set; }

        public DbSet<Tenant> Tenants { get; set; }

        public override int SaveChanges()
        {
            ApplyTenantRules();
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            
            ApplyTenantRules();
            ApplyAuditInfo();
            return base.SaveChangesAsync();
        }

    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints 


            var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var employeeId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            modelBuilder.Entity<Tenant>().HasData(
                new Tenant
                {
                    Id = tenantId,
                    Name = "Demo Tenant",
                    Code = "DEMO",
                    Description = "Seeder Tenant",
                    IsActive = true,
                    ContactEmail = "admin@demo.com",
                    ContactPhone = "+962790000000",
                    CreatedAt = new DateTime(2024, 1, 1),
                    CreatedBy = Guid.Parse("00000000-0000-0000-0000-000000000000")
                }
            );

            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    TenantId = tenantId,
                    Code = "IT",
                    DeptName = "IT Department",
                    Description = "Tech Dept",
                    Location = "HQ",
                    DepartmentManagerId = null,
                    CreatedAt = new DateTime(2024, 1, 1),
                    CreatedBy = Guid.Empty
                }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = employeeId,
                    TenantId = tenantId,
                    FirstName = "Test",
                    LastName = "Employee",
                    Email = "test@demo.com",
                    PhoneNumber = "0790000000",
                    Position = "Developer",
                    Address = "Amman",
                    DepartmentId = 1,
                    DateOfBirth = new DateTime(1995, 1, 1),
                    EmploymentStartDate = new DateTime(2024, 1, 1),
                    PasswordHash = "Test@123",
                    Gender = (Gender)1,
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1),
                    CreatedBy = Guid.Empty
                }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = RoleNames.SystemAdmin,
                    Description = "System administrator",
                    CreatedAt = new DateTime(2024, 1, 1),
                    CreatedBy = Guid.Empty,
                    TenantId = tenantId
                },
                 new Role
                 {
                     Id = 2,
                     Name = RoleNames.HrAdmin,
                     Description = "Hr Manager",
                     CreatedAt = new DateTime(2024, 1, 1),
                     CreatedBy = Guid.Empty,
                     TenantId = tenantId

                 },
                new Role
                {
                    Id = 3,
                    Name = RoleNames.Manager,
                    Description = "Manager",
                    CreatedAt = new DateTime(2024, 1, 1),
                    CreatedBy = Guid.Empty,
                    TenantId = tenantId
                },
                new Role
                {
                    Id = 4,
                    Name = RoleNames.Employee,
                    Description = "Regular employee",
                    CreatedAt = new DateTime(2024, 1, 1),
                    CreatedBy = Guid.Empty,
                    TenantId = tenantId
                },
                new Role
                {
                    Id = 5,
                    Name = RoleNames.Recruiter,
                    Description = "Recruitment role",
                    CreatedAt = new DateTime(2024, 1, 1),
                    CreatedBy = Guid.Empty,
                    TenantId = tenantId
                }
            );
            modelBuilder.Entity<Employee>(entity =>
               {
                   entity.HasKey(e => e.Id);


                   entity.HasOne(e => e.Department)
                            .WithMany(d => d.Employees)
                            .HasForeignKey(e => e.DepartmentId)
                            .OnDelete(DeleteBehavior.Restrict);


                   entity.HasOne(e => e.Manager)
                   .WithMany(m => m.Subordinates)
                            .HasForeignKey(e => e.ManagerId)
                            .OnDelete(DeleteBehavior.Restrict);

                   entity.HasIndex(e => e.Email).IsUnique(true);

                   entity.HasOne(e => e.Tenant)
                         .WithMany()
                         .HasForeignKey(e => e.TenantId)
                         .OnDelete(DeleteBehavior.Restrict);

                   entity.HasQueryFilter(e =>
                       !_currentTenant.IsSet ||
                         e.TenantId == _currentTenant.TenantId);

               });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.HasOne(d => d.DepartmentManager)
                      .WithMany()
                      .HasForeignKey(d => d.DepartmentManagerId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Tenant)
                        .WithMany()
                        .HasForeignKey(d => d.TenantId)
                        .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(d =>
                      !_currentTenant.IsSet ||
                    d.TenantId == _currentTenant.TenantId);
            });

            modelBuilder.Entity<JobPosition>(entity =>
            {
                entity.HasKey(jp => jp.Id);

                entity.HasOne(jp => jp.Department)
                      .WithMany()
                      .HasForeignKey(jp => jp.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);

              

                entity.HasOne(jp => jp.Tenant)
                      .WithMany()
                      .HasForeignKey(jp => jp.TenantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(jp =>
                      !_currentTenant.IsSet ||
                    jp.TenantId == _currentTenant.TenantId);
            });
            modelBuilder.Entity<JobApplication>(entity =>
            {
                entity.HasKey(ja => ja.Id);

                entity.HasOne(ja => ja.JobPosition)
                      .WithMany(jp => jp.JobApplications)
                      .HasForeignKey(ja => ja.JobPositionId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ja => ja.Tenant)
                      .WithMany()
                      .HasForeignKey(ja => ja.TenantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ja => ja.DocumentCv)
                      .WithMany()
                      .HasForeignKey(ja => ja.DocumentCvId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ja => ja.ReviewedByEmployee)
                      .WithMany()
                      .HasForeignKey(ja => ja.ReviewedByEmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(ja =>
                      !_currentTenant.IsSet ||
                    ja.TenantId == _currentTenant.TenantId);
            });

            modelBuilder.Entity<DocumentCv>(entity =>
            {
                entity.HasKey(dc => dc.Id);

                entity.HasOne(dc => dc.Tenant)
                      .WithMany()
                      .HasForeignKey(dc => dc.TenantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(dc =>
                      !_currentTenant.IsSet ||
                    dc.TenantId == _currentTenant.TenantId);

            });
            modelBuilder.Entity<DocumentEmployeeInfo>(entity =>
            {
                entity.HasKey(dei => dei.Id);

                entity.HasOne(dei => dei.Employee)
                      .WithMany()
                      .HasForeignKey(dei => dei.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(dei => dei.Tenant)
                        .WithMany()
                        .HasForeignKey(dei => dei.TenantId)
                        .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(dei =>
                     !_currentTenant.IsSet ||
                    dei.TenantId == _currentTenant.TenantId);

            });
            modelBuilder.Entity<GenericRequest>(entity =>
            {
                entity.HasKey(gr => gr.Id);
                entity.HasOne(gr => gr.RequestedByEmployee)
                      .WithMany()
                      .HasForeignKey(gr => gr.RequestedByEmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(gr => gr.Tenant)
                        .WithMany()
                        .HasForeignKey(gr => gr.TenantId)
                        .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(gr =>
                      !_currentTenant.IsSet ||
                    gr.TenantId == _currentTenant.TenantId);
            });
            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(lr => lr.Id);
                entity.HasOne(lr => lr.GenericRequest)
                      .WithOne()
                      .HasForeignKey<LeaveRequest>(lr => lr.Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(lr => lr.Tenant)
                      .WithMany()
                      .HasForeignKey(lr => lr.TenantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(lr =>
                      !_currentTenant.IsSet ||
                    lr.TenantId == _currentTenant.TenantId);
            });
            modelBuilder.Entity<EmployeeDataChange>(entity =>
            {
                entity.HasKey(edc => edc.Id);
                entity.HasOne(edc => edc.GenericRequest)
                      .WithOne()
                      .HasForeignKey<EmployeeDataChange>(edc => edc.Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(edc => edc.Tenant)
                        .WithMany()
                        .HasForeignKey(edc => edc.TenantId)
                        .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(edc =>
                      !_currentTenant.IsSet ||
                    edc.TenantId == _currentTenant.TenantId);
            });
            modelBuilder.Entity<RequestHistory>(entity =>
            {
                entity.HasKey(h => h.Id);

                entity.HasOne(h => h.GenericRequest)
                      .WithMany()
                      .HasForeignKey(h => h.GenericRequestId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.PerformedByEmployee)
                      .WithMany()
                      .HasForeignKey(h => h.PerformedByEmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.Tenant)
                        .WithMany()
                        .HasForeignKey(h => h.TenantId)
                        .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(h =>
                      !_currentTenant.IsSet ||
                    h.TenantId == _currentTenant.TenantId);
            });

            modelBuilder.Entity<Tenant>(entity =>
             {
                 entity.HasKey(t => t.Id);
                 entity.HasIndex(t => t.Code).IsUnique(true);
             });

            modelBuilder.Entity<EmployeeRole>(entity =>
            {
                entity.HasKey(er => er.Id);

                entity.HasIndex(er => new { er.EmployeeId, er.RoleId }).IsUnique();

                entity.HasOne(er => er.Employee)
                .WithMany(e => e.EmployeeRoles)
                .HasForeignKey(er => er.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(er => er.Role)
                .WithMany(r => r.EmployeeRoles)
                .HasForeignKey(er => er.RoleId)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasQueryFilter(er =>
                      !_currentTenant.IsSet ||
                    er.TenantId == _currentTenant.TenantId);
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Name).IsRequired();

                entity.HasMany(r => r.EmployeeRoles)
                .WithOne(er => er.Role)
                .HasForeignKey(er => er.RoleId)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasQueryFilter(r =>
                      !_currentTenant.IsSet ||
                    r.TenantId == _currentTenant.TenantId);
            });

        }

        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker
                 .Entries<IAuditableEntity>()
                 .Where(e =>
                 e.State == EntityState.Modified ||
                 e.State == EntityState.Added)
                 .ToList();

            var now = DateTime.Now;
            var currentUserId = _currentUser.EmployeeId ?? Guid.Empty;

            foreach (var entry in entries)
            {
                var entity = entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    if (entity.CreatedAt == default)
                        entity.CreatedAt = now;

                    entity.CreatedBy = currentUserId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedBy = currentUserId;
                    entity.UpdatedAt = now;


                    //don't allow anyone modified the createdAt & createdBy
                    entry.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
                    entry.Property(nameof(IAuditableEntity.CreatedBy)).IsModified = false;
                }

            }
        }

        private void ApplyTenantRules()
        {
            var entries = ChangeTracker.Entries<ITenantEntity>()
                .Where(e =>
                e.State == EntityState.Added ||
                e.State == EntityState.Modified ||
                e.State == EntityState.Deleted)
                .ToList();

            if (!entries.Any())
                return;

            if (!_currentTenant.IsSet || _currentTenant.TenantId == Guid.Empty)
            {
                throw new InvalidOperationException("Tenant must be set before saving tenant entites");
            }

            Guid tenantId = _currentTenant.TenantId;

            foreach (var entry in entries)
            {
                var entity = entry.Entity;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.TenantId = tenantId;
                        break;

                    case EntityState.Modified:
                        var originalTenantId = entry.OriginalValues.GetValue<Guid>(nameof(ITenantEntity.TenantId));
                        if (originalTenantId != tenantId)
                        {
                            throw new InvalidOperationException($"Cross-tenant update detected. Entity tenant = {originalTenantId}, current tenant = {tenantId}");

                        }
                        entry.Property(nameof(ITenantEntity.TenantId)).IsModified = false;
                        break;

                    case EntityState.Deleted:
                        var deletedTenantId =
                            entry.OriginalValues.GetValue<Guid>(nameof(ITenantEntity.TenantId));

                        if (deletedTenantId != tenantId)
                        {
                            throw new InvalidOperationException(
                                $"Cross-tenant delete detected. Entity tenant = {deletedTenantId}, current tenant = {tenantId}");
                        }
                        break;
                }
            }

        }
    }
}
