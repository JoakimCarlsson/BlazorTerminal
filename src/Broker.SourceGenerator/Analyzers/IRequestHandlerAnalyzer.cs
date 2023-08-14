namespace Broker.SourceGenerator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RequestHandlerAnalyzer : DiagnosticAnalyzer
{
    private const string Category = "Usage";
    private const string DiagnosticId = "MissingHandlerId";

    private static readonly LocalizableString Title = "Missing IRequest Handler";
    private static readonly LocalizableString MessageFormat = "No IHandler found for the IRequest: '{0}'";
    private static readonly LocalizableString Description = "Every IRequest should have an associated IHandler.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }


    private void AnalyzeSymbol(
        SymbolAnalysisContext context
        
        )
    {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        bool isGeneric = ImplementsGenericIRequest(namedTypeSymbol, context.Compilation);
        bool isNonGeneric = !isGeneric && ImplementsNonGenericIRequest(namedTypeSymbol, context.Compilation);
    
        if (!isGeneric && !isNonGeneric) return;

        var typeArgCount = isGeneric ? 2 : 1;

        var allHandlers = GetAllTypes(namedTypeSymbol.ContainingAssembly);
        var relevantHandlers = allHandlers.FirstOrDefault(t => IsRelevantCommandHandler(t, namedTypeSymbol, typeArgCount));

        if (relevantHandlers is null) return;

        var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
        context.ReportDiagnostic(diagnostic);
    }

    private bool IsRelevantCommandHandler(
        INamedTypeSymbol handlerType,
        INamedTypeSymbol requestType,
        int requestTypeGenericArgumentCount
        )
    {
        return handlerType.Interfaces
            .Any(i => i.Name == "IHandler" && 
                      i.TypeArguments.Length == requestTypeGenericArgumentCount && 
                      SymbolEqualityComparer.Default.Equals(i.TypeArguments[0], requestType));
    }
    
    private bool ImplementsGenericIRequest(INamedTypeSymbol type, Compilation compilation)
    {
        var request = compilation.GetTypeByMetadataName("Broker.Abstractions.IRequest`1");
        return type.Interfaces.Any(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, request));
    }

    private bool ImplementsNonGenericIRequest(INamedTypeSymbol type, Compilation compilation)
    {
        var request = compilation.GetTypeByMetadataName("Broker.Abstractions.IRequest");
        return type.Interfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, request));
    }
    
    private static IEnumerable<INamedTypeSymbol> GetAllTypes(IAssemblySymbol assembly) => GetAllTypes(assembly.GlobalNamespace);

    private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol namespaceSymbol)
    {
        foreach (var typeSymbol in namespaceSymbol.GetTypeMembers())
            yield return typeSymbol;

        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
        foreach (var type in GetAllTypes(childNamespace))
        {
            yield return type;
        }
    }
}