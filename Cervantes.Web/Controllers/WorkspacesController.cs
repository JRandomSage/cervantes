﻿using Cervantes.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using Cervantes.CORE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Cervantes.Web.Controllers;
[Authorize(Roles = "Admin,SuperUser,User")]
public class WorkspacesController : Controller
{
    private readonly ILogger<WorkspacesController> _logger = null;
    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;

    public WorkspacesController(IProjectManager projectManager, IProjectUserManager projectUserManager,
        ILogger<WorkspacesController> logger)
    {
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        _logger = logger;
    }

    // GET: WorkspaceController
    public IActionResult Index()
    {
        try
        {
            if ((User.FindFirstValue(ClaimTypes.Role) == "Admin") |
                (User.FindFirstValue(ClaimTypes.Role) == "SuperUser"))
            {
                var model = projectManager.GetAll().Where(x => x.Template == false && x.Status == ProjectStatus.Active).ToList();

                return View(model);
            }
            else
            {
                var projects = projectUserManager.GetAll().Where(x => x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).Select(y => y.ProjectId).ToList();
                var model = projectManager.GetAll().Where(x => projects.Contains(x.Id) && x.Status == ProjectStatus.Active).ToList();
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred loading My Workspaces. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }
}