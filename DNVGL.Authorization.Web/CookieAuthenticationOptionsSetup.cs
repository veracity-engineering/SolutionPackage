using DNVGL.Authorization.Web.Abstraction;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Authorization.Web
{
    [ObsoleteAttribute("This function is not ready for use. do not use.", true)]
    public class CookieAuthenticationOptionsSetup : IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        private readonly IUserPermissionReader _userPermissionReader;
        private readonly PermissionOptions _permissionOptions;
        public CookieAuthenticationOptionsSetup(IUserPermissionReader userPermissionReader, PermissionOptions permissionOptions)
        {
            _userPermissionReader = userPermissionReader;
            _permissionOptions = permissionOptions;
        }


        public void Configure(string name, CookieAuthenticationOptions options)
        {
            if(options.Events == null)
            {
                options.Events = new CookieAuthenticationEvents();
            }

            options.Events.AddCookieValidateHandler(_userPermissionReader, _permissionOptions);
        }

        public void Configure(CookieAuthenticationOptions options)
        {
            Configure(Options.DefaultName, options);
        }
    }
}
