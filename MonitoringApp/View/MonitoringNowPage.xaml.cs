using MonitoringApp.ViewModel;

namespace MonitoringApp;

public partial class MonitoringNowPage : ContentPage
{
	public MonitoringNowPage(MonitoringNowViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

    }



    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        var viewmodel = new AllMonitoringValuesViewModel();
        await Navigation.PushAsync(new MainPage(viewmodel));
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ComparePage());
    }
}