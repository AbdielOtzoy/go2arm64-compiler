using analyzer;

public class CompilerVisitor : LanguageBaseVisitor<Object?>
{
    public ArmGenerator c = new ArmGenerator();    

    public CompilerVisitor()
    {

    }

    public override Object? VisitProgram(LanguageParser.ProgramContext context){
        foreach (var declaration in context.declaration())
        {
            Visit(declaration);
        }
        return null;
    }

    public override Object? VisitVarDeclaration(LanguageParser.VarDeclarationContext context){
        return null;
    }

    public override Object? VisitFuncDeclaration(LanguageParser.FuncDeclarationContext context){
        return null;
    }

    public override Object? VisitSlicesDeclaration(LanguageParser.SlicesDeclarationContext context){
        return null;
    }

    public override Object? VisitSlice(LanguageParser.SliceContext context){
        return null;
    }

    public override Object? VisitMatrixDeclaration(LanguageParser.MatrixDeclarationContext context){
        return null;
    }

    public override Object? VisitStructDeclaration(LanguageParser.StructDeclarationContext context){
        return null;
    }

    public override Object? VisitExprStmt(LanguageParser.ExprStmtContext context){
        return null;
    }

    public override Object? VisitPrintStmt(LanguageParser.PrintStmtContext context){
        c.Comment("Print statement");
        Visit(context.expr());
        c.Pop(Register.X0);
        c.PrintInteger(Register.X0);
        
        return null;
    }

    public override Object? VisitBlockStmt(LanguageParser.BlockStmtContext context){
        return null;
    }

    public override Object? VisitIfStmt(LanguageParser.IfStmtContext context){
        return null;
    }

    public override Object? VisitSwitchStmt(LanguageParser.SwitchStmtContext context){
        return null;
    }

    public override Object? VisitCaseClause(LanguageParser.CaseClauseContext context){
        return null;
    }

    public override Object? VisitDefaultClause(LanguageParser.DefaultClauseContext context){
        return null;
    }

    public override Object? VisitForStmt(LanguageParser.ForStmtContext context){
        return null;
    }

    public override Object? VisitForDeclStmt(LanguageParser.ForDeclStmtContext context){
        return null;
    }

    public override Object? VisitForRangeStmt(LanguageParser.ForRangeStmtContext context){
        return null;
    }

    public override Object? VisitBreakStmt(LanguageParser.BreakStmtContext context){
        return null;
    }

    public override Object? VisitContinueStmt(LanguageParser.ContinueStmtContext context){
        return null;
    }

    public override Object? VisitReturnStmt(LanguageParser.ReturnStmtContext context){
        return null;
    }

    public override Object? VisitNegate(LanguageParser.NegateContext context){
        return null;
    }

    public override Object? VisitFloat(LanguageParser.FloatContext context){
        return null;
    }

    public override Object? VisitNew(LanguageParser.NewContext context){
        return null;
    }

    public override Object? VisitIdentifier(LanguageParser.IdentifierContext context){
        return null;
    }

    public override Object? VisitIndex(LanguageParser.IndexContext context){
        return null;
    }

    public override Object? VisitMatrixIndex(LanguageParser.MatrixIndexContext context){
        return null;
    }

    public override Object? VisitIndexMethod(LanguageParser.IndexMethodContext context){
        return null;
    }

    public override Object? VisitJoinMethod(LanguageParser.JoinMethodContext context){
        return null;
    }

    public override Object? VisitLenMethod(LanguageParser.LenMethodContext context){
        return null;
    }

    public override Object? VisitAppendMethod(LanguageParser.AppendMethodContext context){
        return null;
    }

    public override Object? VisitAtoiMethod(LanguageParser.AtoiMethodContext context){
        return null;
    }

    public override Object? VisitParseFloatMethod(LanguageParser.ParseFloatMethodContext context){
        return null;
    }

    public override Object? VisitTypeOfMethod(LanguageParser.TypeOfMethodContext context){
        return null;
    }

    public override Object? VisitNumber(LanguageParser.NumberContext context){
        var value = context.INT().GetText();
        c.Comment("Constant: " + value);
        c.Mov(Register.X0, int.Parse(value));
        c.Push(Register.X0);
        return null;
    }

    public override Object? VisitRune(LanguageParser.RuneContext context){
        return null;
    }

    public override Object? VisitBool(LanguageParser.BoolContext context){
        return null;
    }

    public override Object? VisitNil(LanguageParser.NilContext context){
        return null;
    }

    public override Object? VisitMulDiv(LanguageParser.MulDivContext context){
        return null;
    }

    public override Object? VisitMod(LanguageParser.ModContext context){
        return null;
    }

    public override Object? VisitAddSub(LanguageParser.AddSubContext context){
        var operation = context.op.Text;
        Visit(context.expr(0));
        Visit(context.expr(1));

        c.Pop(Register.X1);
        c.Pop(Register.X0);

        if (operation == "+")
        {
            c.Add(Register.X0, Register.X0, Register.X1);
        }
        else if (operation == "-")
        {
            c.Sub(Register.X0, Register.X0, Register.X1);
        }

        c.Push(Register.X0);

        return null;
    }

    public override Object? VisitParens(LanguageParser.ParensContext context){
        return null;
    }

    public override Object? VisitRelational(LanguageParser.RelationalContext context){
        return null;
    }

    public override Object? VisitLogical(LanguageParser.LogicalContext context){
        return null;
    }

    public override Object? VisitNot(LanguageParser.NotContext context){
        return null;
    }

    public override Object? VisitAssign(LanguageParser.AssignContext context){
        return null;
    }

    public override Object? VisitString(LanguageParser.StringContext context){
        return null;
    }

    public override Object? VisitEquality(LanguageParser.EqualityContext context){
        return null;
    }

    public override Object? VisitAddAssign(LanguageParser.AddAssignContext context){
        return null;
    }

    public override Object? VisitSubAssign(LanguageParser.SubAssignContext context){
        return null;
    }

    public override Object? VisitInc(LanguageParser.IncContext context){
        return null;
    }

    public override Object? VisitDec(LanguageParser.DecContext context){
        return null;
    }

    public override Object? VisitCallee(LanguageParser.CalleeContext context){
        return null;
    }
}