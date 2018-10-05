using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Caldroid.Xamarin.Com.Roomorama.Caldroid;

namespace CaldroidSamples.Droid
{
    [Activity(Label = "CaldroidSamples", MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/AppTheme")]
    public class CaldroidSampleActivity : AppCompatActivity, ICaldroidListener
    {
        private bool _willUndo;
        private CaldroidFragment _caldroidFragment;
        private CaldroidFragment _dialogCaldroidFragment;

        private TextView _textView;
        private Button _customizeButton;
        private Button _showDialogButton;
        private Bundle _bundleState;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ActivityMain);

            // Setup caldroid fragment
            // **** If you want normal CaldroidFragment, use below line ****
            _caldroidFragment = new CaldroidFragment();

            // //////////////////////////////////////////////////////////////////////
            // **** This is to show customized fragment. If you want customized
            // version, uncomment below line ****
            //       _caldroidFragment = new CaldroidSampleCustomFragment();

            // Setup arguments

            // If Activity is created after rotation
            if (savedInstanceState != null)
            {
                _caldroidFragment.RestoreStatesFromKey(savedInstanceState, "CALDROID_SAVED_STATE");
            }
            // If activity is created from fresh
            else
            {
                _caldroidFragment.Year = DateTime.Now.Year;
                _caldroidFragment.Month = DateTime.Now.Month;
                _caldroidFragment.IsSwipeEnabled = true;
                _caldroidFragment.IsMaxWeekPerMonth = true;

                // Uncomment this to customize startDayOfWeek
                // _caldroidFragment.StartDayOfWeek = (int)System.DayOfWeek.Tuesday; // Tuesday

                // Uncomment this line to use Caldroid in compact mode
                // _caldroidFragment.SquareTextViewCell = false;

                // Uncomment this line to use dark theme
                // _caldroidFragment.ThemeResource = Caldroid.Xamarin.Resource.Style.CaldroidDefaultDark;
            }

            SetCustomResourceForDates();

            // Attach to the activity
            var fragementTransaction = SupportFragmentManager.BeginTransaction();
            fragementTransaction.Replace(Resource.Id.calendar1, _caldroidFragment);
            fragementTransaction.Commit();


            // Setup Caldroid
            _caldroidFragment.AddCaldroidListener(this);

            _textView = FindViewById<TextView>(Resource.Id.textview);
            _customizeButton = FindViewById<Button>(Resource.Id.customize_button);
            _showDialogButton = FindViewById<Button>(Resource.Id.show_dialog_button);

            _customizeButton.Click += CustomizeButton_Click;
            _showDialogButton.Click += ShowDialogButton_Click;

            _bundleState = savedInstanceState;
        }


        void CustomizeButton_Click(object sender, EventArgs e)
        {
            if (_willUndo)
            {
                _customizeButton.Text = GetString(Resource.String.customize);
                _textView.Text = "";

                // Reset calendar
                _caldroidFragment.ClearDisabledDate();
                _caldroidFragment.ClearSelectedDate();
                _caldroidFragment.SetMinDate(null);
                _caldroidFragment.SetMaxDate(null);
                _caldroidFragment.ShowNavigationArrows = (true);
                _caldroidFragment.IsSwipeEnabled = (true);
                _caldroidFragment.UpdateView();
                _willUndo = false;
                return;
            }

            // Else
            _willUndo = true;
            _customizeButton.Text = GetString(Resource.String.undo);


            // Min date is last 7 days
            var minDate = DateTime.Now.AddDays(-7);

            // Max date is next 7 days
            var maxDate = DateTime.Now.AddDays(14);

            // Set selected dates
            var selectedDates = new List<DateTime>();
            selectedDates.Add(DateTime.Now.AddDays(2));
            selectedDates.Add(DateTime.Now.AddDays(3));

            // Set disabled dates
            var disabledDates = new List<DateTime>();

            for (int i = 5; i < 8; i++)
            {
                disabledDates.Add(DateTime.Now.AddDays(i));
            }

            // Customize
            _caldroidFragment.MinDate = minDate;
            _caldroidFragment.MaxDate = maxDate;
            _caldroidFragment.DisabledDate = disabledDates;
            _caldroidFragment.SelectedDate = selectedDates;
            _caldroidFragment.ShowNavigationArrows = false;
            _caldroidFragment.IsSwipeEnabled = false;

            _caldroidFragment.UpdateView();

            // Move to date
            // cal = Calendar.getInstance();
            // cal.add(Calendar.MONTH, 12);
            // caldroidFragment.moveToDate(cal.getTime());

            var text = "Today: " + DateTime.Now.ToShortDateString() + "\n";
            text += "Min Date: " + minDate.ToShortDateString() + "\n";
            text += "Max Date: " + maxDate.ToShortDateString() + "\n";
            text += "Select From Date: " + selectedDates[0].ToShortDateString() + "\n";
            text += "Select To Date: " + selectedDates[1].ToShortDateString() + "\n";
            foreach (var date in disabledDates)
            {
                text += "Disabled Date: " + date.ToShortDateString() + "\n";
            }

            _textView.Text = (text);
        }


        void ShowDialogButton_Click(object sender, EventArgs e)
        {
            // Setup caldroid to use as dialog
            _dialogCaldroidFragment = new CaldroidFragment();
            _dialogCaldroidFragment.AddCaldroidListener(this);

            // If activity is recovered from rotation
            var dialogTag = "CALDROID_DIALOG_FRAGMENT";

            if (_bundleState != null)
            {
                _dialogCaldroidFragment.RestoreDialogStatesFromKey(SupportFragmentManager, _bundleState,
                        "DIALOG_CALDROID_SAVED_STATE", dialogTag);

                Bundle args = _dialogCaldroidFragment.Arguments;

                if (args == null)
                {
                    args = new Bundle();
                    _dialogCaldroidFragment.Arguments = (args);
                }
            }
            else
            {
                // Setup arguments
                Bundle bundle = new Bundle();
                // Setup dialogTitle
                _dialogCaldroidFragment.Arguments = (bundle);
            }

            _dialogCaldroidFragment.Show(SupportFragmentManager, dialogTag);
        }


        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            if (_caldroidFragment != null)
                _caldroidFragment.SaveStatesToKey(outState, "CALDROID_SAVED_STATE");

            if (_dialogCaldroidFragment != null)
                _dialogCaldroidFragment.SaveStatesToKey(outState, "DIALOG_CALDROID_SAVED_STATE");
        }


        private void SetCustomResourceForDates()
        {
            // Min date is last 7 days
            var blueDate = DateTime.Now.AddDays(-7);

            // Max date is next 7 days
            var greenDate = DateTime.Now.AddDays(7);

            if (_caldroidFragment != null)
            {
                ColorDrawable blue = new ColorDrawable(Color.Blue);
                ColorDrawable green = new ColorDrawable(Color.Green);
                _caldroidFragment.SetBackgroundDrawableForDate(blue, blueDate);
                _caldroidFragment.SetBackgroundDrawableForDate(green, greenDate);
                _caldroidFragment.SetTextColorForDate(Resource.Color.white, blueDate);
                _caldroidFragment.SetTextColorForDate(Resource.Color.white, greenDate);
            }
        }

        public void OnSelectDate(DateTime date, View view)
        {
            Toast.MakeText(ApplicationContext, date.ToShortDateString(), ToastLength.Short).Show();
        }

        public void OnLongClickDate(DateTime date, View view)
        {
            Toast.MakeText(ApplicationContext, "Long click " + date.ToShortDateString(), ToastLength.Short).Show();
        }

        public void OnChangeMonth(int year, int month)
        {
            var text = "month: " + month + " year: " + year;
            Toast.MakeText(ApplicationContext, text, ToastLength.Short).Show();
        }

        public void OnCalendarViewCreated()
        {
            Toast.MakeText(ApplicationContext, "Caldroid view is created", ToastLength.Short).Show();
        }
    }
}

