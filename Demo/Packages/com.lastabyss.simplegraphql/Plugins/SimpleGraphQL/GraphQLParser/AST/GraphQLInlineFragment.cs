using System.Collections.Generic;

namespace SimpleGraphQL.GraphQLParser.AST
{
    public class GraphQLInlineFragment : ASTNode, IHasDirectivesNode
    {
        public List<GraphQLDirective> Directives { get; set; }

        public override ASTNodeKind Kind => ASTNodeKind.InlineFragment;

        public GraphQLSelectionSet SelectionSet { get; set; }

        public GraphQLNamedType TypeCondition { get; set; }
    }
}
