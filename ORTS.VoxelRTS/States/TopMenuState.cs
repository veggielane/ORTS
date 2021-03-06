﻿using System;
using System.Windows.Forms;
using ORTS.Core;
using ORTS.Core.Messaging.Messages;
using ORTS.Core.States;
using ORTS.Core.Timing;
using ORTS.VoxelRTS.GameObjects;
using ORTS.VoxelRTS.Messaging;
using MainMenu = System.Windows.Forms.MainMenu;

namespace ORTS.VoxelRTS.States
{
    public class TopMenuState : IState
    {
        private GameEngine _engine;

        public TopMenuState(GameEngine engine)
        {
            _engine = engine;
        }

        public void Update( TickTime tickTime)
        {

        }

        public void Show()
        {
            _engine.Bus.Add(new ObjectCreationRequest(_engine.Timer.LastTickTime, typeof(TopMenu)));
            
        }
        public void Hide()
        {
            
        }

        public void Destroy()
        {

        }

        public void KeyUp(KeyUpMessage m)
        {
            if (m.Key == Keys.Space)
            {
                _engine.Bus.Add(new ObjectsDestroyAll(_engine.Timer.LastTickTime));
                _engine.CurrentState = new MainState(_engine);
            }
                
        }

        public void KeyPress(KeyPressMessage m)
        {
            throw new NotImplementedException();
        }

        public void MouseMove(MouseMoveMessage m)
        {
            throw new NotImplementedException();
        }

        public void MouseButtonDown(MouseButtonDownMessage m)
        {
            throw new NotImplementedException();
        }

        public void MouseButtonUp(MouseButtonUpMessage m)
        {
            throw new NotImplementedException();
        }

        public void MouseWheelChanged(MouseWheelChangedMessage m)
        {
            throw new NotImplementedException();
        }

        public void KeyDown(KeyDownMessage m)
        {

        }
    }
}
