// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
  [SecurityHeaders]
  [Authorize]
  public class DiagnosticsController : Controller
  {
    public async Task<IActionResult> Index()
    {
      var model = new DiagnosticsViewModel(await HttpContext.AuthenticateAsync());
      return View(model);
    }
  }
}