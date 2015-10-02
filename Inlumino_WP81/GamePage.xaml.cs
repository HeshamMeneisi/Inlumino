using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MonoGame.Framework;
using Inlumino_SHARED;
using Facebook.Client;
using Windows.ApplicationModel.Activation;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Inlumino_WP81
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : SwapChainBackgroundPanel
    {
        readonly Game _game;
        public GamePage()
        {
            this.InitializeComponent();
        }

        public GamePage(string launchArguments)
        {
            this.InitializeComponent();
            _game = XamlGame<Game>.Create(launchArguments, Window.Current.CoreWindow, this);
            Manager.StateManager.StateChanged += gamestatechanged;
        }

        private void gamestatechanged(GameState newstate)
        {
            (this.FindName("loginButton") as Facebook.Client.Controls.LoginButton).Visibility = (newstate == GameState.OnStage || newstate == GameState.EditMode) ? Visibility.Collapsed : Visibility.Visible;
        }        
    }
}
