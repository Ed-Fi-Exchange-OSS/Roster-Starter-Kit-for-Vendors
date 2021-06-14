using System;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Roster.ChangeQueries.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateApiConnectionAttribute : TypeFilterAttribute
    {
        public ValidateApiConnectionAttribute()
            : base(typeof(ValidateApiConnectionFilter))
        {

        }
    }
}
