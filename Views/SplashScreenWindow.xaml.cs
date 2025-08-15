using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Orion
{
    public partial class SplashScreenWindow : Window
    {
        public SplashScreenWindow()
        {
            InitializeComponent();
            Loaded += SplashScreen_Loaded;
        }

        private async void SplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarAplicacaoAsync();
        }

        private async Task CarregarAplicacaoAsync()
        {
            for( int i=0;i<=100; i++)
            {
                progressBar.Value = i;
                await Task.Delay(30);
            }

            LoginWindow loginWindow = new LoginWindow();
            object value = loginWindow.Show();
            this.Close();
        }
    }
}
