using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Java.Lang;

namespace Caldroid.Xamarin.Com.Antonyt.InfiniteViewPager
{
    /// <summary>
    /// A PagerAdapter that wraps around another PagerAdapter to handle paging
    /// wrap-around.
    /// </summary>
    public class InfinitePagerAdapter : PagerAdapter
    {
        private PagerAdapter _pagerAdapter;


        /// <summary>
        /// Warning: Scrolling to very high values (1,000,000+) results in
        /// strange drawing behaviour.
        /// </summary>
        /// <value>The count.</value>
        public override int Count => int.MaxValue;

        public override bool IsViewFromObject(View view, Object @object)
        {
            return _pagerAdapter.IsViewFromObject(view, @object);
        }


        public InfinitePagerAdapter(PagerAdapter pagerAdapter)
        {
            _pagerAdapter = pagerAdapter;
        }


        public override Object InstantiateItem(ViewGroup container, int position)
        {
            var virtualPosition = position % PageAdapterCount();

            return _pagerAdapter.InstantiateItem(container, virtualPosition);
        }


        public override void DestroyItem(ViewGroup container, int position, Object @object)
        {
            var virtualPosition = position % PageAdapterCount();

            _pagerAdapter.DestroyItem(container, virtualPosition, @object);
        }


        public override void StartUpdate(ViewGroup container)
        {
            _pagerAdapter.StartUpdate(container);
        }


        public override void FinishUpdate(ViewGroup container)
        {
            _pagerAdapter.FinishUpdate(container);
        }


        public override IParcelable SaveState()
        {
            return _pagerAdapter.SaveState();
        }


        public override void RestoreState(IParcelable state, ClassLoader loader)
        {
            _pagerAdapter.RestoreState(state, loader);
        }


        public int PageAdapterCount()
        {
            return _pagerAdapter.Count;
        }
    }
}
