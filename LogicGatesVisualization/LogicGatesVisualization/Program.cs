using System.Collections.Generic;
using System.Windows.Forms;

namespace LogicGatesVisualization
{
    public class LogicGatesVisualization : FormGameEngine
    {
        List<LogicGate> logicGates;
        List<Wire> wires;
        float scale;
        bool derived;
        Wire derivedWire;
        Vector offset;
        Vector startPan;
        LogicGate selectedLogicGate;
        Connection selectedConnection;
        int selectedLogicGates;
        public override void OnUserCreate()
        {
            ShowConsoleWindow(false);

            AppName = "Logic Gates Visualization";
            scale = 1f;
            offset = -1 * Middle();

            wires = new List<Wire>();
            logicGates = new List<LogicGate>
            {
                new AND(0, 0),
                new IN(-250, -75),
                new IN(-250, 75),
                new OUT(250, 0)
            };

            StoreCircuitsSystem.Initialize(this);
            PlacingSystem.Initialize(this);
            InfoSystem.Initialize(this);
        }

        public override void OnUserUpdate(float fElapsedTime)
        {
            if (!Focused()) return;

            Clear(Pixel.RGB(230, 230, 230));

            Vector mouse = ScreenMousePos();
            Vector mouseBeforeZoom = mouse.ScreenToWorld(offset, scale);
            if (Input.WheelScrollUp()) scale *= 1.1f;
            if (Input.WheelScrollDown()) scale *= 0.9f;
            Vector mouseAfterZoom = mouse.ScreenToWorld(offset, scale);
            offset += mouseBeforeZoom - mouseAfterZoom;

            if (Input.KeyHeld(MouseButtons.Middle))
                offset -= ChangeInScreenMouse() / scale;

            Vector worldMouse = mouse.ScreenToWorld(offset, scale);

            if (Input.KeyPressed(MouseButtons.Left))
            {
                if (PlacingSystem.PlacingLogicGate != null)
                {
                    logicGates.Add(PlacingSystem.PlacingLogicGate);
                    PlacingSystem.PlacingLogicGate = null;
                    goto Exit;
                }
                foreach (var logicGate in logicGates)
                {
                    foreach (var connection in logicGate.Connections)
                    {
                        if ((connection.ScreenPosition - mouse).Magnitude() < connection.DisplaySize * scale)
                        {
                            selectedLogicGate = logicGate;
                            selectedConnection = connection;
                            goto Exit;
                        }
                    }
                }
                foreach (var wire in wires)
                {
                    if (wire.GetLine().DistPointToLine(mouse) <= 5 * scale)
                    {
                        selectedLogicGate = wire.FromGate;
                        selectedConnection = wire.FromConnection;
                        derived = true;
                        derivedWire = wire;
                        goto Exit;
                    }
                }
            Exit:
                startPan = mouse.Clone();
            }
            if (Input.KeyHeld(MouseButtons.Left))
            {
                if (selectedConnection != null)
                {
                    DrawThickLine(startPan, mouse, scale * 5, selectedConnection.State ? Pixel.Green : Pixel.Red);
                }
            }
            if (Input.KeyReleased(MouseButtons.Left))
            {
                if (selectedConnection != null)
                {
                    foreach (var logicGate in logicGates)
                    {
                        foreach (var connection in logicGate.Connections)
                        {
                            if ((connection.ScreenPosition - mouse).Magnitude() < connection.DisplaySize * scale &&
                                connection.WorksAsInput != selectedConnection.WorksAsInput &&
                                logicGate != selectedLogicGate &&
                                !connection.IsConnected && !selectedConnection.IsConnected)
                            {
                                Wire wire;
                                if (connection.WorksAsInput)
                                    wire = new Wire(selectedLogicGate, logicGate, selectedConnection, connection);
                                else
                                    wire = new Wire(selectedLogicGate, logicGate, connection, selectedConnection);
                                connection.IsConnected = true;
                                selectedConnection.IsConnected = true;
                                wires.Add(wire);
                                goto Exit;
                            }
                            if ((connection.ScreenPosition - mouse).Magnitude() < connection.DisplaySize * scale &&
                                connection.WorksAsInput != selectedConnection.WorksAsInput &&
                                logicGate != selectedLogicGate && derived)
                            {
                                Wire wire = new Wire(selectedLogicGate, logicGate, selectedConnection, connection)
                                {
                                    DerivedWire = true,
                                    DerivedPin = derivedWire.GetLine().ProjPointOnLine(startPan),
                                    ParentWire = derivedWire
                                };
                                wire.Calculate();
                                connection.IsConnected = true;
                                wires.Add(wire);
                                goto Exit;
                            }
                            if ((connection.ScreenPosition - mouse).Magnitude() < connection.DisplaySize * scale &&
                                logicGate == selectedLogicGate && logicGate.GateType == GateType.IN)
                            {
                                logicGate.State = !logicGate.State;
                                goto Exit;
                            }
                        }
                    }
                    foreach (var wire in wires)
                    {
                        if ((wire.ToConnection == selectedConnection || wire.FromConnection == selectedConnection) && !derived)
                        {
                            if (!wire.DerivedWire)
                            {
                                wire.FromConnection.IsConnected = false;
                                wire.ToConnection.IsConnected = false;
                                wire.FromConnection.State = false;
                                wire.ToConnection.State = false;
                            }
                            else
                            {
                                wire.ToConnection.IsConnected = false;
                                wire.ToConnection.State = false;
                            }
                            wire.MarkToDelete(wires);
                            wires.RemoveAll(w => w.IsDeleted());
                            goto Exit;
                        }
                    }
                Exit:
                    derived = false;
                    derivedWire = null;
                    selectedLogicGate = null;
                    selectedConnection = null;
                }
            }

            if (Input.KeyPressed(MouseButtons.Right))
            {
                if (PlacingSystem.PlacingLogicGate != null)
                {
                    PlacingSystem.PlacingLogicGate = null;
                    goto Exit;
                }
                if (selectedLogicGates == 0)
                {
                    foreach (var logicGate in logicGates)
                    {
                        Vector v1 = (logicGate.WorldPosition - logicGate.Size() * 0.5f).WorldToScreen(offset, scale);
                        Vector v2 = (logicGate.WorldPosition + logicGate.Size() * 0.5f).WorldToScreen(offset, scale);
                        if (mouse.InsideBounds(v1, v2))
                        {
                            selectedLogicGate = logicGate;
                            selectedLogicGate.Selected = true;
                            break;
                        }
                    }
                }
                else
                {
                    bool none = true;
                    foreach (var logicGate in logicGates)
                    {
                        Vector v1 = (logicGate.WorldPosition - logicGate.Size() * 0.5f).WorldToScreen(offset, scale);
                        Vector v2 = (logicGate.WorldPosition + logicGate.Size() * 0.5f).WorldToScreen(offset, scale);
                        if (mouse.InsideBounds(v1, v2))
                        {
                            if (logicGate.Selected) none = false;
                            else selectedLogicGate = logicGate;
                            break;
                        }
                    }
                    if (none)
                    {
                        selectedLogicGates = 0;
                        foreach (var logicGate in logicGates) logicGate.Selected = false;
                        if (selectedLogicGate != null) selectedLogicGate.Selected = true;
                    }
                }
            Exit:
                startPan = mouse.Clone();
            }
            if (Input.KeyHeld(MouseButtons.Right))
            {
                if (selectedLogicGates == 0)
                {
                    if (selectedLogicGate != null)
                    {
                        selectedLogicGate.WorldPosition += ChangeInScreenMouse() / scale;
                        startPan = mouse.Clone();
                    }
                    else
                    {
                        DrawLine(startPan.ix, startPan.iy, mouse.ix, startPan.iy, Pixel.Green);
                        DrawLine(startPan.ix, startPan.iy, startPan.ix, mouse.iy, Pixel.Green);
                        DrawLine(mouse.ix, startPan.iy, mouse.ix, mouse.iy, Pixel.Green);
                        DrawLine(startPan.ix, mouse.iy, mouse.ix, mouse.iy, Pixel.Green);
                    }
                }
                else
                {
                    foreach (var logicGate in logicGates)
                        if (logicGate.Selected) logicGate.WorldPosition += ChangeInScreenMouse() / scale;
                    startPan = mouse.Clone();
                }
            }
            if (Input.KeyReleased(MouseButtons.Right))
            {
                if (selectedLogicGate != null)
                {
                    selectedLogicGate.Selected = false;
                    selectedLogicGate = null;
                }
                foreach (var logicGate in logicGates)
                {
                    Vector worldStartPan = startPan.ScreenToWorld(offset, scale);
                    if (logicGate.WorldPosition.InsideBounds(worldMouse, worldStartPan))
                    {
                        logicGate.Selected = true;
                        selectedLogicGates++;
                    }
                }
            }

            if (!StoreCircuitsSystem.saveInput.focused)
            {
                if (Input.KeyPressed(Keys.D1)) PlacingSystem.PlacingLogicGate = new IN(worldMouse.x, worldMouse.y);
                if (Input.KeyPressed(Keys.D2)) PlacingSystem.PlacingLogicGate = new OUT(worldMouse.x, worldMouse.y);
                if (Input.KeyPressed(Keys.D3)) PlacingSystem.PlacingLogicGate = new JOINT(worldMouse.x, worldMouse.y);
                if (Input.KeyPressed(Keys.D4)) PlacingSystem.PlacingLogicGate = new NOT(worldMouse.x, worldMouse.y);
                if (Input.KeyPressed(Keys.D5)) PlacingSystem.PlacingLogicGate = new AND(worldMouse.x, worldMouse.y);
                if (Input.KeyPressed(Keys.D6)) PlacingSystem.PlacingLogicGate = new OR(worldMouse.x, worldMouse.y);
            }
            if (Input.KeyHeld(Keys.ControlKey) && Input.KeyPressed(Keys.A))
            {
                logicGates.ForEach(gate => gate.Selected = true);
                selectedLogicGates = logicGates.Count;
            }
            if (Input.KeyPressed(Keys.Escape))
            {
                logicGates.ForEach(gate => gate.Selected = false);
                selectedLogicGates = 0;
            }

            if (Input.KeyPressed(Keys.Delete))
            {
                if (PlacingSystem.PlacingLogicGate != null && PlacingSystem.PlacingLogicGate is CIRCUIT placingCircuitDelete)
                {
                    StoreCircuitsSystem.DeleteCircuitByName(placingCircuitDelete.Name);
                }
                else
                {
                    logicGates.ForEach(gate =>
                    {
                        if (gate.Selected)
                            gate.MarkToDelete();
                    });

                    wires.ForEach(wire =>
                    {
                        if (wire.ToGate.IsDeleted() || wire.FromGate.IsDeleted())
                            wire.MarkToDelete(wires);
                    });

                    wires.RemoveAll(wire => wire.IsDeleted());
                    logicGates.RemoveAll(logicGate => logicGate.IsDeleted());
                }
            }
            if (PlacingSystem.PlacingLogicGate != null && PlacingSystem.PlacingLogicGate is CIRCUIT placingCircuit && Input.KeyPressed(Keys.Space))
            {
                logicGates = placingCircuit.LogicGates;
                wires = placingCircuit.Wires;
                StoreCircuitsSystem.saveInput.value = placingCircuit.Name;
                PlacingSystem.PlacingLogicGate = null;
            }

            logicGates.ForEach(logicGate => logicGate.Render(this, offset, scale));
            wires.ForEach(wire => wire.Render(this, scale));
            logicGates.ForEach(logicGate => logicGate.RenderExtra(this, scale));

            PlacingSystem.ProcessAndRender(worldMouse, offset, scale);
            StoreCircuitsSystem.ProcessAndRender(wires, logicGates);
            InfoSystem.ProcessAndRender();
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            LogicGatesVisualization app = new LogicGatesVisualization();

            if (args?.Length > 0 && args[0] == "fs")
                app.Construct(0, 0, windowMode: WindowMode.Fullscreen);
            else
                app.Construct(1200, 720);
        }
    }
}
