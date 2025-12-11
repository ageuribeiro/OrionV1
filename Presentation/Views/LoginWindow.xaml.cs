using Orion.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        
        public LoginWindow()
        {
            InitializeComponent();

            // Inicialize os campos com os controles definidos no XAML.
            // Certifique-se de que os nomes dos controles no XAML sejam "txtEmailLogin" e "txtPassword".
            txtEmailLogin = (TextBox)FindName("txtEmailLogin");
            txtPassword = (PasswordBox)FindName("txtPassword");
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtEmailLogin.Text) &&
                !string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                // Verificando se os campos de login estão corretos
                if (txtEmailLogin.Text == "ageu87@gmail.com" && txtPassword.Password == "07051982")
                {
                    // Abre a janela de Dashboard
                    DashboardWindow dashboardWindow = new DashboardWindow();
                    dashboardWindow.Show();

                    // Fecha a janela atual de login
                    this.Close();
                }
                else
                {
                    // Mostra uma mensagem de erro
                    MessageBox.Show("Usuário ou senha incorretos.", "Error Email/Password", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Se os dados de login estiverem incorretos
            else
            {
                // Mostra uma mensagem de erro
                MessageBox.Show("Informação Inválida.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}