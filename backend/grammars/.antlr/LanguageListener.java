// Generated from /home/abdielo/dev/compi2/proyecto2/backend/grammars/Language.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.tree.ParseTreeListener;

/**
 * This interface defines a complete listener for a parse tree produced by
 * {@link LanguageParser}.
 */
public interface LanguageListener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by {@link LanguageParser#program}.
	 * @param ctx the parse tree
	 */
	void enterProgram(LanguageParser.ProgramContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#program}.
	 * @param ctx the parse tree
	 */
	void exitProgram(LanguageParser.ProgramContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#declaration}.
	 * @param ctx the parse tree
	 */
	void enterDeclaration(LanguageParser.DeclarationContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#declaration}.
	 * @param ctx the parse tree
	 */
	void exitDeclaration(LanguageParser.DeclarationContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#varDeclaration}.
	 * @param ctx the parse tree
	 */
	void enterVarDeclaration(LanguageParser.VarDeclarationContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#varDeclaration}.
	 * @param ctx the parse tree
	 */
	void exitVarDeclaration(LanguageParser.VarDeclarationContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#funcDeclaration}.
	 * @param ctx the parse tree
	 */
	void enterFuncDeclaration(LanguageParser.FuncDeclarationContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#funcDeclaration}.
	 * @param ctx the parse tree
	 */
	void exitFuncDeclaration(LanguageParser.FuncDeclarationContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#params}.
	 * @param ctx the parse tree
	 */
	void enterParams(LanguageParser.ParamsContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#params}.
	 * @param ctx the parse tree
	 */
	void exitParams(LanguageParser.ParamsContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#slicesDeclaration}.
	 * @param ctx the parse tree
	 */
	void enterSlicesDeclaration(LanguageParser.SlicesDeclarationContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#slicesDeclaration}.
	 * @param ctx the parse tree
	 */
	void exitSlicesDeclaration(LanguageParser.SlicesDeclarationContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#matrixDeclaration}.
	 * @param ctx the parse tree
	 */
	void enterMatrixDeclaration(LanguageParser.MatrixDeclarationContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#matrixDeclaration}.
	 * @param ctx the parse tree
	 */
	void exitMatrixDeclaration(LanguageParser.MatrixDeclarationContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#matrixRows}.
	 * @param ctx the parse tree
	 */
	void enterMatrixRows(LanguageParser.MatrixRowsContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#matrixRows}.
	 * @param ctx the parse tree
	 */
	void exitMatrixRows(LanguageParser.MatrixRowsContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#structDeclaration}.
	 * @param ctx the parse tree
	 */
	void enterStructDeclaration(LanguageParser.StructDeclarationContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#structDeclaration}.
	 * @param ctx the parse tree
	 */
	void exitStructDeclaration(LanguageParser.StructDeclarationContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#structBody}.
	 * @param ctx the parse tree
	 */
	void enterStructBody(LanguageParser.StructBodyContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#structBody}.
	 * @param ctx the parse tree
	 */
	void exitStructBody(LanguageParser.StructBodyContext ctx);
	/**
	 * Enter a parse tree produced by the {@code ExprStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterExprStmt(LanguageParser.ExprStmtContext ctx);
	/**
	 * Exit a parse tree produced by the {@code ExprStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitExprStmt(LanguageParser.ExprStmtContext ctx);
	/**
	 * Enter a parse tree produced by the {@code PrintStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterPrintStmt(LanguageParser.PrintStmtContext ctx);
	/**
	 * Exit a parse tree produced by the {@code PrintStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitPrintStmt(LanguageParser.PrintStmtContext ctx);
	/**
	 * Enter a parse tree produced by the {@code BlockStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterBlockStmt(LanguageParser.BlockStmtContext ctx);
	/**
	 * Exit a parse tree produced by the {@code BlockStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitBlockStmt(LanguageParser.BlockStmtContext ctx);
	/**
	 * Enter a parse tree produced by the {@code IfStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterIfStmt(LanguageParser.IfStmtContext ctx);
	/**
	 * Exit a parse tree produced by the {@code IfStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitIfStmt(LanguageParser.IfStmtContext ctx);
	/**
	 * Enter a parse tree produced by the {@code SwitchStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterSwitchStmt(LanguageParser.SwitchStmtContext ctx);
	/**
	 * Exit a parse tree produced by the {@code SwitchStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitSwitchStmt(LanguageParser.SwitchStmtContext ctx);
	/**
	 * Enter a parse tree produced by the {@code ForRangeStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterForRangeStmt(LanguageParser.ForRangeStmtContext ctx);
	/**
	 * Exit a parse tree produced by the {@code ForRangeStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitForRangeStmt(LanguageParser.ForRangeStmtContext ctx);
	/**
	 * Enter a parse tree produced by the {@code ForStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterForStmt(LanguageParser.ForStmtContext ctx);
	/**
	 * Exit a parse tree produced by the {@code ForStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitForStmt(LanguageParser.ForStmtContext ctx);
	/**
	 * Enter a parse tree produced by the {@code ForDeclStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterForDeclStmt(LanguageParser.ForDeclStmtContext ctx);
	/**
	 * Exit a parse tree produced by the {@code ForDeclStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitForDeclStmt(LanguageParser.ForDeclStmtContext ctx);
	/**
	 * Enter a parse tree produced by the {@code BreakStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterBreakStmt(LanguageParser.BreakStmtContext ctx);
	/**
	 * Exit a parse tree produced by the {@code BreakStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitBreakStmt(LanguageParser.BreakStmtContext ctx);
	/**
	 * Enter a parse tree produced by the {@code ContinueStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterContinueStmt(LanguageParser.ContinueStmtContext ctx);
	/**
	 * Exit a parse tree produced by the {@code ContinueStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitContinueStmt(LanguageParser.ContinueStmtContext ctx);
	/**
	 * Enter a parse tree produced by the {@code ReturnStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterReturnStmt(LanguageParser.ReturnStmtContext ctx);
	/**
	 * Exit a parse tree produced by the {@code ReturnStmt}
	 * labeled alternative in {@link LanguageParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitReturnStmt(LanguageParser.ReturnStmtContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#forInit}.
	 * @param ctx the parse tree
	 */
	void enterForInit(LanguageParser.ForInitContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#forInit}.
	 * @param ctx the parse tree
	 */
	void exitForInit(LanguageParser.ForInitContext ctx);
	/**
	 * Enter a parse tree produced by the {@code CaseClause}
	 * labeled alternative in {@link LanguageParser#caseClauseStmt}.
	 * @param ctx the parse tree
	 */
	void enterCaseClause(LanguageParser.CaseClauseContext ctx);
	/**
	 * Exit a parse tree produced by the {@code CaseClause}
	 * labeled alternative in {@link LanguageParser#caseClauseStmt}.
	 * @param ctx the parse tree
	 */
	void exitCaseClause(LanguageParser.CaseClauseContext ctx);
	/**
	 * Enter a parse tree produced by the {@code DefaultClause}
	 * labeled alternative in {@link LanguageParser#caseClauseStmt}.
	 * @param ctx the parse tree
	 */
	void enterDefaultClause(LanguageParser.DefaultClauseContext ctx);
	/**
	 * Exit a parse tree produced by the {@code DefaultClause}
	 * labeled alternative in {@link LanguageParser#caseClauseStmt}.
	 * @param ctx the parse tree
	 */
	void exitDefaultClause(LanguageParser.DefaultClauseContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#exprList}.
	 * @param ctx the parse tree
	 */
	void enterExprList(LanguageParser.ExprListContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#exprList}.
	 * @param ctx the parse tree
	 */
	void exitExprList(LanguageParser.ExprListContext ctx);
	/**
	 * Enter a parse tree produced by the {@code parseFloatMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterParseFloatMethod(LanguageParser.ParseFloatMethodContext ctx);
	/**
	 * Exit a parse tree produced by the {@code parseFloatMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitParseFloatMethod(LanguageParser.ParseFloatMethodContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Callee}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterCallee(LanguageParser.CalleeContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Callee}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitCallee(LanguageParser.CalleeContext ctx);
	/**
	 * Enter a parse tree produced by the {@code appendMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterAppendMethod(LanguageParser.AppendMethodContext ctx);
	/**
	 * Exit a parse tree produced by the {@code appendMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitAppendMethod(LanguageParser.AppendMethodContext ctx);
	/**
	 * Enter a parse tree produced by the {@code New}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterNew(LanguageParser.NewContext ctx);
	/**
	 * Exit a parse tree produced by the {@code New}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitNew(LanguageParser.NewContext ctx);
	/**
	 * Enter a parse tree produced by the {@code MulDiv}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterMulDiv(LanguageParser.MulDivContext ctx);
	/**
	 * Exit a parse tree produced by the {@code MulDiv}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitMulDiv(LanguageParser.MulDivContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Parens}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterParens(LanguageParser.ParensContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Parens}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitParens(LanguageParser.ParensContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Logical}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterLogical(LanguageParser.LogicalContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Logical}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitLogical(LanguageParser.LogicalContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Index}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterIndex(LanguageParser.IndexContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Index}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitIndex(LanguageParser.IndexContext ctx);
	/**
	 * Enter a parse tree produced by the {@code String}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterString(LanguageParser.StringContext ctx);
	/**
	 * Exit a parse tree produced by the {@code String}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitString(LanguageParser.StringContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Identifier}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterIdentifier(LanguageParser.IdentifierContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Identifier}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitIdentifier(LanguageParser.IdentifierContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Number}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterNumber(LanguageParser.NumberContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Number}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitNumber(LanguageParser.NumberContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Bool}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterBool(LanguageParser.BoolContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Bool}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitBool(LanguageParser.BoolContext ctx);
	/**
	 * Enter a parse tree produced by the {@code indexMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterIndexMethod(LanguageParser.IndexMethodContext ctx);
	/**
	 * Exit a parse tree produced by the {@code indexMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitIndexMethod(LanguageParser.IndexMethodContext ctx);
	/**
	 * Enter a parse tree produced by the {@code lenMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterLenMethod(LanguageParser.LenMethodContext ctx);
	/**
	 * Exit a parse tree produced by the {@code lenMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitLenMethod(LanguageParser.LenMethodContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Equality}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterEquality(LanguageParser.EqualityContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Equality}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitEquality(LanguageParser.EqualityContext ctx);
	/**
	 * Enter a parse tree produced by the {@code SubAssign}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterSubAssign(LanguageParser.SubAssignContext ctx);
	/**
	 * Exit a parse tree produced by the {@code SubAssign}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitSubAssign(LanguageParser.SubAssignContext ctx);
	/**
	 * Enter a parse tree produced by the {@code atoiMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterAtoiMethod(LanguageParser.AtoiMethodContext ctx);
	/**
	 * Exit a parse tree produced by the {@code atoiMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitAtoiMethod(LanguageParser.AtoiMethodContext ctx);
	/**
	 * Enter a parse tree produced by the {@code MatrixIndex}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterMatrixIndex(LanguageParser.MatrixIndexContext ctx);
	/**
	 * Exit a parse tree produced by the {@code MatrixIndex}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitMatrixIndex(LanguageParser.MatrixIndexContext ctx);
	/**
	 * Enter a parse tree produced by the {@code joinMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterJoinMethod(LanguageParser.JoinMethodContext ctx);
	/**
	 * Exit a parse tree produced by the {@code joinMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitJoinMethod(LanguageParser.JoinMethodContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Dec}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterDec(LanguageParser.DecContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Dec}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitDec(LanguageParser.DecContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Mod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterMod(LanguageParser.ModContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Mod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitMod(LanguageParser.ModContext ctx);
	/**
	 * Enter a parse tree produced by the {@code AddSub}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterAddSub(LanguageParser.AddSubContext ctx);
	/**
	 * Exit a parse tree produced by the {@code AddSub}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitAddSub(LanguageParser.AddSubContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Relational}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterRelational(LanguageParser.RelationalContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Relational}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitRelational(LanguageParser.RelationalContext ctx);
	/**
	 * Enter a parse tree produced by the {@code typeOfMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterTypeOfMethod(LanguageParser.TypeOfMethodContext ctx);
	/**
	 * Exit a parse tree produced by the {@code typeOfMethod}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitTypeOfMethod(LanguageParser.TypeOfMethodContext ctx);
	/**
	 * Enter a parse tree produced by the {@code AddAssign}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterAddAssign(LanguageParser.AddAssignContext ctx);
	/**
	 * Exit a parse tree produced by the {@code AddAssign}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitAddAssign(LanguageParser.AddAssignContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Nil}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterNil(LanguageParser.NilContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Nil}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitNil(LanguageParser.NilContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Float}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterFloat(LanguageParser.FloatContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Float}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitFloat(LanguageParser.FloatContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Not}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterNot(LanguageParser.NotContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Not}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitNot(LanguageParser.NotContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Slice}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterSlice(LanguageParser.SliceContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Slice}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitSlice(LanguageParser.SliceContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Assign}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterAssign(LanguageParser.AssignContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Assign}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitAssign(LanguageParser.AssignContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Negate}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterNegate(LanguageParser.NegateContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Negate}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitNegate(LanguageParser.NegateContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Rune}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterRune(LanguageParser.RuneContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Rune}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitRune(LanguageParser.RuneContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Inc}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterInc(LanguageParser.IncContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Inc}
	 * labeled alternative in {@link LanguageParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitInc(LanguageParser.IncContext ctx);
	/**
	 * Enter a parse tree produced by the {@code FuncCall}
	 * labeled alternative in {@link LanguageParser#call}.
	 * @param ctx the parse tree
	 */
	void enterFuncCall(LanguageParser.FuncCallContext ctx);
	/**
	 * Exit a parse tree produced by the {@code FuncCall}
	 * labeled alternative in {@link LanguageParser#call}.
	 * @param ctx the parse tree
	 */
	void exitFuncCall(LanguageParser.FuncCallContext ctx);
	/**
	 * Enter a parse tree produced by the {@code Get}
	 * labeled alternative in {@link LanguageParser#call}.
	 * @param ctx the parse tree
	 */
	void enterGet(LanguageParser.GetContext ctx);
	/**
	 * Exit a parse tree produced by the {@code Get}
	 * labeled alternative in {@link LanguageParser#call}.
	 * @param ctx the parse tree
	 */
	void exitGet(LanguageParser.GetContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#args}.
	 * @param ctx the parse tree
	 */
	void enterArgs(LanguageParser.ArgsContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#args}.
	 * @param ctx the parse tree
	 */
	void exitArgs(LanguageParser.ArgsContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#fields}.
	 * @param ctx the parse tree
	 */
	void enterFields(LanguageParser.FieldsContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#fields}.
	 * @param ctx the parse tree
	 */
	void exitFields(LanguageParser.FieldsContext ctx);
	/**
	 * Enter a parse tree produced by {@link LanguageParser#fieldInit}.
	 * @param ctx the parse tree
	 */
	void enterFieldInit(LanguageParser.FieldInitContext ctx);
	/**
	 * Exit a parse tree produced by {@link LanguageParser#fieldInit}.
	 * @param ctx the parse tree
	 */
	void exitFieldInit(LanguageParser.FieldInitContext ctx);
}