namespace SimpleGraphQL.GraphQLParser.AST
{
    public class GraphQLScalarValue : GraphQLValue
    {
        private readonly ASTNodeKind _kind;

        public GraphQLScalarValue(ASTNodeKind kind)
        {
            _kind = kind;
        }

        public override ASTNodeKind Kind => _kind;

        public string Value { get; set; }

        public override string ToString() => Kind == ASTNodeKind.StringValue ? $"\"{Value}\"" : Value;
    }
}
