
using MonitoringApp.ViewModel;



namespace MonitoringApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage(AllMonitoringValuesViewModel viewModel)
        {

            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var viewmodel = new MonitoringNowViewModel();
            await Navigation.PushAsync(new MonitoringNowPage(viewmodel));
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ComparePage());
        }
        
    }
}
