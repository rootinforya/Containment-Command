using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;


namespace Containment
{
    public class Containment
    {
        public static Containment Singleton { get; private set; }

        [PluginEntryPoint("Containment Command", "1.0.0", "This plugin adds a command for SCPs that let them commit suicide", "rootinforya")]
        void LoadPlugin()
        {
            Log.Info("Loading Containment Command...");
            Singleton = this;
            Log.Info("Containment Command Loaded!");
        }
    }
}

