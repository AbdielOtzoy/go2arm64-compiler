using analyzer;

public class InterpreterVisitor : LanguageBaseVisitor<ValueWrapper>
{

    public ValueWrapper defaultValue = new VoidBody();
    public string output = "";
    public Enviorment env;
    public List<Symbol> symbols = new List<Symbol>(); // table of symbols

    public InterpreterVisitor()
    {
        env = new Enviorment(null);
    }

    public string getJSONSymbols()
    {
        string json = "[";
        foreach (var symbol in symbols)
        {
            json += "{";
            json += $"\"id\": \"{symbol.id}\",";
            json += $"\"type\": \"{symbol.type}\",";
            json += $"\"kind\": \"{symbol.kind}\",";
            json += $"\"line\": \"{symbol.token.Line}\",";
            json += $"\"column\": \"{symbol.token.Column}\"";
            json += "},";
        }
        json = json.Remove(json.Length - 1);
        json += "]";

        return json;
    }

    // VisitProgram
    public override ValueWrapper VisitProgram(LanguageParser.ProgramContext context)
    {
        foreach (var declaration in context.declaration())
        {
            Visit(declaration);
        }
        return defaultValue;
    }

    public void ExecuteMain(LanguageParser.ProgramContext programContext)
    {
        // Buscar la función main
        var searchVisitor = new SearchVisitor();
        var mainFunctionContext = searchVisitor.Visit(programContext);

        if (mainFunctionContext != null)
        {
            // Declarar la función main en el entorno
            string mainName = mainFunctionContext.ID(0).GetText();
            var mainFunction = new ForeignFunction(env, mainFunctionContext);
            env.Declare(mainName, new FunctionValue(mainFunction, mainName), mainFunctionContext.Start);

            // Ejecutar la función main
            var mainFunctionValue = env.Get(mainName, mainFunctionContext.Start);
            if (mainFunctionValue is FunctionValue functionValue)
            {
                functionValue.invocable.Invoke(new List<ValueWrapper>(), this); // Invocar la función main sin argumentos
            }
            else
            {
                throw new Exception("La función main no es invocable");
            }
        }
        else
        {
            throw new Exception("No se encontró la función main");
        }
    }



    // VisitVarDeclaration
    public override ValueWrapper VisitVarDeclaration(LanguageParser.VarDeclarationContext context)
    {
        string name = context.ID(0).GetText();
        // si no tiene valor asignado, se le asigna un valor por defecto segun el tipo
        if (context.expr() == null)
        {
            string type = context.TYPE().GetText();
            ValueWrapper inferValue = type switch
            {
                "int" => new IntValueWrapper(0),
                "float64" => new FloatValueWrapper(0),
                "string" => new StringValueWrapper(""),
                "bool" => new BoolValueWrapper(false),
                "rune" => new RuneValueWrapper(' '),
                "nil" => new NilValueWrapper(),
                _ => throw new SemanticError("Invalid type", context.Start)
            };
            env.Declare(name, inferValue, context.Start);
            symbols.Add(new Symbol(name, context.TYPE().GetText(), "variable", context.Start));
            return defaultValue;
        }
        ValueWrapper value = Visit(context.expr());
        env.Declare(name, value, context.Start);
        string type_var = value switch
        {
            IntValueWrapper => "int",
            FloatValueWrapper => "float64",
            StringValueWrapper => "string",
            BoolValueWrapper => "bool",
            RuneValueWrapper => "rune",
            NilValueWrapper => "nil",
            ArrayValueWrapper => "[]int",
            MatrixValueWrapper => "[][]int",
            StructValue => "struct",
            _ => "unknown"
        };
        symbols.Add(new Symbol(name, type_var, "variable", context.Start));
        return defaultValue;
    }

    // VisitFuncDeclaration
    public override ValueWrapper VisitFuncDeclaration(LanguageParser.FuncDeclarationContext context)
    {
        return defaultValue;
    }

    // VisitSlicesDeclaration
    public override ValueWrapper VisitSlicesDeclaration(LanguageParser.SlicesDeclarationContext context)
    {
        string name = context.ID().GetText();

        // Verificar si la variable ya existe
        if (env.Exists(name))
        {
            throw new SemanticError($"Variable {name} already declared", context.Start);
        }

        // Inicializar el array
        ValueWrapper[] values = new ValueWrapper[context.exprList()?.expr().Length ?? 0];

        // Llenar el array con los valores
        if (context.exprList() != null)
        {
            for (int i = 0; i < context.exprList().expr().Length; i++)
            {
                values[i] = Visit(context.exprList().expr(i));
                Console.WriteLine("Valor: " + values[i]);
            }
        }

        // Declarar la variable en el entorno
        env.Declare(name, new ArrayValueWrapper(values), context.Start);
        symbols.Add(new Symbol(name, "[]int", "slice", context.Start));

        return defaultValue;
    }

    // VisitSlice
    public override ValueWrapper VisitSlice(LanguageParser.SliceContext context)
    {
        // '[]' TYPE '{' exprList? '}'                   # Slice
        // exprList: expr (',' expr)*;

        // Crear un array con los valores
        ValueWrapper[] values = new ValueWrapper[context.exprList()?.expr().Length ?? 0];

        // Llenar el array con los valores
        if (context.exprList() != null)
        {
            for (int i = 0; i < context.exprList().expr().Length; i++)
            {
                values[i] = Visit(context.exprList().expr(i));
            }
        }

        return new ArrayValueWrapper(values);
    }

    public override ValueWrapper VisitMatrixDeclaration(LanguageParser.MatrixDeclarationContext context)
    {
        Console.WriteLine("MatrixDeclaration");

        string name = context.ID().GetText();

        if (env.Exists(name))
        {
            throw new SemanticError($"Variable {name} already declared", context.Start);
        }

        List<ValueWrapper[]> rows = new List<ValueWrapper[]>();

        var matrixRowsContext = context.matrixRows();

        foreach (var child in matrixRowsContext.children)
        {
            if (child is LanguageParser.ExprListContext exprListContext)
            {
                List<ValueWrapper> values = new List<ValueWrapper>();

                foreach (var expr in exprListContext.expr())
                {
                    values.Add(Visit(expr));
                }

                rows.Add(values.ToArray());
            }
        }

        env.Declare(name, new MatrixValueWrapper(rows.ToArray()), context.Start);
        symbols.Add(new Symbol(name, "[][]int", "slice", context.Start));

        return defaultValue;
    }

    // VisitStructDeclaration
    public override ValueWrapper VisitStructDeclaration(LanguageParser.StructDeclarationContext context)
    {
        Dictionary<string, LanguageParser.VarDeclarationContext> fields = new Dictionary<string, LanguageParser.VarDeclarationContext>();
        Dictionary<string, ForeignFunction> methods = new Dictionary<string, ForeignFunction>();

        foreach (var field in context.structBody())
        {
            if (field.varDeclaration() != null)
            {
                var varDeclaration = field.varDeclaration();
                Console.WriteLine("Field: " + varDeclaration.ID(0).GetText());
                fields.Add(varDeclaration.ID(0).GetText(), varDeclaration);
            }

        }

        Struct newStruct = new Struct(context.ID().GetText(), fields, methods);

        Console.WriteLine("Struct: " + newStruct.Name);

        env.Declare(context.ID().GetText(), new StructValue(newStruct), context.Start);
        symbols.Add(new Symbol(context.ID().GetText(), "struct", "variable", context.Start));

        return defaultValue;
    }


    // VisitExprStmt
    public override ValueWrapper VisitExprStmt(LanguageParser.ExprStmtContext context)
    {
        Visit(context.expr());
        return defaultValue;
    }

    private string ReplaceEscapeSequences(string input)
    {
        return input
            .Replace("\\n", "\n")  // Salto de línea
            .Replace("\\t", "\t")  // Tabulación
            .Replace("\\r", "\r")  // Retorno de carro
            .Replace("\\\"", "\"") // Comilla doble
            .Replace("\\'", "'")   // Comilla simple
            .Replace("\\\\", "\\"); // Barra invertida
    }

    public override ValueWrapper VisitPrintStmt(LanguageParser.PrintStmtContext context)
    {
        return defaultValue;
    }


    // VisitBlockStmt
    public override ValueWrapper VisitBlockStmt(LanguageParser.BlockStmtContext context)
    {
        Enviorment prevEnv = env;
        env = new Enviorment(env);

        foreach (var statement in context.declaration())
        {
            Visit(statement);
        }

        env = prevEnv;
        return defaultValue;
    }

    // VisitIfStmt
    public override ValueWrapper VisitIfStmt(LanguageParser.IfStmtContext context)
    {
        ValueWrapper condition = Visit(context.expr());
        if (condition is BoolValueWrapper boolValue)
        {
            if (boolValue.Value)
            {
                Visit(context.statement(0));
            }
            else if (context.statement().Length > 1)
            {
                Visit(context.statement(1));
            }
        }
        else
        {
            throw new SemanticError("Invalid condition", context.Start);
        }
        return defaultValue;
    }

    // VisitSwitchStmt
    public override ValueWrapper VisitSwitchStmt(LanguageParser.SwitchStmtContext context)
    {
        ValueWrapper switchValue = Visit(context.expr());
        bool caseMatched = false;

        foreach (var caseClause in context.caseClauseStmt())
        {
            if (caseClause is LanguageParser.CaseClauseContext caseClauseStmt)
            {
                ValueWrapper caseValue = Visit(caseClauseStmt.expr());
                if (switchValue.Equals(caseValue))
                {
                    caseMatched = true;
                    foreach (var declaration in caseClauseStmt.declaration())
                    {
                        try
                        {
                            Visit(declaration);
                        }
                        catch (BreakException)
                        {
                            break;
                        }
                    }
                    break;
                }
            }
            else if (caseClause is LanguageParser.DefaultClauseContext defaultContext)
            {
                if (!caseMatched)
                {
                    foreach (var declaration in defaultContext.declaration())
                    {
                        try
                        {
                            Visit(declaration);
                        }
                        catch (BreakException)
                        {
                            break;
                        }
                    }
                }
            }
        }

        return defaultValue;
    }

    // VisitCaseClause
    public override ValueWrapper VisitCaseClause(LanguageParser.CaseClauseContext context)
    {
        return defaultValue;
    }

    // VisitDefaultClause
    public override ValueWrapper VisitDefaultClause(LanguageParser.DefaultClauseContext context)
    {
        return defaultValue;
    }

    // VisitForStmt
    public override ValueWrapper VisitForStmt(LanguageParser.ForStmtContext context)
    {
        // | 'for' expr statement # ForStmt;
        ValueWrapper condition = Visit(context.expr());

        if (condition is not BoolValueWrapper)
        {
            throw new SemanticError("Invalid condition", context.Start);
        }

        while ((condition as BoolValueWrapper).Value)
        {
            try
            {
                Visit(context.statement());
                condition = Visit(context.expr());
            }
            catch (BreakException)
            {
                break;
            }
            catch (ContinueException)
            {
                condition = Visit(context.expr());
            }
        }

        return defaultValue;
    }

    // VisitForDeclStmt
    public override ValueWrapper VisitForDeclStmt(LanguageParser.ForDeclStmtContext context)
    {
        // | 'for' varDeclaration expr statement # ForDeclStmt;
        Enviorment prevEnv = env;
        env = new Enviorment(env);

        if (context.forInit() != null)
        {
            Visit(context.forInit());
        }

        VisitForBody(context);

        env = prevEnv;
        return defaultValue;
    }

    // VisitForBody
    public void VisitForBody(LanguageParser.ForDeclStmtContext context)
    {
        while (true)
        {
            if (context.expr(0) != null) // Si hay una condición
            {
                ValueWrapper condition = Visit(context.expr(0));
                if (condition is not BoolValueWrapper boolValue || !boolValue.Value)
                {
                    break; // Salir del bucle si la condición es falsa
                }
            }

            try
            {
                // 4. Ejecutar el cuerpo del for
                Visit(context.statement());
            }
            catch (BreakException)
            {
                break; // Salir del bucle si se encuentra un 'break'
            }
            catch (ContinueException)
            {
                // Continuar con la siguiente iteración
            }

            // 5. Ejecutar el incremento (si existe)
            if (context.expr(1) != null)
            {
                Visit(context.expr(1));
            }
        }
    }

    public override ValueWrapper VisitForRangeStmt(LanguageParser.ForRangeStmtContext context)
    {
        Console.WriteLine("ForRangeStmt");

        // Obtener los nombres de las variables y el slice
        string indexName = context.ID(0).GetText();
        string valueName = context.ID(1).GetText();
        string sliceName = context.ID(2).GetText();

        ValueWrapper sliceValue = env.Get(sliceName, context.Start);

        if (sliceValue is not ArrayValueWrapper arrayValue)
        {
            throw new SemanticError("Variable is not an array", context.Start);
        }

        for (int i = 0; i < arrayValue.Value.Length; i++)
        {
            Enviorment loopEnv = new Enviorment(env);

            loopEnv.Declare(indexName, new IntValueWrapper(i), context.Start);
            loopEnv.Declare(valueName, arrayValue.Value[i], context.Start);

            Enviorment previousEnv = env;
            env = loopEnv;

            Visit(context.statement());

            env = previousEnv;
        }

        return defaultValue;
    }

    

    // VisitBreakStmt
    public override ValueWrapper VisitBreakStmt(LanguageParser.BreakStmtContext context)
    {
        throw new BreakException();
    }

    // VisitContinueStmt
    public override ValueWrapper VisitContinueStmt(LanguageParser.ContinueStmtContext context)
    {
        throw new ContinueException();
    }

    // VisitReturnStmt
    public override ValueWrapper VisitReturnStmt(LanguageParser.ReturnStmtContext context)
    {
        ValueWrapper value = defaultValue;
        if (context.expr() != null)
        {
            value = Visit(context.expr());
        }

        throw new ReturnException(value);
    }


    // VisitNegate 
    public override ValueWrapper VisitNegate(LanguageParser.NegateContext context)
    {
        ValueWrapper value = Visit(context.expr());
        return value switch
        {
            IntValueWrapper intValue => new IntValueWrapper(-intValue.Value),
            FloatValueWrapper floatValue => new FloatValueWrapper(-floatValue.Value),
            _ => throw new SemanticError("Invalid value", context.Start)
        };
    }

    // VisitFloat
    public override ValueWrapper VisitFloat(LanguageParser.FloatContext context)
    {
        return new FloatValueWrapper(float.Parse(context.GetText()));
    }

    // VisitNew
    public override ValueWrapper VisitNew(LanguageParser.NewContext context)
    {
        ValueWrapper structValue = env.Get(context.ID().GetText(), context.Start);
        if (structValue is not StructValue)
        {
            throw new SemanticError("Invalid struc  t", context.Start);
        }

        List<ValueWrapper> args = new List<ValueWrapper>();

        /*
            args: fieldInit (',' fieldInit)*;
            fieldInit: ID ':' expr;
        */

        if (context.fields() != null)
        {
            foreach (var fieldInit in context.fields().fieldInit())
            {
                string name = fieldInit.ID().GetText();
                ValueWrapper value = Visit(fieldInit.expr());
                Console.WriteLine("Name: " + name + " Value: " + value);
                args.Add(value);
            }
        }

        var instance = ((StructValue)structValue).Struct.Invoke(args, this);

        return instance;
    }

    // VisitIdentifier
    public override ValueWrapper VisitIdentifier(LanguageParser.IdentifierContext context)
    {
        string name = context.ID().GetText();
        return env.Get(name, context.Start);
    }

    // VisitIndex
    public override ValueWrapper VisitIndex(LanguageParser.IndexContext context)
    {
        string name = context.ID().GetText();
        ValueWrapper index = Visit(context.expr());

        if (index is not IntValueWrapper intValue)
        {
            throw new SemanticError("Array index must be an integer", context.Start);
        }

        var array = env.Get(name, context.Start);

        // ver si es de tipo array entonces devolver el valor en el indice
        // si es matriz devolver la fila

        if (array is ArrayValueWrapper arrayValue)
        {
            // ver si el indice esta dentro del rango
            if (intValue.Value < 0 || intValue.Value >= arrayValue.Value.Length)
            {
                throw new SemanticError("Index out of range", context.Start);
            }
            return arrayValue.Value[intValue.Value];
        }
        else if (array is MatrixValueWrapper matrixValue)
        {
            return new ArrayValueWrapper(matrixValue.Value[intValue.Value]);
        }
        else
        {
            throw new SemanticError("Variable is not an array", context.Start);
        }
    }

    // VisitMatrixIndex
    public override ValueWrapper VisitMatrixIndex(LanguageParser.MatrixIndexContext context)
    {
        string name = context.ID().GetText();
        ValueWrapper row = Visit(context.expr(0));
        ValueWrapper col = Visit(context.expr(1));

        if (row is not IntValueWrapper rowValue)
        {
            throw new SemanticError("Row index must be an integer", context.Start);
        }

        if (col is not IntValueWrapper colValue)
        {
            throw new SemanticError("Column index must be an integer", context.Start);
        }

        var matrix = env.Get(name, context.Start);

        if (matrix is MatrixValueWrapper matrixValue)
        {
            return matrixValue.Value[rowValue.Value][colValue.Value];
        }
        else
        {
            throw new SemanticError("Variable is not a matrix", context.Start);
        }
    }

    // VisitIndexMethod
    public override ValueWrapper VisitIndexMethod(LanguageParser.IndexMethodContext context)
    {
        Console.WriteLine("IndexMethod");
        string name = context.ID().GetText();
        ValueWrapper value = Visit(context.expr());
        // Get the slice and find the value and if exists return the index
        if (value is IntValueWrapper intValue)
        {
            var slices = env.Get(name, context.Start);
            // find the value in the slice
            if (slices is ArrayValueWrapper array)
            {
                for (int i = 0; i < array.Value.Length; i++)
                {
                    if (array.Value[i] is IntValueWrapper intWrapper && intWrapper.Value == intValue.Value)
                    {
                        return new IntValueWrapper(i);
                    }
                }
            }
            // return -1
            return new IntValueWrapper(-1);
        }
        else
        {
            throw new SemanticError("Array index must be an integer", context.Start);
        }

    }

    // VisitJoinMethod
    public override ValueWrapper VisitJoinMethod(LanguageParser.JoinMethodContext context)
    {
        string name = context.ID().GetText();
        ValueWrapper value = Visit(context.expr());
        if (value is StringValueWrapper stringValue)
        {
            var slices = env.Get(name, context.Start);
            if (slices is ArrayValueWrapper array)
            {
                string result = "";
                bool first = true;
                foreach (var item in array.Value)
                {
                    if (!first)
                    {
                        result += stringValue.Value;
                    }
                    result += item switch
                    {
                        IntValueWrapper intValue => intValue.Value.ToString(),
                        FloatValueWrapper floatValue => floatValue.Value.ToString(),
                        StringValueWrapper valuestring => valuestring.Value,
                        BoolValueWrapper boolValue => boolValue.Value.ToString(),
                        RuneValueWrapper runeValue => runeValue.Value.ToString(),
                        _ => throw new SemanticError("Invalid value", context.Start)
                    };
                    first = false;
                }
                return new StringValueWrapper(result);

            }
            else
            {
                throw new SemanticError("Variable is not a slice", context.Start);
            }
        }
        else
        {
            throw new SemanticError("Value must be a string", context.Start);
        }
    }

    // VisitLenMethod
    public override ValueWrapper VisitLenMethod(LanguageParser.LenMethodContext context)
    {
        // | 'len' '(' expr ')'                              # lenMethod

        // si expr es un id, entonces retornar la longitud del slice
        // si es el acceso a un índice de una matriz, retornar la longitud de la fila

        ValueWrapper value = Visit(context.expr());

        if (value is ArrayValueWrapper array)
        {
            return new IntValueWrapper(array.Value.Length);
        }
        else if (value is MatrixValueWrapper matrix)
        {
            return new IntValueWrapper(matrix.Value.Length);
        }
        else
        {
            throw new SemanticError("Invalid value", context.Start);
        }

    }

    // VisitAppendMethod
    public override ValueWrapper VisitAppendMethod(LanguageParser.AppendMethodContext context)
    {
        // add the value to the slice and return the new slice
        string name = context.ID().GetText();
        ValueWrapper value = Visit(context.expr());
        var slices = env.Get(name, context.Start);
        if (slices is ArrayValueWrapper array)
        {
            ArrayValueWrapper newArray = new ArrayValueWrapper(new ValueWrapper[array.Value.Length + 1]);
            for (int i = 0; i < array.Value.Length; i++)
            {
                newArray.Value[i] = array.Value[i];
            }
            newArray.Value[array.Value.Length] = value;
            env.Assign(name, newArray, context.Start);
            return newArray;
        }
        else if (slices is MatrixValueWrapper matrix)
        {
            // el value debe ser un array, agregarlo a la matriz y devolver la matriz
            if (value is ArrayValueWrapper arrayValue)
            {
                List<ValueWrapper[]> newMatrix = new List<ValueWrapper[]>(matrix.Value);
                newMatrix.Add(arrayValue.Value);
                env.Assign(name, new MatrixValueWrapper(newMatrix.ToArray()), context.Start);
                return new MatrixValueWrapper(newMatrix.ToArray());
            }
            else
            {
                throw new SemanticError("Value must be an array", context.Start);
            }
        }
        else
        {
            throw new SemanticError("Variable is not a slice", context.Start);
        }
    }

    // VisitAtoiMethod
    public override ValueWrapper VisitAtoiMethod(LanguageParser.AtoiMethodContext context)
    {
        ValueWrapper value = Visit(context.expr());
        if (value is StringValueWrapper stringValue)
        {
            if (int.TryParse(stringValue.Value, out int result))
            {
                return new IntValueWrapper(result);
            }
            else
            {
                throw new SemanticError("strconv.Atoi: parsing error", context.Start);
            }
        }
        else
        {
            throw new SemanticError("strconv.Atoi: argument must be a string", context.Start);
        }
    }

    // VisitParseFloatMethod
    public override ValueWrapper VisitParseFloatMethod(LanguageParser.ParseFloatMethodContext context)
    {
        ValueWrapper value = Visit(context.expr());
        if (value is StringValueWrapper stringValue)
        {
            if (float.TryParse(stringValue.Value, out float result))
            {
                return new FloatValueWrapper(result);
            }
            else
            {
                throw new SemanticError("strconv.ParseFloat: parsing error", context.Start);
            }
        }
        else
        {
            throw new SemanticError("strconv.ParseFloat: argument must be a string", context.Start);
        }
    }

    // VisitTypeOfMethod
    public override ValueWrapper VisitTypeOfMethod(LanguageParser.TypeOfMethodContext context)
    {
        // 'reflect' '.' 'TypeOf' '(' ID ')'
        string name = context.ID().GetText();
        ValueWrapper value = env.Get(name, context.Start);

        if (value is ArrayValueWrapper)
        {
            // []int, []float64, []string, []bool, []rune
            switch ((value as ArrayValueWrapper).Value[0])
            {
                case IntValueWrapper _:
                    return new StringValueWrapper("[]int");
                case FloatValueWrapper _:
                    return new StringValueWrapper("[]float64");
                case StringValueWrapper _:
                    return new StringValueWrapper("[]string");
                case BoolValueWrapper _:
                    return new StringValueWrapper("[]bool");
                case RuneValueWrapper _:
                    return new StringValueWrapper("[]rune");
                default:
                    throw new SemanticError("Invalid type", context.Start);
            }
        }

        return new StringValueWrapper(value switch
        {
            IntValueWrapper => "int",
            FloatValueWrapper => "float64",
            StringValueWrapper => "string",
            BoolValueWrapper => "bool",
            RuneValueWrapper => "rune",
            NilValueWrapper => "nil",
            ArrayValueWrapper => "",
            StructValue => "struct",
            _ => throw new SemanticError("Invalid value", context.Start)
        });
    }

    // VisitNumber
    public override ValueWrapper VisitNumber(LanguageParser.NumberContext context)
    {
        return new IntValueWrapper(int.Parse(context.GetText()));
    }

    //VisitRune
    public override ValueWrapper VisitRune(LanguageParser.RuneContext context)
    {
        return new RuneValueWrapper(context.GetText()[1]);
    }


    // VisitBool
    public override ValueWrapper VisitBool(LanguageParser.BoolContext context)
    {
        return new BoolValueWrapper(bool.Parse(context.GetText()));
    }

    // VisitNil
    public override ValueWrapper VisitNil(LanguageParser.NilContext context)
    {
        return new NilValueWrapper();
    }


    // VisitMulDiv
    public override ValueWrapper VisitMulDiv(LanguageParser.MulDivContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;

        return (left, right, op) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "*") => new IntValueWrapper(leftInt.Value * rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "*") => new FloatValueWrapper(leftFloat.Value * rightInt.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "*") => new FloatValueWrapper(leftInt.Value * rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "*") => new FloatValueWrapper(leftFloat.Value * rightFloat.Value),
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "/") => new IntValueWrapper(leftInt.Value / rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "/") => new FloatValueWrapper(leftFloat.Value / rightInt.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "/") => new FloatValueWrapper(leftInt.Value / rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "/") => new FloatValueWrapper(leftFloat.Value / rightFloat.Value),
            _ => throw new SemanticError("Invalid operation", context.Start)
        };


    }

    // VisitMod
    public override ValueWrapper VisitMod(LanguageParser.ModContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));

        return (left, right) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt) => new IntValueWrapper(leftInt.Value % rightInt.Value),
            _ => throw new SemanticError("Invalid operation", context.Start)
        };
    }

    // VisitAddSub
    public override ValueWrapper VisitAddSub(LanguageParser.AddSubContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;

        return (left, right, op) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "+") => new IntValueWrapper(leftInt.Value + rightInt.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "+") => new FloatValueWrapper(leftInt.Value + rightFloat.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "+") => new FloatValueWrapper(leftFloat.Value + rightInt.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "+") => new FloatValueWrapper(leftFloat.Value + rightFloat.Value),
            (StringValueWrapper leftString, StringValueWrapper rightString, "+") => new StringValueWrapper(leftString.Value + rightString.Value),
            (StringValueWrapper leftString, IntValueWrapper rightInt, "+") => new StringValueWrapper(leftString.Value + rightInt.Value),
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "-") => new IntValueWrapper(leftInt.Value - rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "-") => new FloatValueWrapper(leftFloat.Value - rightInt.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "-") => new FloatValueWrapper(leftInt.Value - rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "-") => new FloatValueWrapper(leftFloat.Value - rightFloat.Value),
            _ => throw new SemanticError("Invalid operation", context.Start)
        };
    }

    // VisitParens  
    public override ValueWrapper VisitParens(LanguageParser.ParensContext context)
    {
        return Visit(context.expr());
    }

    // VisitRelational
    public override ValueWrapper VisitRelational(LanguageParser.RelationalContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;

        /*
            int op int -> bool
            float op float -> bool
            int op float -> bool
            float op int -> bool
            rune op rune -> bool
        */

        return (left, right, op) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "<") => new BoolValueWrapper(leftInt.Value < rightInt.Value),
            (IntValueWrapper leftInt, IntValueWrapper rightInt, ">") => new BoolValueWrapper(leftInt.Value > rightInt.Value),
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "<=") => new BoolValueWrapper(leftInt.Value <= rightInt.Value),
            (IntValueWrapper leftInt, IntValueWrapper rightInt, ">=") => new BoolValueWrapper(leftInt.Value >= rightInt.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "<") => new BoolValueWrapper(leftFloat.Value < rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, ">") => new BoolValueWrapper(leftFloat.Value > rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "<=") => new BoolValueWrapper(leftFloat.Value <= rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, ">=") => new BoolValueWrapper(leftFloat.Value >= rightFloat.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "<") => new BoolValueWrapper(leftInt.Value < rightFloat.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, ">") => new BoolValueWrapper(leftInt.Value > rightFloat.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "<=") => new BoolValueWrapper(leftInt.Value <= rightFloat.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, ">=") => new BoolValueWrapper(leftInt.Value >= rightFloat.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "<") => new BoolValueWrapper(leftFloat.Value < rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, ">") => new BoolValueWrapper(leftFloat.Value > rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "<=") => new BoolValueWrapper(leftFloat.Value <= rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, ">=") => new BoolValueWrapper(leftFloat.Value >= rightInt.Value),
            _ => throw new SemanticError("Invalid operation", context.Start)
        };
    }

    //VisitLogical
    public override ValueWrapper VisitLogical(LanguageParser.LogicalContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;

        return (left, right, op) switch
        {
            (BoolValueWrapper leftBool, BoolValueWrapper rightBool, "&&") => new BoolValueWrapper(leftBool.Value && rightBool.Value),
            (BoolValueWrapper leftBool, BoolValueWrapper rightBool, "||") => new BoolValueWrapper(leftBool.Value || rightBool.Value),
            _ => throw new SemanticError("Invalid operation", context.Start)
        };
    }

    //VisitNot
    public override ValueWrapper VisitNot(LanguageParser.NotContext context)
    {
        ValueWrapper value = Visit(context.expr());
        return value switch
        {
            BoolValueWrapper boolValue => new BoolValueWrapper(!boolValue.Value),
            _ => throw new SemanticError("Invalid value", context.Start)
        };
    }

    // VisitAssign
    public override ValueWrapper VisitAssign(LanguageParser.AssignContext context)
    {
        var asignee = context.expr(0);
        ValueWrapper value = Visit(context.expr(1));

        if (asignee is LanguageParser.IdentifierContext idContext)
        {
            string id = idContext.ID().GetText();
            env.Assign(id, value, context.Start);
            return defaultValue;
        }
        else if (asignee is LanguageParser.CalleeContext calleContext)
        {
            ValueWrapper callee = Visit(calleContext.expr());

            for (int i = 0; i < calleContext.call().Length; i++)
            {
                var action = calleContext.call(i);

                if (i == calleContext.call().Length - 1)
                {
                    if (action is LanguageParser.GetContext propertyAcces)
                    {
                        if (callee is InstanceValue instanceValue)
                        {
                            var instance = instanceValue.Instance;
                            var propertyName = propertyAcces.ID().GetText();
                            instance.Set(propertyName, value);
                        }
                        else
                        {
                            throw new SemanticError("Invalid property access", context.Start);
                        }
                    }

                }

                if (action is LanguageParser.FuncCallContext funcCall)
                {
                    if (callee is FunctionValue functionvalue)
                    {
                        callee = VisitCall(functionvalue.invocable, funcCall.args());
                    }
                    else
                    {
                        throw new SemanticError("Invalid function call", context.Start);
                    }
                }
                else if (action is LanguageParser.GetContext propertyAcces)
                {
                    if (callee is InstanceValue instanceValue)
                    {
                        callee = instanceValue.Instance.Get(propertyAcces.ID().GetText());
                    }
                    else
                    {
                        throw new SemanticError("Invalid property access", context.Start);
                    }
                }
            }

            return callee;
        }
        else if (asignee is LanguageParser.IndexContext indexContext)
        {
            string name = indexContext.ID().GetText();
            ValueWrapper index = Visit(indexContext.expr());
            if (index is not IntValueWrapper intValue)
            {
                throw new SemanticError("Array index must be an integer", context.Start);
            }
            int i = intValue.Value;
            ValueWrapper array = env.Get(name, context.Start);
            if (array is not ArrayValueWrapper arrayValue)
            {
                throw new SemanticError("Variable is not an array", context.Start);
            }
            arrayValue.Value[i] = value;
            return defaultValue;
        }
        else if (asignee is LanguageParser.MatrixIndexContext matrixIndexContext)
        {
            string name = matrixIndexContext.ID().GetText();
            ValueWrapper row = Visit(matrixIndexContext.expr(0));
            ValueWrapper col = Visit(matrixIndexContext.expr(1));
            if (row is not IntValueWrapper rowValue)
            {
                throw new SemanticError("Row index must be an integer", context.Start);
            }
            if (col is not IntValueWrapper colValue)
            {
                throw new SemanticError("Column index must be an integer", context.Start);
            }
            int i = rowValue.Value;
            int j = colValue.Value;
            ValueWrapper matrix = env.Get(name, context.Start);
            if (matrix is not MatrixValueWrapper matrixValue)
            {
                throw new SemanticError("Variable is not a matrix", context.Start);
            }
            matrixValue.Value[i][j] = value;
            return defaultValue;
        }
        else
        {
            throw new SemanticError("Invalid assignee", context.Start);
        }

        return defaultValue;

    }


    // VisitString
    public override ValueWrapper VisitString(LanguageParser.StringContext context)
    {
        string text = context.GetText();
        string processedText = text.Substring(1, text.Length - 2); // Remueve las comillas inicial y final
        return new StringValueWrapper(processedText);
    }


    // VisitEquality
    public override ValueWrapper VisitEquality(LanguageParser.EqualityContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;


        return (left, right, op) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "==") => new BoolValueWrapper(leftInt.Value == rightInt.Value),
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "!=") => new BoolValueWrapper(leftInt.Value != rightInt.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "==") => new BoolValueWrapper(leftFloat.Value == rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "!=") => new BoolValueWrapper(leftFloat.Value != rightFloat.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "==") => new BoolValueWrapper(leftInt.Value == rightFloat.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "!=") => new BoolValueWrapper(leftInt.Value != rightFloat.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "==") => new BoolValueWrapper(leftFloat.Value == rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "!=") => new BoolValueWrapper(leftFloat.Value != rightInt.Value),
            (BoolValueWrapper leftBool, BoolValueWrapper rightBool, "==") => new BoolValueWrapper(leftBool.Value == rightBool.Value),
            (BoolValueWrapper leftBool, BoolValueWrapper rightBool, "!=") => new BoolValueWrapper(leftBool.Value != rightBool.Value),
            (StringValueWrapper leftString, StringValueWrapper rightString, "==") => new BoolValueWrapper(leftString.Value == rightString.Value),
            (StringValueWrapper leftString, StringValueWrapper rightString, "!=") => new BoolValueWrapper(leftString.Value != rightString.Value),
            (RuneValueWrapper leftRune, RuneValueWrapper rightRune, "==") => new BoolValueWrapper(leftRune.Value == rightRune.Value),
            (RuneValueWrapper leftRune, RuneValueWrapper rightRune, "!=") => new BoolValueWrapper(leftRune.Value != rightRune.Value),
            // Comparación con NilValueWrapper
            (NilValueWrapper _, NilValueWrapper _, "==") => new BoolValueWrapper(true),
            (NilValueWrapper _, NilValueWrapper _, "!=") => new BoolValueWrapper(false),
            (NilValueWrapper _, _, "==") => new BoolValueWrapper(false),
            (NilValueWrapper _, _, "!=") => new BoolValueWrapper(true),
            (_, NilValueWrapper _, "==") => new BoolValueWrapper(false),
            (_, NilValueWrapper _, "!=") => new BoolValueWrapper(true),

            _ => throw new SemanticError("Invalid operation", context.Start)
        };
    }

    // VisitorAddAssign
    public override ValueWrapper VisitAddAssign(LanguageParser.AddAssignContext context)
    {
        string name = context.ID().GetText();
        ValueWrapper value = Visit(context.expr());
        ValueWrapper left = env.Get(name, context.Start);

        ValueWrapper result = (left, value) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt) => new IntValueWrapper(leftInt.Value + rightInt.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat) => new FloatValueWrapper(leftInt.Value + rightFloat.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt) => new FloatValueWrapper(leftFloat.Value + rightInt.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat) => new FloatValueWrapper(leftFloat.Value + rightFloat.Value),
            (StringValueWrapper leftString, StringValueWrapper rightString) => new StringValueWrapper(leftString.Value + rightString.Value),
            (StringValueWrapper leftString, IntValueWrapper rightInt) => new StringValueWrapper(leftString.Value + rightInt.Value),
            _ => throw new SemanticError("Invalid operation", context.Start)
        };

        env.Assign(name, result, context.Start);
        return defaultValue;
    }

    // VisitSubAssign
    public override ValueWrapper VisitSubAssign(LanguageParser.SubAssignContext context)
    {
        string name = context.ID().GetText();
        ValueWrapper value = Visit(context.expr());
        ValueWrapper left = env.Get(name, context.Start);

        ValueWrapper result = (left, value) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt) => new IntValueWrapper(leftInt.Value - rightInt.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat) => new FloatValueWrapper(leftInt.Value - rightFloat.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt) => new FloatValueWrapper(leftFloat.Value - rightInt.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat) => new FloatValueWrapper(leftFloat.Value - rightFloat.Value),
            _ => throw new SemanticError("Invalid operation", context.Start)
        };

        env.Assign(name, result, context.Start);
        return defaultValue;
    }

    // VisitInc
    public override ValueWrapper VisitInc(LanguageParser.IncContext context)
    {
        string name = context.ID().GetText();
        ValueWrapper value = env.Get(name, context.Start);

        ValueWrapper result = value switch
        {
            IntValueWrapper intValue => new IntValueWrapper(intValue.Value + 1),
            FloatValueWrapper floatValue => new FloatValueWrapper(floatValue.Value + 1),
            _ => throw new SemanticError("Invalid operation", context.Start)
        };

        env.Assign(name, result, context.Start);
        return defaultValue;
    }

    // VisitDec
    public override ValueWrapper VisitDec(LanguageParser.DecContext context)
    {
        string name = context.ID().GetText();
        ValueWrapper value = env.Get(name, context.Start);

        ValueWrapper result = value switch
        {
            IntValueWrapper intValue => new IntValueWrapper(intValue.Value - 1),
            FloatValueWrapper floatValue => new FloatValueWrapper(floatValue.Value - 1),
            _ => throw new SemanticError("Invalid operation", context.Start)
        };

        env.Assign(name, result, context.Start);
        return defaultValue;
    }

    // VisitCalle
    public override ValueWrapper VisitCallee(LanguageParser.CalleeContext context)
    {
        ValueWrapper callee = Visit(context.expr());

        foreach (var call in context.call())
        {
            if (call is LanguageParser.FuncCallContext funcCall)
            {
                if (callee is FunctionValue functionvalue)
                {
                    callee = VisitCall(functionvalue.invocable, funcCall.args());
                }
                else
                {
                    throw new SemanticError("Invalid function call", context.Start);
                }
            }
            else if (call is LanguageParser.GetContext propertyAcces)
            {
                if (callee is InstanceValue instanceValue)
                {
                    Console.WriteLine("Property access", propertyAcces.ID().GetText());
                    callee = instanceValue.Instance.Get(propertyAcces.ID().GetText());
                }
                else
                {
                    throw new SemanticError("Invalid property access", context.Start);
                }
            }
        }

        return callee;
    }

    // VisitCall
    public ValueWrapper VisitCall(Invocable invocable, LanguageParser.ArgsContext context)
    {
        List<ValueWrapper> args = new List<ValueWrapper>();

        /*
            call: '(' args? ')' #FuncCall | '.' ID #Get;
            args: expr (',' expr)*;
        */

        if (context != null)
        {
            foreach (var expr in context.expr())
            {
                args.Add(Visit(expr));
            }
        }
        //TODO: implementar validación de argumentos

        return invocable.Invoke(args, this);
    }


}