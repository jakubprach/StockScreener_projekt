using System;
using System.Collections.Generic;
using System.Linq;
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

        // GET: StockPurchases
        public async Task<IActionResult> Index()
        {
            
            string userName = HttpContext.User.Identity.Name;
            foreach (var cos in _context.StockPurchase.Where(s => s.UserName == userName))
            {
                var securities = await Yahoo.Symbols(cos.StockIndex).Fields(Field.Symbol, Field.RegularMarketPrice, Field.FiftyTwoWeekHigh).QueryAsync();
                var stock = securities[cos.StockIndex];
                var price = (stock[Field.RegularMarketPrice] * cos.SharesQuantity) - (cos.SharesQuantity * cos.BoughtAt); // or, you could use aapl.RegularMarketPrice directly for typed-value
                cos.WinLoss = price;
            }
          
            return View(await _context.StockPurchase.Where(s => s.UserName == userName)
                                      .ToListAsync());
        }

        // GET: StockPurchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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

        // GET: StockPurchases/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StockPurchases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BoughtAt,StockIndex,SharesQuantity,UserName")] StockPurchase stockPurchase)
        {
            string userName = HttpContext.User.Identity.Name;
            if (ModelState.IsValid)
            {
                var securities = await Yahoo.Symbols(stockPurchase.StockIndex).Fields(Field.Symbol, Field.RegularMarketPrice, Field.FiftyTwoWeekHigh).QueryAsync();
                stockPurchase.UserName = userName;
                stockPurchase.WinLoss = 0;
                if (securities.ContainsKey(stockPurchase.StockIndex))
                {
                    _context.Add(stockPurchase);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(stockPurchase);
        }

        // GET: StockPurchases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
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

        // POST: StockPurchases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BoughtAt,StockIndex,SharesQuantity,UserName")] StockPurchase stockPurchase)
        {
            if (id != stockPurchase.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

        // GET: StockPurchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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

        // POST: StockPurchases/Delete/5
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
