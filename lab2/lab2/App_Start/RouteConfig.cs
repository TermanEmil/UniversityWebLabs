using System;
using Microsoft.AspNetCore.Routing;

public class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes)
    {
        //routes.IgnoreRoute();
        routes.MapRoutes();
        routes.MapRoutes();
    }
}
