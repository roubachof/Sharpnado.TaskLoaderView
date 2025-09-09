using System.Runtime.CompilerServices;

namespace Sample.Views;

public partial class ErrorEmulatorView : ContentView
{
    public static readonly BindableProperty LabelFontFamilyProperty = BindableProperty.Create(
        nameof(LabelFontFamily),
        typeof(string),
        typeof(ErrorEmulatorView));

    public static readonly BindableProperty LabelColorProperty = BindableProperty.Create(
        nameof(LabelColor),
        typeof(Color),
        typeof(ErrorEmulatorView));

    public static readonly BindableProperty ValueFontFamilyProperty = BindableProperty.Create(
        nameof(ValueFontFamily),
        typeof(string),
        typeof(ErrorEmulatorView));

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor),
        typeof(Color),
        typeof(ErrorEmulatorView));

    public ErrorEmulatorView()
    {
        InitializeComponent();
    }

    public string LabelFontFamily
    {
        get => (string)GetValue(LabelFontFamilyProperty);
        set => SetValue(LabelFontFamilyProperty, value);
    }

    public Color LabelColor
    {
        get => (Color)GetValue(LabelColorProperty);
        set => SetValue(LabelColorProperty, value);
    }

    public string ValueFontFamily
    {
        get => (string)GetValue(ValueFontFamilyProperty);
        set => SetValue(ValueFontFamilyProperty, value);
    }

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(LabelFontFamily):
                Label.FontFamily = LabelFontFamily;
                break;
            case nameof(LabelColor):
                Label.TextColor = LabelColor;
                break;
            case nameof(ValueFontFamily):
                Picker.FontFamily = ValueFontFamily;
                break;
            case nameof(AccentColor):
                Picker.TextColor = AccentColor;
                break;
        }
    }
}