using GraphQLParser;
using GraphQLParser.Exceptions;

namespace graphql_preprocessor.GraphQl;

public class ParserGraphQlValidator : IGraphQlValidator
{
    public void Validate(string graphqlContent)
    {
        // Check if the input is null or empty
        if (string.IsNullOrWhiteSpace(graphqlContent))
        {
            throw new ArgumentException("The original request cannot be null or empty.", nameof(graphqlContent));
        }

        try
        {
            // Uses GraphQl Parser to validate the input
            // Might not be the most effective as later we go through the file again to find the operations
            // But makes sure the content is valid
            Parser.Parse(graphqlContent);
                
        }
        catch (GraphQLSyntaxErrorException e)
        {
            throw new InvalidOperationException(e.Description);
        }
        catch (GraphQLMaxDepthExceededException e)
        {
            throw new InvalidOperationException(e.Description);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Error while trying to parse the graphQL file", e);
        }
    }
}