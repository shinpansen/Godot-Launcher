using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace GodotLauncher.Scripts.Tools;

public static class RoslynTools
{
    public static object EvaluateExpression(string expression)
    {
        return CSharpScript.EvaluateAsync(expression).Result;
    }

    public static string RewriteExpression(string expression, Dictionary<string, object> values)
    {
        var syntax = SyntaxFactory.ParseExpression(expression);
        var rewriter = new ValueSubstitutionRewriter(values);
        var rewritten = rewriter.Visit(syntax);
        return rewritten.ToFullString();
    }

    public static HashSet<string> ExtractExpressionProperties(string expression)
    {
        var syntaxTree = SyntaxFactory.ParseExpression(expression);
        var identifiers = new HashSet<string>();
        syntaxTree.DescendantNodes().ToList().ForEach(node =>
        {
            switch (node)
            {
                case IdentifierNameSyntax id:
                    identifiers.Add(id.Identifier.Text);
                    break;

                case MemberAccessExpressionSyntax member:
                    identifiers.Add(member.ToString());
                    break;
            }
        });
        return identifiers;
    }
}
