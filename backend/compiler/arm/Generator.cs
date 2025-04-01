using System.Text;

public class ArmGenerator {
    private readonly List<string> _instructions = new List<string>();
    private readonly StandarLibrary _standardLibrary = new StandarLibrary();

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

    public void Ldr(string rd, string rs1, int offset = 0) {
        _instructions.Add($"LDR {rd}, [{rs1}, #{offset}]");
    }

    public void Mov(string rd, int imm) {
        _instructions.Add($"MOV {rd}, #{imm}");
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

    public void Comment(string comment) {
        _instructions.Add($"// {comment}");
    }

    public override string ToString() {
        var sb = new StringBuilder();
        sb.AppendLine(".text");
        sb.AppendLine(".global _start");
        sb.AppendLine("_start:");

        EndProgram();

        foreach (var instruction in _instructions) {
            sb.AppendLine(instruction);
        }

        sb.AppendLine("\n\n\n // Standard Library");
        sb.AppendLine(_standardLibrary.GetFunctinoDefinitions());

        return sb.ToString();
    }
}   