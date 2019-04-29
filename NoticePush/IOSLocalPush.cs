using UnityEngine;

namespace BToolkit
{
    //本地推送
    public class IOSLocalPush
    {

        //APP从后台回来时调用
        public static void CleanNotification()
        {
#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
		UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification (); 
		notification.applicationIconBadgeNumber = -1; 
		UnityEngine.iOS.NotificationServices.PresentLocalNotificationNow (notification); 
		UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications (); 
		UnityEngine.iOS.NotificationServices.ClearLocalNotifications (); 
#endif
        }

        //APP退到后台时调用
        public static void SetNotification(int iconBadgeNum, string msg, System.DateTime newDate, bool isRepeatDay)
        {
#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
		UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification();
		notification.fireDate = newDate;	
		notification.alertBody = msg;
		notification.applicationIconBadgeNumber = iconBadgeNum;
		notification.hasAction = true;
		if(isRepeatDay){
			//是否每天定期循环
			notification.repeatCalendar = UnityEngine.iOS.CalendarIdentifier.ChineseCalendar;
			notification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
		}
		notification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notification);
#endif
        }
    }
}