namespace LogicGatesVisualization
{
    public static class InfoSystem
    {
        static FormGameEngine fge;
        static ButtonUI infoButton;

        static bool showInfoMessage = false;

        public static void Initialize(FormGameEngine fge)
        {
            InfoSystem.fge = fge;

            infoButton = new ButtonUI(0, fge.WindowHeight() - 40, 40, 40, "?");
        }

        public static void ProcessAndRender()
        {
            if (infoButton.clicked)
            {
                showInfoMessage = !showInfoMessage;
            }

            if (showInfoMessage)
            {
                fge.Clear(Pixel.RGB(230, 230, 230));
                fge.DrawString(10,10,
                "This app helps you learn and visualize the workings of logic gates\n\n" +
                "- Middle-Click to drag the entire plane around\n\n" +
                "- Scroll-Wheel to zoom in and out\n\n" +
                "- Right-Click a gate to move it around or multi selecte\n\n" +
                "  - You can press Delete key to remove the selected ones\n\n" +
                "    from the screen\n\n" +
                "- Left-Click from one connection to another to connect them\n\n" +
                "- Ctrl + A to select everything on the screen\n\n" +
                "- In the top right you can find the default gates and also the custom\n\n" +
                "  created ones,\n\n" +
                "  - After clicking a gate button you can drag it and place it on the\n\n" +
                "    plane with another Left-Click,\n\n" +
                "  - Defocus the gate clicked before you placed it with Right-Click,\n\n" +
                "  - Delete the gate (if it is a custom one) with Delete key or\n\n" +
                "  - Expand its inner components on the plane (if it is a custom one)\n\n" +
                "    with Space key\n\n" +
                "- Keys 1-6 can also be used to place the default gates seen in the left\n\n" +
                "  top corner\n\n" +
                "- Save and reuse the circuit created with the Save button in the right\n\n" +
                "  bottom corner\n\n"
                , Pixel.Black, 2);
            }

            infoButton.Render(fge);
        }
    }
}
