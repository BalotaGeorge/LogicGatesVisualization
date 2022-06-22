using System.Collections.Generic;

namespace LogicGatesVisualization
{
    public class NOT : LogicGate
    {
        public Connection Input;
        public Connection Output;
        public NOT(float WorldPositionX, float WorldPositionY) : base(GateType.NOT, WorldPositionX, WorldPositionY)
        {
            Height = 70;
            Input = new Connection(-Width * 0.5f, 0f, true);
            Output = new Connection(Width * 0.5f, 0f, false);
            Connections = new List<Connection> { Input, Output };
        }
        public override void UpdateState()
        {
            State = !Input.State;
            Output.State = State;
        }
        public override void Render(FormGameEngine fge, Vector offset, float scale)
        {
            UpdateState();
            base.Render(fge, offset, scale);
        }
    }
}
