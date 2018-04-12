using System;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Extensions
{
    public static class _ControllerExtensions
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
