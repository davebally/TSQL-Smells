using System.Reflection;
using RedGate.SIPFrameworkShared;

namespace TSQLSmellsSSMS.Examples
{
    class ObjectExplorerSubmenu : SubmenuSimpleOeMenuItemBase
    {
        private readonly ICommandImage m_CommandImage = new CommandImageForEmbeddedResources(Assembly.GetExecutingAssembly(), "TSQLSmellsSSMS.Examples.rg_icon.ico");

        public ObjectExplorerSubmenu(SimpleOeMenuItemBase[] subMenus)
            : base(subMenus)
        {
        }

        public override string ItemText
        {
            get { return "TSQL Smells"; }
        }

        public override bool AppliesTo(ObjectExplorerNodeDescriptorBase oeNode)
        {
            
        
            return GetApplicableChildren(oeNode).Length > 0;
        }
    }
}