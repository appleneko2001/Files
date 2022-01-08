namespace Files.Views.Converters.Parameters
{
    public class BoolToDoubleConverterParameter
    {
        private double _valueOnFalse;
        private double _valueOnTrue;

        public double ValueOnFalse
        {
            get => _valueOnFalse;
            set => _valueOnFalse = value;
        }

        public double ValueOnTrue
        {
            get => _valueOnTrue;
            set => _valueOnTrue = value;
        }
    }
}