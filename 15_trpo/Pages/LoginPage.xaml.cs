using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            MainWindow.setTitle("Авторизация");
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (PinBox.Password == "1234")
            {
                NavigationService.Navigate( new MainPage(true));
            }
            else
            {
                MessageBox.Show("Неверный PIN-код!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VisitorButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage(false));
        }
    }
}
