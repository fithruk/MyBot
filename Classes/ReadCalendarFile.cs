using Ical.Net;
using Ical.Net.CalendarComponents;
using System.Dynamic;
using Ical.Net.DataTypes;

namespace MyBot.Classes;

public class ReadCalendarFile
{
    private string icsContent;
    private Calendar calendar;
    public ReadCalendarFile(string path)
    {
        this.icsContent = File.ReadAllText(path);
        this.calendar = Calendar.Load(this.icsContent);
    }

    public List<CalendarEvent> GetEvents()
    {
        return this.calendar.Events.OrderBy(e => e.Start.Date).ToList();
    }
    
    public Dictionary<string, List<DateTime>> LoadUsers()
    {
        var events = this.GetEvents();
        Dictionary<string, List<DateTime>> dates = new Dictionary<string, List<DateTime>>();
        Console.WriteLine(events.Count);
        for (int i = 0; i < events.Count; i++)
        {
            List<string> splitedName = events[i].Summary.Split(' ').ToList();
            if (splitedName[0] == "Событие") continue;
            if (splitedName[0] == "Разовое") continue;
            if (splitedName.Count < 2) continue;
            
            string fullName = string.Join(" ", splitedName.OrderBy(s => s)).Trim();

            if (!dates.ContainsKey(fullName))
            {
                dates.Add(fullName, new List<DateTime>(){new DateTime(events[i].Start.Date, events[i].Start.Time.Value)});
            }
            else
            {
                dates[fullName].Add(new DateTime(events[i].Start.Date, events[i].Start.Time.Value));
            }
            
        }
        return dates;
    }
}