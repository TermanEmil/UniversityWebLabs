using System;
using Microsoft.AspNetCore.Html;

namespace lab2.Views.Shared._NavBar
{
    /// <summary>
    /// A helper class for definind NavBar view functionalities.
    /// </summary>
    public static class NavBar
    {
        /// <summary>
        /// Create a nav button and apply the "active" class depending on the
        /// title.
        /// </summary>
        public static IHtmlContent NewHtmlBtn(
            string title,
            string btnName,
            string url = null)
        {
            if (url == null)
                url = btnName;

            var activeClass = (title.Equals(btnName)) ? "active" : "";
            var result = string.Format("<li class='{0}'><a href='{1}'>{2}" +
                                       "</a></li>",
                                        @activeClass,
                                        @url,
                                        @btnName);

            return new HtmlString(result);
        }
    }
}
