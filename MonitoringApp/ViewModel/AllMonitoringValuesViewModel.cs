using CommunityToolkit.Mvvm.Input;
using MonitoringApp.Model;
using System.Collections.ObjectModel;
using MonitoringApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;


namespace MonitoringApp.ViewModel
{
    public partial class AllMonitoringValuesViewModel : ObservableObject
    {
        MonitoringMethods monitoringMethods;

        [ObservableProperty]
        private string samplingInterval = string.Empty;
        [ObservableProperty]
        private string timeInterval = string.Empty;
        [ObservableProperty]
        private string date = string.Empty;
        [ObservableProperty]
        private string startHour = string.Empty;
        [ObservableProperty]
        private string endHour = string.Empty;

        [ObservableProperty]
        private string valueSelected = string.Empty;
        [ObservableProperty]
        private string valueType = string.Empty;
        [ObservableProperty]
        private string samplingCompareInterval = string.Empty;
        [ObservableProperty]
        private string timeCompareInterval = string.Empty;
        [ObservableProperty]
        private string dateFirstCompare = string.Empty;
        [ObservableProperty]
        private string dateSecondCompare = string.Empty;
        [ObservableProperty]
        private string startCompareHour = string.Empty;
        [ObservableProperty]
        private string endCompareHour = string.Empty;

        public ObservableCollection<MonitoringValues> Values { get; } = new(); 
        public ObservableCollection<CompareValues> CompareValues { get; } = new();
        public AllMonitoringValuesViewModel()
        {
            monitoringMethods = new MonitoringMethods();

            
        }




        [RelayCommand]
        public async Task LoadMonitoringValues()
        {
            try
            {

                Values.Clear();
                var values = await monitoringMethods.GetMonitoringValues(SamplingInterval,TimeInterval,Date,StartHour,EndHour);
                foreach (var value in values)
                {
                    Values.Add(value);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Че-то не так: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task LoadCompareValues()
        {
            try
            {
                CompareValues.Clear();
                var values = await monitoringMethods.GetCompareValues(SamplingCompareInterval, TimeCompareInterval, ValueSelected, ValueType, DateFirstCompare, DateSecondCompare, StartCompareHour, EndCompareHour);
                foreach (var value in values)
                {
                    CompareValues.Add(value);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Че-то не так: {ex.Message}", "OK");
            }
        }






    }

}
