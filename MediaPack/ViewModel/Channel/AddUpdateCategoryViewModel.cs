using MediaPack.Data;
using MediaPack.Models.Channel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MediaPack.ViewModel.Channel
{
    public class AddUpdateCategoryViewModel : WindowViewModel
    {
        public AddUpdateCategoryViewModel(Window window, Category category = null) : base(window)
        {
            mWindow = window;
            WindowMinimumHeight = 350;
            WindowMinimumWidth = 500;

            Title = category != null ? $"Kategori Güncelle: {category.Title}" : "Kategori Ekle";
            Category = category != null ? category : new Category();


            CloseCommand = new RelayCommand(p =>
            {
                AddOrUpdate();

                mWindow.Close();
            });
        }

        public Category Category { get; set; }

        public void AddOrUpdate()
        {
            if (Category != null && !string.IsNullOrEmpty(Category.Title))
            {
                using var db = new AppDbContext();

                _ = Category.Id > 0 ? db.Categories.Update(Category) : db.Categories.Add(Category);
                db.SaveChanges();
            }
        }
    }
}
