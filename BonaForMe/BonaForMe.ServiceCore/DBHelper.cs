using BonaForMe.DomainCore.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonaForMe.ServiceCore
{
    public class DBHelper
    {
        public static void SetBaseValues(IBaseEntity oldModel, IBaseEntity newModel)
        {
            newModel.Id = oldModel.Id;
            newModel.DateCreated = oldModel.DateCreated;
            newModel.DateModified = oldModel.DateModified;
            newModel.UserModified = oldModel.UserModified;
            newModel.UserCreated = oldModel.UserCreated;
            newModel.IsActive = oldModel.IsActive;
            newModel.IsDeleted = oldModel.IsDeleted;
        }
        public static void SetBaseValuesInt(IBaseEntityInt oldModel, IBaseEntityInt newModel)
        {
            newModel.Id = oldModel.Id;
            newModel.DateCreated = oldModel.DateCreated;
            newModel.DateModified = oldModel.DateModified;
            newModel.UserModified = oldModel.UserModified;
            newModel.UserCreated = oldModel.UserCreated;
            newModel.IsActive = oldModel.IsActive;
            newModel.IsDeleted = oldModel.IsDeleted;
        }
    }
}
