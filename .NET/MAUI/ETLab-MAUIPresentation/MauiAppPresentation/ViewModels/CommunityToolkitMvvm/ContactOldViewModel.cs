using System.Windows.Input;

namespace MauiAppPresentation.ViewModels
{
    internal class ContactOldViewModel : BaseViewModel
    {
        private string? name;

        public string? Name
        {
            get => name; 
            set => SetProperty(ref name, value);
        }

        private ICommand? saveCommand;

        private ICommand SaveCommand => 
            saveCommand ??= new Command(Save);

        private void Save() 
        {
            // TODO Save...
        }
    }
}
