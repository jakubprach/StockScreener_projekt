using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockScreener.Data;
using StockScreener.Models;
using YahooFinanceApi;

namespace StockScreener.Controllers.Stock
{
    public class StockPurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockPurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult NotLoggedIn()
        {
            return View();
        }

        public IActionResult IndexError()
        {
            return View();
        }
        public IActionResult InputError()
        {
            return View();
        }

        // GET: StockPurchases
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated) { return RedirectToAction("NotLoggedIn"); }
            string userName = HttpContext.User.Identity.Name;
            foreach (var cos in _context.StockPurchase.Where(s => s.UserName == userName))
            {
                var securities = await Yahoo.Symbols(cos.StockIndex).Fields(Field.Symbol, Field.RegularMarketPrice, Field.FiftyTwoWeekHigh).QueryAsync();
                var stock = securities[cos.StockIndex];
                var price = Math.Round((stock[Field.RegularMarketPrice] * cos.SharesQuantity) - (cos.SharesQuantity * cos.BoughtAt),2);
                cos.WinLoss = price;
            }
          
            return View(await _context.StockPurchase.Where(s => s.UserName == userName)
                                      .ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!User.Identity.IsAuthenticated) { return RedirectToAction("NotLoggedIn"); }
            if (id == null)
            {
                return NotFound();
            }

            var stockPurchase = await _context.StockPurchase
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockPurchase == null)
            {
                return NotFound();
            }

            return View(stockPurchase);
        }

        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated) { return RedirectToAction("NotLoggedIn"); }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BoughtAt,StockIndex,SharesQuantity,UserName")] StockPurchase stockPurchase)
        {
            string userName = HttpContext.User.Identity.Name;
            try
            {
                var securities = await Yahoo.Symbols(stockPurchase.StockIndex).Fields(Field.Symbol, Field.RegularMarketPrice, Field.FiftyTwoWeekHigh).QueryAsync();
                stockPurchase.UserName = userName;
                stockPurchase.WinLoss = 0;
                if (!TryValidateModel(stockPurchase)) { return RedirectToAction("InputError"); }

                if (securities.ContainsKey(stockPurchase.StockIndex))
                    {          
                        _context.Add(stockPurchase);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));                  
                    }
            }
            catch(Exception e)
            {
                return RedirectToAction("IndexError");
            }
            
            return View(stockPurchase);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (!User.Identity.IsAuthenticated) { return RedirectToAction("NotLoggedIn"); }
            if (id == null)
            {
                return NotFound();
            }

            var stockPurchase = await _context.StockPurchase.FindAsync(id);
            if (stockPurchase == null)
            {
                return NotFound();
            }
            return View(stockPurchase);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BoughtAt,StockIndex,SharesQuantity,UserName")] StockPurchase stockPurchase)
        {
            var securities = await Yahoo.Symbols(stockPurchase.StockIndex).Fields(Field.Symbol, Field.RegularMarketPrice, Field.FiftyTwoWeekHigh).QueryAsync();
            string userName = HttpContext.User.Identity.Name;
            if (id != stockPurchase.Id)
            {
                return NotFound();
            }

            if (!TryValidateModel(stockPurchase)) { return RedirectToAction("InputError"); }
            if (securities.ContainsKey(stockPurchase.StockIndex))
            {
                try
                {
                    stockPurchase.UserName = userName;
                    _context.Update(stockPurchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockPurchaseExists(stockPurchase.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(stockPurchase);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!User.Identity.IsAuthenticated) { return RedirectToAction("NotLoggedIn"); }
            if (id == null)
            {
                return NotFound();
            }

            var stockPurchase = await _context.StockPurchase
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockPurchase == null)
            {
                return NotFound();
            }

            return View(stockPurchase);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stockPurchase = await _context.StockPurchase.FindAsync(id);
            _context.StockPurchase.Remove(stockPurchase);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockPurchaseExists(int id)
        {
            return _context.StockPurchase.Any(e => e.Id == id);
        }
    }
}
