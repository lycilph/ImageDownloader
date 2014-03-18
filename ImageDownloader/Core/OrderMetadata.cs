using System.ComponentModel;

namespace Core
{
    public interface OrderMetadata
    {
        [DefaultValue(int.MaxValue)]
        int Order { get; }
    }
}
