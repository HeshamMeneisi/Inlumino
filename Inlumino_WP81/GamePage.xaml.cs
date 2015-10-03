using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;
using Inlumino_SHARED;
using Windows.Phone.UI.Input;
using System;
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

        private void backpressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = Manager.StateManager.SwitchBack();
        }

        public GamePage(string launchArguments)
        {
            this.InitializeComponent();
            _game = XamlGame<Game>.Create(launchArguments, Window.Current.CoreWindow, this);
            Manager.StateManager.StateChanged += gamestatechanged;
            // HardwareButtons.BackPressed += backpressed; This is handled in Game.Update()
            VirtualKeyboard.VisibilityChanged += vkvisib;
        }

        private void vkvisib(bool obj)
        {
            gamestatechanged(Manager.StateManager.State);
        }

        private void gamestatechanged(GameState newstate)
        {
            (this.FindName("loginButton") as Facebook.Client.Controls.LoginButton).Visibility = (newstate == GameState.OnStage || newstate == GameState.EditMode || VirtualKeyboard.IsVisible ? Visibility.Collapsed : Visibility.Visible);
        }
    }
}
