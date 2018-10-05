using System;
using Android.Views;

namespace Caldroid.Xamarin.Com.Roomorama.Caldroid
{
    /// <summary>
    /// CaldroidListener inform when user clicks on a valid date (not within disabled
    /// dates, and valid between min/max dates).
    /// 
    /// The method onChangeMonth is optional, user can always override this to listen
    /// to month change event.
    /// 
    /// @author thomasdao
    /// </summary>
    public interface ICaldroidListener
    {
        /// <summary>
        /// Inform client user has clicked on a date
        /// </summary>
        /// <param name="date">Date.</param>
        /// <param name="view">View.</param>
        void OnSelectDate(DateTime date, View view);


        /// <summary>
        /// Inform client user has long clicked on a date
        /// </summary>
        /// <param name="date">Date.</param>
        /// <param name="view">View.</param>
        void OnLongClickDate(DateTime date, View view);


        /// <summary>
        /// Inform client that calendar has changed month
        /// </summary>
        /// <param name="year">Year.</param>
        /// <param name="month">Month.</param>
        void OnChangeMonth(int year, int month);


        /// <summary>
        /// Inform client that CaldroidFragment view has been created and views are
        /// no longer null. Useful for customization of Button and TextViews.
        /// </summary>
        void OnCalendarViewCreated();
    }
}