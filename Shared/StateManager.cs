using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    public class StateManager
    {
        Dictionary<GameState, IState> gameStates;
        IState currentGameState;

        public StateManager()
        {
            gameStates = new Dictionary<GameState, IState>();
            currentGameState = null;
        }

        public void AddGameState(GameState name, IState state)
        {
            gameStates[name] = state;
        }

        public IState GetGameState(GameState name)
        {
            return gameStates[name];
        }

        public void SwitchTo(GameState name, IState newstate = null, params object[] args)
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

        public IState CurrentGameState
        {
            get
            {
                return currentGameState;
            }
        }

        public GameState State { get; private set; }

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
    public enum GameState { MainMenu,OnStage,
        EditMode,
        SaveLevel,
        SelectLevel,
        DeleteLevel
    }
}
