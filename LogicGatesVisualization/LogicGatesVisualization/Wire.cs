using System.Collections.Generic;

namespace LogicGatesVisualization
{
    public class Wire
    {
        public LogicGate FromGate;
        public LogicGate ToGate;
        public Connection FromConnection;
        public Connection ToConnection;
        public Wire ParentWire;
        public Vector DerivedPin;
        public bool DerivedWire;
        public bool State;
        public float t;

        private bool Deleted;

        public Wire(LogicGate FromGate, LogicGate ToGate, Connection FromConnection, Connection ToConnection)
        {
            this.FromGate = FromGate;
            this.ToGate = ToGate;
            this.FromConnection = FromConnection;
            this.ToConnection = ToConnection;
        }
        public bool IsDeleted()
        {
            return Deleted;
        }
        public Line GetLine()
        {
            return new Line(DerivedWire ? DerivedPin : FromConnection.ScreenPosition, ToConnection.ScreenPosition);
        }
        public void MarkToDelete(List<Wire> wires)
        {
            Deleted = true;
            ToConnection.IsConnected = false;
            ToConnection.State = false;
            if (!DerivedWire)
                FromConnection.IsConnected = false;
            wires.ForEach(wire =>
            {
                if (wire.ParentWire == this)
                    wire.MarkToDelete(wires);
            });
        }
        public void Calculate()
        {
            if (ParentWire.DerivedPin != null)
                t = (DerivedPin - ParentWire.DerivedPin).Magnitude() /
                    (ParentWire.ToConnection.ScreenPosition - ParentWire.DerivedPin).Magnitude();
            else
                t = (DerivedPin - ParentWire.FromConnection.ScreenPosition).Magnitude() /
                    (ParentWire.ToConnection.ScreenPosition - ParentWire.FromConnection.ScreenPosition).Magnitude();
        }
        public void UpdateState()
        {
            State = FromConnection.State;
            ToConnection.State = State;
        }
        public void Render(FormGameEngine fge, float scale)
        {
            UpdateState();

            if (DerivedWire && ParentWire != null)
            {
                if (ParentWire.DerivedPin != null)
                    DerivedPin = (ParentWire.ToConnection.ScreenPosition - ParentWire.DerivedPin) * t + ParentWire.DerivedPin;
                else
                    DerivedPin = (ParentWire.ToConnection.ScreenPosition - ParentWire.FromConnection.ScreenPosition) * t + 
                        ParentWire.FromConnection.ScreenPosition;

                fge.DrawThickLine(DerivedPin, ToConnection.ScreenPosition, scale * 5, State ? Pixel.Green : Pixel.Red);
            }
            else
            {
                fge.DrawThickLine(FromConnection.ScreenPosition, ToConnection.ScreenPosition, scale * 5, State ? Pixel.Green : Pixel.Red);
            }
        }
    }
}
