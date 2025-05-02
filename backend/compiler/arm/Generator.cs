using System.Text;

public class StackObject {
    public enum StackObjectType { Int, Float, String, Bool, Undefined, Rune, Array }
    public StackObjectType Type { get; set; }
    public int Length { get; set; }
    public int Depth { get; set; }
    public string? Id { get; set; }
    public int Offset { get; set; }
}

public class ArmGenerator {
    public List<string> _instructions = new List<string>();
    public List<string> funcInstructions = new List<string>();
    public string dataSection = "";
    private readonly StandardLibrary _standardLibrary = new StandardLibrary();
    private List<StackObject> _stack = new List<StackObject>();
    private int _depth = 0;
    private int labelCounter = 0;

    public string GetLabel() {
        return $"L{labelCounter++}";
    }

    public void SetLabel(string label) {
        _instructions.Add($"{label}:");
    }

    public StackObject TopObject() {
        if (_stack.Count == 0) {
            throw new Exception("Stack is empty");
        }
        return _stack.Last();
    }

    // stack operations
    public void PushObject(StackObject obj) {
        _stack.Add(obj);
    }

    public void PopObject() {
        Comment("Popping object");
        _stack.RemoveAt(_stack.Count - 1);
    }

    public StackObject GetFrameLocal(int index) {
        var obj = _stack.Where(o => o.Type == StackObject.StackObjectType.Undefined).ToList()[index];
        return obj;
    }

    public void PushConstant(StackObject obj, object value) {
        switch (obj.Type) {
            case StackObject.StackObjectType.Int:
                Mov(Register.X0, int.Parse(value.ToString()));
                Push(Register.X0);
                break;
            case StackObject.StackObjectType.Float:
                long floatBits = BitConverter.DoubleToInt64Bits(double.Parse(value.ToString()));

                short[] floatParts = new short[4];
                for (int i = 0; i < 4; i++) {
                    floatParts[i] = (short)((floatBits >> (i * 16)) & 0xFFFF);
                }

                _instructions.Add($"MOVZ X0, #{floatParts[0]}, LSL #0");
                for (int i = 1; i < 4; i++) {
                    _instructions.Add($"MOVK X0, #{floatParts[i]}, LSL #{i * 16}");
                }
                Push(Register.X0);
    
                break;
            case StackObject.StackObjectType.String:
                List<byte> stringBytes = Utils.StringTo1ByteArray((string)value);
                Push(Register.HP);
                for (int i = 0; i < stringBytes.Count; i++) {
                    var charCode = stringBytes[i];

                    Comment("Pushing char: " + charCode + " (" + (char)charCode + ")");
                    Mov("W0", charCode);
                    Strb("W0", Register.HP);
                    Mov(Register.X0, 1);
                    Add(Register.HP, Register.HP, Register.X0);
                }
                break;
            case StackObject.StackObjectType.Bool:
                Mov(Register.X0, value.ToString() == "true" ? 1 : 0);
                Push(Register.X0);
                break;
            case StackObject.StackObjectType.Rune:
                Console.WriteLine("Pushing rune: " + value);
                var runeValue =  Utils.StringToByte((string)value);
                Console.WriteLine("Pushing rune value: " + runeValue);
                
                Mov(Register.X0, runeValue);
                Push(Register.X0);
                break;
        }

        PushObject(obj);
    }

    public StackObject IntObject() {
        return new StackObject {
            Type = StackObject.StackObjectType.Int,
            Length = 8,
            Depth = _depth,
            Id = null
        };
    }

    public StackObject FloatObject() {
        return new StackObject {
            Type = StackObject.StackObjectType.Float,
            Length = 8,
            Depth = _depth,
            Id = null
        };
    }

    public StackObject BoolObject() {
        return new StackObject {
            Type = StackObject.StackObjectType.Bool,
            Length = 8,
            Depth = _depth,
            Id = null
        };
    }

    public StackObject StringObject() {
        return new StackObject {
            Type = StackObject.StackObjectType.String,
            Length = 8,
            Depth = _depth,
            Id = null
        };
    }

    public StackObject RuneObject() {
        return new StackObject {
            Type = StackObject.StackObjectType.Rune,
            Length = 8,
            Depth = _depth,
            Id = null
        };
    }

    public StackObject ArrayObject() {
        return new StackObject {
            Type = StackObject.StackObjectType.Array,
            Length = 8,
            Depth = _depth,
            Id = null
        };
    } 

    public StackObject CloneObject(StackObject obj) {
        return new StackObject {
            Type = obj.Type,
            Length = obj.Length,
            Depth = obj.Depth,
            Id = obj.Id
        };
    }

    // Environment operations
    public void NewScope() {
        _depth++;
    }

    public int EndScope() {
        int byteOffset = 0;

        for(int i = _stack.Count - 1; i >= 0; i--) {
            if (_stack[i].Depth == _depth) {
                byteOffset += _stack[i].Length;
                _stack.RemoveAt(i);
            } else {
                break;
            }
        }
        _depth--;
        return byteOffset;
    }

    public void TagObject(string id) {
        _stack.Last().Id = id;
    }

    public (int, StackObject) GetObject(string id) {
        int byteOffset = 0;
        for (int i = _stack.Count - 1; i >= 0; i--) {
            if (_stack[i].Id == id) {
                return (byteOffset, _stack[i]);
            }
            byteOffset += _stack[i].Length;
        }
        throw new Exception("Object with id " + id + " not found");
    }

    public StackObject PopObject(string rd) {
        var obj = _stack.Last();
        _stack.RemoveAt(_stack.Count - 1);

        Pop(rd);
        return obj;
    }

    public void Add(string rd, string rs1, string rs2) {
        _instructions.Add($"ADD {rd}, {rs1}, {rs2}");
    }

    public void Sub(string rd, string rs1, string rs2) {
        _instructions.Add($"SUB {rd}, {rs1}, {rs2}");
    }

    public void Mul(string rd, string rs1, string rs2) {
        _instructions.Add($"MUL {rd}, {rs1}, {rs2}");
    }

    public void Div(string rd, string rs1, string rs2) {
        _instructions.Add($"SDIV {rd}, {rs1}, {rs2}");
    }

    public void Addi(string rd, string rs1, int imm) {
        _instructions.Add($"ADDI {rd}, {rs1}, #{imm}");
    }

    public void Str(string rs1, string rs2, int offset = 0) {
        _instructions.Add($"STR {rs1}, [{rs2}, #{offset}]");
    }

    public void Strb(string rs1, string rs2) {
        _instructions.Add($"STRB {rs1}, [{rs2}]");
    }

    public void Ldr(string rd, string rs1, int offset = 0) {
        _instructions.Add($"LDR {rd}, [{rs1}, #{offset}]");
    }

    public void Mov(string rd, int imm) {
        _instructions.Add($"MOV {rd}, #{imm}");
    }
    public void Fmov(string rd, float imm) {
        _instructions.Add($"FMOV {rd}, #{imm}");
    }

    public void Fadd(string rd, string rs1, string rs2) {
        _instructions.Add($"FADD {rd}, {rs1}, {rs2}");
    }

    public void Fsub(string rd, string rs1, string rs2) {
        _instructions.Add($"FSUB {rd}, {rs1}, {rs2}");
    }
    public void Fmul(string rd, string rs1, string rs2) {
        _instructions.Add($"FMUL {rd}, {rs1}, {rs2}");
    }
    public void Fdiv(string rd, string rs1, string rs2) {
        _instructions.Add($"FDIV {rd}, {rs1}, {rs2}");
    }

    public void Scvtf(string rd, string rs) {
        _instructions.Add($"SCVTF {rd}, {rs}");
    }

    public void Mod(string rd, string rs1, string rs2) {
        _instructions.Add($"UDIV X3, {rs1}, {rs2}");
        _instructions.Add($"MUL X4, X3, {rs2}");
        _instructions.Add($"SUB {rd}, {rs1}, X4");
    }

    public void Push(string rs) {
        _instructions.Add($"STR {rs}, [SP, #-8]!");
    }

    public void Pop(string rd) {
        _instructions.Add($"LDR {rd}, [SP], #8");
    }

    public void Svc(){
        _instructions.Add($"SVC #0");
    }

    public void EndProgram(){
        Mov(Register.X0, 0);
        Mov(Register.X8, 93);
        Svc();
    }

    public void PrintInteger(string rs) {
        _standardLibrary.Use("print_integer");
        _instructions.Add($"MOV X0, {rs}");
        _instructions.Add($"BL print_integer");
    }

    public void PrintString(string rs) {
        _standardLibrary.Use("print_string");
        _instructions.Add($"MOV X0, {rs}");
        _instructions.Add($"BL print_string");
    }

    public void PrintRune(string rs) {
        _standardLibrary.Use("print_rune");
        _instructions.Add($"MOV X0, {rs}");
        _instructions.Add($"BL print_rune");
    }

    public void PrintBool(string rs) {
        _standardLibrary.Use("print_bool");
        _instructions.Add($"MOV X0, {rs}");
        _instructions.Add($"BL print_bool");
    }

    public void PrintFloat(string rs) {
        _standardLibrary.Use("print_double");
        _instructions.Add($"BL print_double");
    }

    public void PrintArray(string name) {
        _standardLibrary.Use("print_array");
        _standardLibrary.Use("print_integer");
        _instructions.Add($"ADR X0, {name}");
        _instructions.Add($"ADR X1, {name}_size");
        _instructions.Add($"LDR W1, [X1]");
        _instructions.Add($"BL print_array");
    }

    public void PrintNewLine() {
        _standardLibrary.Use("print_newline");
        _instructions.Add($"BL print_newline");
    }

    public void Cmp(string rs1, string rs2) {
        _instructions.Add($"CMP {rs1}, {rs2}");
    }

    public void B(string label) {
        _instructions.Add($"B {label}");
    }

    public void Br(string label) {
        _instructions.Add($"BR {label}");
    }

    public void Bl(string label) {
        _instructions.Add($"BL {label}");
    }

    public void Equal(string label) {
        _instructions.Add($"BEQ {label}");
    }

    public void NotEqual(string label) {
        _instructions.Add($"BNE {label}");
    }

    public void LessThan(string label) {
        _instructions.Add($"BLT {label}");
    }
    public void LessThanOrEqual(string label) {
        _instructions.Add($"BLE {label}");
    }

    public void GreaterThan(string label) {
        _instructions.Add($"BGT {label}");
    }

    public void GreaterThanOrEqual(string label) {
        _instructions.Add($"BGE {label}");
    }
    
    public void And(string rd, string rs1, string rs2) {
        _instructions.Add($"AND {rd}, {rs1}, {rs2}");
    }
    public void Or(string rd, string rs1, string rs2) {
        _instructions.Add($"ORR {rd}, {rs1}, {rs2}");
    }
    public void Not(string rd, string rs) {
        _instructions.Add($"EOR {rd}, {rs}, #1");
    }

    public void Fcmp(string rs1, string rs2) {
        _instructions.Add($"FCMP {rs1}, {rs2}");
    }

    public void FEqual(string label) {
        _instructions.Add($"BEQ {label}");
    }
    public void FNotEqual(string label) {
        _instructions.Add($"BNE {label}");
    }

    public void FLessThan(string label) {
        _instructions.Add($"BLO {label}");
    }
    public void FLessThanOrEqual(string label) {
        _instructions.Add($"BLE {label}");
    }
    public void FGreaterThan(string label) {
        _instructions.Add($"BHI {label}");
    }
    public void FGreaterThanOrEqual(string label) {
        _instructions.Add($"BHS {label}");
    }

    public void switchCase(string rs, int val, string label){
        _instructions.Add($"CMP {rs}, #{val}");
        _instructions.Add($"BEQ {label}");
    }

    public void p2align(int val) {
        _instructions.Add($".p2align {val}");
    }

    public void Cbz(string rs, string label) {
        _instructions.Add($"CBZ {rs}, {label}");
    }

    public void Adr(string rd, string label) {
        _instructions.Add($"ADR {rd}, {label}");
    }

    public void Comment(string comment) {
        _instructions.Add($"// {comment}");
    }

    public override string ToString() {
        var sb = new StringBuilder();
        sb.AppendLine(".data");
        sb.AppendLine("heap: .space 4096");
        sb.AppendLine(dataSection);
        sb.AppendLine(".text");
        sb.AppendLine(".global _start");
        sb.AppendLine("_start:");
        sb.AppendLine("    adr x10, heap");

        EndProgram();

        foreach (var instruction in _instructions) {
            sb.AppendLine(instruction);
        }

        sb.AppendLine("\n\n\n // Function Definitions");
        funcInstructions.ForEach(i => sb.AppendLine(i));

        sb.AppendLine("\n\n\n // Standard Library");
        sb.AppendLine(_standardLibrary.GetFunctionDefinitions());

        return sb.ToString();
    }
}   