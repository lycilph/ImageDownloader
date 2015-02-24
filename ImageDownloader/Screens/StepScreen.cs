using Caliburn.Micro.ReactiveUI;
using Panda.ApplicationCore.Validation;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;

namespace ImageDownloader.Screens
{
    public abstract class StepScreen : ReactiveScreen, ISupportValidation
    {
        private readonly Validator validator = new Validator();

        public abstract bool CanNext { get; protected set; }
        public abstract bool CanPrevious { get; protected set; }

        private IScreen _Option;
        public IScreen Option
        {
            get { return _Option; }
            set { this.RaiseAndSetIfChanged(ref _Option, value); }
        }

        public string this[string column_name]
        {
            get { return validator.Validate(column_name); }
        }

        public string Error { get { return validator.Error; } }
        
        public void AddValidationRule<S, T>(ValidationRule<S, T> rule)
        {
            validator.AddValidationRule(rule);
        }

        public void RemoveValidationRule(string property)
        {
            validator.RemoveValidationRule(property);
        }
    }
}
