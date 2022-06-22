using System.Collections.Generic;

namespace LogicGatesVisualization
{
    public class OR : LogicGate
    {
        public Connection InputOne;
        public Connection InputTwo;
        public Connection Output;
        public OR(float WorldPositionX, float WorldPositionY) : base(GateType.OR, WorldPositionX, WorldPositionY)
        {
            Height = 140;
            InputOne = new Connection(-Width * 0.5f, -Height * 0.225f, true);
            InputTwo = new Connection(-Width * 0.5f, Height * 0.225f, true);
            Output = new Connection(Width * 0.5f, 0f, false);
            Connections = new List<Connection> { InputOne, InputTwo, Output };
        }
        public override void UpdateState()
        {
            State = InputOne.State || InputTwo.State;
            Output.State = State;
        }
        public override void Render(FormGameEngine fge, Vector offset, float scale)
        {
            UpdateState();
            base.Render(fge, offset, scale);
        }
    }
}
