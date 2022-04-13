using Avalonia;
using Avalonia.Media;

namespace Files.Views.Converters.Parameters.PathGen
{
    public class FolderCardGenRecipe : IPathGenRecipe
    {
        public double CardLabelLength { get; set; } = 48;

        public double CardAngleSize { get; set; } = 4;

        public double CardHeight { get; set; } = 40;
        
        public void BuildRecipe(StreamGeometryContext ctx, Rect b)
        {
            ctx.BeginFigure(b.TopLeft, true);

            var topCenter1 = new Point(CardLabelLength, b.Top);

            ctx.LineTo(topCenter1);

            var arcSize = new Size(CardAngleSize, CardAngleSize);
            
            ctx.ArcTo(topCenter1, arcSize, 90, false, SweepDirection.Clockwise);

            var cardRightBottom = new Point(CardLabelLength, CardHeight);

            ctx.LineTo(cardRightBottom);

            var rightTop = new Point(b.Right, CardHeight);
            
            ctx.LineTo(rightTop);
            
            ctx.LineTo(new Point(b.Right, b.Height));
            
            ctx.LineTo(new Point(b.Left, b.Height));
        }
    }
}