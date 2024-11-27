﻿namespace SimpleGraphQL.GraphQLParser.AST
{
    public class GraphQLVariableDefinition : ASTNode
    {
        public object DefaultValue { get; set; }

        public override ASTNodeKind Kind => ASTNodeKind.VariableDefinition;

        public GraphQLType Type { get; set; }

        public GraphQLVariable Variable { get; set; }
    }
}