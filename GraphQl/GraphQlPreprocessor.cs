using System.Text.RegularExpressions;
using GraphQLParser;
using GraphQLParser.Exceptions;

namespace graphql_preprocessor.GraphQl
{
    public class GraphQlPreprocessor<T> where T : IGraphQlValidator, new()
    {
        private readonly T _validator = new T();
        
        public string PreProcess(string grapQlfileContent, string? operationName)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(grapQlfileContent))
            {
                throw new ArgumentException("The content of the graphQL file is empty.", nameof(grapQlfileContent));
            }
            
            if (string.IsNullOrWhiteSpace(operationName))
            {
                throw new ArgumentException("The operation name is empty.", nameof(operationName));
            }

            // Validate the format of the graphQl content
            _validator.Validate(grapQlfileContent);
            
            // Extract the operation and required fragments
            return ExtractOperationAndFragment(grapQlfileContent, operationName);

        }

        public List<string> ExtractOperations(string grapQlfileContent)
        {
            if (string.IsNullOrWhiteSpace(grapQlfileContent))
            {
                throw new ArgumentException("The content of the graphQL file is empty.", nameof(grapQlfileContent));
            }
            // Validate the format of the graphQl content
            _validator.Validate(grapQlfileContent);

            var operations = new List<string>();
            var operationMatches = Regex.Matches(grapQlfileContent, $@"\bquery\s+(\w+)\b\s*{{", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Match operationMatch in operationMatches)
            {
                operations.Add(operationMatch.Groups[1].Value);
            }

            return operations;
        }
        private static string ExtractOperationAndFragment(string grapQlfileContent, string? operationName)
        {
            // Find the operation
            var operationMatch = Regex.Match(grapQlfileContent, $@"\bquery\s+{operationName}\b\s*{{", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (!operationMatch.Success)
            {
                throw new InvalidOperationException($"Operation '{operationName}' not found");
            }

            // Extract the operation from the whole file content
            int nbBrackets = 0;
            int operationStartIndex = operationMatch.Index;
            int operationEndIndex = operationStartIndex;
            for (int i = operationStartIndex; i < grapQlfileContent.Length; i++)
            {
                if (grapQlfileContent[i] == '{')
                {
                    nbBrackets++;
                }
                else if (grapQlfileContent[i] == '}')
                {
                    nbBrackets--;
                    if (nbBrackets == 0)
                    {
                        operationEndIndex = i;
                        break;
                    }
                }
            }

            if (operationEndIndex <= operationStartIndex)
            {
                throw new InvalidOperationException("Operation is not properly formatted");
            }

            string operation = grapQlfileContent.Substring(operationStartIndex, operationEndIndex - operationStartIndex + 1);

            // Find all fragment definitions
            var fragmentMatches = Regex.Matches(grapQlfileContent, @"\bfragment\s+(\w+)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Include all fragments used by the operation
            foreach (Match fragmentMatch in fragmentMatches)
            {
                string fragmentName = fragmentMatch.Groups[1].Value;
                if (operation.Contains(fragmentName))
                {
                    int fragmentStartIndex = fragmentMatch.Index;
                    int fragmentEndIndex = fragmentStartIndex;
                    nbBrackets = 0; // Reset bracket count for fragments

                    for (int i = fragmentStartIndex; i < grapQlfileContent.Length; i++)
                    {
                        if (grapQlfileContent[i] == '{')
                        {
                            nbBrackets++;
                        }
                        else if (grapQlfileContent[i] == '}')
                        {
                            nbBrackets--;
                            if (nbBrackets == 0)
                            {
                                fragmentEndIndex = i;
                                break;
                            }
                        }
                    }

                    if (fragmentEndIndex <= fragmentStartIndex)
                    {
                        throw new InvalidOperationException("Could not find the end of the fragment.");
                    }

                    string fragment = grapQlfileContent.Substring(fragmentStartIndex, fragmentEndIndex - fragmentStartIndex + 1);
                    operation += "\n\n" + fragment;
                }
            }

            return operation;
        }
    }

}

