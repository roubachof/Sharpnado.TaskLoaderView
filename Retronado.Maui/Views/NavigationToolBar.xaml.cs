using System.Runtime.CompilerServices;
using Sample.Infrastructure;
using Sample.Navigation;

using Sharpnado.Tasks;

namespace Sample.Views;

public enum ToolBarTheme
{
    Retro = 0,
    Standard = 1,
}

public partial class NavigationToolBar : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(NavigationToolBar));

    public static readonly BindableProperty ThemeProperty = BindableProperty.Create(
        nameof(Theme),
        typeof(ToolBarTheme),
        typeof(NavigationToolBar));

    private readonly INavigationService _navigationService;

    public NavigationToolBar()
    {
        InitializeComponent();
        _navigationService = DependencyContainer.Instance.GetInstance<INavigationService>();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public ToolBarTheme Theme
    {
        get => (ToolBarTheme)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(Title):
                TitleLabel.Text = Title;
                break;

            case nameof(Theme):
                UpdateTheme();
                break;
        }
    }

    private void UpdateTheme()
    {
        if (Theme == ToolBarTheme.Retro)
        {
            return;
        }

        BackImage.Margin = new Thickness(15);
        BackImage.HorizontalOptions = LayoutOptions.Start;
        BackImage.Aspect = Aspect.AspectFit;
        BackImage.Source = ResourcesHelper.GetResource<FontImageSource>("IconBack");

        BorderImage.IsVisible = false;

        TitleLabel.Margin = new Thickness(0, -5, 0, 0);
        TitleLabel.TextColor = ResourcesHelper.GetResourceColor("TextWhitePrimaryColor");
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        TaskMonitor.Create(_navigationService.NavigateBackAsync());
    }
}