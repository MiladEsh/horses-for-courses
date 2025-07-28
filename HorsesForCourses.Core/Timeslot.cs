namespace HorsesForCourses.Core;

public class Timeslot
{
    public Weekday Day { get; }
    public TimeOnly Start { get; }
    public TimeOnly End { get; }

    public Timeslot(Weekday day, TimeOnly start, TimeOnly end)
    {
        if (start >= end)
            throw new ArgumentException("Start time must be before end time");

        if (start < new TimeOnly(9, 0))
            throw new ArgumentException("Timeslot cannot start before 09:00");

        if (end > new TimeOnly(17, 0))
            throw new ArgumentException("Timeslot cannot end after 17:00");

        Day = day;
        Start = start;
        End = end;
    }

    public bool OverlapsWith(Timeslot other)
    {
        if (Day != other.Day)
            return false;

        return Start < other.End && other.Start < End;
    }

    public TimeSpan Duration => End - Start;
}