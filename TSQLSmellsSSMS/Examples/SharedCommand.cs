using System;
using System.Reflection;
using RedGate.SIPFrameworkShared;

namespace TSQLSmellsSSMS.Examples
{
    public class SharedCommand : ISharedCommandWithExecuteParameter, ISharedCommand
    {
        private readonly ISsmsFunctionalityProvider4 m_Provider;
        private readonly Action<string> m_LogMessage;
        private readonly ICommandImage m_CommandImage = new CommandImageForEmbeddedResources(Assembly.GetExecutingAssembly(), "TSQLSmellsSSMS.Examples.rg_icon.ico");

        public SharedCommand(ISsmsFunctionalityProvider4 provider, Action<string> logMessageCallback)
        {
            m_Provider = provider;
            m_LogMessage = logMessageCallback;
        }

        public string Name { get { return "RedGate_Sample_Command"; } }
        public void Execute(object parameter)
        {
            Execute();
        }

        public void Execute()
        {
            m_LogMessage("SharedCommand executed.");
        }

        public string Caption { get { return "Red Gate Sample Command"; } }
        public string Tooltip { get { return "Tooltip"; }}
        public ICommandImage Icon { get { return m_CommandImage; } }
        public string[] DefaultBindings { get { return new[] { "global::Ctrl+Alt+D" }; } }
        public bool Visible { get { return true; } }
        public bool Enabled { get { return true; } }
    }
}