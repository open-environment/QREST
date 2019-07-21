using System.Linq;
using System.Web.Mvc;

namespace QREST.App_Logic.BusinessLogicLayer
{
    public static class HTMLHelpers
    {
        public static string ActivePage(this HtmlHelper helper, string controller, string actions)
        {
            string currentController = helper.ViewContext.RouteData.Values["Controller"].ToString();
            string currentAction = helper.ViewContext.RouteData.Values["Action"].ToString();
            string[] acceptedActions = actions.Trim().Split(',').Distinct().ToArray();
            if (currentController == controller && acceptedActions.Contains(currentAction))
                return "active open";
            else
                return "";
        }
    }
}