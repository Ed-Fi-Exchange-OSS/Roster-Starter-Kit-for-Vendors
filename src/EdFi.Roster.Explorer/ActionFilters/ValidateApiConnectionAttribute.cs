using System;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Roster.Explorer.ActionFilters
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
