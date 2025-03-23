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

    public void PrintEvents()
    {
        this.GetEvents().ForEach(e => Console.WriteLine(e.Start.Date.ToShortDateString()));
    }
}