using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Accounting.API.Swagger;

public sealed class CompanyHeaderOperationFilter : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        operation.Parameters ??= new List<IOpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Company-Id",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Selected company ID",
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Format = "uuid"
            }
        });
    }
}