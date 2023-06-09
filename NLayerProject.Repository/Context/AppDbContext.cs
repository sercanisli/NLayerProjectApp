﻿using Microsoft.EntityFrameworkCore;
using NLayerProject.Core.Models;
using NLayerProject.Repository.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NLayerProject.Repository.Context
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet <Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }



        //To be accessed via product object
       public DbSet<ProductFeature> ProductFeatures { get; set; }



        public override int SaveChanges()
        {
            foreach (var item in ChangeTracker.Entries())
            {
                if (item.Entity is BaseEntity entityReference)
                {
                    switch (item.Entity)
                    {
                        case EntityState.Added:
                            {
                                entityReference.CreatedDate = DateTime.Now;
                                break;
                            }
                        case EntityState.Modified:
                            {
                                entityReference.UpdatedDate = DateTime.Now;
                                break;
                            }
                    }
                }
            }
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries())
            {
                if (item.Entity is BaseEntity entityReference)
                {
                    switch (item.State)
                    {
                        case EntityState.Added:
                            {
                                entityReference.CreatedDate = DateTime.Now;
                                break;
                            }
                        case EntityState.Modified:
                            {
                                Entry(entityReference).Property(x => x.CreatedDate).IsModified = false;
                                entityReference.UpdatedDate = DateTime.Now;
                                break;
                            }
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //We accessed our configurations from here and run it.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            //******** - Seed Process in DbContext.
            modelBuilder.Entity<ProductFeature>().HasData(new ProductFeature()
            {
                Id=1, Color="Red", Height=100, Width=100, ProductId=1
            },
            new ProductFeature()
            {
                Id = 2, Color = "Blue", Height = 300, Width = 100, ProductId = 2
            });
            //******** - Seed Process in DbContext.


            base.OnModelCreating(modelBuilder);

            //modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }
    }
}
