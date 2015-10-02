using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Inlumino_SHARED;
using Android.Content;
using Android.Graphics;
using Android.Widget;
using Xamarin.Facebook;
using Xamarin.Facebook.Share.Widget;
using Xamarin.Facebook.Login.Widget;
using Xamarin.Facebook.AppEvents;
using Xamarin.Facebook.Share.Model;
using Xamarin.Facebook.Share;
using Xamarin.Facebook.Login;
using System.Collections.Generic;

[assembly: Permission(Name = Android.Manifest.Permission.Internet)]
[assembly: Permission(Name = Android.Manifest.Permission.WriteExternalStorage)]
[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/app_id")]
[assembly: MetaData("com.facebook.sdk.ApplicationName", Value = "@string/app_name")]
namespace Inlumino_ANDROID
{
    [Activity(Label = "Inlumino"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = Android.Content.PM.LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.SensorPortrait
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class Activity : Microsoft.Xna.Framework.AndroidGameActivity
    {
        static readonly string[] PERMISSIONS = new[] { "publish_actions" };

        const String PENDING_ACTION_BUNDLE_KEY = "com.facebook.samples.hellofacebook:PendingAction";
        Button backBtn;
        ProfilePictureView profilePictureView;
        TextView greeting;
        PendingAction pendingAction = PendingAction.NONE;
        bool canPresentShareDialog;
        bool canPresentShareDialogWithPhotos;
        ICallbackManager callbackManager;
        ProfileTracker profileTracker;
        ShareDialog shareDialog;
        FacebookCallback<SharerResult> shareCallback;

        enum PendingAction
        {
            NONE,
            POST_PHOTO,
            POST_STATUS_UPDATE
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidEnvironment.UnhandledExceptionRaiser += HandleAndroidException;

            base.OnCreate(savedInstanceState);

            FacebookSdk.SdkInitialize(this.ApplicationContext);

            callbackManager = CallbackManagerFactory.Create();
            
            var loginCallback = new FacebookCallback<LoginResult>
            {
                HandleSuccess = loginResult =>
                {
                    HandlePendingAction();
                    UpdateUI();
                    Common.FBLoggedIn(Profile.CurrentProfile.Id,AccessToken.CurrentAccessToken.Token,Common.JavaDateToDateTime(AccessToken.CurrentAccessToken.Expires));                    
                },
                HandleCancel = () =>
                {
                    if (pendingAction != PendingAction.NONE)
                    {
                        ShowAlert(
                            GetString(Resource.String.cancelled),
                            GetString(Resource.String.permission_not_granted));
                        pendingAction = PendingAction.NONE;
                    }
                    UpdateUI();
                },
                HandleError = loginError =>
                {
                    if (pendingAction != PendingAction.NONE
                        && loginError is FacebookAuthorizationException)
                    {
                        ShowAlert(
                            GetString(Resource.String.cancelled),
                            GetString(Resource.String.permission_not_granted));
                        pendingAction = PendingAction.NONE;
                    }
                    UpdateUI();
                }
            };

            LoginManager.Instance.RegisterCallback(callbackManager, loginCallback);

            shareCallback = new FacebookCallback<SharerResult>
            {
                HandleSuccess = shareResult =>
                {
                    Console.WriteLine("HelloFacebook: Success!");

                    if (shareResult.PostId != null)
                    {
                        var title = Parent.GetString(Resource.String.error);
                        var id = shareResult.PostId;
                        var alertMsg = Parent.GetString(Resource.String.successfully_posted_post, id);

                        ShowAlert(title, alertMsg);
                    }
                },
                HandleCancel = () =>
                {
                    Console.WriteLine("HelloFacebook: Canceled");
                },
                HandleError = shareError =>
                {
                    Console.WriteLine("HelloFacebook: Error: {0}", shareError);

                    var title = Parent.GetString(Resource.String.error);
                    var alertMsg = shareError.Message;

                    ShowAlert(title, alertMsg);
                }
            };

            shareDialog = new ShareDialog(this);
            shareDialog.RegisterCallback(callbackManager, shareCallback);

            if (savedInstanceState != null)
            {
                var name = savedInstanceState.GetString(PENDING_ACTION_BUNDLE_KEY);
                pendingAction = (PendingAction)Enum.Parse(typeof(PendingAction), name);
            }

            g = new Game();
            SetContentView((View)g.Services.GetService(typeof(View)));
            g.Run();
            Common.HandleFB += SwitchToFBPage;
        }
        Game g;
        bool fbinit = false;
        public void SwitchToFBPage()
        {
            SetContentView(Resource.Layout.main);
            if (!fbinit)
            {
                var loginbtn = FindViewById<LoginButton>(Resource.Id.loginBtn);
                loginbtn.SetReadPermissions("email");
                fbinit = true;
                profileTracker = new CustomProfileTracker
                {
                    HandleCurrentProfileChanged = (oldProfile, currentProfile) =>
                    {
                        UpdateUI();
                        HandlePendingAction();
                    }
                };

                profilePictureView = FindViewById<ProfilePictureView>(Resource.Id.profilePicture);

                greeting = FindViewById<TextView>(Resource.Id.greeting);

                //post
                //PerformPublish(PendingAction.POST_STATUS_UPDATE, canPresentShareDialog);

                backBtn = FindViewById<Button>(Resource.Id.backBtn);
                backBtn.Click += (sender, e) =>
                {
                    SwitchToGame();
                };
                //post with photo
                //PerformPublish(PendingAction.POST_PHOTO, canPresentShareDialogWithPhotos);                

                // Can we present the share dialog for regular links?
                canPresentShareDialog = ShareDialog.CanShow(Java.Lang.Class.FromType(typeof(ShareLinkContent)));

                // Can we present the share dialog for photos?
                canPresentShareDialogWithPhotos = ShareDialog.CanShow(Java.Lang.Class.FromType(typeof(SharePhotoContent)));
                UpdateUI();
            }
        }
        internal void SwitchToGame()
        {
            SetContentView((View)g.Services.GetService(typeof(View)));
            fbinit = false;
        }
        private void HandleAndroidException(object sender, RaiseThrowableEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                throw new Exception(e.Exception.Message + "\n" + e.Exception.StackTrace);
            }
            else
            {
                DataHandler.SaveData<string>(e.Exception.Message + "\n" + e.Exception.StackTrace, "log_" + this.Handle);
            }
        }


        void ShowAlert(string title, string msg, string buttonText = null)
        {
            new AlertDialog.Builder(Parent)
                .SetTitle(title)
                .SetMessage(msg)
                .SetPositiveButton(buttonText, (s2, e2) => { })
                .Show();
        }

        protected override void OnResume()
        {
            base.OnResume();

            AppEventsLogger.ActivateApp(this);

            if (fbinit)
                UpdateUI();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            outState.PutString(PENDING_ACTION_BUNDLE_KEY, pendingAction.ToString());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        protected override void OnPause()
        {
            base.OnPause();

            AppEventsLogger.DeactivateApp(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            profileTracker.StopTracking();
        }

        private void UpdateUI()
        {
            var enableButtons = AccessToken.CurrentAccessToken != null;

            //postStatusUpdateButton.Enabled = (enableButtons || canPresentShareDialog);
            //postPhotoButton.Enabled = (enableButtons || canPresentShareDialogWithPhotos);            

            var profile = Profile.CurrentProfile;

            if (enableButtons && profile != null)
            {
                profilePictureView.ProfileId = profile.Id;
                greeting.Text = GetString(Resource.String.hello_user, new Java.Lang.String(profile.FirstName));
            }
            else
            {
                profilePictureView.ProfileId = null;
                greeting.Text = null;
            }
        }

        private void HandlePendingAction()
        {
            PendingAction previouslyPendingAction = pendingAction;
            // These actions may re-set pendingAction if they are still pending, but we assume they
            // will succeed.
            pendingAction = PendingAction.NONE;

            switch (previouslyPendingAction)
            {
                case PendingAction.POST_PHOTO:
                    PostPhoto();
                    break;
                case PendingAction.POST_STATUS_UPDATE:
                    PostStatusUpdate();
                    break;
            }
        }


        void PostStatusUpdate()
        {
            var profile = Profile.CurrentProfile;

            var linkContent = new ShareLinkContent.Builder()
                .SetContentTitle("Hello Facebook")
                .SetContentDescription("The 'Hello Facebook' sample showcases simple Facebook integration")
                .SetContentUrl(Android.Net.Uri.Parse("http://developer.facebook.com/docs/android"))
                .JavaCast<ShareLinkContent.Builder>()
                .Build();

            if (canPresentShareDialog)
                shareDialog.Show(linkContent);
            else if (profile != null && HasPublishPermission())
                ShareApi.Share(linkContent, shareCallback);
            else
                pendingAction = PendingAction.POST_STATUS_UPDATE;

        }
        Bitmap image;
        private void PostPhoto()
        {
            var sharePhoto = new SharePhoto.Builder()
                .SetBitmap(image).Build().JavaCast<SharePhoto>();

            var photos = new List<SharePhoto>();
            photos.Add(sharePhoto);

            var sharePhotoContent = new SharePhotoContent.Builder()
                .SetPhotos(photos).Build();

            if (canPresentShareDialogWithPhotos)
                shareDialog.Show(sharePhotoContent);
            else if (HasPublishPermission())
                ShareApi.Share(sharePhotoContent, shareCallback);
            else
                pendingAction = PendingAction.POST_PHOTO;
        }

        bool HasPublishPermission()
        {
            var accessToken = AccessToken.CurrentAccessToken;
            return accessToken != null && accessToken.Permissions.Contains("publish_actions");
        }

        void PerformPublish(PendingAction action, bool allowNoToken)
        {
            var accessToken = AccessToken.CurrentAccessToken;

            if (accessToken != null)
            {
                pendingAction = action;
                if (HasPublishPermission())
                {
                    HandlePendingAction();
                    return;
                }
                else
                {
                    LoginManager.Instance.LogInWithPublishPermissions(this, PERMISSIONS);
                    return;
                }
            }

            if (allowNoToken)
            {
                pendingAction = action;
                HandlePendingAction();
            }
        }

        class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
        {
            public Action HandleCancel { get; set; }
            public Action<FacebookException> HandleError { get; set; }
            public Action<TResult> HandleSuccess { get; set; }

            public void OnCancel()
            {
                var c = HandleCancel;
                if (c != null)
                    c();
            }

            public void OnError(FacebookException error)
            {
                var c = HandleError;
                if (c != null)
                    c(error);
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                var c = HandleSuccess;
                if (c != null)
                    c(result.JavaCast<TResult>());
            }
        }

        class CustomProfileTracker : ProfileTracker
        {
            public delegate void CurrentProfileChangedDelegate(Profile oldProfile, Profile currentProfile);

            public CurrentProfileChangedDelegate HandleCurrentProfileChanged { get; set; }

            protected override void OnCurrentProfileChanged(Profile oldProfile, Profile currentProfile)
            {
                var p = HandleCurrentProfileChanged;
                if (p != null)
                    p(oldProfile, currentProfile);
            }
        }
    }
}

