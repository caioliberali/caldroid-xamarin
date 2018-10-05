using System.Collections.Generic;
using Android.Support.V4.App;

namespace Caldroid.Xamarin.Com.Roomorama.Caldroid
{
    /// <summary>
    /// MonthPagerAdapter holds 4 fragments, which provides fragment for current
    /// month, previous month and next month. The extra fragment helps for recycle
    /// fragments.
    /// 
    /// @author thomasdao
    /// </summary>
    public class MonthPagerAdapter : FragmentPagerAdapter
    {
        private List<DateGridFragment> _dateGridFragmentList;


        public List<DateGridFragment> DateGridFragmentList
        {
            get { return LoadOrCreateDateFragmentList(); }
            set { _dateGridFragmentList = value; }
        }


        /// <summary>
        /// We need 4 gridviews for previous month, current month and next month,
        /// and 1 extra fragment for fragment recycle.
        /// </summary>
        /// <value>The count.</value>
        public override int Count
        {
            get { return CaldroidFragment.NUMBER_OF_PAGES; }
        }


        public MonthPagerAdapter(FragmentManager fragmentManager)
            : base(fragmentManager)
        {

        }


        public override Fragment GetItem(int position)
        {
            return DateGridFragmentList[position];
        }


        private List<DateGridFragment> LoadOrCreateDateFragmentList()
        {
            if (_dateGridFragmentList == null)
            {
                _dateGridFragmentList = new List<DateGridFragment>();

                for (var i = 0; i < Count; i++)
                {
                    _dateGridFragmentList.Add(new DateGridFragment());
                }
            }

            return _dateGridFragmentList;
        }
    }
}