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
using Orion.Controllers;
using Orion.ViewModels;

namespace Orion.Views
{
    /// <summary>
    /// Interaction logic for UsuarioBuscar.xaml
    /// </summary>
    public partial class UsuarioBuscar : Page
    {
        public UsuarioBuscar()
        {
            InitializeComponent();
            DataContext = new UsuarioViewModel();
        }
    }
}
