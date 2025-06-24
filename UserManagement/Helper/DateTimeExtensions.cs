namespace UserManagement.Helper;

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek)
    {
        int diff = date.DayOfWeek - startOfWeek;

        if (diff < 0)
        {
            diff += 7;
        }

        return date.AddDays(-diff).Date; 
    }
}
