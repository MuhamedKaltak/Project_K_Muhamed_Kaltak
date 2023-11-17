using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project_K.Enums;
using Project_K.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class ProductCreationViewModel : BaseViewModel
    {
        ProductFactoryService productFactoryService;

        [ObservableProperty]
        bool hasSelectedCategory;

        [ObservableProperty]
        int productCategoryIndex = -1;

        //[ObservableProperty]
        //ProductCategoryEnum[] categoryArray = (ProductCategoryEnum[])Enum.GetValues(typeof(ProductCategoryEnum));

        public ObservableCollection<ProductCategoryEnum> ProductCategories { get; set; } = new ObservableCollection<ProductCategoryEnum>(Enum.GetValues(typeof(ProductCategoryEnum)).Cast<ProductCategoryEnum>());

        public ObservableCollection<ProductItemEnum> ProductItems { get; set; } = new ObservableCollection<ProductItemEnum>();



        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public byte[] Picture { get; set; }
        public string Discriminator { get; set; }


        public ProductCreationViewModel(ProductFactoryService productFactoryService)
        {
            this.productFactoryService = productFactoryService;
        }


        [RelayCommand]
        public void ProcessProductCategory()
        {
           HasSelectedCategory = true;

           productFactoryService.GetProductItemsFromCategory(ProductCategories[ProductCategoryIndex], ProductItems);

        }

    }
}
