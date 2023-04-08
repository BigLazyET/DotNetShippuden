using MauiAppPresentation.Pages.CommunityToolkitMvvm;

namespace MauiAppPresentation;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count+=11;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

    private async void OnCTMClicked(object sender, EventArgs e)
    {
		//await Navigation.PushAsync(new ContactPage(), true);
		await Shell.Current.GoToAsync("ctm/contact");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

		if( DeviceInfo.Current.Platform == DevicePlatform.Android)
			platformLbl.Text = "I am on Android";
		else if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            platformLbl.Text = "I am on iOS";
        else if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            platformLbl.Text = "I am on Windows";
    }
}

