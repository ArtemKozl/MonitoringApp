using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Devices.Sensors;
using MonitoringApp.Model;
using Npgsql;
using System.Diagnostics;
using System.Globalization;



namespace MonitoringApp.Services
{
    public class MonitoringMethods
    {
        public MonitoringMethods() { }

        string connectionString = "****";

        List<MonitoringValues> Data = new();
        List<CompareValues> CompareData = new();
        List<MonitoringActualValues> ActualData = new();

        public async Task<List<MonitoringActualValues>> GetActualValues()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var command = new NpgsqlCommand("SELECT\r\n    \"timestamp\",\r\n" +
                        "    (\"data\"->>'0x0')::integer * 0.1 AS \"temp\"\r\nFROM monitoring m" +
                        " \r\nWHERE (\"data\"->>'0x0')::integer <> 0  \r\n " +
                        "   AND \"timestamp\" >= NOW() - INTERVAL '3 hours' - INTERVAL '1 second'\r\n" +
                        "    AND \"timestamp\" <= NOW() - INTERVAL '3 hours';", conn))
                    using (var reader = await command.ExecuteReaderAsync())
                    {

                        Data.Clear();
                        while (await reader.ReadAsync())
                        {
                            var values = new MonitoringActualValues()
                            {
                                TimeStamp = reader.GetDateTime(reader.GetOrdinal("timestamp")).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                Temp = reader.GetDouble(reader.GetOrdinal("temp")),
                            };
                            ActualData.Add(values);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Че-то не так: {ex.Message}", "OK");
            }
            return ActualData;
        }
        public async Task<List<MonitoringValues>> GetMonitoringValues(string samplingInterval, string timeInterval, string date, string startHour, string endHour)
        {
            DateTime dateForCommand = DateTime.Parse(date);

            int day = dateForCommand.Day;
            int month = dateForCommand.Month;
            int year = dateForCommand.Year;

            string whereCommand = string.Empty;
            string commandSQL = string.Empty;
   
            if (timeInterval == "Месяц")
            {
                commandSQL = $"SELECT time_bucket('{samplingInterval}', \"timestamp\") AS five_min," +
            "\r\nround((avg(\"0x0\")*0.1),1) as \"temp\"," +
            "\r\n round(avg(\"0x2\"),1) as \"light\"," +
            "\r\n  round((avg(\"0xb\")),1) as \"air\"," +
            "\r\n  max(\"0x0\")*0.1 as \"max_temp\"," +
            "\r\n  min(\"0x0\")*0.1 as \"min_temp\"," +
            "\r\n max(\"0x2\") as \"max_light\"," +
            "\r\n min(\"0x2\") as \"min_light\"," +
            "\r\n  max(\"0xb\") as \"max_air\"," +
            "\r\n  min(\"0xb\") as \"min_air\"\r\n" +
            "FROM view_monitoring vm" +
            $" \r\nWHERE EXTRACT(month  FROM \"timestamp\") = {month} and EXTRACT(year FROM \"timestamp\") = {year}\r\n" +
            "GROUP BY five_min\r\n" +
            "ORDER BY five_min;";
            }
            else
            {
                if (timeInterval == "Час(ы)")
                {
                    DateTime dateTime = DateTime.Parse(date); 
                    string formattedDate = dateTime.ToString("MM/dd/yyyy");
                    whereCommand = $"WHERE \"timestamp\" BETWEEN '{formattedDate} {startHour}' AND '{formattedDate} {endHour}'";
                }
                else if (timeInterval == "День")
                    whereCommand = $"WHERE EXTRACT(day FROM \"timestamp\") = {day} AND EXTRACT(month FROM \"timestamp\") = {month} and EXTRACT(year FROM \"timestamp\") = {year}";
                else if (timeInterval == "Неделя")
                    whereCommand = $"WHERE \"timestamp\" BETWEEN TO_DATE('{date}', 'DD.MM.YYYY') AND (TO_DATE('{date}', 'DD.MM.YYYY') + INTERVAL '7 days')";
                commandSQL = $"SELECT \r\n    time_bucket('{samplingInterval}', \"timestamp\") AS five_min," +
                        "\r\n    round((avg((\"data\"->>'0x0')::integer) * 0.1),2) AS \"temp\"," +
                        "\r\n    round(avg((\"data\"->>'0x2')::integer),2) AS \"light\"," +
                        "\r\n    round(avg((\"data\"->>'0xb')::integer),2) AS \"air\"," +
                        "\r\n      max((\"data\"->>'0x0')::integer)*0.1 as \"max_temp\"," +
                        "\r\n  min((\"data\"->>'0x0')::integer)*0.1 as \"min_temp\"," +
                        "\r\n max((\"data\"->>'0x2')::integer) as \"max_light\"," +
                        "\r\n min((\"data\"->>'0x2')::integer) as \"min_light\"," +
                        "\r\n  max((\"data\"->>'0xb')::integer) as \"max_air\"," +
                        "\r\n  min((\"data\"->>'0xb')::integer) as \"min_air\"\r\nFROM monitoring m " +
                        $"{whereCommand}" +
                        " GROUP BY five_min\r\n" +
                        "ORDER BY five_min;";
                
            }

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var command = new NpgsqlCommand($"{commandSQL}", conn))
                    using (var reader = await command.ExecuteReaderAsync())
                    {

                        Data.Clear();
                        while (await reader.ReadAsync())
                        {
                            var values = new MonitoringValues()
                            {
                                TimeStamp = reader.GetDateTime(reader.GetOrdinal("five_min")).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                Temp = reader.GetDouble(reader.GetOrdinal("temp")),
                                Light = reader.GetDouble(reader.GetOrdinal("light")),
                                Air = reader.GetDouble(reader.GetOrdinal("air")),
                                MaxTemp = reader.GetDouble(reader.GetOrdinal("max_temp")),
                                MinTemp = reader.GetDouble(reader.GetOrdinal("min_temp")),
                                MaxLight = reader.GetDouble(reader.GetOrdinal("max_light")),
                                MinLight = reader.GetDouble(reader.GetOrdinal("min_light")),
                                MaxAir = reader.GetDouble(reader.GetOrdinal("max_air")),
                                MinAir = reader.GetDouble(reader.GetOrdinal("min_air"))
                            };
                            Data.Add(values);
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Че-то не так: {ex.Message}", "OK");
            }
            return Data;
        }

        public async Task<List<CompareValues>> GetCompareValues(string samplingCompareInterval, string timeCompareInterval, string value, string valueType, string dateFirstCompare, string dateSecondCompare, string startCompareHour, string endCompareHour)
        {
            DateTime dateFirstForCommand = DateTime.Parse(dateFirstCompare);

            int dayFirst = dateFirstForCommand.Day;
            int monthFirst = dateFirstForCommand.Month;
            int yearFirst = dateFirstForCommand.Year;

            DateTime dateSecondForCommand = DateTime.Parse(dateSecondCompare);

            int daySecond = dateSecondForCommand.Day;
            int monthSecond = dateSecondForCommand.Month;
            int yearSecond = dateSecondForCommand.Year;

            string valueTypeForCommand = string.Empty;
            string commandFirstValue = string.Empty;
            string commandSecondValue = string.Empty;
            string whereCommandFirst = string.Empty;
            string whereCommandSecond = string.Empty;

            if (value == "Температура")
                value = "0x0";
            else if (value == "Освещенность")
                value = "0x2";
            else if (value == "Загрязненность воздуха")
                value = "0xb";
            
            if (valueType == "Среднее значение")
            {
                if (timeCompareInterval == "Месяц")
                {
                    if (value == "0x0")
                        valueTypeForCommand = "round((avg(\"0x0\")*0.1),1)";
                    else
                        valueTypeForCommand = $"round(avg(\"{value}\"),1)";
                }
                else
                {
                    if (value == "0x0")
                        valueTypeForCommand = "round((avg((\"data\"->>'0x0')::integer) * 0.1),2)";
                    else
                        valueTypeForCommand = $"round(avg((\"{value}\"->>'0x2')::integer),2)";
                }
                
            }
            else if (valueType == "Максимальное значение")
            {
                if(timeCompareInterval == "Месяц")
                {
                    if (value == "0x0")
                        valueTypeForCommand = "max(\"0x0\")*0.1";
                    else
                        valueTypeForCommand = $"max(\"{value}\")";
                }
                else
                {
                    if (value == "0x0")
                        valueTypeForCommand = "max((\"data\"->>'0x0')::integer)*0.1";
                    else
                        valueTypeForCommand = $" max((\"{value}\"->>'0x2')::integer)";
                }
            }
            else if (valueType == "Минимальное значение")
            {
                if (timeCompareInterval == "Месяц")
                {
                    if (value == "0x0")
                        valueTypeForCommand = "min(\"0x0\")*0.1";
                    else
                        valueTypeForCommand = $"min(\"{value}\")";
                }
                else
                {
                    if (value == "0x0")
                        valueTypeForCommand = "min((\"data\"->>'0x0')::integer)*0.1";
                    else
                        valueTypeForCommand = $"min((\"data\"->>'{value}')::integer)";
                }
                
            }

            if (timeCompareInterval == "Месяц")
            {
                commandFirstValue = $"SELECT time_bucket('{samplingCompareInterval}', \"timestamp\") AS five_min," +
            $"\r\n{valueTypeForCommand} as \"firstValue\"" +
            "FROM view_monitoring vm" +
            $" \r\nWHERE EXTRACT(month  FROM \"timestamp\") = {monthFirst} and EXTRACT(year FROM \"timestamp\") = {yearFirst}\r\n" +
            "GROUP BY five_min\r\n" +
            "ORDER BY five_min;";
                commandSecondValue = $"SELECT time_bucket('{samplingCompareInterval}', \"timestamp\") AS five_min," +
            $"\r\n{valueTypeForCommand} as \"secondValue\"" +
            "FROM view_monitoring vm" +
            $" \r\nWHERE EXTRACT(month  FROM \"timestamp\") = {monthSecond} and EXTRACT(year FROM \"timestamp\") = {yearSecond}\r\n" +
            "GROUP BY five_min\r\n" +
            "ORDER BY five_min;";
            }
            else
            {
                if (timeCompareInterval == "Час(ы)")
                {
                    DateTime dateTimefirst = DateTime.Parse(dateFirstCompare);
                    string formattedDateFirst = dateTimefirst.ToString("MM/dd/yyyy");

                    DateTime dateTimeSecond = DateTime.Parse(dateSecondCompare);
                    string formattedDateSecond = dateTimeSecond.ToString("MM/dd/yyyy");

                    whereCommandFirst = $"WHERE \"timestamp\" BETWEEN '{formattedDateFirst} {startCompareHour}' AND '{formattedDateFirst} {endCompareHour}'";
                    whereCommandSecond = $"WHERE \"timestamp\" BETWEEN '{formattedDateSecond} {startCompareHour}' AND '{formattedDateSecond} {endCompareHour}'";
                }
                else if (timeCompareInterval == "День")
                {
                    whereCommandFirst = $"WHERE EXTRACT(day FROM \"timestamp\") = {dayFirst} AND EXTRACT(month FROM \"timestamp\") = {monthFirst} and EXTRACT(year FROM \"timestamp\") = {yearFirst}";
                    whereCommandSecond = $"WHERE EXTRACT(day FROM \"timestamp\") = {daySecond} AND EXTRACT(month FROM \"timestamp\") = {monthSecond} and EXTRACT(year FROM \"timestamp\") = {yearSecond}";
                }   
                else if (timeCompareInterval == "Неделя")
                {
                    whereCommandFirst = $"WHERE \"timestamp\" BETWEEN TO_DATE('{dateFirstCompare}', 'DD.MM.YYYY') AND (TO_DATE('{dateFirstCompare}', 'DD.MM.YYYY') + INTERVAL '7 days')";
                    whereCommandSecond = $"WHERE \"timestamp\" BETWEEN TO_DATE('{dateSecondCompare}', 'DD.MM.YYYY') AND (TO_DATE('{dateSecondCompare}', 'DD.MM.YYYY') + INTERVAL '7 days')";
                }

                    commandFirstValue = $"SELECT \r\n    time_bucket('{samplingCompareInterval}', \"timestamp\") AS five_min," +
                $"\r\n    round(avg((\"data\"->>'{value}')::integer),2) AS \"firstValue\"" +
                $"FROM monitoring m \r\n" +
                $"{whereCommandFirst}" +
                " GROUP BY five_min\r\n" +
                "ORDER BY five_min;";

                commandSecondValue = $"SELECT \r\n    time_bucket('{samplingCompareInterval}', \"timestamp\") AS five_min," +
                    $"\r\n    round(avg((\"data\"->>'{value}')::integer),2) AS \"secondValue\"\r\n" +
                    $"FROM monitoring m \r\n" +
                    $"{whereCommandSecond}" +
                    " GROUP BY five_min\r\n" +
                    "ORDER BY five_min;";

            }

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (var firstCommand = new NpgsqlCommand(commandFirstValue, conn))
                    using (var reader1 = await firstCommand.ExecuteReaderAsync())
                    {
                        CompareData.Clear();
                        while (await reader1.ReadAsync())
                        {
                            var values = new CompareValues()
                            {
                                TimeStamp = reader1.GetDateTime(reader1.GetOrdinal("five_min")).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                FirstValue = reader1.GetDouble(reader1.GetOrdinal("firstValue")),
                            };
                            CompareData.Add(values);
                        }
                    }
                    using (var secondCommand = new NpgsqlCommand(commandSecondValue, conn)) 
                    using (var reader = await secondCommand.ExecuteReaderAsync())
                    {
                        foreach (var item in CompareData) 
                        {
                            if (await reader.ReadAsync())
                            {
                                item.TimeStamp += " - "+reader.GetDateTime(reader.GetOrdinal("five_min")).ToString("yyyy-MM-dd HH:mm:ss.fff");
                                item.SecondValue = reader.GetDouble(reader.GetOrdinal("secondValue"));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Че-то не так: {ex.Message}", "OK");
            }

            return CompareData;
        }


    }
}
