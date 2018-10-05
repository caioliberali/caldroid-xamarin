using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Caldroid.Xamarin.Com.Roomorama.Caldroid
{
    public class WeekdayArrayAdapter : ArrayAdapter<string>
    {
        private readonly LayoutInflater _layoutInflater;


        public WeekdayArrayAdapter(Context context, int resourceTextViewId, IList<string> objects, int resourceTheme)
            : base(context, resourceTextViewId, objects)
        {
            _layoutInflater = GetLayoutInflater(Context, resourceTheme);
        }


        public override bool AreAllItemsEnabled()
        {
            return false;
        }


        public override bool IsEnabled(int position)
        {
            return false;
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var customTextView = (TextView)_layoutInflater.Inflate(Resource.Layout.weekday_textview, null);
            var item = GetItem(position);

            customTextView.SetText(item, TextView.BufferType.Normal);

            return customTextView;
        }


        private LayoutInflater GetLayoutInflater(Context context, int resourceTheme)
        {
            var contextWrapper = new ContextThemeWrapper(context, resourceTheme);
            var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);

            return inflater.CloneInContext(contextWrapper);
        }
    }
}