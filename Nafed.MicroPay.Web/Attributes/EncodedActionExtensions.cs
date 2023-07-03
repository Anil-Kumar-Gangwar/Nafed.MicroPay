using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MicroPay.Web
{
    public static class EncodedActionExtensions
    {
        public static MvcHtmlString EncodedActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            string queryString = string.Empty;
            string htmlAttributesString = string.Empty;
            if (routeValues != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(routeValues);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    if (i > 0)
                    {
                        queryString += "?";
                    }
                    queryString += d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }

            object newRouteValues = new { q = Encrypt(queryString) };

            string url = UrlHelper.GenerateUrl(null, actionName, controllerName, null, null, null, new RouteValueDictionary(newRouteValues), htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, true);
            TagBuilder tagBuilder = new TagBuilder("a")
            {
                InnerHtml = (!String.IsNullOrEmpty(linkText)) ? HttpUtility.HtmlEncode(linkText) : String.Empty

            };
            tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            tagBuilder.MergeAttribute("href", url);


            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        private static string Encrypt(string plainText)
        {
            string key = "@m2qJzeyjXLBK!axPV$Bvg3QUP";
            byte[] EncryptKey = { };
            byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
            EncryptKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByte = Encoding.UTF8.GetBytes(plainText);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, des.CreateEncryptor(EncryptKey, IV), CryptoStreamMode.Write);
            cStream.Write(inputByte, 0, inputByte.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }

        public static IHtmlString EncodedImageActionLink(this HtmlHelper htmlHelper, string linkText, string action, string controller, object routeValues, object htmlAttributes, string imageSrc, bool confirm = false, string datatToggle = "")
        {
            string queryString = string.Empty;
            string htmlAttributesString = string.Empty;
            if (routeValues != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(routeValues);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    if (i > 0)
                    {
                        queryString += "?";
                    }
                    queryString += d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }

            object newRouteValues = new { q = Encrypt(queryString) };

            string url = UrlHelper.GenerateUrl(null, action, controller, null, null, null, new RouteValueDictionary(newRouteValues), htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, true);

            var img = new TagBuilder("img");
            img.Attributes.Add("src", VirtualPathUtility.ToAbsolute(imageSrc));

            TagBuilder tagBuilder = new TagBuilder("a")
            {
                InnerHtml = img.ToString(TagRenderMode.SelfClosing)

            };
            tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            tagBuilder.MergeAttribute("href", url);
            if (confirm)
                tagBuilder.MergeAttribute("data-toggle", "confirm");
            else
            {
                if (!string.IsNullOrEmpty(datatToggle))
                    tagBuilder.MergeAttribute("data-toggle", datatToggle);
            }
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static IHtmlString EncodedImage(this HtmlHelper htmlHelper, string linkText, string action, string controller, object routeValues, object htmlAttributes, string imageSrc)
        {
            string queryString = string.Empty;
            string htmlAttributesString = string.Empty;
            if (routeValues != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(routeValues);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    if (i > 0)
                    {
                        queryString += "?";
                    }
                    queryString += d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }

            object newRouteValues = new { q = Encrypt(queryString) };

            string url = UrlHelper.GenerateUrl(null, action, controller, null, null, null, new RouteValueDictionary(newRouteValues), htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, true);

            var tagBuilder = new TagBuilder("img");
            //img.Attributes.Add("src", VirtualPathUtility.ToAbsolute(imageSrc));

            //TagBuilder tagBuilder = new TagBuilder("a")
            //{
            //    InnerHtml = img.ToString(TagRenderMode.SelfClosing)

            //};
            tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            tagBuilder.MergeAttribute("src", url);

            return MvcHtmlString.Create(tagBuilder.ToString());
        }
    }
}