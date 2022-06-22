using System.Collections.Generic;
using System.Linq;

namespace LogicGatesVisualization
{
    public static class PlacingSystem
    {
        public static LogicGate PlacingLogicGate;
        static FormGameEngine fge;
        static List<ButtonUI> buttons;

        public static void Initialize(FormGameEngine fge)
        {
            PlacingSystem.fge = fge;

            PlacingLogicGate = null;

            var defaultGates = new List<string> { "IN", "OUT", "JOINT", "NOT", "AND", "OR" };
            defaultGates.AddRange(StoreCircuitsSystem.SavedCircuits.Select(circuit => circuit.Name));
            
            var offsetX = 0;
            var offsetY = 0;
            var padding = 20;

            buttons = new List<ButtonUI>();
            defaultGates.ForEach(gate =>
            {
                if (offsetX + gate.Length * 8 + padding + 1 > fge.ScreenWidth())
                {
                    offsetX = 0;
                    offsetY += 40;
                }
                buttons.Add(new ButtonUI(offsetX, offsetY, gate.Length * 8 + padding, 39, gate));
                offsetX += gate.Length * 8 + padding + 1;
            });
        }

        public static void ProcessAndRender(Vector worldMouse, Vector offset, float scale)
        {
            foreach (var button in buttons)
            {
                if (button.clicked)
                {
                    PlacingLogicGate = CreatePlacingLogicGate(worldMouse, button.text);
                    break;
                }
            }

            if (PlacingLogicGate != null)
            {
                PlacingLogicGate.WorldPosition = worldMouse.Clone();
                PlacingLogicGate.Render(fge, offset, scale);
                PlacingLogicGate.RenderExtra(fge, scale);
            }

            foreach (var button in buttons)
                button.Render(fge);
        }

        private static LogicGate CreatePlacingLogicGate(Vector position, string name)
        {
            switch (name)
            {
                case "IN": return new IN(position.x, position.y);
                case "OUT": return new OUT(position.x, position.y);
                case "JOINT": return new JOINT(position.x, position.y);
                case "NOT": return new NOT(position.x, position.y);
                case "AND": return new AND(position.x, position.y);
                case "OR": return new OR(position.x, position.y);
                default:
                    {
                        return StoreCircuitsSystem.GetCircuitByName(name);
                    };
            }
        }
    }
}
