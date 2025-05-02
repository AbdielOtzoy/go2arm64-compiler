using analyzer;

public class ForeignFunction : Invocable 
{
    private Enviorment clousure;
    private LanguageParser.FuncDeclarationContext context;

    public ForeignFunction(Enviorment clousure, LanguageParser.FuncDeclarationContext context)
    {
        this.clousure = clousure;
        this.context = context;
    }

    

    public int Arity(){
        return 1;
    }
    
    public ValueWrapper Invoke(List<ValueWrapper> args, InterpreterVisitor visitor)
    {

        return visitor.defaultValue;
    }

    public ForeignFunction Bind(Instance instance, string referenceName){
        var hiddenEnv = new Enviorment(clousure);
        // TODO: implement the reference name struct instead of "this"
        hiddenEnv.Declare(referenceName, new InstanceValue(instance), null);
        return new ForeignFunction(hiddenEnv, context);
    }

}