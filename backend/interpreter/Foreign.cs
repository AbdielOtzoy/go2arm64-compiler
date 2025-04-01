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
        if(context.@params() == null) {
            return 0;
        }
        return context.@params().ID().Length;
    }
    
    public ValueWrapper Invoke(List<ValueWrapper> args, InterpreterVisitor visitor)
    {
        var newEnv = new Enviorment(clousure);
        var beforeCallEnv = visitor.env;
        visitor.env = newEnv;

        if(context.@params() != null) {
            for (int i = 0; i < context.@params().ID().Length; i++)
            {
                newEnv.Declare(context.@params().ID(i).GetText(), args[i], null);
            }
        }

        try
        {
            foreach(var statement in context.declaration()) {
            visitor.Visit(statement);
        }
        }
        catch (ReturnException e)
        {
            visitor.env = beforeCallEnv;
            return e.Value;
        }

        visitor.env = beforeCallEnv;
        return visitor.defaultValue;
    }

    public ForeignFunction Bind(Instance instance, string referenceName){
        var hiddenEnv = new Enviorment(clousure);
        // TODO: implement the reference name struct instead of "this"
        hiddenEnv.Declare(referenceName, new InstanceValue(instance), null);
        return new ForeignFunction(hiddenEnv, context);
    }

}