using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarouselBarSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        public ICommand SomeCommand { get; private set; }

        public MainPage()
        {
            SomeCommand = new Command(SomeCommandExecute);

            InitializeComponent();
            BindingContext = this;
        }

        private void SomeCommandExecute(object index)
        {
            Debug.WriteLine("SomeCommand executed");
        }
    }
}
