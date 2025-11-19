using HrMangmentSystem_Domain.Entities.Documents;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Domain.Entities.History;
using HrMangmentSystem_Domain.Entities.Recruitment;
using HrMangmentSystem_Domain.Entities.Requests;
using HrMangmentSystem_Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HrMangmentSystem_Infrastructure.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

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

        public DbSet<History> Histories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints 

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

               });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.HasOne(d => d.DepartmentManager)
                      .WithMany()
                      .HasForeignKey(d => d.DepartmentManagerId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<JobPosition>(entity =>
            {
                entity.HasKey(jp => jp.Id);

                entity.HasOne(jp => jp.Department)
                      .WithMany()
                      .HasForeignKey(jp => jp.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(jp => jp.CreatedByEmployee)
                      .WithMany()
                      .HasForeignKey(jp => jp.CreatedByEmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<JobApplication>(entity =>
            {
                entity.HasKey(ja => ja.Id);

                entity.HasOne(ja => ja.JobPosition)
                      .WithMany(jp => jp.JobApplications)
                      .HasForeignKey(ja => ja.JobPositionId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ja => ja.DocumentCv)
                      .WithMany()
                      .HasForeignKey(ja => ja.DocumentCvId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DocumentCv>(entity =>
            {
                entity.HasKey(dc => dc.Id);

            });
            modelBuilder.Entity<DocumentEmployeeInfo>(entity =>
            {
                entity.HasKey(dei => dei.Id);

                entity.HasOne(dei => dei.Employee)
                      .WithMany()
                      .HasForeignKey(dei => dei.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<GenericRequest>(entity =>
            {
                entity.HasKey(gr => gr.Id);
                entity.HasOne(gr => gr.RequestedByEmployee)
                      .WithMany()
                      .HasForeignKey(gr => gr.RequestedByEmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(lr => lr.Id);
                entity.HasOne(lr => lr.GenericRequest)
                      .WithOne()
                      .HasForeignKey<LeaveRequest>(lr => lr.Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<EmployeeDataChange>(entity =>
            {
                entity.HasKey(edc => edc.Id);
                entity.HasOne(edc => edc.GenericRequest)
                      .WithOne()
                      .HasForeignKey<EmployeeDataChange>(edc => edc.Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<History>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.HasOne(h => h.PerformedByEmployee)
                      .WithMany()
                      .HasForeignKey(h => h.PerformedByEmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EmployeeRole>()
                .HasKey(er => new { er.EmployeeId, er.RoleId });

            modelBuilder.Entity<EmployeeRole>()
                .HasOne(er => er.Employee)
                .WithMany(e => e.EmployeeRoles)
                .HasForeignKey(er => er.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);


        }

    }
}
