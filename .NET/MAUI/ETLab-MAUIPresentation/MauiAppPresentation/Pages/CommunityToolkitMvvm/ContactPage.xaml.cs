using MauiAppPresentation.ViewModels;

namespace MauiAppPresentation.Pages.CommunityToolkitMvvm;

public partial class ContactPage : ContentPage
{
	private readonly ContactViewModel contactViewModel;

	public ContactPage()
	{
		InitializeComponent();

		//contactViewModel = (ContactViewModel)BindingContext;
	}

    private void NameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
		if(string.IsNullOrWhiteSpace(e.NewTextValue))
		{
			contactViewModel.Age = null;
		}
    }
}