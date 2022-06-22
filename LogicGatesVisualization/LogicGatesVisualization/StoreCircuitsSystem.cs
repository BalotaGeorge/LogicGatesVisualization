using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace LogicGatesVisualization
{
    public static class StoreCircuitsSystem
    {
        public static List<CIRCUIT> SavedCircuits = new List<CIRCUIT>();

        static FormGameEngine fge;
        static string filepath = "../../saved_circuits.txt";
        static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };
        static ButtonUI saveButton;
        public static InputUI saveInput;

        static string warnMessage = "";
        static float warnMessageTimer = 0f;

        public static void Initialize(FormGameEngine fge)
        {
            StoreCircuitsSystem.fge = fge;
            InitSavedCircuits();

            saveButton = new ButtonUI(fge.WindowWidth() - 100, fge.WindowHeight() - 40, 99, 40, "Save");
            saveInput = new InputUI(fge.WindowWidth() - 300, fge.WindowHeight() - 40, 199, 40) { placeholder = "new circuit name" };
        }

        public static void ProcessAndRender(List<Wire> wires, List<LogicGate> logicGates)
        {
            if (saveButton.clicked)
            {
                if (saveInput.value.Length == 0)
                {
                    warnMessage = "Specify a name for this circuit";
                    warnMessageTimer = 3f;
                    goto Exit;
                }

                if (SavedCircuits.Find(c => c.Name == saveInput.value) != null || 
                   new List<string> { "IN", "OUT", "JOINT", "NOT", "AND", "OR" }.Find(c => c == saveInput.value) != null)
                {
                    warnMessage = "Circuit with this name already exists";
                    warnMessageTimer = 3f;
                    goto Exit;
                }

                var circuit = new CIRCUIT(0, 0, logicGates, wires, 
                    saveInput.value.Length > 0 ? saveInput.value : "new circuit");
                saveInput.value = "";

                SavedCircuits.Add(circuit);

                SaveCircuitsToDisk();
                PlacingSystem.Initialize(fge);
                wires.Clear();
                logicGates.Clear();
                InitSavedCircuits();

            Exit:;
            }

            if (warnMessageTimer > 0)
            {
                fge.DrawString(fge.ScreenWidth() - warnMessage.Length * 8 - 4, fge.ScreenHeight() - 56, warnMessage, Pixel.Red);
                warnMessageTimer -= Time.fElapsedTime;
            } 

            saveButton.Render(fge);
            saveInput.Render(fge);
        }

        public static void DeleteCircuitByName(string name)
        {
            SavedCircuits.RemoveAll(circuit => circuit.Name == name);
            SaveCircuitsToDisk();
            PlacingSystem.Initialize(fge);
        }

        public static CIRCUIT GetCircuitByName(string name)
        {
            var savedCircuit = SavedCircuits.Find(circuit => circuit.Name == name);
            if (savedCircuit is null) return null;
            return Clone(savedCircuit);
        }

        private static void SaveCircuitsToDisk()
        {
            var savedCircuitsJson = JsonConvert.SerializeObject(SavedCircuits, jsonSerializerSettings);
            File.WriteAllText(filepath, savedCircuitsJson);
        }

        private static void InitSavedCircuits()
        {
            if (!File.Exists(filepath)) File.Create(filepath);
            var savedCircuitsJson = File.ReadAllText(filepath);
            SavedCircuits = JsonConvert.DeserializeObject<List<CIRCUIT>>(savedCircuitsJson, jsonSerializerSettings) ?? new List<CIRCUIT>();
        }

        private static T Clone<T>(T obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj, jsonSerializerSettings), jsonSerializerSettings);
        }
    }
}
