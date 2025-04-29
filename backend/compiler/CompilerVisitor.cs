using analyzer;

public class FunctionMetadata {
    public int FrameSize;
    public StackObject.StackObjectType ReturnType;
}

public class CompilerVisitor : LanguageBaseVisitor<Object?>
{
    public ArmGenerator c = new ArmGenerator();  
    private string continueLabel = "";
    private string breakLabel = "";
    private string returnLabel = "";
    private Dictionary<string, FunctionMetadata> functions = new Dictionary<string, FunctionMetadata>();
    private string? insideFunction = null;
    private int framePointerOffset = 0;

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
        var varName = context.ID(0).GetText();
        c.Comment("Variable declaration: " + varName);
        Console.WriteLine("Variable declaration: " + varName);
        Visit(context.expr());

        if (insideFunction != null) {
            var localObject = c.GetFrameLocal(framePointerOffset);
            var valueObject = c.PopObject(Register.X0);

            c.Mov(Register.X1, framePointerOffset*8);
            c.Sub(Register.X1, Register.FP, Register.X1);
            c.Str(Register.X0, Register.X1);

            localObject.Type = valueObject.Type;
            framePointerOffset++;

            return null;
        }

        c.TagObject(varName);
        return null;
    }

    public override Object? VisitFuncDeclaration(LanguageParser.FuncDeclarationContext context){
        int baseOffset = 2;
        int paramsOffset = 0;

        if (context.@params() != null) {
            paramsOffset = context.@params().ID().Length;
        } 

        FrameVisitor frameVisitor = new FrameVisitor(baseOffset + paramsOffset);

        foreach (var declaration in context.declaration())
        {
            frameVisitor.Visit(declaration);
        }

        var frame = frameVisitor.Frame;
        int localOffset = frame.Count;
        int returnOffset = 1;

        int totalFrameSize = baseOffset + paramsOffset + localOffset + returnOffset;

        string funcName = context.ID(0).GetText();
        StackObject.StackObjectType funcType = GetType(context.TYPE()?.GetText() ?? "undefined");


        functions.Add(funcName, new FunctionMetadata
        {
            FrameSize = totalFrameSize,
            ReturnType = funcType,
        });

        var prevInstrucions = c._instructions;
        c._instructions = new List<string>();


        if (context.@params() != null)
        {
        var paramCounter = 0;
        foreach (var param in context.@params().ID())
        {
            Console.WriteLine("Param: " + param.GetText());
            c.PushObject(new StackObject
            {
                Type = GetType(context.@params().TYPE(paramCounter).GetText()),
                Id = param.GetText(),
                Offset = paramCounter + baseOffset,
                Length = 8
            });
            paramCounter++;
        }
        }

        foreach(FrameElement element in frame) {
            c.PushObject(new StackObject
            {
                Type = StackObject.StackObjectType.Undefined,
                Id = element.Name,
                Offset = element.Offset,
                Length = 8
            });
        }


        insideFunction = funcName;
        framePointerOffset = 0;

        returnLabel = c.GetLabel();

        c.Comment("Function: " + funcName);
        c.SetLabel(funcName);

        // Al inicio de la función, justo después de c.SetLabel(funcName)
        c.Comment("Function prologue");
        c._instructions.Add("STR x30, [SP, #-8]!");
        

        foreach (var declaration in context.declaration())
        {
            Visit(declaration);
        }

        c.SetLabel(returnLabel);

        c.Comment("Epilogue");
        c.Add(Register.X0, Register.SP, Register.XZR);
        c.Ldr(Register.LR, Register.X0);
        c.Br(Register.LR);

        c.Comment("End of function: " + funcName);

        for(int i = 0; i < paramsOffset + localOffset; i++) {
            c.PopObject();
        }

        foreach(var instructinos in c._instructions) {
            c.funcInstructions.Add(instructinos);
        }
        c._instructions = prevInstrucions;
        insideFunction = null;


        return null;
    }

    StackObject.StackObjectType GetType(string type)
    {
        switch (type)
        {
            case "int":
                return StackObject.StackObjectType.Int;
            case "float":
                return StackObject.StackObjectType.Float;
            case "string":
                return StackObject.StackObjectType.String;
            case "bool":
                return StackObject.StackObjectType.Bool;
            case "void":
                return StackObject.StackObjectType.Undefined;
            case "undefined":
                return StackObject.StackObjectType.Undefined;
            default:
                throw new ArgumentException("Invalid function type");
        }
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
        c.Comment("Expression statement");
        Visit(context.expr());
        c.PopObject(Register.X0);
        return null;
    }

    public override Object? VisitPrintStmt(LanguageParser.PrintStmtContext context){
        c.Comment("Print statement");
        Object? expresion = Visit(context.expr());

        //try convert stack objects to registers
        if (expresion is StackObject stackObj)
        {
            Console.WriteLine("Stack object: " + stackObj.Type);
            if (stackObj.Type == StackObject.StackObjectType.Float)
            {
                var popValue = c.PopObject(Register.D0);
                c.Comment("Printing float");
                c.Comment("Popping float");
                c.PrintFloat(Register.D0);
                return null;
            }
            else if (stackObj.Type == StackObject.StackObjectType.Bool)
            {
                var popValue = c.PopObject(Register.X0);
                c.Comment("Printing boolean");
                c.Comment("Popping boolean");
                c.PrintBool(Register.X0);
                return null;
            }
            else if (stackObj.Type == StackObject.StackObjectType.Int)
            {
                var popValue = c.PopObject(Register.X0);
                c.Comment("Printing integer");
                c.Comment("Popping integer");
                c.PrintInteger(Register.X0);
                return null;
            }
            else if (stackObj.Type == StackObject.StackObjectType.String)
            {
                var popValue = c.PopObject(Register.X0);
                c.Comment("Printing string");
                c.Comment("Popping string");
                c.PrintString(Register.X0);
                return null;
            }

            return null;
        }

        c.Comment("Popping");
        var value = c.PopObject(Register.X0);
        if(value.Type == StackObject.StackObjectType.Int) {
            c.Comment("Printing integer");
            c.PrintInteger(Register.X0);
        }
        else if (value.Type == StackObject.StackObjectType.String) {
            c.Comment("Printing string");
            c.PrintString(Register.X0);
        }
        else if (value.Type == StackObject.StackObjectType.Bool) {
            c.Comment("Printing boolean");
            c.PrintBool(Register.X0);
        }
        else if (value.Type == StackObject.StackObjectType.Float) {
            c.Comment("Printing float");
            c.PrintFloat(Register.D0);
        }
        
        return null;
    }

    public override Object? VisitBlockStmt(LanguageParser.BlockStmtContext context){
        c.Comment("Block statement");
        c.NewScope();

        foreach (var declaration in context.declaration())
        {
            Visit(declaration);
        }

        int bytesToRemove = c.EndScope();

        if(bytesToRemove > 0) {
            c.Comment("Popping local variables");
            c.Mov(Register.X0, bytesToRemove);
            c.Add(Register.SP, Register.SP, Register.X0);
            c.Comment("Popped local variables");
        }

        return null;
    }

    public override Object? VisitIfStmt(LanguageParser.IfStmtContext context){
        c.Comment("If statement");
        var expresion = Visit(context.expr());
        var condition = c.PopObject(Register.X0);
        c.Comment("Condition: " + condition.Type);

        var hasElse = context.statement().Length > 1;

        if (hasElse) {
            var elseLabel = c.GetLabel();
            var endLabel = c.GetLabel();

            c.Cbz(Register.X0, elseLabel);
            Visit(context.statement(0));
            c.B(endLabel);
            c.SetLabel(elseLabel);
            Visit(context.statement(1));
            c.SetLabel(endLabel);
            
        } else {
            var endLabel = c.GetLabel();
            c.Cbz(Register.X0, endLabel);
            Visit(context.statement(0));
            c.SetLabel(endLabel);
        }

        return null;
    }

    public override Object? VisitSwitchStmt(LanguageParser.SwitchStmtContext context){
        c.Comment("Switch statement");
        var expresion = Visit(context.expr());
        var condition = c.PopObject(Register.X0);
        List<string> caseLabels = new List<string>();

        var endLabel = c.GetLabel();
        var prevBreakLabel = breakLabel;
        breakLabel = endLabel;

        foreach (var caseClause in context.caseClauseStmt()) {
            if (caseClause is LanguageParser.CaseClauseContext caseClauseStmt)
            {
                var caseLabel = c.GetLabel();
                caseLabels.Add(caseLabel);
                int val = int.Parse(caseClauseStmt.expr().GetText());
                c.switchCase(Register.X0, val, caseLabel);
            }
            else if (caseClause is LanguageParser.DefaultClauseContext defaultClauseStmt)
            {   
                var defaultLabel = c.GetLabel();
                c.B(defaultLabel);
                caseLabels.Add(defaultLabel);
            }
            
        }
        int count = 0;
        foreach (var caseClause in context.caseClauseStmt()) {
            if (caseClause is LanguageParser.CaseClauseContext caseClauseStmt)
            {
                var caseLabel = caseLabels[count];
                c.SetLabel(caseLabel);
                foreach (var declaration in caseClauseStmt.declaration())
                {
                    Visit(declaration);
                }
                c.B(endLabel);
                count++;
            }
            else if (caseClause is LanguageParser.DefaultClauseContext defaultClauseStmt)
            {
                var defaultLabel = caseLabels[count];
                c.SetLabel(defaultLabel);
                count++;
                foreach (var declaration in defaultClauseStmt.declaration())
                {
                    Visit(declaration);
                }
            }
            
        }
        c.SetLabel(endLabel);
        c.Comment("End of switch statement");
        return null;
    }

    public override Object? VisitCaseClause(LanguageParser.CaseClauseContext context){
        return null;
    }

    public override Object? VisitDefaultClause(LanguageParser.DefaultClauseContext context){
        return null;
    }

    public override Object? VisitForStmt(LanguageParser.ForStmtContext context){
        c.Comment("While statement");

        var startLabel = c.GetLabel();
        var endLabel = c.GetLabel();
        var prevContinueLabel = continueLabel;
        var prevBreakLabel = breakLabel;
        continueLabel = startLabel;
        breakLabel = endLabel;
        c.SetLabel(startLabel);
        Visit(context.expr());
        c.PopObject(Register.X0);
        c.Cbz(Register.X0, endLabel);
        Visit(context.statement());
        c.B(startLabel);
        c.SetLabel(endLabel);
        c.Comment("End of while statement");

        continueLabel = prevContinueLabel;
        breakLabel = prevBreakLabel;

        return null;
    }

    public override Object? VisitForDeclStmt(LanguageParser.ForDeclStmtContext context){
        c.Comment("For statement");
        c.NewScope();

        var startLabel = c.GetLabel();
        var endLabel = c.GetLabel();
        var incrementLabel = c.GetLabel();

        var prevContinueLabel = continueLabel;
        var prevBreakLabel = breakLabel
;
        continueLabel = incrementLabel;
        breakLabel = endLabel;

        Visit(context.forInit());
        c.SetLabel(startLabel);
        Visit(context.expr(0));
        c.PopObject(Register.X0);
        c.Cbz(Register.X0, endLabel);
        Visit(context.statement());
        c.SetLabel(incrementLabel);
        Visit(context.expr(1));
        c.B(startLabel);
        c.SetLabel(endLabel);
        c.Comment("End of for statement");


        int bytesToRemove = c.EndScope();

        if(bytesToRemove > 0) {
            c.Comment("Popping local variables");
            c.Mov(Register.X0, bytesToRemove);
            c.Add(Register.SP, Register.SP, Register.X0);
            c.Comment("Popped local variables");
        }

        return null;
    }

    public override Object? VisitForRangeStmt(LanguageParser.ForRangeStmtContext context){
        return null;
    }

    public override Object? VisitBreakStmt(LanguageParser.BreakStmtContext context){
        c.Comment("Break statement");
        if (breakLabel != null)
        {
            c.B(breakLabel);
        }

        return null;
    }

    public override Object? VisitContinueStmt(LanguageParser.ContinueStmtContext context){
        c.Comment("Continue statement");
        if (continueLabel != null)
        {
            c.B(continueLabel);
        }

        return null;
    }

    public override Object? VisitReturnStmt(LanguageParser.ReturnStmtContext context){
        c.Comment("Return statement");
        if (context.expr() == null) {
            c.Br(returnLabel);
            return null;
        }

        if (insideFunction == null) throw new Exception("Return statement outside function");

        Visit(context.expr());
        c.PopObject(Register.X0);

        var frameSize = functions[insideFunction].FrameSize;    
        var returnOffset = frameSize - 1;
        c.Mov(Register.X1, returnOffset*8);
        c.Sub(Register.X1, Register.FP, Register.X1);
        c.Str(Register.X0, Register.X1);
        c.B(returnLabel);

        c.Comment("End of return statement");
        return null;         
    }

    public override Object? VisitNegate(LanguageParser.NegateContext context){
        return null;
    }

    public override Object? VisitNew(LanguageParser.NewContext context){
        return null;
    }

    public override Object? VisitIdentifier(LanguageParser.IdentifierContext context){
        var id = context.ID().GetText();
        var (offset, obj) = c.GetObject(id);

        if(insideFunction != null) {
            c.Mov(Register.X0, obj.Offset*8);
            c.Sub(Register.X0, Register.FP, Register.X0);
            c.Ldr(Register.X0, Register.X0);
            c.Push(Register.X0);
            var cloneObject = c.CloneObject(obj);
            cloneObject.Id = null;
            c.PushObject(cloneObject);
            return cloneObject;
        }

        c.Mov(Register.X0, offset);
        c.Add(Register.X0, Register.SP, Register.X0);
        c.Ldr(Register.X0, Register.X0);
        c.Push(Register.X0);
        var newObject = c.CloneObject(obj);
        newObject.Id = null;
        c.PushObject(newObject);
        return newObject;
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
        var intObj = c.IntObject();
        c.PushConstant(intObj, value);
        return intObj;
    }

    public override Object? VisitFloat(LanguageParser.FloatContext context){
        var value = context.FLOAT().GetText();
        c.Comment("Constant: " + value);
        var floatObj = c.FloatObject();
        c.PushConstant(floatObj, value);
        return floatObj;
    }

    public override Object? VisitRune(LanguageParser.RuneContext context){
        return null;
    }

    public override Object? VisitBool(LanguageParser.BoolContext context){
        var value = context.BOOL().GetText();
        c.Comment("Constant: " + value);
        var boolObj = c.BoolObject();
        c.PushConstant(boolObj, value);
        return boolObj;
    }

    public override Object? VisitNil(LanguageParser.NilContext context){
        return null;
    }

    public override Object? VisitMulDiv(LanguageParser.MulDivContext context){
        var operation = context.op.Text;
        Object? left = Visit(context.expr(0));
        Object? right = Visit(context.expr(1));

        c.Comment("Multiplying");
        // try convert stack objects to registers
        if (left is StackObject leftStack && right is StackObject rightStack)
        {
            Console.WriteLine("Left: " + leftStack.Type);
            Console.WriteLine("Right: " + rightStack.Type);

            if (leftStack.Type == StackObject.StackObjectType.Int && rightStack.Type == StackObject.StackObjectType.Int)
            {
                var right1 = c.PopObject(Register.X1);
                var left1 = c.PopObject(Register.X0);

                if (operation == "*")
                {
                    c.Mul(Register.X0, Register.X0, Register.X1);
                }
                else if (operation == "/")
                {
                    c.Div(Register.X0, Register.X0, Register.X1);
                }

                c.Push(Register.X0);
                c.PushObject(c.CloneObject(left1));
            
                return c.IntObject();
            }
            else if (leftStack.Type == StackObject.StackObjectType.Float && rightStack.Type == StackObject.StackObjectType.Float)
            {
                var right1 = c.PopObject(Register.D1);
                var left1 = c.PopObject(Register.D0);
                if (operation == "*")
                {
                    c.Fmul(Register.D0, Register.D0, Register.D1);
                }
                else if (operation == "/")
                {
                    c.Fdiv(Register.D0, Register.D0, Register.D1);
                }
                c.Push(Register.D0);
                c.PushObject(c.CloneObject(left1));
                return c.FloatObject();
            }
            else if (leftStack.Type == StackObject.StackObjectType.Int && rightStack.Type == StackObject.StackObjectType.Float)
            {
                var right1 = c.PopObject(Register.D1);
                var left1 = c.PopObject(Register.X0);
                c.Scvtf(Register.D0, Register.X0);
                if (operation == "*")
                {
                    c.Fmul(Register.D0, Register.D0, Register.D1);
                }
                else if (operation == "/")
                {
                    c.Fdiv(Register.D0, Register.D0, Register.D1);
                }
                c.Push(Register.D0);
                c.PushObject(c.CloneObject(left1));
                return c.FloatObject();
            }
            else if (leftStack.Type == StackObject.StackObjectType.Float && rightStack.Type == StackObject.StackObjectType.Int)
            {
                var right1 = c.PopObject(Register.X1);
                var left1 = c.PopObject(Register.D0);
                c.Scvtf(Register.D1, Register.X1);
                if (operation == "*")
                {
                    c.Fmul(Register.D0, Register.D0, Register.D1);
                }
                else if (operation == "/")
                {
                    c.Fdiv(Register.D0, Register.D0, Register.D1);
                }
                c.Push(Register.D0);
                c.PushObject(c.CloneObject(left1));
                return c.FloatObject();
            }
        }

       return null;
    }

    public override Object? VisitMod(LanguageParser.ModContext context){
        Visit(context.expr(0));
        Visit(context.expr(1));

        c.Comment("Modulo");
        var right = c.PopObject(Register.X1);
        var left = c.PopObject(Register.X0);

        c.Mod(Register.X0, Register.X0, Register.X1);
        c.Push(Register.X0);
        c.PushObject(c.CloneObject(left));
        return c.IntObject();
    }

    public override Object? VisitAddSub(LanguageParser.AddSubContext context){
        var operation = context.op.Text;
        Object? left1 = Visit(context.expr(0));
        Object? right1 = Visit(context.expr(1));

        c.Comment("Adding");
        // try convert stack objects to registers
        if (left1 is StackObject leftStack && right1 is StackObject rightStack)
        {
            Console.WriteLine("Left: " + leftStack.Type);
            Console.WriteLine("Right: " + rightStack.Type);

            if (leftStack.Type == StackObject.StackObjectType.Int && rightStack.Type == StackObject.StackObjectType.Int)
            {
                var right = c.PopObject(Register.X1);
                var left = c.PopObject(Register.X0);

                if (operation == "+")
                {
                    c.Add(Register.X0, Register.X0, Register.X1);
                }
                else if (operation == "-")
                {
                    c.Sub(Register.X0, Register.X0, Register.X1);
                }

                c.Push(Register.X0);
                c.PushObject(c.CloneObject(left));

                return c.IntObject();
            }
            else if (leftStack.Type == StackObject.StackObjectType.Float && rightStack.Type == StackObject.StackObjectType.Float)
            {
                var right = c.PopObject(Register.D1);
                var left = c.PopObject(Register.D0);
                if (operation == "+")
                {
                    c.Fadd(Register.D0, Register.D0, Register.D1);
                }
                else if (operation == "-")
                {
                    c.Fsub(Register.D0, Register.D0, Register.D1);
                }
                c.Push(Register.D0);
                c.PushObject(c.CloneObject(left));
                return c.FloatObject();
            }
            else if (leftStack.Type == StackObject.StackObjectType.Int && rightStack.Type == StackObject.StackObjectType.Float)
            {
                var right = c.PopObject(Register.D1);
                var left = c.PopObject(Register.X0);
                c.Scvtf(Register.D0, Register.X0);
                if (operation == "+")
                {
                    c.Fadd(Register.D0, Register.D0, Register.D1);
                }
                else if (operation == "-")
                {
                    c.Fsub(Register.D0, Register.D0, Register.D1);
                }
                c.Push(Register.D0);
                c.PushObject(c.CloneObject(left));
                return c.FloatObject();
            }
            else if (leftStack.Type == StackObject.StackObjectType.Float && rightStack.Type == StackObject.StackObjectType.Int)
            {
                var right = c.PopObject(Register.X1);
                var left = c.PopObject(Register.D0);
                c.Scvtf(Register.D1, Register.X1);
                if (operation == "+")
                {
                    c.Fadd(Register.D0, Register.D0, Register.D1);
                }
                else if (operation == "-")
                {
                    c.Fsub(Register.D0, Register.D0, Register.D1);
                }
                c.Push(Register.D0);
                c.PushObject(c.CloneObject(left));
                return c.FloatObject();
            }

        }
       
        return null;
    }

    public override Object? VisitParens(LanguageParser.ParensContext context){
        return Visit(context.expr());
    }

    public override Object? VisitRelational(LanguageParser.RelationalContext context){
        var operation = context.op.Text;
        Object? left = Visit(context.expr(0));
        Object? right = Visit(context.expr(1));
        c.Comment("Comparing");

        // try convert stack objects to registers
        if (left is StackObject leftStack && right is StackObject rightStack)
        {
            Console.WriteLine("Left: " + leftStack.Type);
            Console.WriteLine("Right: " + rightStack.Type);

            if (leftStack.Type == StackObject.StackObjectType.Int && rightStack.Type == StackObject.StackObjectType.Int)
            {
                var right1 = c.PopObject(Register.X1);
                var left1 = c.PopObject(Register.X0);

                c.Cmp(Register.X0, Register.X1);
                var trueLabel = c.GetLabel();
                var endLabel = c.GetLabel();

                if (operation == "<")
                {
                    c.LessThan(trueLabel);
                }
                else if (operation == "<=")
                {
                    c.LessThanOrEqual(trueLabel);
                }
                else if (operation == ">")
                {
                    c.GreaterThan(trueLabel);
                }
                else if (operation == ">=")
                {
                    c.GreaterThanOrEqual(trueLabel);
                }

                c.Mov(Register.X0, 0);
                c.Push(Register.X0);
                c.B(endLabel);
                c.SetLabel(trueLabel);
                c.Mov(Register.X0, 1);
                c.Push(Register.X0);
                c.SetLabel(endLabel);
                c.PushObject(c.CloneObject(left1));

                return c.BoolObject();
            } else if (leftStack.Type == StackObject.StackObjectType.Float && rightStack.Type == StackObject.StackObjectType.Float)
            {
                var right1 = c.PopObject(Register.D1);
                var left1 = c.PopObject(Register.D0);

                c.Fcmp(Register.D0, Register.D1);
                var trueLabel = c.GetLabel();
                var endLabel = c.GetLabel();

                if (operation == "<")
                {
                    c.FLessThan(trueLabel);
                }
                else if (operation == "<=")
                {
                    c.FLessThanOrEqual(trueLabel);
                }
                else if (operation == ">")
                {
                    c.FGreaterThan(trueLabel);
                }
                else if (operation == ">=")
                {
                    c.FGreaterThanOrEqual(trueLabel);
                }
                c.Mov(Register.X0, 0);
                c.Push(Register.X0);
                c.B(endLabel);
                c.SetLabel(trueLabel);
                c.Mov(Register.X0, 1);
                c.Push(Register.X0);
                c.SetLabel(endLabel);
                c.PushObject(c.CloneObject(left1));
                return c.BoolObject();
            } else if (leftStack.Type == StackObject.StackObjectType.Int && rightStack.Type == StackObject.StackObjectType.Float)
            {
                var right1 = c.PopObject(Register.D1);
                var left1 = c.PopObject(Register.X0);
                c.Scvtf(Register.D0, Register.X0);
                c.Fcmp(Register.D0, Register.D1);
                var trueLabel = c.GetLabel();
                var endLabel = c.GetLabel();
                if (operation == "<")
                {
                    c.FLessThan(trueLabel);
                }
                else if (operation == "<=")
                {
                    c.FLessThanOrEqual(trueLabel);
                }
                else if (operation == ">")
                {
                    c.FGreaterThan(trueLabel);
                }
                else if (operation == ">=")
                {
                    c.FGreaterThanOrEqual(trueLabel);
                }
                c.Mov(Register.X0, 0);
                c.Push(Register.X0);
                c.B(endLabel);
                c.SetLabel(trueLabel);
                c.Mov(Register.X0, 1);
                c.Push(Register.X0);
                c.SetLabel(endLabel);
                c.PushObject(c.CloneObject(left1));
                return c.BoolObject();
            } else if (leftStack.Type == StackObject.StackObjectType.Float && rightStack.Type == StackObject.StackObjectType.Int)
            {
                var right1 = c.PopObject(Register.X1);
                var left1 = c.PopObject(Register.D0);
                c.Scvtf(Register.D1, Register.X1);
                c.Fcmp(Register.D0, Register.D1);
                var trueLabel = c.GetLabel();
                var endLabel = c.GetLabel();
                if (operation == "<")
                {
                    c.FLessThan(trueLabel);
                }
                else if (operation == "<=")
                {
                    c.FLessThanOrEqual(trueLabel);
                }
                else if (operation == ">")
                {
                    c.FGreaterThan(trueLabel);
                }
                else if (operation == ">=")
                {
                    c.FGreaterThanOrEqual(trueLabel);
                }
                
                c.Mov(Register.X0, 0);
                c.Push(Register.X0);
                c.B(endLabel);
                c.SetLabel(trueLabel);
                c.Mov(Register.X0, 1);
                c.Push(Register.X0);
                c.SetLabel(endLabel);
                c.PushObject(c.CloneObject(left1));
                return c.BoolObject();
            }
        }
        return null;
    }

    public override Object? VisitLogical(LanguageParser.LogicalContext context){
        var operation = context.op.Text;
        Object? left = Visit(context.expr(0));
        Object? right = Visit(context.expr(1));
        c.Comment("Logical operation");

        // try convert stack objects to registers
        if (left is StackObject leftStack && right is StackObject rightStack)
        {
            Console.WriteLine("Left: " + leftStack.Type);
            Console.WriteLine("Right: " + rightStack.Type);

            if (leftStack.Type == StackObject.StackObjectType.Bool && rightStack.Type == StackObject.StackObjectType.Bool)
            {
                var right1 = c.PopObject(Register.X1);
                var left1 = c.PopObject(Register.X0);

                if (operation == "&&")
                {
                    c.And(Register.X0, Register.X0, Register.X1);
                }
                else if (operation == "||")
                {
                    c.Or(Register.X0, Register.X0, Register.X1);
                }

                c.Push(Register.X0);
                c.PushObject(c.CloneObject(left1));

                return c.BoolObject();
            }
        }
        return null;
    }

    public override Object? VisitNot(LanguageParser.NotContext context){
        Object? left = Visit(context.expr());
        c.Comment("Logical NOT operation");

        // try convert stack objects to registers
        if (left is StackObject leftStack)
        {
            Console.WriteLine("Left: " + leftStack.Type);

            if (leftStack.Type == StackObject.StackObjectType.Bool)
            {
                var left1 = c.PopObject(Register.X0);
                c.Not(Register.X0, Register.X0);
                c.Push(Register.X0);
                c.PushObject(c.CloneObject(left1));

                return c.BoolObject();
            }
        }
        return null;
    }

    public override Object? VisitAssign(LanguageParser.AssignContext context){
        var assignee = context.expr(0);
        Console.WriteLine("Assignee: " + assignee.GetText());

        if( assignee is LanguageParser.IdentifierContext idContext) {
        Console.WriteLine("Assignee: " + assignee.GetText());
            string varName = idContext.ID().GetText();
            c.Comment("Assigning to variable: " + varName);
            Visit(context.expr(1));
            var valueObj = c.PopObject(Register.X0);
            var (offset, varObj) = c.GetObject(varName);

            if(insideFunction != null) {
                c.Mov(Register.X1, varObj.Offset*8);
                c.Sub(Register.X1, Register.FP, Register.X1);
                c.Str(Register.X0, Register.X1);
                return null;
            }

            c.Mov(Register.X1, offset);
            c.Add(Register.X1, Register.SP, Register.X1);
            c.Str(Register.X0, Register.X1);

            varObj.Type = valueObj.Type;
            
            c.Push(Register.X0);
            c.PushObject(c.CloneObject(valueObj));
            return null;
        }

        return null;
    }

    public override Object? VisitString(LanguageParser.StringContext context){
        var value = context.STRING().GetText().Trim('"');
        c.Comment("Constant: " + value);
        var stringObj = c.StringObject();
        c.PushConstant(stringObj, value);
        return stringObj;
    }

    public override Object? VisitEquality(LanguageParser.EqualityContext context){
        var operation = context.op.Text;
        Object? left = Visit(context.expr(0));
        Object? right = Visit(context.expr(1));
        c.Comment("Comparing");

        // try convert stack objects to registers
        if (left is StackObject leftStack && right is StackObject rightStack)
        {
            Console.WriteLine("Left: " + leftStack.Type);
            Console.WriteLine("Right: " + rightStack.Type);

            if (leftStack.Type == StackObject.StackObjectType.Int && rightStack.Type == StackObject.StackObjectType.Int)
            {
                var right1 = c.PopObject(Register.X1);
                var left1 = c.PopObject(Register.X0);

                c.Cmp(Register.X0, Register.X1);
                var trueLabel = c.GetLabel();
                var endLabel = c.GetLabel();

                if (operation == "==")
                {
                    c.Equal(trueLabel); 
                }
                else if (operation == "!=")
                {
                    c.NotEqual(trueLabel);
                }

                c.Mov(Register.X0, 0);
                c.Push(Register.X0);
                c.B(endLabel);
                c.SetLabel(trueLabel);
                c.Mov(Register.X0, 1);
                c.Push(Register.X0);
                c.SetLabel(endLabel);
                c.PushObject(c.CloneObject(left1));
            
                return c.BoolObject();
            } else if (leftStack.Type == StackObject.StackObjectType.Float && rightStack.Type == StackObject.StackObjectType.Float)
            {
                var right1 = c.PopObject(Register.D1);
                var left1 = c.PopObject(Register.D0);
                c.Fcmp(Register.D0, Register.D1);
                var trueLabel = c.GetLabel();
                var endLabel = c.GetLabel();
                if (operation == "==")
                {
                    c.FEqual(trueLabel);
                }
                else if (operation == "!=")
                {
                    c.FNotEqual(trueLabel);
                }
                c.Mov(Register.X0, 0);
                c.Push(Register.X0);
                c.B(endLabel);
                c.SetLabel(trueLabel);
                c.Mov(Register.X0, 1);
                c.Push(Register.X0);
                c.SetLabel(endLabel);
                c.PushObject(c.CloneObject(left1));
                return c.BoolObject();
            } else if (leftStack.Type == StackObject.StackObjectType.Int && rightStack.Type == StackObject.StackObjectType.Float)
            {
                var right1 = c.PopObject(Register.D1);
                var left1 = c.PopObject(Register.X0);
                c.Scvtf(Register.D0, Register.X0);
                c.Fcmp(Register.D0, Register.D1);
                var trueLabel = c.GetLabel();
                var endLabel = c.GetLabel();
                if (operation == "==")
                {
                    c.FEqual(trueLabel);
                }
                else if (operation == "!=")
                {
                    c.FNotEqual(trueLabel);
                }
                c.Mov(Register.X0, 0);
                c.Push(Register.X0);
                c.B(endLabel);
                c.SetLabel(trueLabel);
                c.Mov(Register.X0, 1);
                c.Push(Register.X0);
                c.SetLabel(endLabel);
                c.PushObject(c.CloneObject(left1));
                return c.BoolObject();
            } else if (leftStack.Type == StackObject.StackObjectType.Float && rightStack.Type == StackObject.StackObjectType.Int)
            {
                var right1 = c.PopObject(Register.X1);
                var left1 = c.PopObject(Register.D0);
                c.Scvtf(Register.D1, Register.X1);
                c.Fcmp(Register.D0, Register.D1);
                var trueLabel = c.GetLabel();
                var endLabel = c.GetLabel();
                if (operation == "==")
                {
                    c.FEqual(trueLabel);
                }
                else if (operation == "!=")
                {
                    c.FNotEqual(trueLabel);
                }
                c.Mov(Register.X0, 0);
                c.Push(Register.X0);
                c.B(endLabel);
                c.SetLabel(trueLabel);
                c.Mov(Register.X0, 1);
                c.Push(Register.X0);
                c.SetLabel(endLabel);
                c.PushObject(c.CloneObject(left1));
                return c.BoolObject();
            } else if (leftStack.Type == StackObject.StackObjectType.Bool && rightStack.Type == StackObject.StackObjectType.Bool)
            {
                var right1 = c.PopObject(Register.X1);
                var left1 = c.PopObject(Register.X0);
                c.Cmp(Register.X0, Register.X1);
                var trueLabel = c.GetLabel();
                var endLabel = c.GetLabel();
                if (operation == "==")
                {
                    c.Equal(trueLabel);
                }
                else if (operation == "!=")
                {
                    c.NotEqual(trueLabel);
                }
                c.Mov(Register.X0, 0);
                c.Push(Register.X0);
                c.B(endLabel);
                c.SetLabel(trueLabel);
                c.Mov(Register.X0, 1);
                c.Push(Register.X0);
                c.SetLabel(endLabel);
                c.PushObject(c.CloneObject(left1));
                return c.BoolObject();
            }
            
        }
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
        Console.WriteLine("Callee: " + context.GetText());
        if (context.expr() is not LanguageParser.IdentifierContext idContext) return null;

        string funcName = idContext.ID().GetText();
        var call = context.call()[0];

        if(call is not LanguageParser.FuncCallContext callContext) return null;

        var postFuncCallLabel = c.GetLabel();

        int baseOffset = 2;
        int stackElementSize = 8;

        c.Mov(Register.X0, baseOffset * stackElementSize);
        c.Sub(Register.SP, Register.SP, Register.X0);   

        if(callContext.args() != null) {
            c.Comment("visiting args");
            foreach(var param in callContext.args().expr()) {
                Visit(param);
            }
        }

        c.Mov(Register.X0, stackElementSize*(baseOffset+callContext.args()?.expr().Length ?? 0));
        c.Add(Register.SP, Register.SP, Register.X0);

        Console.WriteLine("Entro2");

        c.Mov(Register.X0, stackElementSize);
        c.Sub(Register.X0, Register.SP, Register.X0);

        c.Adr(Register.X1, postFuncCallLabel);
        c.Push(Register.X1);

        c.Push(Register.FP);
        c.Add(Register.FP, Register.X0, Register.XZR);

        int frameSize = functions[funcName].FrameSize;
        c.Mov(Register.X0, (frameSize - 2)*stackElementSize);
        c.Sub(Register.SP, Register.SP, Register.X0);

        c.Comment("Calling function: " + funcName);
        c.Bl(funcName);
        c.Comment("Function call finished: " + funcName);
        c.SetLabel(postFuncCallLabel);

        var returnOffset = frameSize - 1;
        c.Mov(Register.X4, returnOffset*stackElementSize);
        c.Sub(Register.X4, Register.FP, Register.X4);
        c.Ldr(Register.X4, Register.X4);

        c.Mov(Register.X1, stackElementSize);
        c.Sub(Register.X1, Register.FP, Register.X1);
        c.Ldr(Register.FP, Register.X1);

        c.Mov(Register.X0, stackElementSize*frameSize);
        c.Add(Register.SP, Register.SP, Register.X0);

        c.Push(Register.X4);

         Console.WriteLine("Entro4");
        c.PushObject(new StackObject{
            Type = functions[funcName].ReturnType,
            Id = null,
            Offset = 0,
            Length = 8
        });

        c.Comment("Returning from function: " + funcName);

        return null;

    }
}