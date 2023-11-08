using Project_K.ViewModel;

namespace Project_K.View;

public partial class ProductCreationPage : ContentPage
{
	public ProductCreationPage(ProductCreationViewModel productCreationViewModel)
	{
		InitializeComponent();

		BindingContext = productCreationViewModel;
	}
}