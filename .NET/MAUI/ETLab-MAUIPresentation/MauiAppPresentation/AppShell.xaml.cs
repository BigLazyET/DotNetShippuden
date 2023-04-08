using MauiAppPresentation.Pages.CommunityToolkitMvvm;

namespace MauiAppPresentation;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		RegistRoutes();
	}

	private void RegistRoutes()
	{
		Routing.RegisterRoute("ctm/contact", typeof(ContactPage));
	}
}
