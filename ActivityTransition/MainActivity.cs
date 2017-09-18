using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Android.Resource;
using Android.Database;
using Android.Views;
using Java.Lang;
using System;

namespace ActivityTransition
{
    [Activity(Label = "ActivityTransition", MainLauncher = true, Theme = "@style/MyTransitionTheme")]
    public class MainActivity : Activity
    {
        Spinner spinOutTransactions = null;
        Spinner spinInTransactions = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            spinOutTransactions = FindViewById<Spinner>(Resource.Id.spinOutTransactions);
            spinInTransactions = FindViewById<Spinner>(Resource.Id.spinInTransactions);

            LoadSpinnerData(spinOutTransactions);
            LoadSpinnerData(spinInTransactions);

            FindViewById<Button>(Resource.Id.btnRun).Click += RunTransition;
        }

        private void RunTransition(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($" Out: { spinOutTransactions.SelectedItem.Cast<TransitionType>().TransictionName}");
            System.Diagnostics.Debug.WriteLine($" in: { spinInTransactions.SelectedItem.Cast<TransitionType>().TransictionName}");
            var intent = new Android.Content.Intent(this, typeof(ResultActivity));
            intent.PutExtra("OutTransactions", spinOutTransactions.SelectedItem.Cast<TransitionType>().TransictionName);
            intent.PutExtra("InTransactions", spinInTransactions.SelectedItem.Cast<TransitionType>().TransictionName);
            StartActivity(intent);
            OverridePendingTransition(spinInTransactions.SelectedItem.Cast<TransitionType>().IDTransiction, spinOutTransactions.SelectedItem.Cast<TransitionType>().IDTransiction);
        }

        private async void LoadSpinnerData(Spinner spinner)
        {
            List<TransitionType> _transitions = new List<TransitionType>() { new TransitionType() { IDTransiction = 0, TransictionName = this.GetString(Resource.String.NoTransitionSpinnerText) } };

            List<TransitionType> _systemTransitions = typeof(Resource.Animation).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList().Select(x => new TransitionType() { IDTransiction = (int)x.GetValue(x), TransictionName = x.Name }).ToList();

            _transitions.AddRange(_systemTransitions);

            var transitionAdapter = new TransitionAdapter(this, _transitions.ToArray());

            spinner.Adapter = transitionAdapter;
        }
    }
    public class TransitionAdapter : BaseAdapter<TransitionType>
    {
        List<TransitionType> items;
        Activity context;
        public TransitionAdapter(Activity context, IEnumerable<TransitionType> items)
        : base()
        {
            this.context = context;
            this.items = items.ToList();
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override TransitionType this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
            {
                view = context.LayoutInflater.Inflate(Layout.SimpleListItem1, null);
            }
            view.FindViewById<TextView>(Id.Text1).Text = items[position].TransictionName;
            return view;
        }
    }
    public class TransitionType
    {
        public int IDTransiction { get; set; }
        public string TransictionName { get; set; }
    }
    public static class ObjectTypeHelper
    {
        public static T Cast<T>(this Java.Lang.Object obj) where T : class
        {
            var propertyInfo = obj.GetType().GetProperty("Instance");
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
        }
    }
}