using Module05Exercise01.Services;
using Module05Exercise01.ViewModel;
namespace Module05Exercise01.View;

public partial class ViewPersonal : ContentPage
{
	public ViewPersonal()
	{
		InitializeComponent();

        var personalViewModel = new PersonalViewModel();
        BindingContext = personalViewModel;
    }

    private async void BackViewPersonal(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}