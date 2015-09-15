using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    internal class StateManager
    {
        Dictionary<GameState, IState> gameStates;
        IState currentGameState;

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
            State = name;
            if (gameStates.ContainsKey(name))
            {
                if (newstate != null) currentGameState = gameStates[name] = newstate;
                else currentGameState = gameStates[name];
                currentGameState.OnActivated(args);
            }
            else
                throw new KeyNotFoundException("Could not find game state: " + name);
        }

        internal IState CurrentGameState
        {
            get
            {
                return currentGameState;
            }
        }

        internal GameState State { get; private set; }

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
    internal enum GameState { MainMenu,OnStage,
        EditMode,
        SaveLevel,
        SelectLevel,
        DeleteLevel,
        Options
    }
}
