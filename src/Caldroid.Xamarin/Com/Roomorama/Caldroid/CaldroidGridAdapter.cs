using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Caldroid.Xamarin.Com.Roomorama.Caldroid
{
    public class CaldroidGridAdapter : BaseAdapter
    {
        protected int _year;
        protected int _month;
        protected Context _context;

        private int _dayOfWeek;
        private int _resourceTheme;
        private bool _isMaxWeekPerMonth;
        private bool _isSquareTextViewCell;
        private bool _shouldUpdateDateTime;
        private bool _shouldUpdateResources;
        private LayoutInflater _layoutInflater;

        private int _resourceBackgroundCell = -1;
        private ColorStateList _resourceTextColorDefault;

        private Dictionary<string, object> _dataFromClient = new Dictionary<string, object>();
        private Dictionary<string, object> _dataFromCalendar = new Dictionary<string, object>();
        private HashSet<DateTime> _disabledDatesSet = new HashSet<DateTime>();
        private HashSet<DateTime> _selectedDatesSet = new HashSet<DateTime>();

        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public List<DateTime> DisabledDates { get; set; }
        public List<DateTime> SelectedDates { get; set; }
        public List<DateTime> DateTimes { get; private set; }


        public int StartDayOfWeek
        {
            get { return _dayOfWeek; }
            set
            {
                if (_dayOfWeek != value)
                {
                    _dayOfWeek = value;
                    _shouldUpdateDateTime = true;
                }
            }
        }

        public bool IsMaxWeekPerMonth
        {
            get { return _isMaxWeekPerMonth; }
            set
            {
                if (_isMaxWeekPerMonth != value)
                {
                    _isMaxWeekPerMonth = value;
                    _shouldUpdateDateTime = true;
                }
            }
        }


        public int ThemeResource
        {
            get { return _resourceTheme; }
            private set
            {
                if (_resourceTheme != value)
                {
                    _resourceTheme = value;
                    _shouldUpdateResources = true;
                }
            }
        }

        public bool IsSquareTextViewCell
        {
            get { return _isSquareTextViewCell; }
            set
            {
                if (_isSquareTextViewCell != value)
                {
                    _isSquareTextViewCell = value;
                    _shouldUpdateResources = true;
                }
            }
        }

        public Dictionary<string, object> DataFromCalendar
        {
            get { return _dataFromCalendar; }
            set
            {
                if (HasCalendarDataChanged(_dataFromCalendar, value))
                {
                    _dataFromCalendar = value;
                    UpdateFromCalendarData();
                }
            }
        }

        public Dictionary<string, object> DataFromClient
        {
            get { return _dataFromClient; }
            set { _dataFromClient = value; }
        }


        public CaldroidGridAdapter(Context context, int year, int month,
                                   Dictionary<string, object> dataFromCalendar,
                                   Dictionary<string, object> dataFromClient)
        {
            _year = year;
            _month = month;
            _context = context;
            _dataFromClient = dataFromClient;
            _dataFromCalendar = dataFromCalendar;

            LoadFromCalendarData();

            var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);

            _layoutInflater = CaldroidFragment.ThemeInflater(context, inflater, ThemeResource);
        }


        public override int Count
        {
            get { return DateTimes.Count; }
        }


        public override Java.Lang.Object GetItem(int position)
        {
            var dateTime = DateTimes[position];
            var date = CalendarHelper.ConvertDateTimeToDate(dateTime);

            return date;
        }


        public override long GetItemId(int position)
        {
            return 0;
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            CellView cellView;

            if (convertView == null)
            {
                int resource = IsSquareTextViewCell ? Resource.Layout.square_date_cell :
                                                       Resource.Layout.normal_date_cell;

                cellView = (CellView)_layoutInflater.Inflate(resource, parent, false);
            }
            else
            {
                cellView = (CellView)convertView;
            }

            CustomizeTextView(position, cellView);

            return cellView;
        }


        private void LoadFromCalendarData()
        {
            StartDayOfWeek = (int)_dataFromCalendar[CaldroidFragment.START_DAY_OF_WEEK];
            IsMaxWeekPerMonth = (bool)_dataFromCalendar[CaldroidFragment.MAX_WEEK_PER_MONTH];
            IsSquareTextViewCell = (bool)_dataFromCalendar[CaldroidFragment.SQUARE_TEXT_VIEW_CELL];

            MinDate = (DateTime?)_dataFromCalendar[CaldroidFragment._MIN_DATE_TIME];
            MaxDate = (DateTime?)_dataFromCalendar[CaldroidFragment._MAX_DATE_TIME];
            SelectedDates = (List<DateTime>)_dataFromCalendar[CaldroidFragment.SELECTED_DATES];
            DisabledDates = (List<DateTime>)_dataFromCalendar[CaldroidFragment.DISABLED_DATES];
            ThemeResource = (int)_dataFromCalendar[CaldroidFragment.THEME_RESOURCE];

            if (SelectedDates != null)
                _selectedDatesSet = new HashSet<DateTime>(SelectedDates);

            if (DisabledDates != null)
                _disabledDatesSet = new HashSet<DateTime>(DisabledDates);

            LoadDateTime();
            LoadDefaultResources();
        }


        private void UpdateFromCalendarData()
        {
            StartDayOfWeek = (int)_dataFromCalendar[CaldroidFragment.START_DAY_OF_WEEK];
            IsMaxWeekPerMonth = (bool)_dataFromCalendar[CaldroidFragment.MAX_WEEK_PER_MONTH];
            IsSquareTextViewCell = (bool)_dataFromCalendar[CaldroidFragment.SQUARE_TEXT_VIEW_CELL];

            MinDate = (DateTime?)_dataFromCalendar[CaldroidFragment._MIN_DATE_TIME];
            MaxDate = (DateTime?)_dataFromCalendar[CaldroidFragment._MAX_DATE_TIME];
            SelectedDates = (List<DateTime>)_dataFromCalendar[CaldroidFragment.SELECTED_DATES];
            DisabledDates = (List<DateTime>)_dataFromCalendar[CaldroidFragment.DISABLED_DATES];
            ThemeResource = (int)_dataFromCalendar[CaldroidFragment.THEME_RESOURCE];

            if (SelectedDates != null)
                _selectedDatesSet = new HashSet<DateTime>(SelectedDates);

            if (DisabledDates != null)
                _disabledDatesSet = new HashSet<DateTime>(DisabledDates);

            if (_shouldUpdateDateTime)
                LoadDateTime();

            if (_shouldUpdateResources)
                LoadDefaultResources();
        }


        private void LoadDateTime()
        {
            _shouldUpdateDateTime = false;

            DateTimes = CalendarHelper.GetFullWeeks(_year, _month, StartDayOfWeek, IsMaxWeekPerMonth);
        }


        private void LoadDefaultResources()
        {
            _shouldUpdateResources = false;

            var contextThemeWrapper = new ContextThemeWrapper(_context, ThemeResource);
            var currentTheme = contextThemeWrapper.Theme;

            var cellStyleValue = new TypedValue();
            var cellStyleAttribute = IsSquareTextViewCell ? Resource.Attribute.styleCaldroidSquareCell :
                                                             Resource.Attribute.styleCaldroidNormalCell;

            currentTheme.ResolveAttribute(cellStyleAttribute, cellStyleValue, true);

            var arrayTyped = contextThemeWrapper.ObtainStyledAttributes(cellStyleValue.Data, Resource.Styleable.Cell);

            _resourceBackgroundCell = arrayTyped.GetResourceId(Resource.Styleable.Cell_android_background, -1);
            _resourceTextColorDefault = arrayTyped.GetColorStateList(Resource.Styleable.Cell_android_textColor);

            arrayTyped.Recycle();
        }


        public void SetAdapterDateTime(DateTime dateTime)
        {
            if (_year == dateTime.Year && _month == dateTime.Month)
                return;

            _year = dateTime.Year;
            _month = dateTime.Month;

            DateTimes = CalendarHelper.GetFullWeeks(_year, _month, StartDayOfWeek, IsMaxWeekPerMonth);
        }


        protected void CustomizeTextView(int position, CellView cellView)
        {
            var paddingTop = cellView.PaddingTop;
            var paddingLeft = cellView.PaddingLeft;
            var paddingBottom = cellView.PaddingBottom;
            var paddingRight = cellView.PaddingRight;

            var dateTime = DateTimes[position];

            cellView.ClearCustomStates();
            cellView.SetTextColor(_resourceTextColorDefault);
            cellView.SetBackgroundResource(_resourceBackgroundCell);

            if (dateTime.Date == DateTime.Now.Date)
                cellView.AddCustomState(CellView.STATE_TODAY);

            if (dateTime.Month != _month)
                cellView.AddCustomState(CellView.STATE_PREV_NEXT_MONTH);

            if ((MinDate != null && dateTime.Date < MinDate.Value.Date) ||
                (MaxDate != null && dateTime.Date > MaxDate.Value.Date) ||
                (DisabledDates != null && _disabledDatesSet.Contains(dateTime)))
            {
                cellView.AddCustomState(CellView.STATE_DISABLED);
            }

            if (SelectedDates != null && _selectedDatesSet.Contains(dateTime))
                cellView.AddCustomState(CellView.STATE_SELECTED);

            cellView.RefreshDrawableState();
            cellView.Text = dateTime.ToString("dd");

            SetCustomResources(dateTime, cellView, cellView);

            cellView.SetPadding(paddingLeft, paddingTop, paddingRight, paddingBottom);
        }


        protected void SetCustomResources(DateTime dateTime, View backgroundView, TextView textView)
        {
            var backgroundForDateTime = (Dictionary<DateTime, Drawable>)_dataFromCalendar[CaldroidFragment._BACKGROUND_FOR_DATETIME_MAP];
            if (backgroundForDateTime != null)
            {
                Drawable drawable;

                if (backgroundForDateTime.TryGetValue(dateTime, out drawable))
                    backgroundView.Background = drawable;
            }

            var colorTextForDateTime = (Dictionary<DateTime, int>)_dataFromCalendar[CaldroidFragment._TEXT_COLOR_FOR_DATETIME_MAP];
            if (colorTextForDateTime != null)
            {
                int resourceTextColor;

                if (colorTextForDateTime.TryGetValue(dateTime, out resourceTextColor))
                {
                    var color = new Color(ContextCompat.GetColor(_context, resourceTextColor));

                    textView.SetTextColor(color);
                }
            }
        }


        public bool HasCalendarDataChanged(Dictionary<string, object> origin, Dictionary<string, object> changed)
        {
            if (origin.Count == changed.Count && !origin.Keys.Except(changed.Keys).Any())
                if (origin.OrderBy(kvp => kvp.Key).SequenceEqual(changed.OrderBy(kvp => kvp.Key)))
                    return false;

            return true;
        }
    }
}