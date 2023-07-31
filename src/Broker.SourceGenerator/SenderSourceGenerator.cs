namespace Broker.SourceGenerator;

[Generator]
public sealed class SenderSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //todo use StringWriter & IndentedTextWriter
        
        var compilationProvider = context.CompilationProvider;

        context.RegisterSourceOutput(
            compilationProvider, (productionContext, compilation) =>
            {
                var stringBuilder = new StringBuilder();
                var nonGenericRequestSwitchCases = new StringBuilder();
                var genericRequestSwitchCases = new StringBuilder();

                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    var semanticModel = compilation.GetSemanticModel(syntaxTree);
                    foreach (var classDeclaration in syntaxTree
                                 .GetRoot()
                                 .DescendantNodes()
                                 .OfType<ClassDeclarationSyntax>())
                    {
                        if (semanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol symbol) continue;
                        AppendHandlerServices(symbol, stringBuilder);
                        AppendSwitchCases(symbol, nonGenericRequestSwitchCases, genericRequestSwitchCases);
                    }
                }

                stringBuilder.AppendLine("    services.AddScoped<ISender, Sender>();");

                GenerateHandlerServiceExtensionsFile(productionContext, stringBuilder);
                GenerateSenderClassFile(productionContext, nonGenericRequestSwitchCases, genericRequestSwitchCases);
            });
    }

    private void AppendHandlerServices(INamedTypeSymbol symbol, StringBuilder builder)
    {
        foreach (var @interface in symbol.Interfaces)
        {
            if (@interface.Name != "IHandler") continue;
            var handlerType = symbol.ToString();
            builder.AppendLine($"   services.AddScoped<{@interface}, {handlerType}>();");
        }
    }

    private void AppendSwitchCases(INamedTypeSymbol symbol, StringBuilder nonGenericRequestSwitchCases,
        StringBuilder genericRequestSwitchCases)
    {
        foreach (var @interface in symbol.Interfaces)
        {
            if (@interface.Name != "IHandler") continue;
            var requestType = @interface.TypeArguments[0].ToString();
            if (@interface.TypeArguments.Length == 1)
            {
                nonGenericRequestSwitchCases.AppendLine(
                    $"case {requestType} command: return _serviceProvider.GetRequiredService<IHandler<{requestType}>>().HandleAsync(command, cancellationToken);"
                );
            }
            else if (@interface.TypeArguments.Length == 2)
            {
                var responseType = @interface.TypeArguments[1].ToString();
                genericRequestSwitchCases.AppendLine(
                    $"case {requestType} command: " +
                    $"if (typeof(TResponse) == typeof({responseType})) " +
                    $"{{ " +
                    $"var response =  await _serviceProvider.GetRequiredService<IHandler<{requestType}, {responseType}>>().HandleAsync(command, cancellationToken); " +
                    $"return System.Runtime.CompilerServices.Unsafe.As<{responseType}, TResponse>(ref response); " +
                    $"}} " +
                    $"break;"
                );
            }
        }
    }

    private void GenerateHandlerServiceExtensionsFile(SourceProductionContext context, StringBuilder builder)
    {
        var diSource = $$"""
                         using Microsoft.Extensions.DependencyInjection;
                         using Broker.Abstractions;
                         
                         namespace Broker.SourceGenerator;
                         
                         public static class HandlerServiceCollectionExtensions
                         {
                            public static void AddBroker(this IServiceCollection services)
                            {
                                {{builder}}
                            }
                         }
                         
                         """;

        context.AddSource("HandlerServiceCollectionExtensions.g.cs", diSource);
    }

    private void GenerateSenderClassFile(SourceProductionContext context, StringBuilder nonGenericRequestSwitchCases,
        StringBuilder genericRequestSwitchCases)
    {
        var senderSource = $$"""
                             using System;
                             using System.Threading.Tasks;
                             using System.Threading;
                             using Broker.Abstractions;
                             
                             namespace Broker.SourceGenerator;
                             
                             public class Sender : ISender
                             {
                                 private readonly IServiceProvider _serviceProvider;
                         
                                 public Sender(IServiceProvider serviceProvider)
                                 {
                                     _serviceProvider = serviceProvider;
                                 }
                         
                                 public Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
                                     where TRequest : IRequest
                                 {
                                     switch (request)
                                     {
                                         {{nonGenericRequestSwitchCases}}
                                         default:
                                             throw new InvalidOperationException($"No handler registered for type {request.GetType()}");
                                     }
                                 }
                         
                                 public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
                                 {
                                     switch (request)
                                     {
                                         {{genericRequestSwitchCases}}
                                         default:
                                             throw new InvalidOperationException($"No handler registered for type {request.GetType()}");
                                     }
                                     throw new InvalidOperationException("No handler registered for type");
                                 }
                             }
                             """;

        context.AddSource("Sender.g.cs", senderSource);
    }
}