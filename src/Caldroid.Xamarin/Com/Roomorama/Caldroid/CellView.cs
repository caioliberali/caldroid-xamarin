using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace Caldroid.Xamarin.Com.Roomorama.Caldroid
{
    /// <summary>
    /// Cell view.
    /// 
    /// @author crocodile2u
    /// </summary>
    public class CellView : TextView
    {
        public static readonly int STATE_TODAY = Resource.Attribute.state_date_today;
        public static readonly int STATE_SELECTED = Resource.Attribute.state_date_selected;
        public static readonly int STATE_DISABLED = Resource.Attribute.state_date_disabled;
        public static readonly int STATE_PREV_NEXT_MONTH = Resource.Attribute.state_date_prev_next_month;

        private List<int> _customStates = new List<int>();


        public CellView(Context context) 
            : base(context)
        {

        }


        public CellView(Context context, IAttributeSet attrs) 
            : base(context, attrs)
        {

        }


        public CellView(Context context, IAttributeSet attrs, int defStyleAttr) 
            : base(context, attrs, defStyleAttr)
        {

        }

        protected CellView(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer)
        {

        }


        protected override int[] OnCreateDrawableState(int extraSpace)
        {
            var customStateTotal = _customStates.Count;

            if (customStateTotal > 0)
            {
                int[] drawableState = base.OnCreateDrawableState(extraSpace + customStateTotal);
                int[] stateArray = _customStates.ToArray();

                MergeDrawableStates(drawableState, stateArray);

                return drawableState;
            }

            return base.OnCreateDrawableState(extraSpace);
        }


        public void AddCustomState(int resourceState)
        {
            if (!_customStates.Contains(resourceState))
                _customStates.Add(resourceState);
        }


        public void ClearCustomStates()
        {
            _customStates.Clear();
        }
    }
}