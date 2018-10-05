using System;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using static Android.Widget.AdapterView;

namespace Caldroid.Xamarin.Com.Roomorama.Caldroid
{
    /// <summary>
    /// DateGridFragment contains only 1 gridview with 7 columns to display all the
    /// dates within a month.
    /// 
    /// Client must supply gridAdapter and onItemClickListener before the fragment is
    /// attached to avoid complex crash due to fragment life cycles.
    /// 
    /// @author thomasdao
    /// </summary>
    public class DateGridFragment : Fragment
    {
        private int _resourceGridView;
        private int _resourceTheme;

        public GridView DateGridView { get; private set; }
        public CaldroidGridAdapter CalendarGridAdapter { get; set; }
        public EventHandler<ItemClickEventArgs> OnItemClickEvent { get; set; }
        public EventHandler<ItemLongClickEventArgs> OnItemLongClickEvent { get; set; }


        public int GridViewResource
        {
            set { _resourceGridView = value; }
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (_resourceGridView == 0)
                _resourceGridView = Resource.Layout.date_grid_fragment;

            if (_resourceTheme == 0)
            {
                if (CalendarGridAdapter != null)
                    _resourceTheme = CalendarGridAdapter.ThemeResource;
            }

            if (DateGridView == null)
            {
                var localInflater = CaldroidFragment.ThemeInflater(Activity, inflater, _resourceTheme);

                DateGridView = (GridView)localInflater.Inflate(_resourceGridView, container, false);

                SetupGridView();
            }
            else
            {
                var parent = (ViewGroup)DateGridView.Parent;

                if (parent != null)
                    parent.RemoveView(DateGridView);
            }

            return DateGridView;
        }


        private void SetupGridView()
        {
            if (CalendarGridAdapter != null)
                DateGridView.Adapter = CalendarGridAdapter;

            if (OnItemClickEvent != null)
                DateGridView.ItemClick += OnItemClickEvent;

            if (OnItemLongClickEvent != null)
                DateGridView.ItemLongClick += OnItemLongClickEvent;
        }
    }
}