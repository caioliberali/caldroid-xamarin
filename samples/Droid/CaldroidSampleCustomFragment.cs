using Android.App;
using Caldroid.Xamarin.Com.Roomorama.Caldroid;

namespace CaldroidSamples.Droid
{
    [Activity(Label = "CaldroidSampleCustomFragment")]
    public class CaldroidSampleCustomFragment : CaldroidFragment
    {
        protected override CaldroidGridAdapter CreateCalendarGridAdapter(int year, int month)
        {
            return new CaldroidSampleCustomAdapter(Activity, year, month, DataFromCalendar, DataFromClient);
        }
    }
}
