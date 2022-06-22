using System;
using System.Collections.Generic;

namespace LogicGatesVisualization
{
    public class JOINT : LogicGate
    {
        public Connection Input;
        public Connection Output;
        public JOINT(float WorldPositionX, float WorldPositionY) : base(GateType.JOINT, WorldPositionX, WorldPositionY)
        {
            Height = 30;
            Width = 60;
            Input = new Connection(-Width * 0.25f, 0, true) { DisplaySize = 15 };
            Output = new Connection(Width * 0.25f, 0f, false) { DisplaySize = 15 };
            Connections = new List<Connection> { Input, Output };
        }
        public override void UpdateState()
        {
            State = Input.State;
            Output.State = State;
        }
        public override void Render(FormGameEngine fge, Vector offset, float scale)
        {
            UpdateState();
            ScreenPosition = WorldPosition.WorldToScreen(offset, scale);

            foreach (var connection in Connections)
            {
                connection.ScreenPosition = (WorldPosition + connection.RelativePosition).WorldToScreen(offset, scale);
                connection.Render(fge, scale);
            }

            fge.DrawThickLine(Input.ScreenPosition, Output.ScreenPosition, Math.Max((int)scale * 5, 1), State ? Pixel.Green : Pixel.Red);
        }
        public override void RenderText(FormGameEngine fge, float scale)
        {
            
        }
    }
}
