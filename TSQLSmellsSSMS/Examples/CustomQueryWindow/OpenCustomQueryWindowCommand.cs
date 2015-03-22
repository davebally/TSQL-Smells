using System.Reflection;
using RedGate.SIPFrameworkShared;

namespace TSQLSmellsSSMS.Examples.CustomQueryWindow
{
    internal class OpenCustomQueryWindowCommand : ISharedCommandWithExecuteParameter, ISharedCommand
    {
        private readonly ISsmsFunctionalityProvider6 m_Provider;
        private readonly ICommandImage m_CommandImage = new CommandImageForEmbeddedResources(Assembly.GetExecutingAssembly(), "TSQLSmellsSSMS.Examples.rg_icon.ico");

        public OpenCustomQueryWindowCommand(ISsmsFunctionalityProvider6 provider)
        {
            m_Provider = provider;
        }

        public string Name { get { return "RedGate_Sample_OpenCustomQueryWindow"; } }

        public void Execute(object parameter)
        {
            Execute();
        }

        public void Execute()
        {
            m_Provider.GetQueryWindowManager().CreateAugmentedQueryWindow(string.Empty, "Custom query window", new CustomQueryWindowControl(m_Provider));
        }

        public string Caption { get { return "Open Custom Query Window"; } }
        public string Tooltip { get { return "Tooltip"; }}
        public ICommandImage Icon { get { return m_CommandImage; } }
        public string[] DefaultBindings { get { return new[] { "global::Ctrl+Alt+J" }; } }
        public bool Visible { get { return true; } }
        public bool Enabled { get { return true; } }
    }
}