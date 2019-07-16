using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace PowerfulPal.Neeo.ResourceApi.Constraint
{
    public class ValuesConstraint : IHttpRouteConstraint 
    {
        private readonly string[] _valuesOptions;

        public ValuesConstraint(string valuesOptions)
        {
            _valuesOptions = valuesOptions.Split(new [] {'|'});
        }

        public bool Match(System.Net.Http.HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            object value;
            if (values.TryGetValue(parameterName, out value) && value != null)
            {
                return _valuesOptions.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}