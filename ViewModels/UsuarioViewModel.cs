using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Orion.Models;
using Orion.Services;
using Orion.ViewModels;

namespace Orion.Controllers
{
    public class UsuarioViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<Usuarios> Usuarios { get; set; }
        public ICommand BuscarUsuario { get; set; }

        public UsuarioViewModel()
        {
            BuscarUsuario = new RelayCommand(GetUsuarios);
        }

        public void GetUsuarios(object obj)
        {
            
            var usuariosList = new Usuarios().GetUsuarios();
            Usuarios = new ObservableCollection<Usuarios>(usuariosList);
            NotifyPropertyChanged("Usuarios");
        }
        public void NotifyPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
