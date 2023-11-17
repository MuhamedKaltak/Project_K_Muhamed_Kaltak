using Project_K.Enums;
using Project_K.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Services
{
    public class ProductFactoryService
    {
        private readonly Dictionary<ProductCategoryEnum, List<ProductItemEnum>> CategoryItemMapping;

        public ProductFactoryService() 
        {
            CategoryItemMapping = new Dictionary<ProductCategoryEnum, List<ProductItemEnum>>
            {
                { ProductCategoryEnum.Vehicle, new List<ProductItemEnum> { ProductItemEnum.Car, ProductItemEnum.Motorcycle } },
                { ProductCategoryEnum.Test, new List<ProductItemEnum> { ProductItemEnum.Test0, ProductItemEnum.Test1, ProductItemEnum.Test2 } },
            };
        }


        public void GetProductItemsFromCategory(ProductCategoryEnum productCategoryEnum, ObservableCollection<ProductItemEnum> subProducts) //Ingen poäng att köra på seperat thread
        {
            subProducts.Clear();

            if (CategoryItemMapping.TryGetValue(productCategoryEnum, out var items))
            {
                foreach (var item in items)
                {
                    subProducts.Add(item);
                }
            }
            
        }
    }
}
