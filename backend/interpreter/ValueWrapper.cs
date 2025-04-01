public abstract record ValueWrapper;

public record IntValueWrapper(int Value) : ValueWrapper;

public record FloatValueWrapper(float Value) : ValueWrapper;

public record StringValueWrapper(string Value) : ValueWrapper;

public record BoolValueWrapper(bool Value) : ValueWrapper;

public record RuneValueWrapper(char Value) : ValueWrapper;

public record ArrayValueWrapper(ValueWrapper[] Value) : ValueWrapper;

public record MatrixValueWrapper(ValueWrapper[][] Value) : ValueWrapper;

public record FunctionValue(Invocable invocable, string Name) : ValueWrapper;

public record InstanceValue(Instance Instance) : ValueWrapper;

public record StructValue(Struct Struct) : ValueWrapper;

public record VoidBody : ValueWrapper;

public record NilValueWrapper : ValueWrapper;
