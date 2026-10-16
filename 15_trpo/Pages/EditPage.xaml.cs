using _15_trpo.Models;
using _15_trpo.SingleToneService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _15_trpo.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditPage.xaml
    /// </summary>
    public partial class EditPage : Page, INotifyPropertyChanged // ← Добавь INPC
    {
        public ObservableCollection<Tag> Tags { get; set; } = new();
        public ObservableCollection<Product> Products { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<Brand> Brands { get; set; } = new();
        private int beforeIndex = 0;
        private bool isEdited
            = false;
        private Product _prevProduct;
        private Product _selectedProduct;

        public Product selectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        private Tag _selectedTag;

        public Tag selectedTag
        {
            get => _selectedTag;
            set { _selectedTag = value; OnPropertyChanged(); }
        }

        private Category _selectedCategory;
        public Category selectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        private Brand _selectedBrand;
        public Brand selectedBrand
        {
            get => _selectedBrand;
            set { _selectedBrand = value; OnPropertyChanged(); }
        }

        public readonly _15trpoContext _db = DBService.Instance.Context;

        public EditPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadData();
            MainWindow.setTitle("Управление данными");
        }

        private void LoadData()
        {
            Products.Clear();
            Categories.Clear();
            Brands.Clear();
            Tags.Clear();

            foreach (var p in _db.Products.Include(p => p.Category)
                                          .Include(p => p.Brand)
                                          .Include(p => p.Tags)
                                          .ToList())
            {
                Products.Add(p);
            }
            foreach (var c in _db.Categories.ToList())
                Categories.Add(c);
            foreach (var b in _db.Brands.ToList())
                Brands.Add(b);
            foreach (var t in _db.Tags.ToList())
                Tags.Add(t);

            if (Products.Any() && selectedProduct == null)
                selectedProduct = Products[0];
            if (Categories.Any() && selectedCategory == null)
                selectedCategory = Categories[0];
            if (Brands.Any() && selectedBrand == null)
                selectedBrand = Brands[0];
            if (Tags.Any() && selectedTag == null)
                selectedTag = Tags[0];
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProduct == null || !_db.Products.Contains(selectedProduct)) return;

            if (MessageBox.Show($"Удалить {selectedProduct.Name}?", "Подтверждение",
                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _db.Products.Remove(selectedProduct);
                _db.SaveChanges();
                LoadData();
            }
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (!isEdited)
            {
                Products[beforeIndex] = _prevProduct;
            }
            if (DataGridProducts.SelectedItem is Product product)
            {
                selectedProduct = product;
            }
            beforeIndex = DataGridProducts.SelectedIndex;
            isEdited = false;
            _prevProduct.Id = selectedProduct.Id;
            _prevProduct.Name = selectedProduct.Name;
            _prevProduct.Description = selectedProduct.Description;
            _prevProduct.Price = selectedProduct.Price;
            _prevProduct.CategoryId = selectedProduct.CategoryId;
            _prevProduct.BrandId = selectedProduct.BrandId;
            _prevProduct.Stock = selectedProduct.Stock;
            _prevProduct.CreatedAt = selectedProduct.CreatedAt;
            _prevProduct.Rating = selectedProduct.Rating;
            _prevProduct.Brand = selectedProduct.Brand;
            _prevProduct.Tags = selectedProduct.Tags;
            _prevProduct.Category = selectedProduct.Category;
        }

        private void DataGridCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridCategories.SelectedItem is Category c)
                selectedCategory = c;
        }

        private void DataGridBrands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridBrands.SelectedItem is Brand b)
                selectedBrand = b;
        }

        private void DataGridTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridTags.SelectedItem is Tag t)
                selectedTag = t;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

            if (Validation.GetHasError(NameTb)|| Validation.GetHasError(RatingTb) || Validation.GetHasError(PriceTb) || Validation.GetHasError(StockTb) || Validation.GetHasError(DescTb))
            {
                MessageBox.Show("Исправьте ошибки валидации!");
                return;
            }
            if (selectedProduct.Id != 0)
            {
                MessageBox.Show("Заполните данные о новом продукте и нажмите добавить");
                selectedProduct = new Product
                {
                    Name = "Новый продукт",
                    Description = "",
                    Price = 0,
                    Stock = 0,
                    Rating = 0,
                    CreatedAt = DateTime.Now.ToString("s")
                };
                return;
            }
            if (selectedProduct == null)    
            {
                MessageBox.Show("Нет данных для добавления.");
                return;
            }
            if (string.IsNullOrWhiteSpace(selectedProduct.Name))
            {
                MessageBox.Show("Название продукта не может быть пустым.");
                return;
            }
            if (_db.Products.Any(p => p.Name == selectedProduct.Name))
            {
                MessageBox.Show("Продукт с таким названием уже существует.");
                return;
            }
            

            var newProduct = new Product
            {
                Name = selectedProduct.Name,
                Description = selectedProduct.Description,
                Price = selectedProduct.Price,
                Stock = selectedProduct.Stock,
                Rating = selectedProduct.Rating,
                CreatedAt = string.IsNullOrWhiteSpace(selectedProduct.CreatedAt) ? DateTime.Now.ToString("s") : selectedProduct.CreatedAt,
                CategoryId = selectedProduct.Category?.Id ?? selectedProduct.CategoryId,
                BrandId = selectedProduct.Brand?.Id ?? selectedProduct.BrandId
            };

            if (selectedProduct.Tags != null)
            {
                foreach (var tag in selectedProduct.Tags)
                {
                    var dbTag = _db.Tags.Find(tag.Id);
                    if (dbTag != null)
                        newProduct.Tags.Add(dbTag);
                }
            }
            if (newProduct.Id == 0)
            {
                newProduct.Id =  _db.Products.Max(p => p.Id) + 1;
            }
            _db.Products.Add(newProduct);
            _db.SaveChanges();
            LoadData();
            MessageBox.Show("Продукт успешно добавлен!");
            selectedProduct = Products.FirstOrDefault(p => p.Id == newProduct.Id);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(NameTb) || Validation.GetHasError(RatingTb) || Validation.GetHasError(PriceTb) || Validation.GetHasError(StockTb) || Validation.GetHasError(DescTb))
            {
                MessageBox.Show("Исправьте ошибки валидации!");
                return;
            }
            if (selectedProduct == null)
                return;

            if (selectedProduct.Id == 0)
            {
                MessageBox.Show("Сначала добавьте товар, затем редактируйте его.");
                return;
            }

            var dbProduct = _db.Products.Include(p => p.Tags).FirstOrDefault(p => p.Id == selectedProduct.Id);
            if (dbProduct == null)
            {
                MessageBox.Show("Продукт не найден в базе данных. Возможно, он был удален.");
                return;
            }

            dbProduct.Name = selectedProduct.Name;
            dbProduct.Description = selectedProduct.Description;
            dbProduct.Price = selectedProduct.Price;
            dbProduct.Stock = selectedProduct.Stock;
            dbProduct.Rating = selectedProduct.Rating;
            dbProduct.CreatedAt = string.IsNullOrWhiteSpace(selectedProduct.CreatedAt) ? dbProduct.CreatedAt : selectedProduct.CreatedAt;
            dbProduct.CategoryId = selectedProduct.Category?.Id ?? selectedProduct.CategoryId;
            dbProduct.BrandId = selectedProduct.Brand?.Id ?? selectedProduct.BrandId;
            dbProduct.Tags = selectedProduct.Tags;
            
            _db.SaveChanges();
            LoadData();
            isEdited = true;
            selectedProduct = Products.FirstOrDefault(p => p.Id == dbProduct.Id);
        }

        private void addTag_Click(object sender, RoutedEventArgs e)
        {
           
            if (TagsCMB.SelectedItem is Tag tag)
            {
                if (!selectedProduct.Tags.Contains(tag))
                {
                    int i = DataGridProducts.SelectedIndex;
                    selectedProduct.Tags.Add(tag);
                    _db.SaveChanges();
                    LoadData();
                    DataGridProducts.SelectedIndex = i;
                }
                else
                {
                    MessageBox.Show("Тег уже добавлен к этому продукту.");
                }
            }
        }

        private void delTags_Click(object sender, RoutedEventArgs e)
        {
            selectedProduct.Tags.Clear();
            OnPropertyChanged();
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            
            if (selectedCategory.Id != 0)
            {
                MessageBox.Show("Заполните данные о новой категории и нажмите еще раз добавить"); //////////
                selectedCategory = new Category { Name = "Новая категория" };
                return;
            }
            if (Validation.GetHasError(CatTb))
            {
                MessageBox.Show("Исправьте ошибки валидации!");
                return;
            }
            if (selectedCategory == null)
            {
                MessageBox.Show("Нет данных для добавления.");
                return;
            }
            if (string.IsNullOrWhiteSpace(selectedCategory.Name))
            {
                MessageBox.Show("Название не может быть пустым.");
                return;
            }
            if (_db.Categories.Any(x => x.Name == selectedCategory.Name))
            {
                MessageBox.Show("Категория с таким названием уже существует.");
                return;
            }
            if (selectedCategory.Id == 0)
            {
                selectedCategory.Id = _db.Categories.Max(p => p.Id) + 1; //////
            }
            var cat = new Category { Id = selectedCategory.Id, Name = selectedCategory.Name };
            _db.Categories.Add(cat);
            _db.SaveChanges();
            LoadData();
            MessageBox.Show("Категория успешно добавлена!");//////////
            selectedCategory = Categories.FirstOrDefault(c => c.Id == cat.Id);
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(CatTb))
            {
                MessageBox.Show("Исправьте ошибки валидации!");
                return;
            }
            if (selectedCategory == null || selectedCategory.Id == 0) 
                return;
            var dbCat = _db.Categories.FirstOrDefault(c => c.Id == selectedCategory.Id);
            if (dbCat == null) 
                return;
            if (string.IsNullOrWhiteSpace(selectedCategory.Name))
            {
                MessageBox.Show("Название не может быть пустым.");
                return;
            }
            dbCat.Name = selectedCategory.Name;
            _db.SaveChanges();
            LoadData();
            selectedCategory = Categories.FirstOrDefault(c => c.Id == dbCat.Id);
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCategory == null) return;
            var dbCat = _db.Categories.FirstOrDefault(c => c.Id == selectedCategory.Id);
            if (dbCat == null) return;
            if (MessageBox.Show($"Удалить {dbCat.Name}?", "Подтверждение", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                return;
            _db.Categories.Remove(dbCat);
            _db.SaveChanges();
            LoadData();
        }

        private void AddBrand_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBrand.Id != 0)
            {
                MessageBox.Show("Заполните данные о новой категории и нажмите еще раз добавить"); //////////
                selectedBrand = new Brand { Name = "Новый бренд" };
                return;
            }
            if (Validation.GetHasError(BrandTb))
            {
                MessageBox.Show("Исправьте ошибки валидации!");
                return;
            }
            if (selectedBrand == null)
            {
                MessageBox.Show("Нет данных для добавления.");
                return;
            }
            if (string.IsNullOrWhiteSpace(selectedBrand.Name))
            {
                MessageBox.Show("Название не может быть пустым.");
                return;
            }
            if (_db.Brands.Any(x => x.Name == selectedBrand.Name))
            {
                MessageBox.Show("Бренд с таким названием уже существует.");
                return;
            }
            if (selectedBrand.Id == 0)
            {
                selectedBrand.Id = _db.Brands.Max(p => p.Id) + 1; //////
            }
            var br = new Brand { Id = selectedBrand.Id, Name = selectedBrand.Name };
            _db.Brands.Add(br);
            _db.SaveChanges();
            LoadData();
            MessageBox.Show("Бренд успешно добавлен!");//////////

            selectedBrand = Brands.FirstOrDefault(b => b.Id == br.Id);
        }

        private void EditBrand_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(BrandTb))
            {
                MessageBox.Show("Исправьте ошибки валидации!");
                return;
            }
            if (selectedBrand == null || selectedBrand.Id == 0) return;
            var dbBr = _db.Brands.FirstOrDefault(b => b.Id == selectedBrand.Id);
            if (dbBr == null) return;
            if (string.IsNullOrWhiteSpace(selectedBrand.Name))
            {
                MessageBox.Show("Название не может быть пустым.");
                return;
            }
            dbBr.Name = selectedBrand.Name;
            _db.SaveChanges();
            LoadData();
            selectedBrand = Brands.FirstOrDefault(b => b.Id == dbBr.Id);
        }

        private void DeleteBrand_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBrand == null)
                return;
            var dbBr = _db.Brands.FirstOrDefault(b => b.Id == selectedBrand.Id);
            if (dbBr == null)
                return;
            if (MessageBox.Show($"Удалить {dbBr.Name}?", "Подтверждение", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                return;
            _db.Brands.Remove(dbBr);
            _db.SaveChanges();
            LoadData();
        }

        private void AddTagManage_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTag != null && selectedTag.Id != 0)
            {
                selectedTag = new Tag { Name = "Новый тег" };
                return;
            }
            if (Validation.GetHasError(TagTb))
            {
                MessageBox.Show("Исправьте ошибки валидации!");
                return;
            }
            if (selectedTag == null)
            {
                MessageBox.Show("Нет данных для добавления.");
                return;
            }
            if (string.IsNullOrWhiteSpace(selectedTag.Name))
            {
                MessageBox.Show("Название не может быть пустым.");
                return;
            }
            if (_db.Tags.Any(x => x.Name == selectedTag.Name))
            {
                MessageBox.Show("Тег с таким названием уже существует.");
                return;
            }
            if (selectedTag.Id == 0)
            {
                selectedTag.Id = _db.Tags.Max(p => p.Id) + 1; //////
            }
            var tg = new Tag {Id = selectedTag.Id, Name = selectedTag.Name };
            _db.Tags.Add(tg);
            _db.SaveChanges();
            LoadData();
            MessageBox.Show("Тег успешно добавлен!");//////////

            selectedTag = Tags.FirstOrDefault(t => t.Id == tg.Id);
        }

        private void EditTagManage_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(TagTb))
            {
                MessageBox.Show("Исправьте ошибки валидации!");
                return;
            }
            if (selectedTag == null || selectedTag.Id == 0) return;
            var dbT = _db.Tags.FirstOrDefault(t => t.Id == selectedTag.Id);
            if (dbT == null) return;
            if (string.IsNullOrWhiteSpace(selectedTag.Name))
            {
                MessageBox.Show("Название не может быть пустым.");
                return;
            }
            dbT.Name = selectedTag.Name;
            _db.SaveChanges();
            LoadData();
            selectedTag = Tags.FirstOrDefault(t => t.Id == dbT.Id);
        }

        private void DeleteTagManage_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTag == null) return;
            var dbT = _db.Tags.FirstOrDefault(t => t.Id == selectedTag.Id);
            if (dbT == null) return;
            if (MessageBox.Show($"Удалить {dbT.Name}?", "Подтверждение", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                return;
            _db.Tags.Remove(dbT);
            _db.SaveChanges();
            LoadData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage(true));
        }
    }
}