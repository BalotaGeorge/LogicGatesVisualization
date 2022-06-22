using System.Collections.Generic;

namespace LogicGatesVisualization
{
    public class OUT : LogicGate
    {
        public Connection Input;
        public OUT(float WorldPositionX, float WorldPositionY) : base(GateType.OUT, WorldPositionX, WorldPositionY)
        {
            Width = Height = 60;
            Input = new Connection(0f, 0f, true) { DisplaySize = 30 };
            Connections = new List<Connection> { Input };
        }
        public override void UpdateState()
        {
            State = Input.State;
        }
        public override void Render(FormGameEngine fge, Vector offset, float scale)
        {
            UpdateState();
            ScreenPosition = WorldPosition.WorldToScreen(offset, scale);
            Input.ScreenPosition = (WorldPosition + Input.RelativePosition).WorldToScreen(offset, scale);
            Input.Render(fge, scale);
        }
    }
}
