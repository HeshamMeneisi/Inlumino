using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    internal class StateManager
    {
        Dictionary<GameState, IState> gameStates;
        IState currentGameState;
        Stack<GameState> statetracker = new Stack<GameState>();
        GameState basestate;
        internal StateManager()
        {
            gameStates = new Dictionary<GameState, IState>();
            currentGameState = null;
        }

        internal void AddGameState(GameState name, IState state)
        {
            gameStates[name] = state;
        }

        internal IState GetGameState(GameState name)
        {
            return gameStates[name];
        }

        internal void SwitchTo(GameState name, IState newstate = null, params object[] args)
        {
            TrackState(name);
            if (gameStates.ContainsKey(name))
            {
                VirtualKeyboard.Hide();
                if (newstate != null) currentGameState = gameStates[name] = newstate;
                else currentGameState = gameStates[name];                
                currentGameState.OnActivated(args);
                OnStateChanged();
            }
            else
                throw new KeyNotFoundException("Could not find game state: " + name);
        }

        private void TrackState(GameState state)
        {
            if (statetracker.Count == 0)
                basestate = state;
            else if (state == basestate)
                statetracker.Clear();
            statetracker.Push(state);
        }
        internal void ResetBaseState(GameState state)
        {
            basestate = state;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whether or not back was handled.</returns>
        internal bool SwitchBack()
        {
            if(statetracker.Count > 1)
            {
                statetracker.Pop();
                SwitchTo(statetracker.Pop());
                return true;
            }
            return false;
        }
        private void OnStateChanged()
        {
            if (StateChanged != null) StateChanged(State);
        }

        internal IState CurrentGameState
        {
            get
            {
                return currentGameState;
            }
        }

        internal GameState State { get { return statetracker.Peek(); } }
        public Action<GameState> StateChanged { get; internal set; }

        public void Update(GameTime gameTime)
        {
            if (currentGameState != null)
                currentGameState.Update(gameTime);
        }

        public void Draw(SpriteBatch batch)
        {
            if (currentGameState != null)
                currentGameState.Draw(batch);
        }
    }
    internal enum GameState
    {
        MainMenu, OnStage,
        EditMode,
        SaveLevel,
        SelectLevel,
        Options,
        PackageSelector,
        SaveLevelCloud
    }
}
