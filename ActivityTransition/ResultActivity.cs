using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;

namespace ActivityTransition
{
    [Activity(Label = "ActivityTransition")]
    public class ResultActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Result);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);


            var OutTransactions = Intent.GetStringExtra("OutTransactions") ?? string.Empty;
            var InTransactions = Intent.GetStringExtra("InTransactions") ?? string.Empty;
            try
            {
                FindViewById<EditText>(Resource.Id.choosedInTransition).Text = LoadFile((int)typeof(Resource.Raw).GetField(Intent.GetStringExtra("InTransactions")).GetValue(null));
                FindViewById<EditText>(Resource.Id.choosedInTransition).LongClick += CopyToClipboard;
            }
            catch
            {
                FindViewById<EditText>(Resource.Id.choosedInTransition).Text = GetString(Resource.String.NoTransitionSpinnerText);
                FindViewById<EditText>(Resource.Id.choosedInTransition).Enabled = false;
            }
            try
            {
                FindViewById<EditText>(Resource.Id.choosedOutTransition).Text = LoadFile((int)typeof(Resource.Raw).GetField(Intent.GetStringExtra("OutTransactions")).GetValue(null));
                FindViewById<EditText>(Resource.Id.choosedOutTransition).LongClick += CopyToClipboard;
            }
            catch
            {
                FindViewById<EditText>(Resource.Id.choosedOutTransition).Text = GetString(Resource.String.NoTransitionSpinnerText);
                FindViewById<EditText>(Resource.Id.choosedOutTransition).Enabled = false;
            }
        }

        private void CopyToClipboard(object sender, System.EventArgs e)
        {
            ClipboardManager clipboard = (ClipboardManager)GetSystemService(Context.ClipboardService);
            ClipData clip = ClipData.NewPlainText(((TextView)sender).Id.ToString(), ((TextView)sender).Text);
            clipboard.PrimaryClip = clip;
            Toast.MakeText(this, GetString(Resource.String.AddedToClipboard), ToastLength.Short).Show();
        }

        public string LoadFile(int resourceId)
        {
            //var a = Resources.GetAnimation(resourceId);
            ////get the file as a stream  
            //var inputStream = Resources.GetXml(resourceId);
            var stringContent = string.Empty;
            if (resourceId > 0)
            {
                using (StreamReader reader = new StreamReader(Resources.OpenRawResource(resourceId)))
                {
                    stringContent = reader.ReadToEnd();
                }
            }
            return stringContent;
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // set the menu layout on Main Activity  
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    {
                        Finish();
                        return true;
                    }
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}