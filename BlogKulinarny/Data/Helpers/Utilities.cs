using Microsoft.AspNetCore.Mvc;

namespace BlogKulinarny.Data.Helpers;

public static class Utilities
{
    private static Controller controller;

    public static UriBuilder GetBuilder(Controller controller)
    {
        var uriBuilder = new UriBuilder(controller.Request.Scheme, controller.Request.Host.Host,
            controller.Request.Host.Port ?? -1);
        return uriBuilder;
    }
}