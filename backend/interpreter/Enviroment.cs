public class Enviorment
{
    private Dictionary<string, ValueWrapper> variables = new Dictionary<string, ValueWrapper>();

    private Enviorment? parent;

    public Enviorment(Enviorment? parent = null)
    {
        this.parent = parent;
    }

    public ValueWrapper Get(string name, Antlr4.Runtime.IToken token)
    
    {
        if (variables.ContainsKey(name))
        {
            return variables[name];
        }
        else if (parent != null)
        {
            return parent.Get(name, token);
        }
        else
        {
            throw new SemanticError($"Variable {name} not found", token);
        }
    }
    public bool Exists(string name)
    {
        if (variables.ContainsKey(name))
        {
            return true;
        }
        else if (parent != null)
        {
            return parent.Exists(name);
        }
        return false;
    }

    public void Declare(string name, ValueWrapper value,  Antlr4.Runtime.IToken token)
    {
        if (variables.ContainsKey(name))
        {
            throw new SemanticError($"Variable {name} already declared", token);
        }
        Console.WriteLine("Declarando variable: " + name + " con valor: " + value);
        variables[name] = value;
    }

    public ValueWrapper Assign(string name, ValueWrapper value, Antlr4.Runtime.IToken token)
    {
        if (variables.ContainsKey(name))
        {
            string valueType = variables[name].GetType().ToString();
            string valueToAssignType = value.GetType().ToString();
            if (valueType != valueToAssignType)
            {
                throw new SemanticError($"Cannot assign {valueToAssignType} to {valueType}", token);
            }
            variables[name] = value;
            return value;  
        }
        else if (parent != null)
        {
            return parent.Assign(name, value, token);
        }
        else
        {
            throw new SemanticError($"Variable {name} not found", token);
        }
    }  

}