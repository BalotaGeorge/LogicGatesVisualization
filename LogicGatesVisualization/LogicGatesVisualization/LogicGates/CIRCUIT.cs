using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicGatesVisualization
{
    public class CIRCUIT : LogicGate
    {
        public List<LogicGate> LogicGates;
        public List<Wire> Wires;
        public string Name;
        public List<(Connection connection, LogicGate logicGate)> linkedConnections;

        public CIRCUIT(float WorldPositionX, float WorldPositionY, List<LogicGate> LogicGates, List<Wire> Wires, string Name) : base(GateType.CIRCUIT, WorldPositionX, WorldPositionY)
        {
            this.LogicGates = LogicGates;
            this.Wires = Wires;
            this.Name = Name;
            Connections = new List<Connection>();
            linkedConnections = new List<(Connection connection, LogicGate logicGate)>();

            var inputsCount = 0;
            var outputsCount = 0;

            LogicGates = LogicGates.OrderBy(gate =>
            {
                if (gate.GateType == GateType.IN) return -1;
                if (gate.GateType == GateType.OUT) return 1;
                return 0;
            }).ToList();

            LogicGates.Sort((a, b) =>
            {
                if ((a.GateType == GateType.IN || a.GateType == GateType.OUT) && a.GateType == b.GateType) return a.ScreenPosition.iy - b.ScreenPosition.iy;
                return 0;
            });

            LogicGates.ForEach(logicGate =>
            {
                if (logicGate.GateType == GateType.IN)
                {
                    var connection = new Connection(0, 0, true);
                    Connections.Add(connection);
                    linkedConnections.Add((connection, logicGate));
                    inputsCount++;
                }

                if (logicGate.GateType == GateType.OUT)
                {
                    var connection = new Connection(0, 0, false);
                    Connections.Add(connection);
                    linkedConnections.Add((connection, logicGate));
                    outputsCount++;
                }
            });

            Width = Math.Max(200, Name.Length * 8 + 140);
            Height = Math.Max(Math.Max(inputsCount, outputsCount) * 70, 40);

            var inputsCountMax = inputsCount;
            var outputsCountMax = outputsCount;

            Connections.ForEach(connection =>
            {
                if (connection.WorksAsInput)
                {
                    inputsCount--;
                    connection.RelativePosition.x = -Width * 0.5f;
                    if (inputsCountMax == 1)
                        connection.RelativePosition.y = 0;
                    else
                    {
                        if (inputsCountMax >= outputsCountMax)
                            connection.RelativePosition.y = Utility.Map(inputsCountMax - inputsCount, 1, inputsCountMax, -(Height / 2 - 40), Height / 2 - 40);
                        else
                            connection.RelativePosition.y = Utility.Map(inputsCountMax - inputsCount, 1, inputsCountMax, -(inputsCountMax * 35 - 40), inputsCountMax * 35 - 40);
                    }
                }
                else
                {
                    outputsCount--;
                    connection.RelativePosition.x = Width * 0.5f;
                    if (outputsCountMax == 1)
                        connection.RelativePosition.y = 0;
                    else
                    {
                        if (outputsCountMax >= inputsCountMax)
                            connection.RelativePosition.y = Utility.Map(outputsCountMax - outputsCount, 1, outputsCountMax, -(Height / 2 - 40), Height / 2 - 40);
                        else
                            connection.RelativePosition.y = Utility.Map(outputsCountMax - outputsCount, 1, outputsCountMax, -(outputsCountMax * 35 - 40), outputsCountMax * 35 - 40);
                    }
                }
            });
        }
        public override void UpdateState()
        {
            linkedConnections.ForEach(linked =>
            {
                if (linked.connection.WorksAsInput)
                    linked.logicGate.State = linked.connection.State;
                else
                    linked.connection.State = linked.logicGate.State;
            });
            Wires.ForEach(wire => wire.UpdateState());
            LogicGates.ForEach(logicGate => logicGate.UpdateState());
        }
        public override void Render(FormGameEngine fge, Vector offset, float scale)
        {
            UpdateState();
            base.Render(fge, offset, scale);
        }

        public override void RenderText(FormGameEngine fge, float scale)
        {
            fge.DrawString(ScreenPosition, Name, Pixel.Black, Math.Max((int)scale, 1), PositionMode.Center);
        }
    }
}
