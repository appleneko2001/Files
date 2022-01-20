using Avalonia;

namespace Files.Views.Converters.Parameters
{
    public class CountToWidthConverterParameter
    {
        private double _minWidth;
        private double _maxWidth;
        private Thickness? _marginOffset;

        public double MinWidth
        {
            get => _minWidth;
            set => _minWidth = value;
        }

        public double MaxWidth
        {
            get => _maxWidth;
            set => _maxWidth = value;
        }

        public Thickness? MarginOffset
        {
            get => _marginOffset;
            set => _marginOffset = value;
        }
    }
}