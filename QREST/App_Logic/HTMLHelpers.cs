﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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


        public static MvcHtmlString LabelForRequired<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string translatedlabelText, object htmlAttributes)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metaData.DisplayName ?? metaData.PropertyName ?? htmlFieldName.Split('.').Last();

            if (metaData.IsRequired)
                labelText = translatedlabelText + "<span class=\"required\" style=\"color:#ac2925;\"> <span style=\"vertical-align: super;\" class=\"fa fa-asterisk\" data-unicode=\"270f\"></span></span>";
            else
                labelText = translatedlabelText;

            if (String.IsNullOrEmpty(labelText))
                return MvcHtmlString.Empty;

            var label = new TagBuilder("label");
            label.Attributes.Add("for", helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(htmlAttributes))
                label.MergeAttribute(prop.Name.Replace('_', '-'), prop.GetValue(htmlAttributes).ToString(), true);

            label.InnerHtml = labelText;
            return MvcHtmlString.Create(label.ToString());
        }

    }
}