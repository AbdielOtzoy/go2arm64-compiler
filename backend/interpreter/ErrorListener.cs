// import baseErrorListener
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

 public class SemanticError: Exception 
 {
    private string message;
    private Antlr4.Runtime.IToken token;

    public SemanticError(string message, Antlr4.Runtime.IToken token)
    {
        this.message = message;
        this.token = token;
    }

    public override string Message
    {
        get
        {
            return message + " at line " + token.Line + " column " + token.Column;
        }
    }
 }

 public class DescriptiveErrorListener : BaseErrorListener, IAntlrErrorListener<int>
 {
    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        Console.WriteLine("Syntax error at line " + line + " column " + charPositionInLine + ": " + msg);
        throw new ParseCanceledException("Syntax error at line " + line + " column " + charPositionInLine + ": " + msg);
    }
 }

 public class SyntaxErrorListener : BaseErrorListener 
    {
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            Console.WriteLine("Syntax error at line " + line + " column " + charPositionInLine + ": " + msg);
            throw new ParseCanceledException("Syntax error at line " + line + " column " + charPositionInLine + ": " + msg);
        }
    }

