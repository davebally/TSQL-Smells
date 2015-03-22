using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLSmellSCA
{
    public class WhileProcessor
    {
        private Smells _smells;

        public WhileProcessor(Smells smells)
        {
            _smells = smells;
        }

        public void ProcessWhileStatement(WhileStatement whileStatement)
        {
            _smells.ProcessTsqlFragment(whileStatement.Predicate);
            _smells.ProcessTsqlFragment(whileStatement.Statement);
        }
    }
}