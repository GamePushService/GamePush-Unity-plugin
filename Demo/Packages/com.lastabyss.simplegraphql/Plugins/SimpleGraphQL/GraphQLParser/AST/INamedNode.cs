﻿namespace SimpleGraphQL.GraphQLParser.AST
{
    public interface INamedNode
    {
        GraphQLName Name { get; set; }
    }
}
