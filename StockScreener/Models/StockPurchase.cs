using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StockScreener.Models
{
    public class StockPurchase
    {
        public int Id { get; set; }
        public double BoughtAt { get; set; }
        public string StockIndex { get; set; }
        public double SharesQuantity { get; set; }
        public string UserName { get; set; }
        public double WinLoss { get; set; }
    }
}
