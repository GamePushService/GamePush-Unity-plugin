using SimpleGraphQL.GraphQLParser.AST;

namespace SimpleGraphQL.GraphQLParser
{
    public class Parser
    {
        private readonly ILexer _lexer;

        public Parser(ILexer lexer)
        {
            _lexer = lexer;
        }

        public GraphQLDocument Parse(ISource source)
        {
            using (var context = new ParserContext(source, _lexer))
            {
                return context.Parse();
            }
        }
    }
}