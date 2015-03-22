using System;
using RedGate.SIPFrameworkShared;
using RedGate.SIPFrameworkShared.ObjectExplorer;
using TSQLSmellsSSMS.Examples;
using TSQLSmellsSSMS.Examples.CustomQueryWindow;
using TSQLSmellsSSMS.Examples.MessagesWindow;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.CodeAnalysis;
using Microsoft.SqlServer.Dac.Model;
using System.IO;

namespace TSQLSmellsSSMS
{
    /// <summary>
    /// You must have SIPFramework installed. You can find a standalone installer here: http://www.red-gate.com/ssmsecosystem
    /// 
    /// SIPFramework hooks into SSMS and launches add ins. You will need to register this sample add-in with SIPFramework. To do this:
    /// 1. Find registry key: HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Red Gate\SIPFramework\Plugins
    /// 2. Create a new string with the name "SampleAddIn".
    /// 3. Set the value to the full file path of SampleSsmsEcosystemAddin.dll.
    ///     For example: C:\Users\david\Documents\SampleSsmsEcosystemAddin\SampleSsmsEcosystemAddin\bin\Debug\SampleSsmsEcosystemAddin.dll
    ///  
    /// </summary>
    public class SampleAddin : ISsmsAddin4
    {
        /// <summary>
        /// Add in meta data
        /// </summary>
        public string Version { get { return "1.0.0.0"; } }
        public string Description { get { return "TSQL Smells for SSMS with Red Gate's SIPFramework."; } }
        public string Name { get { return "TSQL Smells SSMS"; } }
        public string Author { get { return "Dave Ballantyne"; } }
        public string Url { get { return @"https://github.com/red-gate/SampleSsmsEcosystemAddin"; } }

          
        private const string c_MessageWindowGuid = "156D629D-2BC6-43BA-A503-9DD585BABF62";
        private const string c_ResultsWindowGuid = "4285C6C4-272B-498D-85EA-EFF630CC6793";

        private ISsmsFunctionalityProvider6 m_Provider;
        private MessageLog m_MessageLog;
        private IToolWindow m_MessageLogWindow;
        private IToolWindow m_ResultsViewWindow;


        /// <summary>
        /// This is the entry point for your add in.
        /// </summary>
        /// <param name="provider">This gives you access to the SSMS integrations provided by SIPFramework. If there's something missing let me know!</param>
        public void OnLoad(ISsmsExtendedFunctionalityProvider provider)
        {
            m_Provider = (ISsmsFunctionalityProvider6)provider;    //Caste to the latest version of the interface
            
            m_MessageLog = new MessageLog();
            var messagesView = new MessagesView { DataContext = m_MessageLog };
            m_MessageLogWindow = m_Provider.ToolWindow.Create(messagesView, "TSQL Smells", new Guid(c_MessageWindowGuid));
            m_MessageLogWindow.Window.Dock();
            DisplayMessages();

            var ResultsView = new ResultsView.ResultsView{ DataContext=null };
            m_ResultsViewWindow = m_Provider.ToolWindow.Create(ResultsView, "TSQL Smells Results", new Guid(c_ResultsWindowGuid));
            m_ResultsViewWindow.Window.Dock();
            m_ResultsViewWindow.Activate(true);

            try
            {


                LogMessage("Starting");
                //AddMenuBarMenu();
                //LogMessage("Done Bar");
                //AddCustomQueryWindowButton();
                AddObjectExplorerContextMenu();
                AddObjectExplorerListener();
                //AddToolbarButton();
            }
            catch (Exception e)
            {
                LogMessage(e.Message+":"+e.StackTrace);
            }
        }

        private void AddCustomQueryWindowButton()
        {
            var command = new OpenCustomQueryWindowCommand(m_Provider);
            m_Provider.AddToolbarItem(command);
        }  

        private void AddObjectExplorerListener()
        {
            m_Provider.ObjectExplorerWatcher.ConnectionsChanged += (args) => { OnConnectionsChanged(args); };
            m_Provider.ObjectExplorerWatcher.SelectionChanged += (args) => { OnSelectionChanged(args); };
        }

        private void OnSelectionChanged(ISelectionChangedEventArgs args)
        {
            m_MessageLog.AddMessage(string.Format("Object explorer selection: {0}", args.Selection.Path));
        }

        private void OnConnectionsChanged(IConnectionsChangedEventArgs args)
        {
            m_MessageLog.AddMessage("Object explorer connections:");
            int count = 1;
            foreach (var connection in args.Connections)
            {
                m_MessageLog.AddMessage(string.Format("\t{0}: {1}", count, connection.Server));
                count++;
            }
        }

        /// <summary>
        /// Callback when SSMS is beginning to shutdown.
        /// </summary>
        public void OnShutdown()
        {
        }

        /// <summary>
        /// Deprecated. Subscribe to m_Provider.ObjectExplorerWatcher.SelectionChanged
        /// 
        /// Callback when object explorer node selection changes.
        /// </summary>
        /// <param name="node">The node that was selected.</param>
        public void OnNodeChanged(ObjectExplorerNodeDescriptorBase node)
        {
        }
        
        private void AddMenuBarMenu()
        {
            var command = new SharedCommand(m_Provider, LogAndDisplayMessage);
            m_Provider.AddGlobalCommand(command);

            m_Provider.MenuBar.MainMenu.BeginSubmenu("Sample", "Sample")
                .BeginSubmenu("Sub 1", "Sub1")
                    .AddCommand(command.Name)
                    .AddCommand(command.Name)
                .EndSubmenu()
            .EndSubmenu();
        }

        private void AddToolbarButton()
        {
            m_Provider.AddToolbarItem(new SharedCommand(m_Provider, LogAndDisplayMessage));
        }

        private void AddObjectExplorerContextMenu()
        {
            var subMenus = new SimpleOeMenuItemBase[]
            {
                new ObjectExplorerMenuItemDacpac("Extract to Dacpac", m_Provider, LogMessage),
        //        new ObjectExplorerMenuItem("Command 2", m_Provider, LogMessage),
            };
            m_Provider.AddTopLevelMenuItem(new ObjectExplorerSubmenu(subMenus));
        }

        public void LogAndDisplayMessage(string text)
        {
            LogMessage(text);
            DisplayMessages();
        }

        public void LogMessage(string text)
        {
            m_MessageLog.AddMessage(text);
        }

        public void DisplayMessages()
        {
            m_MessageLogWindow.Activate(true);
        }
        
    }
}