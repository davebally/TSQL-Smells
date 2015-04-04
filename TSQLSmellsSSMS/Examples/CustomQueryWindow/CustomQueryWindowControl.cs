using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RedGate.SIPFrameworkShared;

namespace TSQLSmellsSSMS.Examples.CustomQueryWindow
{
    public partial class CustomQueryWindowControl : UserControl
    {
        private readonly ISsmsFunctionalityProvider6 m_Provider;

        public CustomQueryWindowControl(ISsmsFunctionalityProvider6 provider)
        {
            m_Provider = provider;
            InitializeComponent();
        }

        //Convert
        private void button1_Click(object sender, EventArgs e)
        {
            var currentText = GetText();
            var convertedText = currentText.ToUpper();
            m_Provider.QueryWindow.OpenNew(convertedText);
        }

        private string GetText()
        {
            return m_Provider.GetQueryWindowManager().GetActiveAugmentedQueryWindowContents();
        }

    }
}
