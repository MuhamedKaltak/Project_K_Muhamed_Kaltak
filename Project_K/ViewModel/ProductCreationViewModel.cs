﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project_K.Enums;
using Project_K.Model;
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
        [ObservableProperty]
        bool hasSelectedCategory;

        BoolReference currentProductCategorySelected = new BoolReference();
        BoolReference currentProductItemSelected = new BoolReference();

        //Product category UI databinding
        [ObservableProperty]
        BoolReference hasSelectedVehicleCategory = new BoolReference();
        [ObservableProperty]
        BoolReference hasSelectedElectronicCategory = new BoolReference();

        //Product item UI databinding
        //Vehicle
        [ObservableProperty]
        BoolReference hasSelectedCarItem = new BoolReference();
        [ObservableProperty]
        BoolReference hasSelectedMotorcycleItem = new BoolReference();
        //Electronics
        [ObservableProperty]
        BoolReference hasSelectedComputerItem = new BoolReference();
        [ObservableProperty]
        BoolReference hasSelectedPhoneItem = new BoolReference();

        [ObservableProperty]
        int productCategoryIndex = -1;
        
        [ObservableProperty]
        int productItemIndex = -1;

        //[ObservableProperty]
        //ProductCategoryEnum[] categoryArray = (ProductCategoryEnum[])Enum.GetValues(typeof(ProductCategoryEnum));

        private readonly Dictionary<ProductCategoryEnum, List<ProductItemEnum>> CategoryItemMappingDictionary;

        private readonly Dictionary<ProductCategoryEnum, BoolReference> CategoryFieldMappingDictionary;
        private readonly Dictionary<ProductItemEnum, BoolReference> ProductFieldMappingDictionary;

        public ObservableCollection<ProductCategoryEnum> ProductCategories { get; set; } = new ObservableCollection<ProductCategoryEnum>(Enum.GetValues(typeof(ProductCategoryEnum)).Cast<ProductCategoryEnum>());

        public ObservableCollection<ProductItemEnum> ProductItems { get; set; } = new ObservableCollection<ProductItemEnum>();



        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public byte[] Picture { get; set; }
        public string Discriminator { get; set; }


        public ProductCreationViewModel()
        {
            CategoryItemMappingDictionary = new Dictionary<ProductCategoryEnum, List<ProductItemEnum>>
            {
                { ProductCategoryEnum.Electronic, new List<ProductItemEnum> { ProductItemEnum.Computer, ProductItemEnum.Phone} },
                { ProductCategoryEnum.Vehicle, new List<ProductItemEnum> { ProductItemEnum.Car, ProductItemEnum.Motorcycle } },
            };


            CategoryFieldMappingDictionary = new Dictionary<ProductCategoryEnum, BoolReference>
            {
                { ProductCategoryEnum.Electronic, hasSelectedElectronicCategory },
                { ProductCategoryEnum.Vehicle, hasSelectedVehicleCategory },
            };

            ProductFieldMappingDictionary = new Dictionary<ProductItemEnum, BoolReference>
            {
                { ProductItemEnum.Car, hasSelectedCarItem },
                { ProductItemEnum.Motorcycle, hasSelectedMotorcycleItem },
                { ProductItemEnum.Computer, hasSelectedComputerItem },
                { ProductItemEnum.Phone, hasSelectedPhoneItem },
            };
        }


        [RelayCommand]
        public void ProcessProductCategory()
        {

            if (ProductCategoryIndex == -1)
                return;


            HasSelectedCategory = true;

            ProductItems.Clear();

            currentProductCategorySelected.Value = false;
            currentProductItemSelected.Value = false;

            if (CategoryItemMappingDictionary.TryGetValue(ProductCategories[ProductCategoryIndex], out var items))
            {
                foreach (var item in items)
                {
                    ProductItems.Add(item);
                }
            }

            if (CategoryFieldMappingDictionary.ContainsKey(ProductCategories[ProductCategoryIndex]))
            {
                currentProductCategorySelected = CategoryFieldMappingDictionary[ProductCategories[ProductCategoryIndex]];
            }

        }


        [RelayCommand]
        public void ProcessProductItem()
        {
            if (ProductItemIndex == -1)
                return;

            currentProductItemSelected.Value = false;

            if (ProductFieldMappingDictionary.ContainsKey(ProductItems[ProductItemIndex]))
            {
                ProductFieldMappingDictionary[ProductItems[ProductItemIndex]].Value = true;

                currentProductCategorySelected.Value = true;

                currentProductItemSelected = ProductFieldMappingDictionary[ProductItems[ProductItemIndex]];

                //OnPropertyChanged(nameof(ProductFieldMappingDictionary[ProductItems[ProductItemIndex]]));
            }
        }
    }
}
