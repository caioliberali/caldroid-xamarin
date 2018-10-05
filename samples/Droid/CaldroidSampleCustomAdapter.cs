using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Caldroid.Xamarin.Com.Roomorama.Caldroid;

namespace CaldroidSamples.Droid
{
    [Activity(Label = "CaldroidSampleCustomAdapter")]
    public class CaldroidSampleCustomAdapter : CaldroidGridAdapter
    {
        public CaldroidSampleCustomAdapter(Context context, int month, int year,
                                           Dictionary<string, object> dataFromCalendar,
                                           Dictionary<string, object> dateFromClient)
            : base(context, year, month, dataFromCalendar, dateFromClient)
        {

        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
            var cellView = convertView;

            // For reuse
            if (convertView == null)
            {
                cellView = inflater.Inflate(Resource.Layout.CustomCell, null);
            }

            var paddingTop = cellView.PaddingTop;
            var paddingLeft = cellView.PaddingLeft;
            var paddingBottom = cellView.PaddingBottom;
            var paddingRight = cellView.PaddingRight;

            TextView tv1 = cellView.FindViewById<TextView>(Resource.Id.tv1);
            TextView tv2 = cellView.FindViewById<TextView>(Resource.Id.tv2);

            tv1.SetTextColor(Color.Black);

            // Get dateTime of this cell
            var dateTime = DateTimes[position];
            var resources = _context.Resources;

            // Set color of the dates in previous / next month
            if (dateTime.Month != _month)
            {
                var resource = Caldroid.Xamarin.Resource.Color.caldroid_darker_gray;
                var color = new Color(ContextCompat.GetColor(_context, resource));

                tv1.SetTextColor(color);
            }

            var shouldResetDiabledView = false;
            var shouldResetSelectedView = false;

            // Customize for disabled dates and date outside min/max dates
            if ((MinDate != null && dateTime.Date < MinDate.Value.Date) ||
                (MaxDate != null && dateTime.Date > MaxDate.Value.Date) ||
                (DisabledDates != null && DisabledDates.Contains(dateTime)))
            {

                tv1.SetTextColor(CaldroidFragment.DisabledTextColor);

                if (CaldroidFragment.DisabledBackgroundDrawable == -1)
                {
                    cellView.SetBackgroundResource(Caldroid.Xamarin.Resource.Drawable.disable_cell);
                }
                else
                {
                    cellView.SetBackgroundResource(CaldroidFragment.DisabledBackgroundDrawable);
                }

                if (dateTime.Date == DateTime.Now.Date)
                {
                    cellView.SetBackgroundResource(Caldroid.Xamarin.Resource.Drawable.red_border_gray_bg);
                }

            }
            else
            {
                shouldResetDiabledView = true;
            }

            // Customize for selected dates
            if (SelectedDates != null && SelectedDates.Contains(dateTime))
            {
                var resource = Caldroid.Xamarin.Resource.Color.caldroid_sky_blue;
                var color = new Color(ContextCompat.GetColor(_context, resource));

                cellView.SetBackgroundColor(color);

                tv1.SetTextColor(Color.Black);

            }
            else
            {
                shouldResetSelectedView = true;
            }

            if (shouldResetDiabledView && shouldResetSelectedView)
            {
                // Customize for today
                if (dateTime.Date == DateTime.Now.Date)
                {
                    cellView.SetBackgroundResource(Caldroid.Xamarin.Resource.Drawable.red_border);
                }
                else
                {
                    cellView.SetBackgroundResource(Caldroid.Xamarin.Resource.Drawable.cell_bg);
                }
            }

            tv1.Text = "" + dateTime.ToString("dd");
            tv2.Text = "Hi";

            // Somehow after setBackgroundResource, the padding collapse.
            // This is to recover the padding
            cellView.SetPadding(paddingLeft, paddingTop, paddingRight, paddingBottom);

            // Set custom color if required
            SetCustomResources(dateTime, cellView, tv1);

            return cellView;
        }
    }
}