public class BreakException : Exception
{
    public BreakException() : base("Break statement outside of loop")
    {
    }
}

public class ContinueException : Exception
{
    public ContinueException() : base("Continue statement outside of loop")
    {
    }
}

public class ReturnException : Exception
{
    public ValueWrapper Value { get; }

    public ReturnException(ValueWrapper value) : base("Return statement outside of function")
    {
        Value = value;
    }
}