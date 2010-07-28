using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;

using ClubPool.Core;

namespace ClubPool.Web.Code
{
  /// <summary>
  /// This custom model binder supports binding EntityDto arrays, using only the id integers. It and the
  /// DtoArrayValueBinder are mostly copied from the SharpModelBinder in S#arp.
  /// </summary>
  public class ModelBinder : DefaultModelBinder
  {
    protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, 
      PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder) {

      Type propertyType = propertyDescriptor.PropertyType;

      if (IsEntityDtoArray(propertyType)) {
        //use the DtoArrayValueBinder
        return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, new DtoArrayValueBinder());
      }

      return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
    }
  
    private static bool IsEntityDtoArray(Type type) {
      return type.IsArray && type.GetElementType().IsSubclassOf(typeof(EntityDto));
    }

  }


  public class DtoArrayValueBinder : DefaultModelBinder
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
      Type collectionDtoType = collectionType.GetElementType();

      ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

      if (valueProviderResult != null) {
        int countOfDtoIds = (valueProviderResult.RawValue as string[]).Length;
        Array dtos = Array.CreateInstance(collectionDtoType, countOfDtoIds);

        for (int i = 0; i < countOfDtoIds; i++) {
          string rawId = (valueProviderResult.RawValue as string[])[i];

          int id;

          if (string.IsNullOrEmpty(rawId)) {
            return null;
          }
          else {
            id = Int32.Parse(rawId);
          }

          EntityDto dto = Activator.CreateInstance(collectionDtoType) as EntityDto;
          dto.Id = id;
          dtos.SetValue(dto, i);
        }
        return dtos;
      }
      return base.BindModel(controllerContext, bindingContext);
    }

    #endregion
  }
}