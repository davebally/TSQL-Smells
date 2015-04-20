using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.CodeAnalysis;
using Microsoft.SqlServer.Dac.Model;
using System.IO;
using System.Xml;


namespace TSQLSmellsSSDTTest
{
    public class TestProblem
    {
        public int StartColumn;
        public int StartLine;
        public string RuleId;

        public TestProblem(int StartLine, int StartColumn, string RuleId)
        {
            this.StartColumn = StartColumn;
            this.StartLine = StartLine;
            this.RuleId = RuleId;
        }

        public override bool Equals(Object obj)
        {
            TestProblem prb = obj as TestProblem;
            if(prb.RuleId.Equals(this.RuleId,StringComparison.OrdinalIgnoreCase)&&
                prb.StartColumn == this.StartColumn &&
                prb.StartLine == this.StartLine)
            {
                return true;
            }
            return false;

        }

        public override int GetHashCode()
        { 
            return string.Format("{0}:{1}:{2}", RuleId, StartColumn, StartLine).GetHashCode(); 
        }
    };


    
    public class TestModel 
    {
        public List<TestProblem> _ExpectedProblems = new List<TestProblem>();
        public List<TestProblem> _FoundProblems = new List<TestProblem>();
        public List<String>_TestFiles= new List<String>();

        private TSqlModel _Model;



        public void BuildModel()
        {
            _Model = new TSqlModel(SqlServerVersion.Sql110, null);
            AddFilesToModel();

        }

        public void AddFilesToModel()
        {
            foreach (string FileName in _TestFiles)
            {
                String FileContent = "";
                using (var reader = new StreamReader(FileName))
                {
                    FileContent += reader.ReadToEnd();
                }
                _Model.AddObjects(FileContent);
            }
        }

        public void SerializeResultOutput(CodeAnalysisResult result)
        {
            foreach (SqlRuleProblem Problem in result.Problems)
            {
                // Only concern ourselves with our problems
                if (Problem.RuleId.StartsWith("Smells."))
                {
                    TestProblem TestProblem = new TestProblem(Problem.StartLine, Problem.StartColumn, Problem.RuleId);
                    _FoundProblems.Add(TestProblem);
                }

            }

        }
        public void RunSCARules()
        {
            CodeAnalysisService service = new CodeAnalysisServiceFactory().CreateAnalysisService(_Model.Version);
            CodeAnalysisResult result = service.Analyze(_Model);
            SerializeResultOutput(result);

            CollectionAssert.AreEquivalent(_FoundProblems, _ExpectedProblems);
        }

        public void RunTest()
        {
            BuildModel();
            RunSCARules();
        }

    }


    [TestClass]
    public class testConvertDate : TestModel
    {

        public testConvertDate()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\ConvertDate.sql");


            this._ExpectedProblems.Add(new TestProblem(8,7, "Smells.SML006"));


        }

        [TestMethod]
        public void ConvertDate()
        {

            BuildModel();
            RunSCARules();
        }

    }

    [TestClass]
    public class testsqlInjection : TestModel
    {

        public testsqlInjection()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\Inject.sql");
            
            this._ExpectedProblems.Add(new TestProblem(14,10, "Smells.SML043"));
            this._ExpectedProblems.Add(new TestProblem(23,10,  "Smells.SML043"));
            this._ExpectedProblems.Add(new TestProblem(52,10,  "Smells.SML043"));
            this._ExpectedProblems.Add(new TestProblem(88,10,  "Smells.SML043"));
            this._ExpectedProblems.Add(new TestProblem(5,7,  "Smells.SML043"));
        }

        [TestMethod]
        public void SQLInjection()
        {
            RunTest();
        }

    }

    [TestClass]
    public class testCreateViewOrderBy : TestModel
    {

        public testCreateViewOrderBy()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\CreateViewOrderBy.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 1, "Smells.SML028"));
        }

        [TestMethod]
        public void CreateViewOrderBy()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testConvertDateMultipleCond : TestModel
    {

        public testConvertDateMultipleCond()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\ConvertDateMultiCond.sql");

            this._ExpectedProblems.Add(new TestProblem(7, 7, "Smells.SML006"));
            this._ExpectedProblems.Add(new TestProblem(8, 5, "Smells.SML006"));
        }

        [TestMethod]
        public void ConvertDateMultipleCond()
        {

            RunTest();
        }

    }
    /*
    [TestClass]
    public class testDisabledForeignKeyConstraint : TestModel
    {

        public testDisabledForeignKeyConstraint()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\DisabledForeignKey.sql");

            this._ExpectedProblems.Add(new TestProblem(7, 7, "Smells.SML006"));
            this._ExpectedProblems.Add(new TestProblem(8, 5, "Smells.SML006"));
        }

        [TestMethod]
        public void DisabledForeignKeyConstraint()
        {

            RunTest();
        }

    }
    */
    [TestClass]
    public class testConvertInt : TestModel
    {

        public testConvertInt()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\ConvertInt.sql");

            this._ExpectedProblems.Add(new TestProblem(7, 7, "Smells.SML006"));
            
        }

        [TestMethod]
        public void ConvertInt()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testConvertInt2 : TestModel
    {

        public testConvertInt2()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\ConvertInt2.sql");

            this._ExpectedProblems.Add(new TestProblem(7, 14, "Smells.SML006"));

        }

        [TestMethod]
        public void ConvertInt2()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testCreateProcedureNoSchema : TestModel
    {

        public testCreateProcedureNoSchema()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\CreateProcedureNoSchema.sql");

            this._ExpectedProblems.Add(new TestProblem(2, 18, "Smells.SML024"));

        }

        [TestMethod]
        public void CreateProcedureNoSchema()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testCreateTableNoSchema : TestModel
    {

        public testCreateTableNoSchema()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\CreateTableNoSchema.sql");

            this._ExpectedProblems.Add(new TestProblem(1, 1, "Smells.SML027"));

        }

        [TestMethod]
        public void CreateTableNoSchema()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testDeclareCursor : TestModel
    {

        public testDeclareCursor()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\DeclareCursor.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 1, "Smells.SML029"));

        }

        [TestMethod]
        public void DeclareCursor()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testDerived : TestModel
    {

        public testDerived()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\Derived.sql");

            this._ExpectedProblems.Add(new TestProblem(7, 24, "Smells.SML035"));

        }

        [TestMethod]
        public void Derived()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testExec1PartName : TestModel
    {

        public testExec1PartName()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\Exec1PartName.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 6, "Smells.SML021"));

        }

        [TestMethod]
        public void Exec1PartName()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testExecSQL : TestModel
    {

        public testExecSQL()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\ExecSQL.sql");

            this._ExpectedProblems.Add(new TestProblem(6, 1, "Smells.SML012"));

        }

        [TestMethod]
        public void ExecSQL()
        {

            RunTest();
        }

    }



    [TestClass]
    public class testExplicitRangeWindow : TestModel
    {

        public testExplicitRangeWindow()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\ExplicitRangeWindow.sql");

            this._ExpectedProblems.Add(new TestProblem(7, 19, "Smells.SML025"));

        }

        [TestMethod]
        public void ExplicitRangeWindow()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testExists : TestModel
    {

        public testExists()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\Exists.sql");

            //this._ExpectedProblems.Add(new TestProblem(7, 19, "Smells.SML025"));

        }

        [TestMethod]
        public void Exists()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testForceScan : TestModel
    {

        public testForceScan()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\ForceScan.sql");

            this._ExpectedProblems.Add(new TestProblem(6, 30, "Smells.SML044"));

        }

        [TestMethod]
        public void ForceScan()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testImplicitRangeWindow : TestModel
    {

        public testImplicitRangeWindow()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\ImplicitRangeWindow.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 32, "Smells.SML026"));

        }

        [TestMethod]
        public void ImplicitRangeWindow()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testInsertMissingColumnSpecifiers : TestModel
    {

        public testInsertMissingColumnSpecifiers()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\InsertMissingColumnSpecifiers.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 5, "Smells.SML012"));

        }

        [TestMethod]
        public void InsertMissingColumnSpecifiers()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testInsertSelectStar : TestModel
    {

        public testInsertSelectStar()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\InsertSelectStar.sql");

            this._ExpectedProblems.Add(new TestProblem(6, 9, "Smells.SML005"));

        }

        [TestMethod]
        public void InsertSelectStar()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testLagFunction : TestModel
    {

        public testLagFunction()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\LagFunction.sql");

            //this._ExpectedProblems.Add(new TestProblem(6, 9, "Smells.SML005"));

        }

        [TestMethod]
        public void LagFunction()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testMultiCteTest : TestModel
    {

        public testMultiCteTest()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\MultiCteTest.sql");

            this._ExpectedProblems.Add(new TestProblem(8, 10, "Smells.SML005"));

        }

        [TestMethod]
        public void MultiCteTest()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testorderbyordinal : TestModel
    {

        public testorderbyordinal()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\orderbyordinal.sql");

            this._ExpectedProblems.Add(new TestProblem(6, 34, "Smells.SML007"));
            this._ExpectedProblems.Add(new TestProblem(6, 36, "Smells.SML007"));
            this._ExpectedProblems.Add(new TestProblem(6, 38, "Smells.SML007"));

        }

        [TestMethod]
        public void OrderByOrdinal()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testRangeWindow : TestModel
    {

        public testRangeWindow()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\RangeWindow.sql");

            this._ExpectedProblems.Add(new TestProblem(8, 19, "Smells.SML025"));
            
        }

        [TestMethod]
        public void RangeWindow()
        {

            RunTest();
        }

    }



    [TestClass]
    public class testSelectFromTableVar : TestModel
    {

        public testSelectFromTableVar()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\SelectFromTableVar.sql");

            this._ExpectedProblems.Add(new TestProblem(9, 8, "Smells.SML005"));
            this._ExpectedProblems.Add(new TestProblem(4, 1, "Smells.SML033"));

        }

        [TestMethod]
        public void SelectFromTableVar()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testSelectStarFromViewInProc : TestModel
    {

        public testSelectStarFromViewInProc()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\SelectStarFromViewInProc.sql");

            this._ExpectedProblems.Add(new TestProblem(4, 8, "Smells.SML005"));

        }

        [TestMethod]
        public void SelectFromTableVar()
        {

            RunTest();
        }

    }



    [TestClass]
    public class testSelectStarOutOfCteTest : TestModel
    {

        public testSelectStarOutOfCteTest()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\SelectStarOutOfCteTest.sql");

            this._ExpectedProblems.Add(new TestProblem(8, 8, "Smells.SML005"));
            this._ExpectedProblems.Add(new TestProblem(10, 8, "Smells.SML005"));

        }

        [TestMethod]
        public void SelectStarOutOfCteTest()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testSelectTopNoParen : TestModel
    {

        public testSelectTopNoParen()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\SelectTopNoParen.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 9, "Smells.SML034"));

        }

        [TestMethod]
        public void SelectTopNoParen()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testSetNoCountON : TestModel
    {

        public testSetNoCountON()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\SetNoCountON.sql");

            //this._ExpectedProblems.Add(new TestProblem(5, 9, "Smells.SML034"));

        }

        [TestMethod]
        public void SetNoCountON()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testSets : TestModel
    {

        public testSets()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\Sets.sql");

            this._ExpectedProblems.Add(new TestProblem(10, 1, "Smells.SML013"));
            this._ExpectedProblems.Add(new TestProblem(4, 1, "Smells.SML014"));
            this._ExpectedProblems.Add(new TestProblem(5, 1, "Smells.SML015"));
            this._ExpectedProblems.Add(new TestProblem(6, 1, "Smells.SML016"));
            this._ExpectedProblems.Add(new TestProblem(7, 1, "Smells.SML017"));
            this._ExpectedProblems.Add(new TestProblem(8, 1, "Smells.SML018"));
            this._ExpectedProblems.Add(new TestProblem(9, 1, "Smells.SML019"));
            this._ExpectedProblems.Add(new TestProblem(2, 18, "Smells.SML030"));
        }

        [TestMethod]
        public void Sets()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testSets2 : TestModel
    {

        public testSets2()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\Sets2.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 16, "Smells.SML008"));
            this._ExpectedProblems.Add(new TestProblem(6, 15, "Smells.SML009"));
            this._ExpectedProblems.Add(new TestProblem(7, 1, "Smells.SML020"));
            this._ExpectedProblems.Add(new TestProblem(8, 1, "Smells.SML022"));
            
        }

        [TestMethod]
        public void Sets2()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testSetTransactionIsolationLevel : TestModel
    {

        public testSetTransactionIsolationLevel()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\SetTransactionIsolationLevel.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 1, "Smells.SML010"));
            

        }

        [TestMethod]
        public void SetTransactionIsolationLevel()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testSingleCharAlias : TestModel
    {

        public testSingleCharAlias()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\SingleCharAlias.sql");

            this._ExpectedProblems.Add(new TestProblem(6, 8, "Smells.SML011"));


        }

        [TestMethod]
        public void SingleCharAlias()
        {

            RunTest();
        }

    }



    [TestClass]
    public class testTableHints : TestModel
    {

        public testTableHints()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TableHints.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 1, "Smells.SML004"));


        }

        [TestMethod]
        public void TableHints()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testCrossServerJoin : TestModel
    {

        public testCrossServerJoin()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestCrossServerJoin.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 18, "Smells.SML001"));


        }

        [TestMethod]
        public void CrossServerJoin()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testOnePartNamedSelect : TestModel
    {

        public testOnePartNamedSelect()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestOnePartNamedSelect.sql");

            this._ExpectedProblems.Add(new TestProblem(6, 19, "Smells.SML002"));


        }

        [TestMethod]
        public void OnePartNamedSelect()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testSelectStarBeginEndBlock : TestModel
    {

        public testSelectStarBeginEndBlock()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestSelectStarBeginEndBlock.sql");

            this._ExpectedProblems.Add(new TestProblem(6, 9, "Smells.SML005"));


        }

        [TestMethod]
        public void SelectStarBeginEndBlock()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testSelectStarInCteTest : TestModel
    {

        public testSelectStarInCteTest()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestSelectStarInCteTest.sql");

            this._ExpectedProblems.Add(new TestProblem(7, 8, "Smells.SML005"));


        }

        [TestMethod]
        public void SelectStarInCteTest()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testSelectStarIninlineTVF : TestModel
    {

        public testSelectStarIninlineTVF()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestSelectStarIninlineTVF.sql");

            this._ExpectedProblems.Add(new TestProblem(6, 9, "Smells.SML005"));


        }

        [TestMethod]
        public void SelectStarIninlineTVF()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testSelectStarInMultiStatementTVF : TestModel
    {

        public testSelectStarInMultiStatementTVF()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestSelectStarInMultiStatementTVF.sql");

            this._ExpectedProblems.Add(new TestProblem(12, 10, "Smells.SML005"));
            this._ExpectedProblems.Add(new TestProblem(8, 10, "Smells.SML033"));


        }

        [TestMethod]
        public void SelectStarInMultiStatementTVF()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testSelectStarInScalarUDF : TestModel
    {

        public testSelectStarInScalarUDF()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestSelectStarInScalarUDF.sql");

            this._ExpectedProblems.Add(new TestProblem(9, 10, "Smells.SML005"));
            this._ExpectedProblems.Add(new TestProblem(5, 10, "Smells.SML033"));


        }

        [TestMethod]
        public void SelectStarInScalarUDF()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testSelectStarInWhileLoop : TestModel
    {

        public testSelectStarInWhileLoop()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestSelectStarInWhileLoop.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 9, "Smells.SML005"));


        }

        [TestMethod]
        public void SelectStarInWhileLoop()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testWithExists : TestModel
    {

        public testWithExists()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestWithExists.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 18, "Smells.SML005"));


        }

        [TestMethod]
        public void WithExists()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testWithExistsAndNestedSelectStar : TestModel
    {

        public testWithExistsAndNestedSelectStar()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestWithExistsAndNestedSelectStar.sql");

            this._ExpectedProblems.Add(new TestProblem(4,18, "Smells.SML005"));
            this._ExpectedProblems.Add(new TestProblem(5, 9, "Smells.SML005"));


        }

        [TestMethod]
        public void WithExistsAndNestedSelectStar()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testWithExistsAndNestedSelectStarInlineIF : TestModel
    {

        public testWithExistsAndNestedSelectStarInlineIF()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestWithExistsAndNestedSelectStarInlineIF.sql");

            this._ExpectedProblems.Add(new TestProblem(4, 18, "Smells.SML005"));
            this._ExpectedProblems.Add(new TestProblem(4, 51, "Smells.SML005"));


        }

        [TestMethod]
        public void WithExistsAndNestedSelectStarInlineIF()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testWithNoLock : TestModel
    {

        public testWithNoLock()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestWithNoLock.sql");

            this._ExpectedProblems.Add(new TestProblem(4, 42, "Smells.SML003"));
            


        }

        [TestMethod]
        public void WithNoLock()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testWithNoLockIndexhint : TestModel
    {

        public testWithNoLockIndexhint()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestWithNoLockIndexhint.sql");

            this._ExpectedProblems.Add(new TestProblem(4, 42, "Smells.SML003"));
            this._ExpectedProblems.Add(new TestProblem(4, 49, "Smells.SML045"));



        }

        [TestMethod]
        public void WithNoLockIndexhint()
        {

            RunTest();
        }

    }
    [TestClass]
    public class testWithNoLockInWhiteList : TestModel
    {

        public testWithNoLockInWhiteList()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestWithNoLockInWhiteList.sql");

          //  this._ExpectedProblems.Add(new TestProblem(4, 42, "Smells.SML003"));
            
        }

        [TestMethod]
        public void WithNoLockInWhiteList()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testWithNoLockNoWith : TestModel
    {

        public testWithNoLockNoWith()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TestWithNoLockNoWith.sql");

            this._ExpectedProblems.Add(new TestProblem(4, 38, "Smells.SML003"));
            
        }

        [TestMethod]
        public void WithNoLockNoWith()
        {

            RunTest();
        }

    }



    [TestClass]
    public class testUnion : TestModel
    {

        public testUnion()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\UnionTest.sql");

            this._ExpectedProblems.Add(new TestProblem(5, 8, "Smells.SML005"));
            this._ExpectedProblems.Add(new TestProblem(7, 8, "Smells.SML005"));
            this._ExpectedProblems.Add(new TestProblem(9, 8, "Smells.SML005"));

        }

        [TestMethod]
        public void Union()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testUnnamedPrimaryKey : TestModel
    {

        public testUnnamedPrimaryKey()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\UnnamedPK.sql");

            //this._ExpectedProblems.Add(new TestProblem(5, 8, "Smells.SML005"));
            //this._ExpectedProblems.Add(new TestProblem(7, 8, "Smells.SML005"));
            //this._ExpectedProblems.Add(new TestProblem(9, 8, "Smells.SML005"));

        }

        [TestMethod]
        public void UnnamedPrimaryKey()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testWhiteListTest : TestModel
    {

        public testWhiteListTest()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\WhiteListTest.sql");

            //this._ExpectedProblems.Add(new TestProblem(5, 8, "Smells.SML005"));
            //this._ExpectedProblems.Add(new TestProblem(7, 8, "Smells.SML005"));
            //this._ExpectedProblems.Add(new TestProblem(9, 8, "Smells.SML005"));

        }

        [TestMethod]
        public void WhiteList()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testSingleLineComment : TestModel
    {

        public testSingleLineComment()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\SingleLineComment.sql");

            //this._ExpectedProblems.Add(new TestProblem(5, 8, "Smells.SML005"));
            //this._ExpectedProblems.Add(new TestProblem(7, 8, "Smells.SML005"));
            //this._ExpectedProblems.Add(new TestProblem(9, 8, "Smells.SML005"));

        }

        [TestMethod]
        public void SingleLineComment()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testTempTableWithNamedPK : TestModel
    {

        public testTempTableWithNamedPK()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TempTableWithNamedPK.sql");

            this._ExpectedProblems.Add(new TestProblem(14, 3, "Smells.SML038"));
            //this._ExpectedProblems.Add(new TestProblem(7, 8, "Smells.SML005"));
            //this._ExpectedProblems.Add(new TestProblem(9, 8, "Smells.SML005"));

        }

        [TestMethod]
        public void TempTableWithNamedPK()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testTempTableWithNamedDefConstraint : TestModel
    {

        public testTempTableWithNamedDefConstraint()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TempTableWithNamedDefConstraint.sql");

            this._ExpectedProblems.Add(new TestProblem(14, 3, "Smells.SML039"));
            
        }

        [TestMethod]
        public void TempTableWithNamedDefConstraint()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testTempTableWithNamedFK : TestModel
    {

        public testTempTableWithNamedFK()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TempTableWithNamedFK.sql");

            //this._ExpectedProblems.Add(new TestProblem(14, 3, "Smells.SML040"));
            
        }

        [TestMethod]
        public void TempTableWithNamedFK()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testTempTableWithNamedCheckConstraint : TestModel
    {

        public testTempTableWithNamedCheckConstraint()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\TempTableWithNamedCheckConstraint.sql");

            this._ExpectedProblems.Add(new TestProblem(14, 16, "Smells.SML040"));
            
        }

        [TestMethod]
        public void TempTableWithNamedCheckConstraint()
        {

            RunTest();
        }

    }


    [TestClass]
    public class testEqualsNull : TestModel
    {

        public testEqualsNull()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\EqualsNull.sql");
            this._ExpectedProblems.Add(new TestProblem(13, 39, "Smells.SML046"));

            
        }

        [TestMethod]
        public void EqualsNull()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testDeprecatedType : TestModel
    {

        public testDeprecatedType()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\DeprecatedTypes.sql");
            this._ExpectedProblems.Add(new TestProblem(4, 16, "Smells.SML047"));


        }

        [TestMethod]
        public void DeprecatedTypes()
        {

            RunTest();
        }

    }

    [TestClass]
    public class testDeprecatedTypeSP : TestModel
    {

        public testDeprecatedTypeSP()
        {
            this._TestFiles.Add("..\\..\\..\\TSQLSmellsTest\\DeprecatedTypesSP.sql");
            this._ExpectedProblems.Add(new TestProblem(4, 14, "Smells.SML047"));
            this._ExpectedProblems.Add(new TestProblem(5, 14, "Smells.SML047"));

        }

        [TestMethod]
        public void DeprecatedTypesSP()
        {

            RunTest();
        }

    }
    

    
    
}


