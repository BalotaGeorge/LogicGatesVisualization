namespace LogicGatesVisualization
{
    public class Connection
    {
        public Vector RelativePosition;
        public Vector ScreenPosition;
        public int DisplaySize;
        public bool IsConnected;
        public bool WorksAsInput;
        public bool State;
        public bool Deleted;
        public Connection(float RelativePositionX, float RelativePositionY, bool WorksAsInput)
        {
            DisplaySize = 20;
            RelativePosition = new Vector(RelativePositionX, RelativePositionY);
            ScreenPosition = new Vector();
            this.WorksAsInput = WorksAsInput;
        }
        public void Render(FormGameEngine fge, float scale)
        {
            fge.FillCircle(ScreenPosition, (int)(DisplaySize * scale), State ? Pixel.Green : Pixel.Red);
        }
    }
}
