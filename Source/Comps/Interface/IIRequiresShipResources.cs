namespace Jaxxa.EnhancedDevelopment.Core.Comp.Interface
{
    public interface IRequiresShipResources
    {
        
        bool Satisfied { get;  }
                
        string StatusString { get; }

        bool UseResources();

    }
}