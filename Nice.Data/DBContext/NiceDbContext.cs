﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Nice.Domain.Infrastructure;
using Nice.Mapping.SystemManage;


namespace Nice.Data.DBContext
{

    public class NiceDbContext : DbContext
    {

        public NiceDbContext()
            : base("NiceDbContext")
        {
            //this.Configuration.AutoDetectChangesEnabled = false;
            //this.Configuration.ValidateOnSaveEnabled = false;
            //this.Configuration.LazyLoadingEnabled = false;
            //this.Configuration.ProxyCreationEnabled = false;

            Configuration.ProxyCreationEnabled = true;
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.LazyLoadingEnabled = true;
            Configuration.EnsureTransactionsForFunctionsAndCommands = false;
            Configuration.UseDatabaseNullSemantics = true;
            Configuration.ValidateOnSaveEnabled = true;

            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<NiceDbContext, Configuration>());
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 8));

            var modelTypes = new List<Type>();
            var assemb = typeof (BaseEntity).Assembly;
            try
            {
                var types = assemb.GetTypes().Where(t => t.IsClass && t.IsPublic && !t.IsAbstract && (!t.IsNested && !t.IsGenericType) && t.BaseType != null && t.BaseType == typeof (BaseEntity)).ToList();
                if (types.Count > 0)
                    modelTypes.AddRange(types);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var mappingAssemb = typeof (UserMap).Assembly;

            //获取映射模型
            var mapTypes = mappingAssemb.GetTypes()
                .Where(type => !string.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof (EntityTypeConfiguration<>)).ToList();
            mapTypes.ForEach(t =>
            {
                dynamic instance = Activator.CreateInstance(t);
                modelBuilder.Configurations.Add(instance);
            });

            modelTypes.ForEach(modelBuilder.RegisterEntityType);

            base.OnModelCreating(modelBuilder);
        }

    }

}