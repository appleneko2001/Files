using System;
using Avalonia;
using Avalonia.Media;

namespace Files.Views.Converters.Parameters
{
    public class RoundedCornerBrushFactoryParameter
    {
        private CornerRadius _cornerRadius;
        private IBrush _brush;
        private string _reuseId;
        private bool _updateWhenSizeChanged;

        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set => _cornerRadius = value;
        }

        public IBrush Brush
        {
            get => _brush;
            set => _brush = value;
        }

        public string ReuseId
        {
            get => _reuseId;
            set => _reuseId = value;
        }

        public bool UpdateWhenSizeChanged
        {
            get => _updateWhenSizeChanged;
            set => _updateWhenSizeChanged = value;
        }
    }
}