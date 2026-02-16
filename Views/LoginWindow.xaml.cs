using Orion.Views;
using System.Windows;

namespace Orion.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        public LoginWindow()
        {

            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtEmailLogin.Text) &&
               !string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                // Verificando se os campos de login estão corretos
                if (txtEmailLogin.Text == "ageu87@gmail.com" && txtPassword.Password == "07051982")
                {
                    // Abre a janela de Lista de usuários
                    //UserLisWindow userList = new UserLisWindow();
                    //userList.Show();

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