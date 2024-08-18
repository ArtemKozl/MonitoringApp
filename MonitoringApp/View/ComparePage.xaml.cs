using MonitoringApp.ViewModel;

namespace MonitoringApp.View;

public partial class ComparePage : ContentPage
{
    public ComparePage()
    {
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var viewmodel = new AllMonitoringValuesViewModel();
        await Navigation.PushAsync(new MainPage(viewmodel));
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        var viewmodel = new MonitoringNowViewModel();
        await Navigation.PushAsync(new MonitoringNowPage(viewmodel));
    }
}