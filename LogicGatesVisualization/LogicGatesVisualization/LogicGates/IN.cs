using System.Collections.Generic;

namespace LogicGatesVisualization
{
    public class IN : LogicGate
    {
        public Connection Output;
        public IN(float WorldPositionX, float WorldPositionY) : base(GateType.IN, WorldPositionX, WorldPositionY)
        {
            Width = Height = 60;
            Output = new Connection(0f, 0f, false) { DisplaySize = 30 };
            Connections = new List<Connection> { Output };
            State = true;
        }
        public override void UpdateState()
        {
            Output.State = State;
        }
        public override void Render(FormGameEngine fge, Vector offset, float scale)
        {
            UpdateState();
            ScreenPosition = WorldPosition.WorldToScreen(offset, scale);
            Output.ScreenPosition = (WorldPosition + Output.RelativePosition).WorldToScreen(offset, scale);
            Output.Render(fge, scale);
        }
    }
}
