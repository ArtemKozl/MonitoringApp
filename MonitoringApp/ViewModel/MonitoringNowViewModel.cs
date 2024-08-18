using CommunityToolkit.Mvvm.Input;
using MonitoringApp.Model;
using System.Collections.ObjectModel;
using MonitoringApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MonitoringApp.ViewModel
{
    public partial class MonitoringNowViewModel
    {
        MonitoringMethods monitoringMethods;

        public ObservableCollection<MonitoringActualValues> ActualValues { get; } = new();
        public MonitoringNowViewModel()
        {
            monitoringMethods = new MonitoringMethods();

        }
        [RelayCommand]
        public async Task StartMonitoring()
        {
            var values = await monitoringMethods.GetActualValues();
            foreach (var value in values)
            {
                ActualValues.Add(value);
            }
        }
    }
}
