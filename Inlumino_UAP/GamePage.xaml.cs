using System;
using Inlumino_SHARED;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Inlumino_UAP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        readonly Game _game;

        public GamePage()
        {
            this.InitializeComponent();

            // Create the game.
            var launchArguments = string.Empty;
            _game = MonoGame.Framework.XamlGame<Game>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);
            Manager.StateManager.StateChanged += gamestatechanged;
            //SystemNavigationManager.GetForCurrentView().BackRequested += backpressed; This is handled in Game.Update()
            VirtualKeyboard.VisibilityChanged += vkvisib;
        }

        private void vkvisib(bool obj)
        {
            gamestatechanged(Manager.StateManager.State);
        }

        private void backpressed(object sender, BackRequestedEventArgs e)
        {
            e.Handled = Manager.StateManager.SwitchBack();
        }

        internal void gamestatechanged(GameState newstate)
        {
            (this.FindName("loginButton") as Facebook.Client.Controls.LoginButton).Visibility = (newstate == GameState.OnStage || newstate == GameState.EditMode || VirtualKeyboard.IsVisible ? Visibility.Collapsed : Visibility.Visible);
        }
    }
}
