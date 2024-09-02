using System.Collections.Generic;

namespace SimpleGraphQL.GraphQLParser.AST
{
    public class GraphQLListValue : GraphQLValue
    {
        private readonly ASTNodeKind _kind;

        public GraphQLListValue(ASTNodeKind kind)
        {
            _kind = kind;
        }

        public string AstValue { get; set; }

        public override ASTNodeKind Kind => _kind;

        public List<GraphQLValue> Values { get; set; }

        public override string ToString() => AstValue;
    }
}
