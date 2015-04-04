using System;
using System.Data.SqlClient;
using RedGate.SIPFrameworkShared;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.CodeAnalysis;
using Microsoft.SqlServer.Dac.Model;
using System.IO;


namespace TSQLSmellsSSMS.Examples
{

    /*
    public class ObjectExplorerMenuItem : ActionSimpleOeMenuItemBase
    {
        private readonly string m_Label;
        private readonly ISsmsFunctionalityProvider6 m_Provider;
        private readonly Action<string> m_LogMessage;

        public ObjectExplorerMenuItem(string label, ISsmsFunctionalityProvider6 provider, Action<string> logMessageCallback)
        {
            m_Label = label;
            m_Provider = provider;
            m_LogMessage = logMessageCallback;
        }

        /// <summary>
        /// Determines if this menu item should be displayed on this node's context menu.
        /// 
        /// Add-ins should cast oeNode to IOeNode.
        /// </summary>
        /// <param name="oeNode">The object explorer menu the user has right-clicked on.</param>
        /// <returns>true is the menu should be visible.</returns>
        public override bool AppliesTo(ObjectExplorerNodeDescriptorBase oeNode)
        {
            return true;
        }

        /// <summary>
        /// The text displayed for this menu item.
        /// </summary>
        public override string ItemText
        {
            get { return m_Label; }
        }

        /// <summary>
        /// New add-ins implementing this function should caste node to IOeNode.
        /// 
        /// Unfortunately base classes were included in the public interfaces. These can't be removed without breaking backwards compatibility.
        /// </summary>
        public override void OnAction(ObjectExplorerNodeDescriptorBase node)
        {
            var oeNode = (IOeNode) node;
            if (oeNode == null)
            {
                m_Provider.QueryWindow.OpenNew("null");
                m_LogMessage("This shouldn't happen unless you somehow have an ancient version of SIPFw installed.");
                return;
            }

            m_LogMessage(string.Format("Object explorer node clicked: {0}", oeNode.Name));
            IDatabaseObjectInfo databaseObjectInfo;
            IConnectionInfo connectionInfo;
            if (oeNode.TryGetDatabaseObject(out databaseObjectInfo) && oeNode.TryGetConnection(out connectionInfo))
            {
                using (var connection = new SqlConnection(connectionInfo.ConnectionString))
                {
                    connection.Open();
                    var sql = m_Provider.ServerManagementObjects.ScriptAsAlter(connection,
                                                                               databaseObjectInfo.DatabaseName,
                                                                               databaseObjectInfo.Schema,
                                                                               databaseObjectInfo.ObjectName);
                    m_Provider.QueryWindow.OpenNew(sql, databaseObjectInfo.ObjectName, connectionInfo.ConnectionString);
                }
            }
            else
                m_Provider.QueryWindow.OpenNew(string.Format("Name: {0}\nPath: {1}", oeNode.Name, oeNode.Path));
        }
    }
    */
    public class ObjectExplorerMenuItemDacpac : ActionSimpleOeMenuItemBase
    {
        private readonly string m_Label;
        private readonly ISsmsFunctionalityProvider6 m_Provider;
        private readonly Action<string> m_LogMessage;

        public ObjectExplorerMenuItemDacpac(string label, ISsmsFunctionalityProvider6 provider, Action<string> logMessageCallback)
        {
            m_Label = label;
            m_Provider = provider;
            m_LogMessage = logMessageCallback;
        }

        /// <summary>
        /// Determines if this menu item should be displayed on this node's context menu.
        /// 
        /// Add-ins should cast oeNode to IOeNode.
        /// </summary>
        /// <param name="oeNode">The object explorer menu the user has right-clicked on.</param>
        /// <returns>true is the menu should be visible.</returns>
        public override bool AppliesTo(ObjectExplorerNodeDescriptorBase oeNode)
        {
            IOeNode n = (IOeNode)oeNode;
            return (n.Type.ToString().Equals("Database"));
            
        }

        /// <summary>
        /// The text displayed for this menu item.
        /// </summary>
        public override string ItemText
        {
            get { return m_Label; }
        }

        /// <summary>
        /// New add-ins implementing this function should caste node to IOeNode.
        /// 
        /// Unfortunately base classes were included in the public interfaces. These can't be removed without breaking backwards compatibility.
        /// </summary>
        /// 
        public override void OnAction(ObjectExplorerNodeDescriptorBase node)
        {
            var oeNode = (IOeNode)node;
            if (oeNode == null)
            {
                m_Provider.QueryWindow.OpenNew("null");
                m_LogMessage("This shouldn't happen unless you somehow have an ancient version of SIPFw installed.");
                return;
            }

            String PrcFile = ExtractDacpac(oeNode);

            
        }

        public string ExtractDacpac(IOeNode oeNode)
        {
            IConnectionInfo connectionInfo;
            if (oeNode.TryGetConnection(out connectionInfo))
            {


                string extractedPackagePath = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName() + ".dacpac";
                string OutFile = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName() + ".xml";

                DacServices services = new DacServices("Server=" + connectionInfo.Server + ";Integrated Security=true;");
                services.Extract(extractedPackagePath, oeNode.Name, "AppName", new Version(1, 0));

                using (TSqlModel model = TSqlModel.LoadFromDacpac(extractedPackagePath,
                new ModelLoadOptions(DacSchemaModelStorageType.Memory, loadAsScriptBackedModel: true)))
                {

                    CodeAnalysisService service = new CodeAnalysisServiceFactory().CreateAnalysisService(model.Version);
                    //service.ResultsFile = OutFile;
                    CodeAnalysisResult result = service.Analyze(model);
                    string res="" ;
                    result.SerializeResultsToXml(res);

                    
                }
                return OutFile;
            }
            else
            {

                m_LogMessage("Failed");
            }
            return null;
        }
    }

}
