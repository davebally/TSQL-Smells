using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSQLSmellsSSMS.ResultsView
{
    class TSQLSmell
    {
        public string Rule;
        public string ProblemDescription;
        public string SourceFile;
        public int    Line;
        public int    Column;
        public string Severity;
    }
}
