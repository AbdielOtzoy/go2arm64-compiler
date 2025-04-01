public class Instance {
    private Struct _struct;
    public Dictionary<string, ValueWrapper> _fields;

    public Instance(Struct _struct) {
        this._struct = _struct;
        _fields = new Dictionary<string, ValueWrapper>();
    }

    public void Set(string name, ValueWrapper value){
        if(_struct.Fields.ContainsKey(name)){
            _fields[name] = value;
        }else{
            throw new SemanticError($"Field {name} not found in struct {_struct.Name}", null);
        }
    }

    public ValueWrapper Get(string name){
        if(_fields.ContainsKey(name)){
            Console.WriteLine($"Getting field {name} with value {_fields[name]}");  
            return _fields[name];
        }
        

        var method = _struct.GetMethod(name);
        string referenceName = _struct.GetReferenceNameByMethod(name);
        Console.WriteLine($"Getting method {name} with value {method} and reference name {referenceName}");
        if(method != null){
            Console.WriteLine($"Getting method {name} with value {method}");
            return new FunctionValue(method.Bind(this, referenceName), name);
        }

        throw new SemanticError($"Field {name} not found in struct {_struct.Name}", null);
    }
}