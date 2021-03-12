using AutoMapper;
using AutoMapper.QueryableExtensions.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Servize.Utility
{
    public class PatchEntities
    {
        public static void PatchEntity<DTO, DBO>(DbContext dbContext, IMapper mapper, DBO existingEntity, DTO updatedEntity) where DTO : class where DBO : class
        {
            EntityEntry existingEntry = dbContext.Entry(existingEntity);

            if (existingEntry != null && updatedEntity.GetType().GetProperties() != null)
            {
                var map = mapper.ConfigurationProvider.FindTypeMapFor<DBO, DTO>();

                foreach (PropertyInfo propertyInfo in updatedEntity.GetType().GetProperties())
                {
                    if (propertyInfo == null || propertyInfo.GetValue(updatedEntity) == null)
                        continue;

                    try
                    {
                        PropertyMap propertyMap = map.GetPropertyMapByDestinationProperty(propertyInfo.Name);
                        if (propertyMap == null || String.IsNullOrEmpty(propertyMap.SourceMember.Name))
                            continue;

                        PropertyEntry existingPropEntry = existingEntry.Property(propertyMap.SourceMember.Name);
                        if (existingPropEntry == null || existingPropEntry.CurrentValue == propertyInfo.GetValue(updatedEntity) || existingPropEntry.CurrentValue?.ToString() == propertyInfo.GetValue(updatedEntity)?.ToString())
                            continue;

                        existingPropEntry.CurrentValue = propertyInfo.GetValue(updatedEntity);
                        existingPropEntry.IsModified = true;
                    }
                    catch (Exception ex)
                    {
                        // Mapping not found for a property
                        Log.Error(ex.Message);
                    }
                }
            }
        }
    }
}
