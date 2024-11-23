using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class TimeUtility
{

    public static DateTime GetStartOfweek(this DateTime dateTime)
    {
        var todayMidnight = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        var dow = (int)dateTime.DayOfWeek;
        var dayOfWeek = (dow + 1) % 7 + 1;
        return todayMidnight.AddDays(1 - dayOfWeek);
    }
    public static DateTime GetEndOfweek(this DateTime dateTime)
    {
        var todayMidnight = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        var dow = (int)dateTime.DayOfWeek;
        var dayOfWeek = (dow+1) % 7 + 1;
        return todayMidnight.AddDays(8 - dayOfWeek).AddHours(-1);
    }

    public static string ToPersianDate(this DateTime dateTime)
    {
        if (dateTime < new DateTime(1970, 1, 1))
            return "NA";

        PersianCalendar pc = new PersianCalendar();
        var year = pc.GetYear(dateTime);
        var month = pc.GetMonth(dateTime);
        var day = pc.GetDayOfMonth(dateTime);

        return year + "-" + month + "-" + day;
    }
}
