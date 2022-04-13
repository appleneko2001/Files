using Avalonia;
using Avalonia.Media;

namespace Files.Views.Converters.Parameters.PathGen
{
    public interface IPathGenRecipe
    {
        void BuildRecipe(StreamGeometryContext ctx, Rect bounds);
    }
}