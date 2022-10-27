namespace GraphQL.EntityFramework;

[AttributeUsage(AttributeTargets.Assembly)]
public class OverrideIdAttribute :
    GraphQLAttribute
{
    /// <inheritdoc/>
    public override void Modify(TypeInformation info)
    {
        if (TryGetIdType(info.Type, out var graphType))
        {
            info.GraphType = graphType;
        }
    }
    internal static bool TryGetIdType(Type memberType, [NotNullWhen(true)] out Type? graphType)
    {
        if (memberType == typeof(Guid))
        {
            graphType = typeof(NonNullGraphType<IdGraph>);
            return true;
        }
        if (memberType == typeof(Guid?))
        {
            graphType = typeof(IdGraph);
            return true;
        }

        graphType = null;
        return false;
    }
    internal static void Convert<T>(ref Type? graphType)
    {
        if (typeof(T) == typeof(Guid))
        {
            graphType = typeof(NonNullGraphType<IdGraph>);
        }
        else if (typeof(T) == typeof(Guid?))
        {
            graphType = typeof(IdGraph);
        }
    }
}