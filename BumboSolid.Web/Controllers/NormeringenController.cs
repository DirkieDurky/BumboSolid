﻿using BumboSolid.Data;
using BumboSolid.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Web.Controllers
{
    public class NormeringenController : Controller
    {
        private readonly BumboDbContext _context;

        public NormeringenController(BumboDbContext context)
        {
            _context = context;
        }

        // GET: Normeringen/Index/5
        public ActionResult Index()
        {
            var normList = _context.Norms.ToList();
            return View(normList);
        }

        // GET: Normeringen/Aanmaken
        public ActionResult Aanmaken()
        {
            ViewBag.Function = new SelectList(new List<string> { "Vers", "Kassa", "Vakkenvullen" });
            ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });

            return View(new Norm());
        }

        // POST: Normeringen/Aanmaken
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aanmaken(Norm norm, string DurationUnit)
        {
            if (ModelState.IsValid)
            {
                // Convert Duration to seconds based on the selected DurationUnit
                switch (DurationUnit)
                {
                    case "minutes":
                        norm.Duration *= 60;
                        break;
                    case "hours":
                        norm.Duration *= 3600;
                        break;
                }

                _context.Norms.Add(norm);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Function = new SelectList(new List<string> { "Vers", "Kassa", "Vakkenvullen" });
            ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });
            return View(norm);
        }


        // POST: Normeringen/Bewerken/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bewerken(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Normeringen/Verwijderen/5
        public ActionResult Verwijderen(int id)
        {
            return View();
        }

        // POST: Normeringen/Verwijderen/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verwijderen(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}