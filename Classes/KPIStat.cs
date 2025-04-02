using Ical.Net.CalendarComponents;

namespace MyBot.Classes;

public class KPIStat
{
    private readonly string _pathOutputFile = @"C:\Users\User\source\repos\fithruk\MyBot\TempDB\output.txt";
    private List<CalendarEvent> _events;
    private Dictionary<string, HashSet<string>> _monthClientsInArrays = new();
    private HashSet<string> _oldClients = new ();
    public KPIStat(List<CalendarEvent> events)
    {
        this._events = events.Where(e => e.Start.Date < DateOnly.FromDateTime(DateTime.Now)).ToList();
    }

    private void _sortOutClientsInArrays()
    {
        HashSet<string> current = new HashSet<string>();
        string currentKey = null;

        for (int i = 0; i < this._events.Count; i++)
        {
            var currentEvent = this._events[i];
            string name = string.Join(' ', currentEvent.Summary.Split(' ').OrderBy(s => s)).Trim();
            if(name.Contains("Событие") || name.Contains("Разовое") || name.Contains("Тест") || name.Contains("Пробное")) 
                continue;
            
            string key = $"{currentEvent.Start.Date.Month}.{currentEvent.Start.Date.Year}";
            
            if (currentKey != null && key != currentKey)
            {
                this._monthClientsInArrays.TryAdd(currentKey, new HashSet<string>(current));
                current.Clear();
            }
        
            currentKey = key;
            current.Add(name);
        }
       
        if (currentKey != null && current.Count > 0)
        {
            this._monthClientsInArrays.TryAdd(currentKey, new HashSet<string>(current));
        }
    }

    public string? GetKPIMonthByMonth()
{
    int proceeded = 0;
    int leaved = 0;
    int newClients = 0;

    try
    {
        this._sortOutClientsInArrays();
        
        using (StreamWriter sw = new StreamWriter(_pathOutputFile, false)) 
        {
            for (int i = 0; i < this._monthClientsInArrays.Count; i++)
            {
                var current = this._monthClientsInArrays.ElementAt(i);
                var next = this._monthClientsInArrays.ElementAtOrDefault(i + 1);
                sw.WriteLine($"{current.Key} - {next.Key}");
                for (int j = 0; j < current.Value.Count; j++)
                {
                    var client = current.Value.ElementAt(j);

                    if (next.Value is not null && next.Value.Contains(client) &&
                        this._oldClients.Contains(client))
                    {
                        sw.WriteLine(client + " Proceeded");
                        proceeded++;
                    }

                    if (next.Value is not null && !next.Value.Contains(client) &&
                        this._oldClients.Contains(client))
                    {
                        sw.WriteLine(client + " Leaved");
                        leaved++;
                    }

                    if (!this._oldClients.Contains(client))
                    {
                        sw.WriteLine(client + " New");
                        this._oldClients.Add(client);
                        newClients++;
                    }
                }

                double extendedPercent = current.Value.Count > 0 ? (proceeded / (double)current.Value.Count) * 100 : 0;
                double leavedPercent = current.Value.Count > 0 ? (leaved / (double)current.Value.Count) * 100 : 0;
                double newPercent = current.Value.Count > 0 ? (newClients / (double)current.Value.Count) * 100 : 0;
                
               
                sw.WriteLine($"{proceeded} - proceeded {leaved} - leaved: {newClients} - new ");
                sw.WriteLine($"Extended percent: {extendedPercent:F2}%");
                sw.WriteLine($"Leaved percent: {leavedPercent:F2}%");
                sw.WriteLine($"New percent: {newPercent:F2}%");
                sw.WriteLine();

                proceeded = 0;
                leaved = 0;
                newClients = 0;
            }
            return this._pathOutputFile;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"Ошибка при записи в файл: {e}");
    }

    return null;
}
    
}