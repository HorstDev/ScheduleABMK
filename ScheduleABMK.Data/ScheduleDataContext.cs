using Microsoft.EntityFrameworkCore;
using ScheduleABMK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleABMK.Data
{
    public class ScheduleDataContext : DbContext
    {
        public ScheduleDataContext(DbContextOptions<ScheduleDataContext> options) : base(options)
        {
        }

        private Role[] AllRoles()
        {
            const string studentRoleName = "Student";
            const string teacherRoleName = "Teacher";
            const string adminRoleName = "Admin";

            var studentRole = new Role { Id = Guid.NewGuid(), Name = studentRoleName };
            var teacherRole = new Role { Id = Guid.NewGuid(), Name = teacherRoleName };
            var adminRole = new Role { Id = Guid.NewGuid(), Name = adminRoleName };

            return new Role[] { studentRole, teacherRole, adminRole };
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(AllRoles());

            modelBuilder.UseSerialColumns();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
