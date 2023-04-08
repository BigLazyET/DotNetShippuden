using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;

namespace MauiAppPresentation.ViewModels
{
    public partial class ContactValidatorViewModel : ObservableValidator
    {
        [ObservableProperty]
        private string? name = "Bar";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required]
        [Range(10,200)]
        //[CustomValidation(typeof(ContactValidatorViewModel),nameof(CustomValidateAge))]
        private int? age;

        public ContactValidatorViewModel()
        {
            Name = "Foo";
        }

        [RelayCommand]
        private void Save()
        {
            ValidateAllProperties();

            // TODO Save...
        }

        public static ValidationResult CustomValidateAge(int? age, ValidationContext context)
       {
            if (age != null && age.Value < 10)
                return new ValidationResult("The age can not less than 10");
            return ValidationResult.Success;
        }
    }
}
