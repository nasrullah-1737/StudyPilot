namespace StudyPilot.Helpers;

public static class DateHelper
{
    public static int ToAppDayOfWeek(DateTime dateTime)
    {
        var day = (int)dateTime.DayOfWeek;
        return day == 0 ? 7 : day;
    }
}
