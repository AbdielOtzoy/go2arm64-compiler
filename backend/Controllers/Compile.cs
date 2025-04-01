using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using analyzer;
using Antlr4.Runtime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Antlr4.Runtime.Misc;
// json
using System.Text.Json;
using System.Net.Http;
using System.Text;

namespace api.Controllers
{
    [Route("compile")]
    [ApiController] // Agrega esta anotación para mejores validaciones automáticas
    public class CompileController : ControllerBase
    {
        private readonly ILogger<CompileController> _logger;

        public CompileController(ILogger<CompileController> logger)
        {
            _logger = logger;
        }

        // GET /compile
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Compile API is running");
        }

        // GET /compile/error
        [HttpGet("error")]
        public IActionResult Error()
        {
            return Problem("An error occurred");
        }

        public class CompileRequest
        {
            [Required]
            public required string Code { get; set; }
        }

        // POST /compile
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CompileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Compiling code: {0}", request.Code);

            var inputStream = new AntlrInputStream(request.Code);
            var lexer = new LanguageLexer(inputStream);

            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new DescriptiveErrorListener());

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new LanguageParser(tokenStream);

            parser.RemoveErrorListeners();
            parser.AddErrorListener(new SyntaxErrorListener());

            try
            {
                var tree = parser.program();

                // var interpreter = new InterpreterVisitor();
                // interpreter.Visit(tree);
                // interpreter.ExecuteMain(tree);
                // var symbolsTable = interpreter.getJSONSymbols();

                var compiler = new CompilerVisitor();
                compiler.Visit(tree);

                return Ok(new { result = compiler.c.ToString() });
            }
            catch (ParseCanceledException e)
            {
                Console.WriteLine("mensaje:" + e.Message);
                return BadRequest( new { message = e.Message });
            }
            catch (RecognitionException e)
            {
                Console.WriteLine("mensaje:" + e.Message);
                return BadRequest( new { message = e.Message });
            }
            catch (SemanticError e)
            {
                Console.WriteLine("mensaje:" + e.Message);
                return BadRequest( new { message = e.Message });
            }

        }

        [HttpPost("ast")]
        public async Task<IActionResult> GetAST([FromBody] CompileRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string grammarPath = Path.Combine(Directory.GetCurrentDirectory(), "grammars", "Language.g4");

            var grammar = "";
            
            try
            {
                if(System.IO.File.Exists(grammarPath))
                {
                    grammar = System.IO.File.ReadAllText(grammarPath);
                }
                else
                {
                    return BadRequest(new { message = "Grammar file not found" });
                }
            }
            catch (System.Exception)
            {
                
                return BadRequest(new { message = "Error reading grammar file" });
            }

            var payload = new
            {
                grammar,
                lexgrammar = "",
                input = request.Code,
                start = "program"
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var context = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using( var client = new HttpClient()){
                try
                {
                    HttpResponseMessage response = await client.PostAsync("http://lab.antlr.org/parse/", context);
                    response.EnsureSuccessStatusCode();

                    string result = await response.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(result);
                    var root = doc.RootElement;

                    if(root.TryGetProperty("result", out JsonElement resultElement) &&  
                        resultElement.TryGetProperty("svgtree", out JsonElement svgtreeElement))
                        {
                            string svgtree = svgtreeElement.GetString() ?? string.Empty;
                            return Ok(new { svgtree });
                        }

                    return BadRequest(new { message = "Error parsing AST" });
                }
                catch (System.Exception ex)
                {
                    
                    _logger.LogError("Error parsing AST", ex);
                    return BadRequest(new { message = "Error parsing AST" });
                }
            }
        }
    }
}
