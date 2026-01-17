using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GodotLauncher.Scripts.Tools;

class ValueSubstitutionRewriter : CSharpSyntaxRewriter
{
    private readonly Dictionary<string, object> _values;

    public ValueSubstitutionRewriter(Dictionary<string, object> values)
    {
        _values = values;
    }

    public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (!_values.TryGetValue(node.Identifier.Text, out var value))
            return base.VisitIdentifierName(node);

        if (value is null)
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NullLiteralExpression
            );
        }

        return value switch
        {
            bool b => SyntaxFactory.LiteralExpression(
                b ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),

            int i => SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(i)),

            float d => SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(d)),

            double d => SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(d)),

            string s => SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(s)),

            DateTime d => SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(d.ToString())),

            _ => throw new NotSupportedException(
                $"Unsupported literal type: {value.GetType()}")
        };
    }
}
