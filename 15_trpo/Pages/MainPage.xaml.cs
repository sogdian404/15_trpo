using _15_trpo.Models;
using _15_trpo.SingleToneService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage(bool isManager)
        {
            InitializeComponent();
            MainWindow.setTitle("Главное окно");

            _db = new _15trpoContext();

            LoadData();

            ProductsView = CollectionViewSource.GetDefaultView(Products);
            ProductsView.Filter = FilterProducts;

            if (!isManager)
            {
                ManagerBtn.Visibility = Visibility.Hidden;
                RoleText.Text = "Посетитель";
            }
            else
            {
                ManagerBtn.Visibility = Visibility.Visible;
                RoleText.Text = "Менеджер";
            }
            DataContext = this;
        }

            private _15trpoContext _db = DBService.Instance.Context; 
            public ObservableCollection<Product> Products { get; set; } = new();
            public ICollectionView ProductsView { get; set; }

            public string SearchQuery { get; set; } = "";
            public string PriceFrom { get; set; }
            public string PriceTo { get; set; }

            public Visibility ManagerControlsVisibility { get; set; }

            private void LoadData()
            {
                Products.Clear();
                foreach (var p in _db.Products.Include(p=>p.Category).Include(p => p.Brand).Include(p=>p.Tags).ToList())
                    Products.Add(p);
                CategoryFilter.ItemsSource = _db.Categories.ToList();
                BrandFilter.ItemsSource = _db.Brands.ToList();
            }

        private bool FilterProducts(object obj)
            {
                if (obj is not Product product) return false;

            if (!string.IsNullOrEmpty(SearchQuery) &&
                !product.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.IsNullOrEmpty(PriceFrom) && double.TryParse(PriceFrom, out var min) && product.Price < min)
                return false;
            if (!string.IsNullOrEmpty(PriceTo) && double.TryParse(PriceTo, out var max) && product.Price > max)
                return false;
            if (CategoryFilter.SelectedItem is Category cat && product.Category?.Id != cat.Id)
                return false;
            if (BrandFilter.SelectedItem is Brand br && product.Brand?.Id != br.Id)
                return false;

            return true;
            }
        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchQuery = string.Empty;
            PriceFrom = string.Empty;
            PriceTo = string.Empty;

            if (CategoryFilter != null)
                CategoryFilter.SelectedItem = null;
            if (BrandFilter != null)
                BrandFilter.SelectedItem = null;

            ProductsView.Refresh();

        }
        private void Filter_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ProductsView.Refresh();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductsView.SortDescriptions.Clear();
            var cb = (ComboBox)sender;
            var selected = (ComboBoxItem)cb.SelectedItem;
            switch (selected.Tag)
            {
                case "Name":
                    ProductsView.SortDescriptions.Add(new SortDescription("Name",
                    ListSortDirection.Ascending));
                    break;
                case "PriceAsc":
                    ProductsView.SortDescriptions.Add(new SortDescription("Price",
                    ListSortDirection.Ascending));
                    break;
                case "PriceDesc":
                    ProductsView.SortDescriptions.Add(new SortDescription("Price",
                    ListSortDirection.Descending));
                    break;
                case "StockAsc":
                    ProductsView.SortDescriptions.Add(new SortDescription("Stock",
                    ListSortDirection.Ascending));
                    break;
                case "StockDesc":
                    ProductsView.SortDescriptions.Add(new SortDescription("Stock",
                    ListSortDirection.Descending));
                    break;
            }
            ProductsView.Refresh();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
            {
                ProductsView.Refresh();
            }

        private void ManagerBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
