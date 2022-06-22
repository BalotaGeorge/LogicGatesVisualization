using System;
using System.Collections.Generic;

namespace LogicGatesVisualization
{
    public enum GateType
    {
        IN,
        OUT,
        JOINT,
        NOT,
        AND,
        OR,
        CIRCUIT
    }

    public abstract class LogicGate
    {
        public GateType GateType;
        public Vector WorldPosition;
        public Vector ScreenPosition;
        public List<Connection> Connections;
        public int Width;
        public int Height;
        public bool Selected;
        public bool State;
        private bool Deleted;

        public LogicGate(GateType GateType, float WorldPositionX, float WorldPositionY)
        {
            this.GateType = GateType;
            WorldPosition = new Vector(WorldPositionX, WorldPositionY);
            ScreenPosition = new Vector();
            Width = Math.Max(200, GateType.ToString().Length * 8 + 140);
            Height = 40;
        }
        public Vector Size()
        {
            return new Vector(Width, Height);
        }
        public bool IsDeleted()
        {
            return Deleted;
        }
        public void MarkToDelete()
        {
            Deleted = true;
        }
        public abstract void UpdateState();
        public virtual void Render(FormGameEngine fge, Vector offset, float scale)
        {
            ScreenPosition = WorldPosition.WorldToScreen(offset, scale);

            foreach (var connection in Connections)
            {
                connection.ScreenPosition = (WorldPosition + connection.RelativePosition).WorldToScreen(offset, scale);
                fge.DrawThickLine(connection.ScreenPosition, connection.ScreenPosition + 
                    Vector.New((connection.WorksAsInput ? 1 : -1) * 40 * scale, 0), Math.Max((int)scale, 1), Pixel.Black);
                connection.Render(fge, scale);
            }

            fge.DrawThickRectangle(ScreenPosition, (int)((Width - 80) * scale), (int)(Height * scale), 
                Math.Max((int)scale, 1), Pixel.Black, PositionMode.Center);
        }
        public void RenderExtra(FormGameEngine fge, float scale)
        {
            RenderText(fge, scale);
            RenderSelected(fge, scale);
        }
        public virtual void RenderText(FormGameEngine fge, float scale)
        {
            fge.DrawString(ScreenPosition, GateType.ToString(), Pixel.Black, Math.Max((int)scale, 1), PositionMode.Center);
        }
        public virtual void RenderSelected(FormGameEngine fge, float scale)
        {
            if (Selected)
            {
                fge.DrawRectangle(ScreenPosition, (int)(Width * scale), (int)(Height * scale), Pixel.DarkGreen, PositionMode.Center);
            }
        }
    }
}
