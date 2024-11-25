﻿using System.Diagnostics;

namespace SimpleGraphQL.GraphQLParser.AST
{
    [DebuggerDisplay("{Value}")]
    public class GraphQLName : ASTNode
    {
        public override ASTNodeKind Kind => ASTNodeKind.Name;

        public string Value { get; set; }
    }
}