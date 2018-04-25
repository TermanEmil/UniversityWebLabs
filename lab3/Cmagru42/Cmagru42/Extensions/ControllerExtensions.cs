using System;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult RedirectToActionCtrl(
            this Controller ctrl,
            string actionName,
            string ctrlName)
        {
            return ctrl.RedirectToAction(
                actionName, ctrlName.Replace("Controller", null));
        }
    }
}
