using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Infrastructure.EntityFramework
{
  public class ClubPoolContext : DbContext, IDbContext
  {
    public DbSet<Division> Divisions { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<MatchPlayer> MatchPlayers { get; set; }
    public DbSet<MatchResult> MatchResults { get; set; }
    public DbSet<Meet> Meets { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<SkillLevel> SkillLevels { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<User> Users { get; set; }

    private IDbTransaction transaction;

    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
      var schema = "clubpool";
      modelBuilder.Entity<Division>().ToTable("Divisions", schema);
      modelBuilder.Entity<Match>().ToTable("Matches", schema);
      modelBuilder.Entity<MatchPlayer>().ToTable("MatchPlayers", schema);
      modelBuilder.Entity<MatchResult>().ToTable("MatchResults", schema);

      modelBuilder.Entity<Meet>().ToTable("Meets", schema);
      modelBuilder.Entity<Meet>().HasMany(m => m.Teams).WithMany().Map(cfg => cfg.ToTable("MeetsTeams", schema));

      modelBuilder.Entity<Role>().ToTable("Roles", schema);
      modelBuilder.Entity<Season>().ToTable("Seasons", schema);
      modelBuilder.Entity<SkillLevel>().ToTable("SkillLevels", schema);

      modelBuilder.Entity<Team>().ToTable("Teams", schema);
      modelBuilder.Entity<Team>().HasMany(t => t.Players).WithMany().Map(cfg => cfg.ToTable("TeamsUsers", schema));

      modelBuilder.Entity<User>().ToTable("Users", schema);
      modelBuilder.Entity<User>().HasMany(u => u.Roles).WithMany(r => r.Users).Map(cfg => cfg.ToTable("RolesUsers", schema));
    }

    public void CommitTransaction() {
      if (null != transaction)
        transaction.Commit();
    }

    public void RollbackTransaction() {
      if (null != transaction)
        transaction.Rollback();
    }

    public IDisposable BeginTransaction() {
      if (Database.Connection.State != ConnectionState.Open) {
        Database.Connection.Open();
      }
      transaction = Database.Connection.BeginTransaction();
      return transaction;
    }

  }

  public class ClubPoolInitializer : CreateDatabaseIfNotExists<ClubPoolContext>
  {

  }
}