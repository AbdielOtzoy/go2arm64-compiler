using analyzer;

public class Struct : Invocable {
    public string Name { get; set; }
    public Dictionary<string, LanguageParser.VarDeclarationContext> Fields { get; set; }
    public Dictionary<string, ForeignFunction> Methods { get; set; }
    public List<string> referenceNames { get; set; } = new List<string>();

    public Struct(string name, Dictionary<string, LanguageParser.VarDeclarationContext> fields, Dictionary<string, ForeignFunction> methods) {
        Name = name;
        Fields = fields;
        Methods = methods;
    }

    // add method to struct with name func and reference name
    public void AddMethod(string funcName, ForeignFunction method, string referenceName){
        Methods.Add(funcName, method);
        referenceNames.Add(referenceName);
    }

    public ForeignFunction? GetMethod(string name){
        if(Methods.ContainsKey(name)){
            return Methods[name];
        }
        return null;
    }

    public string GetReferenceNameByMethod(string name){
        if(Methods.ContainsKey(name)){
            return referenceNames[Methods.Keys.ToList().IndexOf(name)];
        }
        return "";
    }

    public int Arity() {
        var constructor = GetMethod("constructor");
        if(constructor != null){
            return constructor.Arity();
        }
        return 0;
    }

    public ValueWrapper Invoke(List<ValueWrapper> args, InterpreterVisitor visitor) {
        var newInstance = new Instance(this);

        // Inicializar campos con valores predeterminados
        foreach(var field in Fields){
            var name = field.Key;
            var value = field.Value;

            if(value.expr() != null){
                var varValue = visitor.Visit(value.expr());
                newInstance.Set(name, varValue);
            } else {
                newInstance.Set(name, visitor.defaultValue);
            }
        }

        // assign args to fields in order
        foreach(var arg in args){
            var name = Fields.Keys.ElementAt(args.IndexOf(arg));
            newInstance.Set(name, arg);
        }

        //print
        foreach(var field in newInstance._fields){
            Console.WriteLine($"{field.Key} = {field.Value}");
        }

        return new InstanceValue(newInstance);
    }
}