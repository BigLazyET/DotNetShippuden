using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace MauiAppPresentation.ViewModels
{
    //[ObservableObject]
    [INotifyPropertyChanged]
    internal partial class ContactViewModel //: ObservableObject
    {
        [ObservableProperty]
        private string? name = "Bar";

        [ObservableProperty]
        private int? age;

        public ContactViewModel()
        {
            Name = "Foo";
        }

        [RelayCommand]
        private void Save()
        {
            // TODO Save...
        }
    }
}
