using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Windows.Media.Imaging;
using MediaPack.Models;
using MediaPack.Helpers;
using MediaPack.Models.Channel.Entities;
using MediaPack.Models.Channel.Enums;
using MediaPack.Models.Common.Entities;
using Microsoft.EntityFrameworkCore.Design;
using System.Linq;

namespace MediaPack.Data
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext()
        {
            if (!Database.EnsureCreated()) return;
            Database.Migrate();

            var Json = System.IO.File.ReadAllText(@"C:\Users\e9396\source\repos\MultiMedia\MultiMedia\bin\x64\Debug\data\countries.json");

            var countriesJson = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<XClass>>(Json);

            var flagsPath = @"C:\Users\e9396\Desktop\GitHub\KOR-URL-Shortener\src\public\assets\themes\default\libs\world-countries\flags\128x128\";

            using var db = new AppDbContext();

            foreach (var countryJson in countriesJson)
            {
                var country = new Models.Common.Entities.Country
                {
                    Name = countryJson.name,
                    PhoneCode = countryJson.phoneCode,
                    Alpha2code = countryJson.alpha2code,
                    Alpha3code = countryJson.alpha3code,
                    Flag = (flagsPath + countryJson.alpha2code.ToLower(new System.Globalization.CultureInfo("en-US", false)) + ".png").PathToBitmapImage()
                };

                if (!db.Countries.Any(x => x.Name == country.Name))
                {
                    db.Countries.Add(country);
                    db.SaveChanges();
                }
            }
        }

        #region Channel DbSets

        public DbSet<Category> Categories { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Capture> Captures { get; set; }
        public DbSet<SpendTime> SpendTimes { get; set; }
        public DbSet<Task> Tasks { get; set; }

        #endregion

        #region Common

        public DbSet<Country> Countries { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }


        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            optionsBuilder.UseSqlite("Data Source=MediaPack.db;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Channel Entities

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.ChannelType)
                    .HasConversion(c => c.ToString(),
                        c => (ChannelType)Enum.Parse(typeof(ChannelType), c))
                    .IsUnicode(false);
                entity.Property(x => x.IsFavorite)
                    .HasConversion(c => Convert.ToInt32(c),
                        c => Convert.ToBoolean(c));
                entity.Property(x => x.Logo)
                    .HasConversion(c => c.BitmapImageToPath(),
                        c => c.PathToBitmapImage());
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.StartTime)
                    .HasConversion(c => c.TimeSpanToTimeString(),
                        c => TimeSpan.Parse(c));
                entity.Property(x => x.EndTime)
                    .HasConversion(c => c.TimeSpanToTimeString(),
                        c => TimeSpan.Parse(c));
                entity.Property(x => x.Status)
                    .HasConversion(c => Convert.ToInt32(c),
                        c => Convert.ToBoolean(c));
            });

            modelBuilder.Entity<Capture>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Thumbnail)
                    .HasConversion(c => c.BitmapImageToPath(),
                        c => c.PathToBitmapImage());
                entity.Property(x => x.CaptureDate)
                    .HasConversion(c => c.ToString("yyyy-MM-dd HH:mm:ss", Settings.CultureInfo),
                        c => DateTime.Parse(c));
            });

            modelBuilder.Entity<SpendTime>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.AddedDate)
                    .HasConversion(c => c.ToString("yyyy-MM-dd HH:mm:ss", Settings.CultureInfo),
                        c => DateTime.Parse(c));
                entity.Property(x => x.Spendtime)
                    .HasConversion(c => c.TimeSpanToTimeString(),
                        c => TimeSpan.Parse(c));
            });

            #endregion

            #region Common Entities

            modelBuilder.Entity<AppSetting>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.IsEditable)
                    .HasConversion(c => Convert.ToInt32(c),
                        c => Convert.ToBoolean(c));
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Name).IsUnique();
                entity.Property(x => x.Flag)
                    .HasConversion(c => c.ImageToByteArray(entity.Property(x => x.Id)),
                        c => c.ByteArrayToBitmapImage(entity.Property(x => x.Id)));
            });

            #endregion


            base.OnModelCreating(modelBuilder);
        }
    }

    public class XClass
    {
        public string name { get; set; }
        public string phoneCode { get; set; }
        public string alpha2code { get; set; }
        public string alpha3code { get; set; }
    }
}