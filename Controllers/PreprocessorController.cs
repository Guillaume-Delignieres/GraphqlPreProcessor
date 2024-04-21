using System.ComponentModel.DataAnnotations;
using graphql_preprocessor.GraphQl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace graphql_preprocessor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreprocessorController : Controller
    {
        private readonly GraphQlPreprocessor<ParserGraphQlValidator> _graphQlPreProcessor = new();

        [HttpPost]
        [Route("/preprocess")]
        // Consume plain text - might be easier for direct usage
        [Consumes("text/plain")]
        public IResult PreProcess([FromQuery, BindRequired] string? operation, [FromBody] string queryToProcess)
        {
            if (string.IsNullOrWhiteSpace(queryToProcess))
                return Results.BadRequest("Query to process is empty");

            try
            {
                // Experimented with different kind of validator, one based on regex, but complexity explored.
                // This validator ensure the file is parsable and an actual graphQL file
                var preProcessedTest = _graphQlPreProcessor.PreProcess(queryToProcess, operation);
                return Results.Text(preProcessedTest);
            }
            catch (InvalidOperationException e)
            {
                return Results.BadRequest(e.Message);
            }
            catch (ArgumentException e)
            {
                return Results.BadRequest(e.Message);

            }
        }
    }
}
