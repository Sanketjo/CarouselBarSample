using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarouselBarSample
{
    public class CarouselButton : BindableObject
    {
        public string Title { get; set; }

        public static readonly BindableProperty SelectionCommandProperty =
            BindableProperty.Create(nameof(SelectionCommand),
                                    typeof(ICommand),
                                    typeof(CarouselButton),
                                    default(ICommand));

        public ICommand SelectionCommand
        {
            get { return (ICommand)GetValue(SelectionCommandProperty); }
            set { SetValue(SelectionCommandProperty, value); }
        }
    }

    public class CarouselBar : ScrollView
    {
        private readonly StackLayout _stack;

        public IList<CarouselButton> Buttons { get; set; }

        public IList<View> Children
        {
            get
            {
                return _stack.Children;
            }
        }

        public double Spacing
        {
            get
            {
                return _stack.Spacing;
            }
        }

        public static readonly BindableProperty SelectedTextColorProperty =
            BindableProperty.Create(nameof(SelectedTextColor),
                                    typeof(Color),
                                    typeof(CarouselBar),
                                    default(Color));

        public static readonly BindableProperty UnselectedTextColorProperty =
            BindableProperty.Create(nameof(UnselectedTextColor),
                                    typeof(Color),
                                    typeof(CarouselBar),
                                    default(Color));

        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create(nameof(SelectedIndex),
                                    typeof(int),
                                    typeof(CarouselBar),
                                    0,
                                    BindingMode.TwoWay,
                                    propertyChanged: (bindable, oldValue, newValue) =>
                                    {
                                        ((CarouselBar)bindable).OnSelectedIndexChanged(oldValue, newValue);
                                    });

        public bool IsScrollEnable { get; private set; }

        #region bindable properties
        public Color SelectedTextColor
        {
            get { return (Color)GetValue(SelectedTextColorProperty); }
            set { SetValue(SelectedTextColorProperty, value); }
        }

        public Color UnselectedTextColor
        {
            get { return (Color)GetValue(UnselectedTextColorProperty); }
            set { SetValue(UnselectedTextColorProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        } 
        #endregion

        public CarouselBar()
        {
            Orientation = ScrollOrientation.Horizontal;

            _stack = new StackLayout
            {
                VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false),
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,
                Padding = new Thickness(20, 0, 0, 0)
            };

            Content = _stack;

            var buttons = new ObservableCollection<CarouselButton>();
            buttons.CollectionChanged += OnButtonsChanged;
            Buttons = buttons;
        }

        private void OnButtonsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _stack.Children.Clear();
            foreach (var item in Buttons)
            {
                var label = new Label
                {
                    Text = item.Title,
                    TextColor = UnselectedTextColor
                };

                _stack.Children.Add(label);

                label.GestureRecognizers.Add(
                    new TapGestureRecognizer
                    {
                        Command = new Command(() =>
                        {
                            SelectedIndex = _stack.Children.IndexOf(label);
                            if (item.SelectionCommand != null && item.SelectionCommand.CanExecute(SelectedIndex))
                            {
                                item.SelectionCommand.Execute(SelectedIndex);
                            }
                        })
                    });
            }

            OnSelectedIndexChanged(0, 0);
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var size = _stack.Measure(widthConstraint, heightConstraint, MeasureFlags.None);

            IsScrollEnable = size.Request.Width > widthConstraint;

            return base.OnMeasure(widthConstraint, heightConstraint);
        }

        private void OnSelectedIndexChanged(object oldValue, object newValue)
        {
            var oldIndex = (int)oldValue;
            var newIndex = (int)newValue;

            var unselectedLabel = (Label)_stack.Children[oldIndex];
            var selectedLabel = (Label)_stack.Children[newIndex];

            unselectedLabel.TextColor = UnselectedTextColor;
            selectedLabel.TextColor = SelectedTextColor;
        }
    }
}
