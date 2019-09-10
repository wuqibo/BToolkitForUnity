using System;
using System.Globalization;

namespace BToolkit
{
    public class DateUtils
    {
        public const string FormatDay = "yyyy-MM-dd";
        public const string FormatNormal = "yyyy-MM-dd HH:mm:ss";
        public const string FormatNoSpace = "yyyyMMddHHmmss";

        /// <summary>
        /// DateTime转字符串
        /// </summary>
        public static DateTime StringToDateTime(string time,string format= FormatNormal)
        {
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = format;
            return Convert.ToDateTime(time, dtFormat);
        }

        /// <summary>
        /// 字符串转DateTime
        /// </summary>
        public static string DateTimeToString(DateTime time, string format = FormatNormal)
        {
            return time.ToString(format);
        }

        /// <summary>
        /// 获取两个时间的相隔天数
        /// </summary>
        public static int GetSubDays(DateTime timeNew, DateTime timePast)
        {
            return timeNew.Subtract(timePast).Days;
        }
		
		/// <summary>
        /// 比较两个时间的大小
        /// </summary>
        public static bool CompareTime(DateTime bigerTime, DateTime smallerTime)
        {
            return bigerTime.Ticks - smallerTime.Ticks > 0;
        }
    }
}