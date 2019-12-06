using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyzer
{
    public static class CsharpAnalyzer
    {
        public static async Task<IEnumerable<string>> AnalyzeAsync(string path)
        {
            var code = await GetCode(path);
            return AnalyzeInternal(code);
        }

        internal static IEnumerable<string> AnalyzeInternal(string code)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();
            
            var method = root.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .First();
            
            var possibleAssignments = FindAssignments(method.Body);
            
            foreach (var assignmentExpressionSyntax in possibleAssignments.Reverse())
            {
                
                if(assignmentExpressionSyntax.Right is LiteralExpressionSyntax literalExpressionSyntax )
                {
                    yield return literalExpressionSyntax.Token.Text;
                }
            }
        }

        private static IEnumerable<AssignmentExpressionSyntax> FindAssignments(
            StatementSyntax scope, 
            int scopeLevel = 0,
            int? lastScopeLevelWithAssignment = null)
        {
            int? scopeLevelWithAssignment = lastScopeLevelWithAssignment;
            
            foreach (var childNode in scope.ChildNodes().Reverse())
            {
                // x has already been assigned in outer scope
                if (scopeLevelWithAssignment <= scopeLevel)
                {
                    break;
                }

                if (childNode is ExpressionStatementSyntax expressionStatementSyntax)
                {
                    if (expressionStatementSyntax.Expression is AssignmentExpressionSyntax assignmentExpressionSyntax)
                    {
                        yield return assignmentExpressionSyntax;
                        scopeLevelWithAssignment = scopeLevel;
                    }
                }
                
                if (childNode is IfStatementSyntax ifStatementSyntax)
                {
                    var assignments = FindAssignments(ifStatementSyntax.Statement, ++scopeLevel, scopeLevelWithAssignment);
                    foreach (var innerAssignmentExpressionSyntax in assignments)
                    {
                        yield return innerAssignmentExpressionSyntax;
                    }
                }
            }
        }

        private static Task<string> GetCode(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllTextAsync(path, Encoding.UTF8);
            }
            
            throw new InvalidOperationException("There is no file");
        }
    }
}