﻿using System;
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
            _game = XamlGame<Game>.Create(launchArguments, Window.Current.CoreWindow, this);
        }
    }
}