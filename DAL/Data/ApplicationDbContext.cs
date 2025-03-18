using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Diningtable> Diningtables { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemModifiergroupMapping> ItemModifiergroupMappings { get; set; }

    public virtual DbSet<Itemsminmaxmapping> Itemsminmaxmappings { get; set; }

    public virtual DbSet<Itemsmodifiergroupminmaxmapping> Itemsmodifiergroupminmaxmappings { get; set; }

    public virtual DbSet<Itemsrmodifiersgroup> Itemsrmodifiersgroups { get; set; }

    public virtual DbSet<Modifieritem> Modifieritems { get; set; }

    public virtual DbSet<Modifieritemsmodifiersgroup> Modifieritemsmodifiersgroups { get; set; }

    public virtual DbSet<Modifiersgroup> Modifiersgroups { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Rolespermission> Rolespermissions { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userdetail> Userdetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=pizzashop_main;Username=postgres;Password=Sendrock@1");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("category_pkey");

            entity.ToTable("category");

            entity.HasIndex(e => e.Name, "category_name_key").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.Modifieddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifieddate");
            entity.Property(e => e.Modifyiedby).HasColumnName("modifyiedby");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.CategoryCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .HasConstraintName("fk_createdby");

            entity.HasOne(d => d.ModifyiedbyNavigation).WithMany(p => p.CategoryModifyiedbyNavigations)
                .HasForeignKey(d => d.Modifyiedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_modifyiedby");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("city_pkey");

            entity.ToTable("city");

            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CityName)
                .HasMaxLength(100)
                .HasColumnName("city_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.StateId).HasColumnName("state_id");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("city_state_id_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("country_pkey");

            entity.ToTable("country");

            entity.HasIndex(e => e.CountryCode, "country_country_code_key").IsUnique();

            entity.HasIndex(e => e.CountryName, "country_country_name_key").IsUnique();

            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(10)
                .HasColumnName("country_code");
            entity.Property(e => e.CountryName)
                .HasMaxLength(100)
                .HasColumnName("country_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
        });

        modelBuilder.Entity<Diningtable>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("diningtables_pkey");

            entity.ToTable("diningtables");

            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Modifieddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifieddate");
            entity.Property(e => e.Modifyiedby).HasColumnName("modifyiedby");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'available'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("status");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.DiningtableCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_diningtables_createdby");

            entity.HasOne(d => d.ModifyiedbyNavigation).WithMany(p => p.DiningtableModifyiedbyNavigations)
                .HasForeignKey(d => d.Modifyiedby)
                .HasConstraintName("fk_diningtables_modifyiedby");

            entity.HasOne(d => d.Section).WithMany(p => p.Diningtables)
                .HasForeignKey(d => d.SectionId)
                .HasConstraintName("fk_diningtables_section");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("items_pkey");

            entity.ToTable("items");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.DefaultTax).HasColumnName("default_tax");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Isavailable).HasColumnName("isavailable");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.ItemName)
                .HasMaxLength(50)
                .HasColumnName("item_name");
            entity.Property(e => e.Modifieddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifieddate");
            entity.Property(e => e.Modifyiedby).HasColumnName("modifyiedby");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.ShortCode)
                .HasMaxLength(20)
                .HasColumnName("short_code");
            entity.Property(e => e.TaxPercentage).HasColumnName("tax_percentage");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.Unit)
                .HasColumnType("character varying")
                .HasColumnName("unit");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("items_category_id_fkey");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.ItemCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .HasConstraintName("fk_createdby");

            entity.HasOne(d => d.ModifyiedbyNavigation).WithMany(p => p.ItemModifyiedbyNavigations)
                .HasForeignKey(d => d.Modifyiedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_modifyiedby");
        });

        modelBuilder.Entity<ItemModifiergroupMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_modifiergroup_mapping_pkey");

            entity.ToTable("item_modifiergroup_mapping");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ModifierGroupId).HasColumnName("modifier_group_id");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemModifiergroupMappings)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("item_modifiergroup_mapping_item_id_fkey");

            entity.HasOne(d => d.ModifierGroup).WithMany(p => p.ItemModifiergroupMappings)
                .HasForeignKey(d => d.ModifierGroupId)
                .HasConstraintName("item_modifiergroup_mapping_modifier_group_id_fkey");
        });

        modelBuilder.Entity<Itemsminmaxmapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("itemsminmaxmapping_pkey");

            entity.ToTable("itemsminmaxmapping");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.MaxValue).HasColumnName("max_value");
            entity.Property(e => e.MinValue).HasColumnName("min_value");

            entity.HasOne(d => d.Item).WithMany(p => p.Itemsminmaxmappings)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("itemsminmaxmapping_item_id_fkey");
        });

        modelBuilder.Entity<Itemsmodifiergroupminmaxmapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("itemsmodifiergroupminmaxmapping_pkey");

            entity.ToTable("itemsmodifiergroupminmaxmapping");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.MaxValue).HasColumnName("max_value");
            entity.Property(e => e.MinValue).HasColumnName("min_value");
            entity.Property(e => e.ModifiergroupId).HasColumnName("modifiergroup_id");

            entity.HasOne(d => d.Item).WithMany(p => p.Itemsmodifiergroupminmaxmappings)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("itemsmodifiergroupminmaxmapping_item_id_fkey");

            entity.HasOne(d => d.Modifiergroup).WithMany(p => p.Itemsmodifiergroupminmaxmappings)
                .HasForeignKey(d => d.ModifiergroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("itemsmodifiergroupminmaxmapping_modifiergroup_id_fkey");
        });

        modelBuilder.Entity<Itemsrmodifiersgroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("itemsrmodifiers_pkey");

            entity.ToTable("itemsrmodifiersgroup");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('itemsrmodifiers_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ModifiergroupId).HasColumnName("modifiergroup_id");

            entity.HasOne(d => d.Item).WithMany(p => p.Itemsrmodifiersgroups)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("itemsrmodifiers_item_id_fkey");

            entity.HasOne(d => d.Modifiergroup).WithMany(p => p.Itemsrmodifiersgroups)
                .HasForeignKey(d => d.ModifiergroupId)
                .HasConstraintName("itemsrmodifiers_modifier_id_fkey");
        });

        modelBuilder.Entity<Modifieritem>(entity =>
        {
            entity.HasKey(e => e.ModifierId).HasName("modifieritems_pkey");

            entity.ToTable("modifieritems");

            entity.Property(e => e.ModifierId).HasColumnName("modifier_id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Modifieddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifieddate");
            entity.Property(e => e.ModifierName)
                .HasMaxLength(50)
                .HasColumnName("modifier_name");
            entity.Property(e => e.Modifyiedby).HasColumnName("modifyiedby");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.ModifieritemCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifieritems_createdby_fkey");

            entity.HasOne(d => d.ModifyiedbyNavigation).WithMany(p => p.ModifieritemModifyiedbyNavigations)
                .HasForeignKey(d => d.Modifyiedby)
                .HasConstraintName("modifieritems_modifyiedby_fkey");
        });

        modelBuilder.Entity<Modifieritemsmodifiersgroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modifieritemsmodifiersgroup_pkey");

            entity.ToTable("modifieritemsmodifiersgroup");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ModifierId).HasColumnName("modifier_id");
            entity.Property(e => e.ModifiergroupId).HasColumnName("modifiergroup_id");

            entity.HasOne(d => d.Modifier).WithMany(p => p.Modifieritemsmodifiersgroups)
                .HasForeignKey(d => d.ModifierId)
                .HasConstraintName("modifieritemsmodifiersgroup_modifier_id_fkey");

            entity.HasOne(d => d.Modifiergroup).WithMany(p => p.Modifieritemsmodifiersgroups)
                .HasForeignKey(d => d.ModifiergroupId)
                .HasConstraintName("modifieritemsmodifiersgroup_modifiergroup_id_fkey");
        });

        modelBuilder.Entity<Modifiersgroup>(entity =>
        {
            entity.HasKey(e => e.ModifiergroupId).HasName("modifiersgroup_pkey");

            entity.ToTable("modifiersgroup");

            entity.Property(e => e.ModifiergroupId).HasColumnName("modifiergroup_id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Modifieddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifieddate");
            entity.Property(e => e.Modifyiedby).HasColumnName("modifyiedby");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.ModifiersgroupCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifiersgroup_createdby_fkey");

            entity.HasOne(d => d.ModifyiedbyNavigation).WithMany(p => p.ModifiersgroupModifyiedbyNavigations)
                .HasForeignKey(d => d.Modifyiedby)
                .HasConstraintName("modifiersgroup_modifyiedby_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("permission_pkey");

            entity.ToTable("permission");

            entity.HasIndex(e => e.PermissionName, "permission_permission_name_key").IsUnique();

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.PermissionName)
                .HasMaxLength(20)
                .HasColumnName("permission_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Roleid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("roleid");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RoleCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("roles_created_by_fkey");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RoleUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("roles_updated_by_fkey");
        });

        modelBuilder.Entity<Rolespermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rolespermission_pkey");

            entity.ToTable("rolespermission");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Candelete).HasColumnName("candelete");
            entity.Property(e => e.Canedit).HasColumnName("canedit");
            entity.Property(e => e.Canview).HasColumnName("canview");
            entity.Property(e => e.Isenable)
                .HasDefaultValueSql("false")
                .HasColumnName("isenable");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Modifieddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifieddate");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.Roleid).HasColumnName("roleid");

            entity.HasOne(d => d.Permission).WithMany(p => p.Rolespermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("fk_permission");

            entity.HasOne(d => d.Role).WithMany(p => p.Rolespermissions)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("fk_role");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("section_pkey");

            entity.ToTable("section");

            entity.Property(e => e.SectionId).HasColumnName("section_id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Modifieddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifieddate");
            entity.Property(e => e.Modifyiedby).HasColumnName("modifyiedby");
            entity.Property(e => e.SectionName)
                .HasMaxLength(100)
                .HasColumnName("section_name");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.SectionCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_section_createdby");

            entity.HasOne(d => d.ModifyiedbyNavigation).WithMany(p => p.SectionModifyiedbyNavigations)
                .HasForeignKey(d => d.Modifyiedby)
                .HasConstraintName("fk_section_modifyiedby");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("state_pkey");

            entity.ToTable("state");

            entity.Property(e => e.StateId).HasColumnName("state_id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.StateName)
                .HasMaxLength(100)
                .HasColumnName("state_name");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("state_country_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .HasColumnName("password");
            entity.Property(e => e.Rememberme)
                .HasDefaultValueSql("false")
                .HasColumnName("rememberme");
        });

        modelBuilder.Entity<Userdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("userdetail_pkey");

            entity.ToTable("userdetail");

            entity.HasIndex(e => e.UserName, "userdetail_user_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("isdeleted");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Modifieddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifieddate");
            entity.Property(e => e.Phone)
                .HasMaxLength(13)
                .HasColumnName("phone");
            entity.Property(e => e.Profile).HasColumnName("profile");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("user_name");
            entity.Property(e => e.Zipcode)
                .HasMaxLength(10)
                .HasColumnName("zipcode");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.UserdetailCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userdetail_createdby_fkey");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.UserdetailModifiedbyNavigations)
                .HasForeignKey(d => d.Modifiedby)
                .HasConstraintName("userdetail_modifiedby_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Userdetails)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_role_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserdetailUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("userdetail_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
