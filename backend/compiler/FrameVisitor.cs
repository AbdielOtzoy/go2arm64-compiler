using analyzer;

public class FrameElement {
    public string Name { get; set; }
    public int Offset { get; set; }

    public FrameElement(string name, int offset) {
        Name = name;
        Offset = offset;
    }
   
}

public class FrameVisitor : LanguageBaseVisitor<Object?> {

    public List<FrameElement> Frame;
    public int LocalOffset;
    public int BaseOffset;

    public FrameVisitor(int baseOffset) {
        Frame = new List<FrameElement>();
        LocalOffset = 0;
        BaseOffset = baseOffset;
        
    }
    public override Object? VisitVarDeclaration(LanguageParser.VarDeclarationContext context){
        string name = context.ID(0).GetText();
        
        Frame.Add(new FrameElement(name, BaseOffset + LocalOffset));
        LocalOffset += 1;

        return null;
    }

    public override Object? VisitBlockStmt(LanguageParser.BlockStmtContext context){
        foreach( var declaration in context.declaration()){
            Visit(declaration);
        }
        return null;
    }

    public override Object? VisitIfStmt(LanguageParser.IfStmtContext context) {
        Visit(context.statement(0)); 
        if (context.statement().Length > 1) Visit(context.statement(1));

        return null;
    }

    public override Object? VisitForStmt(LanguageParser.ForStmtContext context){
        Visit(context.statement());
        return null;
    }

    public override Object? VisitForDeclStmt(LanguageParser.ForDeclStmtContext context){
        if(context.forInit().varDeclaration() != null){
            Visit(context.forInit().varDeclaration());
        }
        Visit(context.statement());

        return null;
    }


}