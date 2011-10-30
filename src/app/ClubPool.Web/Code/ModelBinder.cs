using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;

using Iesi.Collections.Generic;

using ClubPool.Core;
using ClubPool.Web.Controllers;

namespace ClubPool.Web.Code
{
  /// <summary>
  /// This custom model binder supports binding EntityViewModelBase collections, using only the id integers. It and
  /// EntityViewModelCollectionValueBinder are mostly copied from the SharpModelBinder in S#arp.
  /// </summary>
  public class ModelBinder : DefaultModelBinder
  {
    protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, 
      PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder) {

      Type propertyType = propertyDescriptor.PropertyType;

      if (IsEntityViewModelCollection(propertyType)) {
        //use the EntityViewModelCollectionValueBinder
        return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, new EntityViewModelCollectionValueBinder());
      }

      return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
    }
  
    private static bool IsEntityViewModelCollection(Type propertyType) {
      bool isSimpleGenericBindableCollection =
          propertyType.IsGenericType &&
          (propertyType.GetGenericTypeDefinition() == typeof(IList<>) ||
           propertyType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
           propertyType.GetGenericTypeDefinition() == typeof(Iesi.Collections.Generic.ISet<>) ||
           propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

      bool isSimpleGenericBindableEntityViewModelCollection =
          isSimpleGenericBindableCollection && propertyType.GetGenericArguments().First().IsSubclassOf(typeof(EntityViewModelBase));

      return isSimpleGenericBindableEntityViewModelCollection;
    }

  }

  public class EntityViewModelCollectionValueBinder : DefaultModelBinder
  {
    #region Implementation of IModelBinder

    /// <summary>
    /// Binds the model to a value by using the specified controller context and binding context.
    /// </summary>
    /// <returns>
    /// The bound value.
    /// </returns>
    /// <param name="controllerContext">The controller context.</param><param name="bindingContext">The binding context.</param>
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
      Type collectionType = bindingContext.ModelType;
      Type collectionEntityType = collectionType.GetGenericArguments().First();

      ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

      if (valueProviderResult != null) {
        int countOfIds = (valueProviderResult.RawValue as string[]).Length;
        Array entityViewModels = Array.CreateInstance(collectionEntityType, countOfIds);

        for (int i = 0; i < countOfIds; i++) {
          string rawId = (valueProviderResult.RawValue as string[])[i];

          int id;

          if (string.IsNullOrEmpty(rawId)) {
            return null;
          }
          else {
            id = Int32.Parse(rawId);
          }

          EntityViewModelBase viewModel = Activator.CreateInstance(collectionEntityType) as EntityViewModelBase;
          viewModel.Id = id;
          entityViewModels.SetValue(viewModel, i);
        }
        return entityViewModels;
      }
      return base.BindModel(controllerContext, bindingContext);
    }

    #endregion
  }
}