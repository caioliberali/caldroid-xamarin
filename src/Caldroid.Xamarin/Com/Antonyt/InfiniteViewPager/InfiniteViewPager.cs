using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;

namespace Caldroid.Xamarin.Com.Antonyt.InfiniteViewPager
{
    /// <summary>
    /// A ViewPager that allows pseudo-infinite paging with a wrap-around
    /// effect. Should be used with an InfinitePagerAdapter.
    /// </summary>
    public class InfiniteViewPager : ViewPager
    {
        public static readonly int OFFSET = 1000;
        public static readonly int MAX_WEEK_PER_MONTH = 6;

        private bool _isMaxWeekPerMonth;
        private int _rowHeight;

        public bool IsMaxWeekPerMonth
        {
            get { return _isMaxWeekPerMonth; }
            set
            {
                _isMaxWeekPerMonth = value;
                _rowHeight = 0;
            }
        }

        public override PagerAdapter Adapter
        {
            get { return base.Adapter; }
            set
            {
                base.Adapter = value;
                SetCurrentItem(OFFSET, true);
            }
        }

        public bool IsEnabled { get; set; } = true;
        public List<DateTime> DaysInMonth { get; set; }


        public InfiniteViewPager(Context context)
            : base(context)
        {

        }


        public InfiniteViewPager(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {

        }


        public override bool OnTouchEvent(MotionEvent e)
        {
            if (IsEnabled)
                return base.OnTouchEvent(e);

            return false;
        }


        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (IsEnabled)
                return base.OnInterceptTouchEvent(ev);

            return false;
        }


        /// <summary>
        /// ViewPager does not respect "wrap_content". The code below tries to
        /// measure the height of the child and set the height of viewpager based on
        /// child height.
        /// 
        /// It was customized from
        /// http://stackoverflow.com/questions/9313554/measuring-a-viewpager
        /// 
        /// Thanks Delyan for his brilliant code.
        /// </summary>
        /// <param name="widthMeasureSpec">Width measure spec.</param>
        /// <param name="heightMeasureSpec">Height measure spec.</param>
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            if (ChildCount > 0 && _rowHeight == 0)
            {
                widthMeasureSpec = MeasureSpec.MakeMeasureSpec(MeasuredWidth, MeasureSpecMode.Exactly);

                var childFirst = GetChildAt(0);
                childFirst.Measure(widthMeasureSpec, MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));

                _rowHeight = childFirst.MeasuredHeight;
            }

            var rows = DaysInMonth.Count / 7;
            var calendarHeight = _isMaxWeekPerMonth ? _rowHeight * MAX_WEEK_PER_MONTH : _rowHeight * rows;
            var verticalScrollOffset = 12;

            calendarHeight = calendarHeight - verticalScrollOffset;

            heightMeasureSpec = MeasureSpec.MakeMeasureSpec(calendarHeight, MeasureSpecMode.Exactly);

            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }
    }
}
