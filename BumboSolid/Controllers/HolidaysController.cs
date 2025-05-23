﻿using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Controllers;

[Authorize(Roles = "Manager")]
[Route("Feestdagen")]
public class HolidaysController : Controller
{
    private readonly BumboDbContext _context;

    public HolidaysController(BumboDbContext context)
    {
        _context = context;
    }

    // GET: FeestdagenController
    [HttpGet("")]
    public ActionResult Index()
    {
        List<HolidayViewModel> holidayViewModels = new List<HolidayViewModel>();

        foreach (Holiday holiday in _context.Holidays.Include(x => x.HolidayDays).ToList())
        {
            List<HolidayDay> holidayDays = holiday.HolidayDays.ToList();

            DateOnly firstDay = holidayDays[0].Date;
            DateOnly lastDay = holidayDays[holidayDays.Count() - 1].Date;

            HolidayViewModel holidayViewModel = new HolidayViewModel() { Name = holiday.Name, FirstDay = firstDay, LastDay = lastDay };

            holidayViewModels.Add(holidayViewModel);
        }

        return View(holidayViewModels);
    }

    // GET: FeestdagenController/Details/5
    [HttpGet("Details/{id:int}")]
    public ActionResult Details(int id)
    {
        return View();
    }

    // GET: FeestdagenController/Aanmaken
    [HttpGet("Aanmaken")]
    public ActionResult Create()
    {
        return View(new HolidayViewModel());
    }

    // POST: FeestdagenController/Aanmaken
    [ValidateAntiForgeryToken]
    [HttpPost("Aanmaken")]
    public ActionResult Create(HolidayViewModel holidayViewModel)
    {
		// Making sure that the Holiday does not already exist
		foreach (Holiday holiday in _context.Holidays)
		{
			if (holiday.Name.Equals(holidayViewModel.Name))
			{
				ModelState.AddModelError("Name", "Er bestaat al een feestdag met deze naam");
                break;
			}
		}
		if (!ModelState.IsValid) return View(holidayViewModel);

        // Check if the model state is still valid before saving to the database
        if (ModelState.IsValid)
        {
			Holiday holiday = new();
			holiday.Name = holidayViewModel.Name;

			// Adding HolidayDay for every day the holiday is active
			for (int i = 0; i <= holidayViewModel.LastDay.DayNumber - holidayViewModel.FirstDay.DayNumber; i++)
			{
				HolidayDay holidayDay = new HolidayDay();
				holidayDay.HolidayName = holidayViewModel.Name;
				holidayDay.Date = holidayViewModel.FirstDay.AddDays(i);
				holidayDay.Impact = 0;
				holidayDay.HolidayNameNavigation = holiday;
				holiday.HolidayDays.Add(holidayDay);
			}

			_context.Holidays.Add(holiday);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        return View(holidayViewModel);
    }

    // GET: FeestdagenController/Bewerken/5
    [HttpGet("Bewerken/{id}")]
    public ActionResult Edit(String id)
    {
        Holiday holiday = _context.Holidays.Include(x => x.HolidayDays).First(h => h.Name == id);
        List<HolidayDay> holidayDays = holiday.HolidayDays;

        HolidayManageViewModel holidayManageViewModel = new HolidayManageViewModel()
        {
            Holiday = holiday,
            FirstDay = holidayDays[0].Date,
            LastDay = holidayDays[holidayDays.Count() - 1].Date,
        };

        holidayManageViewModel = CreateGraph(holidayManageViewModel);

        return View(holidayManageViewModel);
    }

    // POST: FeestdagenController/Bewerken/5
    [ValidateAntiForgeryToken]
    [HttpPost("Bewerken/{id}")]
    public async Task<IActionResult> Edit(HolidayManageViewModel holidayManageViewModel, DateOnly firstDay, DateOnly lastDay)
    {
        ModelState.Remove("");
		var holiday = holidayManageViewModel.Holiday;

        int firstDayDifference = firstDay.DayNumber - holiday.HolidayDays[0].Date.DayNumber;
        int lastDayDifference = lastDay.DayNumber - holiday.HolidayDays[holiday.HolidayDays.Count() - 1].Date.DayNumber;

        if (firstDayDifference != 0 && lastDayDifference != 0)
        {
            ModelState.AddModelError(String.Empty, "Je mag maar één datum tegelijkertijd aanpassen");
            holidayManageViewModel = CreateGraph(holidayManageViewModel);
            return View(holidayManageViewModel);
        }

        if (!ModelState.IsValid)
        {
            holidayManageViewModel = CreateGraph(holidayManageViewModel);
            return View(holidayManageViewModel);
        }

        bool changedDates = false;

        _context.Holidays.Update(holiday);

        // Add or Remove HolidayDays if neccesary
        // Adding days before
        if (firstDayDifference < 0)
        {
            for (int i = 0; i < Math.Abs(firstDayDifference); i++)
            {
                HolidayDay holidayDay = new HolidayDay();
                holidayDay.Date = holiday.HolidayDays[0].Date.AddDays(-i - 1);
                holidayDay.Impact = 0;
                holidayDay.HolidayName = holiday.Name;

                _context.HolidayDays.Add(holidayDay);
            }

            holidayManageViewModel.LastDay = lastDay;
            changedDates = true;
        }

        // Removing days before
        if (firstDayDifference > 0)
        {
            for (int i = 0; i < firstDayDifference; i++)
            {
                HolidayDay holidayDay = holiday.HolidayDays[i];

                _context.HolidayDays.Remove(holidayDay);
            }

            holidayManageViewModel.LastDay = lastDay;
            changedDates = true;
        }

        // Adding days after
        if (lastDayDifference > 0)
        {
            int holidayDays = holiday.HolidayDays.Count() - 1;

            for (int i = 0; i < Math.Abs(lastDayDifference); i++)
            {
                HolidayDay holidayDay = new HolidayDay();
                holidayDay.Date = holiday.HolidayDays[holidayDays].Date.AddDays(i + 1);
                holidayDay.Impact = 0;
                holidayDay.HolidayName = holiday.Name;

                _context.HolidayDays.Add(holidayDay);
            }

            holidayManageViewModel.LastDay = lastDay;
            changedDates = true;
        }

        // Removing days after
        if (lastDayDifference < 0)
        {
            int holidayDays = holiday.HolidayDays.Count();

            for (int i = 0; i < Math.Abs(lastDayDifference); i++)
            {
                HolidayDay holidayDay = holiday.HolidayDays[holidayDays - 1 - i];

                _context.HolidayDays.Remove(holidayDay);
            }

            holidayManageViewModel.LastDay = lastDay;
            changedDates = true;
        }

        await _context.SaveChangesAsync();

        // If dates have been changed the user goes back to the index to make sure the new HolidayDays are handled correctly
        if (changedDates == true) return RedirectToAction(nameof(Index));
        else
        {
            holidayManageViewModel = CreateGraph(holidayManageViewModel);
            return View(holidayManageViewModel);
        }
    }

    // GET: FeestdagenController/Verwijderen/5
    [HttpGet("Verwijderen/{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        if (name == null)
        {
            return NotFound();
        }

        var holiday = await _context.Holidays.FirstOrDefaultAsync(h => h.Name == name);
        if (holiday == null)
        {
            return NotFound();
        }

        return View(holiday);
    }

    // POST: FeestdagenController/Verwijderen/5
    [ActionName("Verwijderen")]
    [ValidateAntiForgeryToken]
    [HttpPost("Verwijderen/{name}")]
    public async Task<IActionResult> DeleteConfirmed(String name)
    {
        var holiday = await _context.Holidays.Include(h => h.HolidayDays).FirstOrDefaultAsync(h => h.Name == name);

        if (holiday != null)
        {
            foreach (HolidayDay holidayDay in holiday.HolidayDays)
            {
                _context.HolidayDays.Remove(holidayDay);
            }

            _context.Holidays.Remove(holiday);
            await _context.SaveChangesAsync();

        }
        return RedirectToAction(nameof(Index));
    }

    // Create the graph requried for Edit
    public HolidayManageViewModel CreateGraph(HolidayManageViewModel holidayManageViewModel)
    {
        Holiday holiday = holidayManageViewModel.Holiday;

        if (holidayManageViewModel != null)
        {
            foreach (HolidayDay holidayDay in holiday.HolidayDays)
            {
                holidayManageViewModel.xValues.Add(holidayDay.Date.Day + "-" + holidayDay.Date.Month);
                holidayManageViewModel.yValues.Add(holidayDay.Impact);

                if (holidayManageViewModel.HighestImpact < holidayDay.Impact) holidayManageViewModel.HighestImpact = holidayDay.Impact;
                else if (holidayManageViewModel.LowestImpact > holidayDay.Impact) holidayManageViewModel.LowestImpact = holidayDay.Impact;
            }
        }

        return holidayManageViewModel;
    }
}
