using analyzer;

public class SearchVisitor : LanguageBaseVisitor<LanguageParser.FuncDeclarationContext>
{
    public override LanguageParser.FuncDeclarationContext VisitProgram(LanguageParser.ProgramContext context)
    {
        foreach (var declaration in context.declaration())
        {
            var funcDecl = declaration.funcDeclaration(); 
            if (funcDecl != null && funcDecl.ID(0).GetText() == "main")
            {
                Console.WriteLine("Found main function!");
                return funcDecl;
            }
        }
        return null; // No se encontró la función main
    }
}
