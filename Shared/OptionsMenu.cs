using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Parse;
using Microsoft.Xna.Framework.Input;

namespace Inlumino_SHARED
{
    internal class OptionsMenu : IState
    {
        UIMenu menu;
        UIButton savebtn;
        UIMenu loginmenu;
        UITextField email;
        UITextField password;
        UIButton login;
        internal OptionsMenu()
        {
            menu = new UIMenu();
            loginmenu = new UIMenu();
            // Need sliders for music & sfx volume
            savebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.SaveButton], savepressed);
            menu.Add(savebtn);

            email = new UITextField(254, Color.White, Color.Black, "Email (Signup/Login)");
            password = new UITextField(12, Color.White, Color.Black, ParseUser.CurrentUser == null ? "Passphrase(Max 12)" : "Logged in!");
            //password.IsPassword = true;   Password only visible at login/signup no need for this
            //login = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.SaveButton], loginpressed);

            loginmenu.Add(email);
            loginmenu.Add(password);
            //loginmenu.Add(login);
        }

        private void loginpressed(UIButton sender)
        {
            throw new NotImplementedException();
        }

        private async void savepressed(UIButton sender)
        {            
            savebtn.Visible = false;            
            //Manager.GameSettings.MusicVolume =
            //Manager.GameSettings.SFXVolume =
            Manager.SaveSettings();
            if (ParseUser.CurrentUser != null && ParseUser.CurrentUser.IsAuthenticated && ParseUser.CurrentUser.Username == email.Text) goto skip;
            if (email.Text == "" || password.Text == "") goto skip;
            // Login/Create
            bool found = await (from user in ParseUser.Query
                                where user.Email == email.Text
                                select user).CountAsync() > 0;
            if (found)
            {
                try
                {
                    await ParseUser.LogInAsync(email.Text, password.Text);
                    // Login was successful.
                    await MessageBox.Show("Login successful!", "Welcome back " + email.Text.Split('@')[0] + "!", new string[] { "OK" });
                }
                catch (Exception e)
                {
                    // The login failed. Check the error to see why.
                    int? r = await MessageBox.Show("Login Failed", e.Message, new string[] { "OK","Forgot password" });
                    if (r == 1)
                    {
                        await ParseUser.RequestPasswordResetAsync(email.Text);
                        await MessageBox.Show("Password reset", "You have been sent a password reset email.", new string[] { "OK" });
                    }
                    goto dontquit;
                }
            }
            else
            {
                if(password.Text.Length < 6)
                {
                    await MessageBox.Show("Password", "Password must be 6-12 letters.", new string[] { "OK" });
                    goto dontquit;
                }
                var user = new ParseUser()
                {
                    Username = email.Text,
                    Password = password.Text,
                    Email = email.Text
                };

#if WINDOWS_UAP
                user["signupsys"] = "WINDOWS";
#else
#if ANDROID
                user["signupsys"] = "ANDROID";
#else
                user["signupsys"] = "ELSE";
#endif
#endif                             
                try
                {
                    await user.SignUpAsync();
                    await MessageBox.Show("Signed Up!", "Please check your mail for confirmation code.", new string[] { "OK" });
                }
                catch (Exception e)
                {
                    // The signup failed. Check the error to see why.
                    await MessageBox.Show("Error", e.Message, new string[] { "OK" });
                    goto dontquit;
                }
            }
        skip:
            Manager.StateManager.SwitchTo(GameState.MainMenu);
            Manager.SyncData();
        dontquit:;
        }

        private void SetupMenu()
        {
            float v = Screen.Height * 0.2f;
            email.Size = password.Size = new Vector2(Screen.Width, v);
            loginmenu.ArrangeInForm(Orientation.Portrait);            
            savebtn.setSizeRelative(0.15f * (Screen.Mode == Orientation.Portrait ? 2 : 1), Screen.Mode);
            savebtn.Position = new Vector2((Screen.Width - savebtn.Size.X) / 2, Screen.Height - savebtn.BoundingBox.Height);
        }
        public void Draw(SpriteBatch batch)
        {
            loginmenu.Draw(batch);
            menu.Draw(batch);
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            if (e is DisplaySizeChangedEvent || e is OrientationChangedEvent)
                SetupMenu();
            menu.HandleEvent(e);
            loginmenu.HandleEvent(e);
        }

        public void OnActivated(params object[] args)
        {
            savebtn.Visible = true;
            if (ParseUser.CurrentUser != null && ParseUser.CurrentUser.IsAuthenticated)
            {
                email.Text = ParseUser.CurrentUser.Username;
                password.Text = "";
            }
            SetupMenu();
        }

        public void Update(GameTime time)
        {
            loginmenu.Update(time);
            menu.Update(time);
        }
    }
}