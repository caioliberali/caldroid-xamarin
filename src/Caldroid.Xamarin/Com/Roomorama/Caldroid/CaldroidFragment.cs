using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Caldroid.Xamarin.Com.Antonyt.InfiniteViewPager;
using Java.Lang;
using Java.Text;
using Java.Util;
using static Android.Support.V4.View.ViewPager;
using static Android.Widget.AdapterView;

namespace Caldroid.Xamarin.Com.Roomorama.Caldroid
{
    public class CaldroidFragment : DialogFragment
    {
        internal const int NUMBER_OF_PAGES = 4;

        internal const string _MIN_DATE_TIME = "_minDateTime";
        internal const string _MAX_DATE_TIME = "_maxDateTime";
        internal const string _BACKGROUND_FOR_DATETIME_MAP = "_backgroundForDateTimeMap";
        internal const string _TEXT_COLOR_FOR_DATETIME_MAP = "_textColorForDateTimeMap";

        internal const string YEAR = "year";
        internal const string MONTH = "month";
        internal const string MIN_DATE = "minDate";
        internal const string MAX_DATE = "maxDate";
        internal const string SELECTED_DATES = "selectedDates";
        internal const string DISABLED_DATES = "disableDates";
        internal const string ENABLE_SWIPE = "enableSwipe";
        internal const string MAX_WEEK_PER_MONTH = "maxWeekPerMonth";
        internal const string START_DAY_OF_WEEK = "startDayOfWeek";
        internal const string SHOW_NAVIGATION_ARROWS = "showNavigationArrows";
        internal const string ENABLE_CLICK_ON_DISABLED_DATES = "enableClickOnDisabledDates";
        internal const string SQUARE_TEXT_VIEW_CELL = "squareTextViewCell";
        internal const string THEME_RESOURCE = "themeResource";
        internal const string DIALOG_TITLE = "dialogTitle";

        public static int DisabledBackgroundDrawable = -1;
        public static Color DisabledTextColor = Color.Gray;

        private int _year = DateTime.Now.Year;
        private int _month = DateTime.Now.Month;
        private int _startDayOfWeek;
        private DateTime? _minDateTime;
        private DateTime? _maxDateTime;
        private List<DateTime> _disabledDates = new List<DateTime>();
        private List<DateTime> _selectedDates = new List<DateTime>();
        private List<DateTime> _daysInMonths;
        private DatePageChangeListener _datePageChangeListener;

        private bool _isViewCreated;
        private bool _isSwipeEnabled = true;
        private bool _isMaxWeekPerMonth = true;
        private bool _showNavigationArrows = true;
        private bool _isClickOnDisabledDatesEnabled;
        private bool _isSquareTextViewCell;
        private string _dialogTitle;
        private Dictionary<DateTime, Drawable> _backgroundForDate = new Dictionary<DateTime, Drawable>();
        private Dictionary<DateTime, int> _colorTextForDate = new Dictionary<DateTime, int>();

        private TextView _monthTitleTextView;
        private Button _buttonLeftArrow;
        private Button _buttonRightArrow;
        private GridView _weekdayGridView;
        private int _resourceTheme = Resource.Style.CaldroidDefault;

        private Dictionary<string, object> _dataFromClient = new Dictionary<string, object>();
        private Dictionary<string, object> _dataFromCalendar = new Dictionary<string, object>();

        private ICaldroidListener _calendarListener;
        private InfiniteViewPager _infiniteDateViewPager;
        private List<DateGridFragment> _dateGridFragments;
        private List<CaldroidGridAdapter> _calendarGridAdapters = new List<CaldroidGridAdapter>();


        public int Year
        {
            get { return _year; }
            set
            {
                if (_isViewCreated)
                {
                    _year = value;
                }
                else
                {
                    Arguments.PutInt(YEAR, value);
                }
            }
        }

        public int Month
        {
            get { return _month; }
            set
            {
                if (_isViewCreated)
                {
                    _month = value;
                }
                else
                {
                    Arguments.PutInt(MONTH, value);
                }
            }
        }

        public DateTime? MinDate
        {
            get { return _minDateTime; }
            set
            {
                if (_isViewCreated)
                {
                    _minDateTime = value;
                }
                else
                {
                    var valueToString = value?.ToString(CalendarHelper.DEFAULT_TIME_FORMAT);

                    Arguments.PutString(MIN_DATE, valueToString);
                }
            }
        }

        public DateTime? MaxDate
        {
            get { return _maxDateTime; }
            set
            {
                if (_isViewCreated)
                {
                    _maxDateTime = value;
                }
                else
                {
                    var valueToString = value?.ToString(CalendarHelper.DEFAULT_TIME_FORMAT);

                    Arguments.PutString(MAX_DATE, valueToString);
                }
            }
        }

        public List<DateTime> SelectedDate
        {
            get { return _selectedDates; }
            set
            {
                if (_isViewCreated)
                {
                    _selectedDates = value;
                }
                else
                {
                    Arguments.PutStringArrayList(SELECTED_DATES, CalendarHelper.ConvertDateTimeToString(value));
                }
            }
        }

        public List<DateTime> DisabledDate
        {
            get { return _disabledDates; }
            set
            {
                if (_isViewCreated)
                {
                    _disabledDates = value;
                }
                else
                {
                    Arguments.PutStringArrayList(DISABLED_DATES, CalendarHelper.ConvertDateTimeToString(value));
                }
            }
        }

        public bool IsSwipeEnabled
        {
            get { return _isSwipeEnabled; }
            set
            {
                if (_isViewCreated)
                {
                    _isSwipeEnabled = value;
                    _infiniteDateViewPager.Enabled = value;
                }
                else
                {
                    Arguments.PutBoolean(ENABLE_SWIPE, value);
                }
            }
        }

        public bool IsMaxWeekPerMonth
        {
            get { return _isMaxWeekPerMonth; }
            set
            {
                if (_isViewCreated)
                {
                    _isMaxWeekPerMonth = value;
                    _infiniteDateViewPager.IsMaxWeekPerMonth = value;
                }
                else
                {
                    Arguments.PutBoolean(MAX_WEEK_PER_MONTH, value);
                }
            }
        }

        public int StartDayOfWeek
        {
            get { return _startDayOfWeek; }
            set
            {
                if (_isViewCreated)
                {
                    var weekdaySaturday = (int)System.DayOfWeek.Saturday;

                    _startDayOfWeek = value <= weekdaySaturday ? value : value % 7;
                }
                else
                {
                    Arguments.PutInt(START_DAY_OF_WEEK, value);
                }
            }
        }

        public bool ShowNavigationArrows
        {
            get { return _showNavigationArrows; }
            set
            {
                if (_isViewCreated)
                {
                    _showNavigationArrows = value;

                    NavigationArrowsVisibility(value);
                }
                else
                {
                    Arguments.PutBoolean(SHOW_NAVIGATION_ARROWS, value);
                }
            }
        }

        public bool EnableClickOnDisabledDates
        {
            get { return _isClickOnDisabledDatesEnabled; }
            set
            {
                if (_isViewCreated)
                {
                    _isClickOnDisabledDatesEnabled = value;
                }
                else
                {
                    Arguments.PutBoolean(ENABLE_CLICK_ON_DISABLED_DATES, value);
                }
            }
        }

        public bool SquareTextViewCell
        {
            get { return _isClickOnDisabledDatesEnabled; }
            set
            {
                if (_isViewCreated)
                {
                    _isSquareTextViewCell = value;
                }
                else
                {
                    Arguments.PutBoolean(SQUARE_TEXT_VIEW_CELL, value);
                }
            }
        }

        public int ThemeResource
        {
            set
            {
                if (_isViewCreated)
                {
                    _resourceTheme = value;
                }
                else
                {
                    Arguments.PutInt(THEME_RESOURCE, value);
                }
            }
        }

        public string DialogTitle
        {
            set
            {
                if (_isViewCreated)
                {
                    _dialogTitle = value;
                }
                else
                {
                    Arguments.PutString(DIALOG_TITLE, value);
                }
            }
        }

        public Dictionary<DateTime, Drawable> BackgroundForDate
        {
            get { return _backgroundForDate; }
            set { _backgroundForDate = value; }
        }

        public Dictionary<DateTime, int> TextColorForDate
        {
            get { return _colorTextForDate; }
            set { _colorTextForDate = value; }
        }

        public Dictionary<string, object> DataFromCalendar
        {
            get { return _dataFromCalendar; }
        }

        public Dictionary<string, object> DataFromClient
        {
            get { return _dataFromClient; }
            set { _dataFromClient = value; }
        }


        public CaldroidFragment()
        {
            Arguments = new Bundle();
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            GetCalendarArguments();
            RetainInstanceIfDialog();

            var localInflater = ThemeInflater(Activity, inflater, _resourceTheme);

            // This is a hack to fix issue localInflater doesn't use the _resourceTheme, make Android
            // complain about layout_width and layout_height missing. I'm unsure about its impact
            // for app that wants to change theme dynamically.
            Activity.SetTheme(_resourceTheme);

            var view = localInflater.Inflate(Resource.Layout.calendar_view, container, false);

            _monthTitleTextView = view.FindViewById<TextView>(Resource.Id.calendar_month_year_textview);

            _buttonRightArrow = view.FindViewById<Button>(Resource.Id.calendar_right_arrow);
            _buttonRightArrow.Click += OnButtonRightArrowClicked;

            _buttonLeftArrow = view.FindViewById<Button>(Resource.Id.calendar_left_arrow);
            _buttonLeftArrow.Click += OnButtonLeftArrowClicked;

            var weekdayArrayAdapter = CreateWeekdayArrayAdapter(_resourceTheme);
            _weekdayGridView = view.FindViewById<GridView>(Resource.Id.weekday_gridview);
            _weekdayGridView.Adapter = weekdayArrayAdapter;

            NavigationArrowsVisibility(_showNavigationArrows);
            SetupDateGridPages(view);

            UpdateView();

            return view;
        }


        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            if (_calendarListener != null)
                _calendarListener.OnCalendarViewCreated();
        }


        public override void OnDetach()
        {
            base.OnDetach();

            try
            {
                var fieldName = "mChildFragmentManager";
                var childFragmentManager = Class.FromType(typeof(Fragment)).GetDeclaredField(fieldName);

                childFragmentManager.Accessible = true;
                childFragmentManager.Set(this, null);
            }
            catch (NoSuchFieldException e)
            {
                throw new RuntimeException(e);
            }
            catch (IllegalAccessException e)
            {
                throw new RuntimeException(e);
            }
        }


        /// <summary>
        /// Below code fixed the issue viewpager disappears in dialog mode on
        /// orientation change.
        /// 
        /// Code taken from Andy Dennie and Zsombor Erdody-Nagy
        /// http://stackoverflow.com/questions/8235080/fragments-dialogfragment-and-screen-rotation
        /// </summary>
        public override void OnDestroyView()
        {
            if (Dialog != null && RetainInstance)
                Dialog.SetDismissMessage(null);

            if (_buttonRightArrow != null)
                _buttonRightArrow.Click -= OnButtonRightArrowClicked;

            if (_buttonLeftArrow != null)
                _buttonLeftArrow.Click -= OnButtonLeftArrowClicked;

            if (_infiniteDateViewPager != null)
                _infiniteDateViewPager.RemoveOnPageChangeListener(_datePageChangeListener);

            for (var i = 0; i < _dateGridFragments.Count; i++)
            {
                var dateGridFragment = _dateGridFragments[i];

                if (dateGridFragment != null)
                {
                    dateGridFragment.OnItemClickEvent -= OnDateGridFragmentItemClicked;
                    dateGridFragment.OnItemLongClickEvent -= OnDateGridFragmentItemLongClicked;
                }
            }

            base.OnDestroyView();
        }


        /// <summary>
        /// To support faster init.
        /// </summary>
        /// <returns>The instance.</returns>
        /// <param name="dialogTitle">Dialog title.</param>
        /// <param name="year">Year.</param>
        /// <param name="month">Month.</param>
        public static CaldroidFragment CreateInstance(string dialogTitle, int year, int month)
        {
            var caldroidFragment = new CaldroidFragment();

            var bundleArgs = new Bundle();
            bundleArgs.PutInt(YEAR, year);
            bundleArgs.PutInt(MONTH, month);
            bundleArgs.PutString(DIALOG_TITLE, dialogTitle);

            caldroidFragment.Arguments = bundleArgs;

            return caldroidFragment;
        }


        public static LayoutInflater ThemeInflater(Context context, LayoutInflater inflater, int resourceTheme)
        {
            var contextThemeWrapper = new ContextThemeWrapper(context, resourceTheme);

            return inflater.CloneInContext(contextThemeWrapper);
        }


        /// <summary>
        /// Initial arguments to the fragment Data can include: month, year,
        /// dialogTitle, showNavigationArrows,(String) disableDates, selectedDates,
        /// minDate, maxDate, squareTextViewCell.
        /// </summary>
        protected void GetCalendarArguments()
        {
            _year = Arguments.GetInt(YEAR, -1);
            _month = Arguments.GetInt(MONTH, -1);
            _minDateTime = ArgumentDateTime(Arguments.GetString(MIN_DATE));
            _maxDateTime = ArgumentDateTime(Arguments.GetString(MAX_DATE));
            _selectedDates = ArgumentDateTimeList(Arguments.GetStringArrayList(SELECTED_DATES));
            _disabledDates = ArgumentDateTimeList(Arguments.GetStringArrayList(DISABLED_DATES));
            _isSwipeEnabled = Arguments.GetBoolean(ENABLE_SWIPE, true);
            _isMaxWeekPerMonth = Arguments.GetBoolean(MAX_WEEK_PER_MONTH, true);
            _startDayOfWeek = ArgumentDayOfWeek(Arguments.GetInt(START_DAY_OF_WEEK, 0));
            _showNavigationArrows = Arguments.GetBoolean(SHOW_NAVIGATION_ARROWS, true);
            _isClickOnDisabledDatesEnabled = Arguments.GetBoolean(ENABLE_CLICK_ON_DISABLED_DATES, false);
            _isSquareTextViewCell = Arguments.GetBoolean(SQUARE_TEXT_VIEW_CELL, DefaultArgumentSquareCell());
            _resourceTheme = Arguments.GetInt(THEME_RESOURCE, Resource.Style.CaldroidDefault);
            _dialogTitle = ArgumentDialogTitle(Arguments.GetString(DIALOG_TITLE));

            _year = _year >= 0 ? _year : DateTime.Now.Year;
            _month = _month >= 0 ? _month : DateTime.Now.Month;

            UpdateDataFromCalendar();

            _isViewCreated = true;
        }


        private DateTime? ArgumentDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return CalendarHelper.ConvertStringToDateTime(value, null);
        }


        private List<DateTime> ArgumentDateTimeList(IList<string> values)
        {
            var dateTimes = new List<DateTime>();

            if (values != null && values.Count > 0)
            {
                foreach (var item in values)
                {
                    var dateTime = CalendarHelper.ConvertStringToDateTime(item, null);

                    dateTimes.Add(dateTime);
                }
            }

            return dateTimes;
        }


        private int ArgumentDayOfWeek(int value)
        {
            var weekdaySaturday = (int)System.DayOfWeek.Saturday;

            return value <= weekdaySaturday ? value : value % 7;
        }


        private string ArgumentDialogTitle(string value)
        {
            if (Dialog != null)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
                }
                else
                {
                    Dialog.SetTitle(value);
                }
            }

            return value;
        }


        private bool DefaultArgumentSquareCell()
        {
            var orientation = Resources.Configuration.Orientation;

            return orientation == Android.Content.Res.Orientation.Portrait;
        }


        private void RetainInstanceIfDialog()
        {
            if (Dialog != null)
            {
                try
                {
                    RetainInstance = true;
                }
                catch (IllegalStateException e)
                {
                    e.PrintStackTrace();
                }
            }
        }


        /// <summary>
        /// Meant to be subclassed. User who wants to provide custom view, need to
        /// provide custom adapter here.
        /// </summary>
        /// <returns>The new weekday adapter.</returns>
        /// <param name="resourceTheme">Theme resource.</param>
        private WeekdayArrayAdapter CreateWeekdayArrayAdapter(int resourceTheme)
        {
            return new WeekdayArrayAdapter(Activity, Android.Resource.Layout.SimpleListItem1, DaysOfWeekTitles(), resourceTheme);
        }


        /// <summary>
        /// To display the week day title
        /// </summary>
        /// <returns>SUN, MON, TUE, WED, THU, FRI, SAT</returns>
        protected List<string> DaysOfWeekTitles()
        {
            var daysOfWeek = new List<string>();
            var formatter = new SimpleDateFormat("EEE", Locale.Default);

            var weekdaySunday = (int)System.DayOfWeek.Sunday;
            var daySunday = new DateTime(2000, 10, 1);
            var day = daySunday.AddDays(_startDayOfWeek - weekdaySunday);

            for (var i = 0; i < 7; i++)
            {
                var date = CalendarHelper.ConvertDateTimeToDate(day);
                var dayFormatted = formatter.Format(date).ToLower();

                daysOfWeek.Add(dayFormatted);
                day = day.AddDays(1);
            }

            return daysOfWeek;
        }


        /// <summary>
        /// Show or hide the navigation arrows.
        /// </summary>
        /// <param name="isVisible">If True show navigation arrows.</param>
        private void NavigationArrowsVisibility(bool isVisible)
        {
            var visibility = isVisible ? ViewStates.Visible : ViewStates.Invisible;

            _buttonRightArrow.Visibility = visibility;
            _buttonLeftArrow.Visibility = visibility;
        }


        /// <summary>
        /// Setup 4 pages contain date grid views. These pages are recycled to use
        /// memory efficient.
        /// </summary>
        /// <param name="view">View.</param>
        private void SetupDateGridPages(View view)
        {
            var currentDateTime = new DateTime(_year, _month, 1);

            _datePageChangeListener = new DatePageChangeListener(this);
            _datePageChangeListener.CurrentDateTime = currentDateTime;

            var currentMonthGridAdapter = CreateCalendarGridAdapter(currentDateTime.Year, currentDateTime.Month);

            _daysInMonths = currentMonthGridAdapter.DateTimes;

            var nextMonthDateTime = CalendarHelper.LastDateTimeOfNextMonth(currentDateTime);
            var nextMonthGridAdapter = CreateCalendarGridAdapter(nextMonthDateTime.Year, nextMonthDateTime.Month);

            var secondNextMonthDateTime = CalendarHelper.LastDateTimeOfNextMonth(nextMonthDateTime);
            var secondNextMonthGridAdapter = CreateCalendarGridAdapter(secondNextMonthDateTime.Year, secondNextMonthDateTime.Month);

            var prevMonthDateTime = CalendarHelper.LastDateTimeOfPreviousMonth(currentDateTime);
            var prevMonthGridAdapter = CreateCalendarGridAdapter(prevMonthDateTime.Year, prevMonthDateTime.Month);

            _calendarGridAdapters.Add(currentMonthGridAdapter);
            _calendarGridAdapters.Add(nextMonthGridAdapter);
            _calendarGridAdapters.Add(secondNextMonthGridAdapter);
            _calendarGridAdapters.Add(prevMonthGridAdapter);

            _datePageChangeListener.CalendarGridAdapters = _calendarGridAdapters;

            _infiniteDateViewPager = view.FindViewById<InfiniteViewPager>(Resource.Id.months_infinite_pager);
            _infiniteDateViewPager.IsEnabled = _isSwipeEnabled;
            _infiniteDateViewPager.IsMaxWeekPerMonth = _isMaxWeekPerMonth;
            _infiniteDateViewPager.DaysInMonth = _daysInMonths;

            var monthPagerAdapter = new MonthPagerAdapter(ChildFragmentManager);

            _dateGridFragments = monthPagerAdapter.DateGridFragmentList;

            for (var i = 0; i < NUMBER_OF_PAGES; i++)
            {
                var dateGridFragment = _dateGridFragments[i];
                var calendarGridAdapter = _calendarGridAdapters[i];

                dateGridFragment.GridViewResource = ResourceGridView();
                dateGridFragment.CalendarGridAdapter = calendarGridAdapter;

                dateGridFragment.OnItemClickEvent += OnDateGridFragmentItemClicked;
                dateGridFragment.OnItemLongClickEvent += OnDateGridFragmentItemLongClicked;
            }

            var infinitePagerAdapter = new InfinitePagerAdapter(monthPagerAdapter);

            _infiniteDateViewPager.Adapter = infinitePagerAdapter;
            _infiniteDateViewPager.AddOnPageChangeListener(_datePageChangeListener);
        }


        /// <summary>
        /// This method can be used to provide different GridView.
        /// </summary>
        /// <returns>The GridView layout.</returns>
        protected int ResourceGridView()
        {
            return Resource.Layout.date_grid_fragment;
        }


        /// <summary>
        /// Meant to be subclassed. User who wants to provide custom view, need to
        /// provide custom adapter here.
        /// </summary>
        /// <returns>The new dates grid adapter.</returns>
        /// <param name="year">Year.</param>  
        /// <param name="month">Month.</param>
        protected virtual CaldroidGridAdapter CreateCalendarGridAdapter(int year, int month)
        {
            return new CaldroidGridAdapter(Activity, year, month, _dataFromCalendar, _dataFromClient);
        }


        /// <summary>
        /// Update view when parameter changes. You should always change all
        /// parameters first, then call this method.
        /// </summary>
        public void UpdateView()
        {
            UpdateNavigationArrows();
            UpdateMonthTitleTextView();
            UpdateDataFromCalendar();

            foreach (var item in _calendarGridAdapters)
            {
                item.DataFromCalendar = _dataFromCalendar;
                item.DataFromClient = _dataFromClient;
                item.NotifyDataSetChanged();
            }
        }


        protected void UpdateNavigationArrows()
        {
            var canGoToPrevMonth = CanGoToPrevMonth(_year, _month);
            var canGoToNextMonth = CanGoToNextMonth(_year, _month);

            _buttonLeftArrow.Enabled = canGoToPrevMonth;
            _buttonRightArrow.Enabled = canGoToNextMonth;
        }


        /// <summary>
        /// Refresh month title text view when user swipe.
        /// </summary>
        protected void UpdateMonthTitleTextView()
        {
            var monthArray = DateFormatSymbols.GetInstance(Locale.Default).GetMonths();
            var monthName = monthArray[_month - 1];
            var monthNameTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(monthName);

            var monthText = string.Concat(monthNameTitle, " ", _year);

            _monthTitleTextView.SetText(monthText, TextView.BufferType.Normal);
        }


        /// <summary>
        /// Updates the data from calendar.
        /// </summary>
        /// <returns>The data from calendar.</returns>
        private void UpdateDataFromCalendar()
        {
            _dataFromCalendar.Clear();
            _dataFromCalendar.Add(_MIN_DATE_TIME, _minDateTime);
            _dataFromCalendar.Add(_MAX_DATE_TIME, _maxDateTime);
            _dataFromCalendar.Add(SELECTED_DATES, _selectedDates);
            _dataFromCalendar.Add(DISABLED_DATES, _disabledDates);
            _dataFromCalendar.Add(MAX_WEEK_PER_MONTH, _isMaxWeekPerMonth);
            _dataFromCalendar.Add(START_DAY_OF_WEEK, _startDayOfWeek);
            _dataFromCalendar.Add(SQUARE_TEXT_VIEW_CELL, _isSquareTextViewCell);
            _dataFromCalendar.Add(THEME_RESOURCE, _resourceTheme);

            _dataFromCalendar.Add(_BACKGROUND_FOR_DATETIME_MAP, _backgroundForDate);
            _dataFromCalendar.Add(_TEXT_COLOR_FOR_DATETIME_MAP, _colorTextForDate);
        }


        /// <summary>
        /// Save current state to bundle outState.
        /// </summary>
        /// <param name="outState">Out state.</param>
        /// <param name="key">Key.</param>
        public void SaveStatesToKey(Bundle outState, string key)
        {
            outState.PutBundle(key, CreateBundle());
        }


        /// <summary>
        /// GresourceThemeved sates of the Calendar Extended. Useful for handling rotation.
        /// It does not need to save state of SQUARE_TEXT_VIEW_CELL because this
        /// may change on orientation change.
        /// </summary>
        /// <returns>The saved states.</returns>
        public Bundle CreateBundle()
        {
            var bundle = new Bundle();

            bundle.PutInt(YEAR, _year);
            bundle.PutInt(MONTH, _month);

            if (_minDateTime != null)
                bundle.PutString(MIN_DATE, _minDateTime.Value.ToString(CalendarHelper.DEFAULT_TIME_FORMAT));

            if (_maxDateTime != null)
                bundle.PutString(MAX_DATE, _maxDateTime.Value.ToString(CalendarHelper.DEFAULT_TIME_FORMAT));

            if (_selectedDates != null && _selectedDates.Count > 0)
                bundle.PutStringArrayList(SELECTED_DATES, CalendarHelper.ConvertDateTimeToString(_selectedDates));

            if (_disabledDates != null && _disabledDates.Count > 0)
                bundle.PutStringArrayList(DISABLED_DATES, CalendarHelper.ConvertDateTimeToString(_disabledDates));

            bundle.PutBoolean(ENABLE_SWIPE, _isSwipeEnabled);
            bundle.PutBoolean(MAX_WEEK_PER_MONTH, _isMaxWeekPerMonth);
            bundle.PutBoolean(SHOW_NAVIGATION_ARROWS, _showNavigationArrows);
            bundle.PutBoolean(SQUARE_TEXT_VIEW_CELL, _isSquareTextViewCell);
            bundle.PutInt(START_DAY_OF_WEEK, _startDayOfWeek);
            bundle.PutInt(THEME_RESOURCE, _resourceTheme);

            if (_dialogTitle != null)
                bundle.PutString(DIALOG_TITLE, _dialogTitle);

            return bundle;
        }


        /// <summary>
        /// Restore state for dialog.
        /// </summary>
        /// <param name="manager">Manager.</param>
        /// <param name="savedInstanceState">Saved instance state.</param>
        /// <param name="key">Key.</param>
        /// <param name="dialogTag">Dialog tag.</param>
        public void RestoreDialogStatesFromKey(FragmentManager manager, Bundle savedInstanceState,
                                               string key, string dialogTag)
        {
            RestoreStatesFromKey(savedInstanceState, key);

            var dialogExisting = (CaldroidFragment)manager.FindFragmentByTag(dialogTag);

            if (dialogExisting != null)
            {
                dialogExisting.Dismiss();

                Show(manager, dialogTag);
            }
        }


        /// <summary>
        /// Restore current states from savedInstanceState.
        /// </summary>
        /// <param name="savedInstanceState">Saved instance state.</param>
        /// <param name="key">Key.</param>
        public void RestoreStatesFromKey(Bundle savedInstanceState, string key)
        {
            if (savedInstanceState != null && savedInstanceState.ContainsKey(key))
                Arguments = savedInstanceState.GetBundle(key);
        }


        public void SetMinDate(Date minDate)
        {
            MinDate = minDate == null ? default(DateTime?) : CalendarHelper.ConvertDateToDateTime(minDate);
        }

        public void SetMinDate(string minDate, string dateFormat)
        {
            var dateTime = default(DateTime?);

            if (!string.IsNullOrWhiteSpace(minDate))
                dateTime = CalendarHelper.ConvertStringToDateTime(minDate, dateFormat);

            MinDate = dateTime;
        }


        public void SetMaxDate(Date maxDate)
        {
            MaxDate = maxDate == null ? default(DateTime?) : CalendarHelper.ConvertDateToDateTime(maxDate);
        }

        public void SetMaxDate(string maxDate, string dateFormat)
        {
            var dateTime = default(DateTime?);

            if (!string.IsNullOrWhiteSpace(maxDate))
                dateTime = CalendarHelper.ConvertStringToDateTime(maxDate, dateFormat);

            MaxDate = dateTime;
        }


        public void SetSelectedDate(DateTime dateTime)
        {
            SelectedDate = new List<DateTime> { dateTime };
        }

        public void SetSelectedDate(List<Date> selectedDates)
        {
            if (selectedDates == null || selectedDates.Count == 0)
                return;

            var dateTimes = new List<DateTime>();

            foreach (var item in selectedDates)
            {
                var dateTime = CalendarHelper.ConvertDateToDateTime(item);

                dateTimes.Add(dateTime);
            }

            SelectedDate = dateTimes;
        }


        public void ClearSelectedDate()
        {
            SelectedDate = new List<DateTime>();
        }


        public void SetDisabledDate(DateTime dateTime)
        {
            DisabledDate = new List<DateTime> { dateTime };
        }

        public void SetDisabledDate(List<Date> disableDates)
        {
            if (disableDates == null || disableDates.Count == 0)
                return;

            var dateTimes = new List<DateTime>();

            foreach (var item in disableDates)
            {
                var dateTime = CalendarHelper.ConvertDateToDateTime(item);

                dateTimes.Add(dateTime);
            }

            DisabledDate = dateTimes;
        }


        public void ClearDisabledDate()
        {
            DisabledDate = new List<DateTime>();
        }


        public void SetBackgroundDrawableForDate(Drawable drawable, DateTime dateTime)
        {
            _backgroundForDate.Add(dateTime, drawable);
        }

        public void SetBackgroundDrawableForDate(Drawable drawable, Date date)
        {
            var dateTime = CalendarHelper.ConvertDateToDateTime(date);

            _backgroundForDate.Add(dateTime, drawable);
        }

        public void SetBackgroundDrawableForDates(Dictionary<Date, Drawable> backgroundForDateMap)
        {
            if (backgroundForDateMap == null || backgroundForDateMap.Count == 0)
                return;

            _backgroundForDate.Clear();

            foreach (var item in backgroundForDateMap)
            {
                var dateTime = CalendarHelper.ConvertDateToDateTime(item.Key);

                _backgroundForDate.Add(dateTime, item.Value);
            }
        }


        public void ClearBackgroundDrawableForDateTime(DateTime dateTime)
        {
            _backgroundForDate.Remove(dateTime);
        }

        public void ClearBackgroundDrawableForDateTimes(List<DateTime> dateTimes)
        {
            if (dateTimes == null || dateTimes.Count == 0)
                return;

            foreach (var item in dateTimes)
            {
                _backgroundForDate.Remove(item);
            }
        }

        public void ClearBackgroundDrawableForDate(Date date)
        {
            var dateTime = CalendarHelper.ConvertDateToDateTime(date);

            _backgroundForDate.Remove(dateTime);
        }

        public void ClearBackgroundDrawableForDates(List<Date> dates)
        {
            if (dates == null || dates.Count == 0)
                return;

            foreach (var item in dates)
            {
                ClearBackgroundDrawableForDate(item);
            }
        }


        public void SetTextColorForDate(int resourceTextColor, DateTime dateTime)
        {
            _colorTextForDate.Add(dateTime, resourceTextColor);
        }

        public void SetTextColorForDate(int resourceTextColor, Date date)
        {
            var dateTime = CalendarHelper.ConvertDateToDateTime(date);

            _colorTextForDate.Add(dateTime, resourceTextColor);
        }

        public void SetTextColorForDates(Dictionary<Date, int> colorTextForDateMap)
        {
            if (colorTextForDateMap == null || colorTextForDateMap.Count == 0)
                return;

            _colorTextForDate.Clear();

            foreach (var item in colorTextForDateMap)
            {
                var dateTime = CalendarHelper.ConvertDateToDateTime(item.Key);

                _colorTextForDate.Add(dateTime, item.Value);
            }
        }


        public void ClearTextColorForDate(Date date)
        {
            var dateTime = CalendarHelper.ConvertDateToDateTime(date);

            _colorTextForDate.Remove(dateTime);
        }

        public void ClearTextColorForDates(List<Date> dates)
        {
            if (dates == null || dates.Count == 0)
                return;

            foreach (var item in dates)
            {
                ClearTextColorForDate(item);
            }
        }


        public void AddCaldroidListener(ICaldroidListener caldroidListener)
        {
            _calendarListener = caldroidListener;
        }


        /// <summary>
        /// Set month and year for the calendar. This is to avoid naive
        /// implementation of manipulating month and year. All dates within same
        /// month/year give same result.
        /// </summary>
        /// <param name="dateTime">Date time.</param>
        public void SetCalendarDate(DateTime dateTime)
        {
            _year = dateTime.Year;
            _month = dateTime.Month;

            if (_calendarListener != null)
                _calendarListener.OnChangeMonth(_year, _month);

            UpdateView();
        }

        public void SetCalendarDate(Date date)
        {
            var dateTime = CalendarHelper.ConvertDateToDateTime(date);

            SetCalendarDate(dateTime);
        }


        /// <summary>
        /// Move calendar to the specified date.
        /// </summary>
        /// <param name="dateTime">Date time.</param>
        public void MoveToDate(DateTime dateTime)
        {
            var dayOfMonthFirst = new DateTime(_year, _month, 1);
            var dayOfMonthLast = new DateTime(_year, _month, DateTime.DaysInMonth(_year, _month));

            // Calendar swipe left when dateTime is in the past
            if (dateTime.Date < dayOfMonthFirst)
            {
                var firstDayOfNextMonth = CalendarHelper.FirstDateTimeOfNextMonth(dateTime);
                var item = _infiniteDateViewPager.CurrentItem;

                _datePageChangeListener.CurrentDateTime = firstDayOfNextMonth;
                _datePageChangeListener.UpdateAdapters(item);

                _infiniteDateViewPager.CurrentItem = item - 1;
            }
            else if (dateTime.Date > dayOfMonthLast)
            {
                var firstDayOfPrevMonth = CalendarHelper.FirstDateTimeOfPreviousMonth(dateTime);
                var item = _infiniteDateViewPager.CurrentItem;

                _datePageChangeListener.CurrentDateTime = firstDayOfPrevMonth;
                _datePageChangeListener.UpdateAdapters(item);

                _infiniteDateViewPager.CurrentItem = item + 1;
            }
        }

        public void MoveToDate(Date date)
        {
            var dateTime = CalendarHelper.ConvertDateToDateTime(date);

            MoveToDate(dateTime);
        }


        private bool CanGoToPrevMonth(int year, int month)
        {
            if (MinDate != null)
            {
                var minDateValue = MinDate.Value;

                var dateTime = new DateTime(year, month, 1);
                var minDate = new DateTime(minDateValue.Year, minDateValue.Month, 1);

                return DateTime.Compare(minDate, dateTime) < 0;
            }

            return true;
        }


        private bool CanGoToNextMonth(int year, int month)
        {
            if (MaxDate != null)
            {
                var maxDateValue = MaxDate.Value;

                var dateTime = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                var maxDate = new DateTime(maxDateValue.Year, maxDateValue.Month, DateTime.DaysInMonth(maxDateValue.Year, maxDateValue.Month));

                return DateTime.Compare(maxDate, dateTime) > 0;
            }

            return true;
        }


        /// <summary>
        /// Set calendar to previous month.
        /// </summary>
        private void GoToPrevMonth()
        {
            _infiniteDateViewPager.SetCurrentItem(_datePageChangeListener.CurrentPage - 1, true);
        }


        /// <summary>
        /// Set calendar to next month.
        /// </summary>
        private void GoToNextMonth()
        {
            _infiniteDateViewPager.SetCurrentItem(_datePageChangeListener.CurrentPage + 1, true);
        }


        private void OnButtonRightArrowClicked(object sender, EventArgs e)
        {
            GoToNextMonth();
        }


        private void OnButtonLeftArrowClicked(object sender, EventArgs e)
        {
            GoToPrevMonth();
        }


        private void OnDateGridFragmentItemClicked(object sender, ItemClickEventArgs e)
        {
            var dateTime = _daysInMonths[e.Position];

            if (_calendarListener != null)
            {
                if (!_isClickOnDisabledDatesEnabled)
                {
                    if ((_minDateTime != null && dateTime.Date < _minDateTime.Value.Date) ||
                        (_maxDateTime != null && dateTime.Date > _maxDateTime.Value.Date) ||
                        (_disabledDates != null && _disabledDates.Contains(dateTime)))
                    {
                        return;
                    }
                }

                _calendarListener.OnSelectDate(dateTime, e.View);
            }
        }


        private void OnDateGridFragmentItemLongClicked(object sender, ItemLongClickEventArgs e)
        {
            var dateTime = _daysInMonths[e.Position];

            if (_calendarListener != null)
            {
                if (!_isClickOnDisabledDatesEnabled)
                {
                    if ((_minDateTime != null && dateTime.Date < _minDateTime.Value.Date) ||
                        (_maxDateTime != null && dateTime.Date > _maxDateTime.Value.Date) ||
                        (_disabledDates != null && _disabledDates.Contains(dateTime)))
                    {
                        return;
                    }
                }

                _calendarListener.OnLongClickDate(dateTime, e.View);
            }
        }


        /// <summary>
        /// DatePageChangeListener refresh the date grid views when user swipe the calendar.
        /// 
        /// @author thomasdao
        /// </summary>
        public class DatePageChangeListener : Java.Lang.Object, IOnPageChangeListener
        {
            private CaldroidFragment _calendarFragment;
            private DateTime _dateTime;

            public int CurrentPage { get; set; } = InfiniteViewPager.OFFSET;
            public List<CaldroidGridAdapter> CalendarGridAdapters { get; set; }

            public DateTime CurrentDateTime
            {
                get { return _dateTime; }
                set
                {
                    _dateTime = value;
                    _calendarFragment.SetCalendarDate(_dateTime);
                }
            }


            public DatePageChangeListener(CaldroidFragment calendarFragment)
            {
                _calendarFragment = calendarFragment;
            }


            public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
            {

            }


            public void OnPageScrollStateChanged(int state)
            {

            }


            public void OnPageSelected(int position)
            {
                UpdateAdapters(position);

                _calendarFragment.SetCalendarDate(_dateTime);

                var currentAdapter = CalendarGridAdapters[Current(position)];

                _calendarFragment._daysInMonths.Clear();
                _calendarFragment._daysInMonths.AddRange(currentAdapter.DateTimes);
            }


            public int Current(int position)
            {
                return position % CaldroidFragment.NUMBER_OF_PAGES;
            }


            private int Next(int position)
            {
                return (position + 1) % CaldroidFragment.NUMBER_OF_PAGES;
            }


            private int Previous(int position)
            {
                return (position + 3) % CaldroidFragment.NUMBER_OF_PAGES;
            }


            public void UpdateAdapters(int position)
            {
                var currentAdapter = CalendarGridAdapters[Current(position)];
                var prevAdapter = CalendarGridAdapters[Previous(position)];
                var nextAdapter = CalendarGridAdapters[Next(position)];

                if (position == CurrentPage)
                {
                    currentAdapter.SetAdapterDateTime(_dateTime);
                    currentAdapter.NotifyDataSetChanged();

                    var prevDateTime = CalendarHelper.LastDateTimeOfPreviousMonth(_dateTime);
                    prevAdapter.SetAdapterDateTime(prevDateTime);
                    prevAdapter.NotifyDataSetChanged();

                    var nextDateTime = CalendarHelper.LastDateTimeOfNextMonth(_dateTime);
                    nextAdapter.SetAdapterDateTime(nextDateTime);
                    nextAdapter.NotifyDataSetChanged();
                }
                // Swipe right
                else if (position > CurrentPage)
                {
                    _dateTime = CalendarHelper.LastDateTimeOfNextMonth(_dateTime);

                    var nextDateTime = CalendarHelper.LastDateTimeOfNextMonth(_dateTime);
                    nextAdapter.SetAdapterDateTime(nextDateTime);
                    nextAdapter.NotifyDataSetChanged();

                }
                // Swipe left
                else
                {
                    _dateTime = CalendarHelper.LastDateTimeOfPreviousMonth(_dateTime);

                    var prevDateTime = CalendarHelper.LastDateTimeOfPreviousMonth(_dateTime);
                    prevAdapter.SetAdapterDateTime(prevDateTime);
                    prevAdapter.NotifyDataSetChanged();
                }

                CurrentPage = position;
            }
        }
    }
}