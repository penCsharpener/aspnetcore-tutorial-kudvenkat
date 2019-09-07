using kudvenkat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kudvenkat.ViewModels {
    public class HomeDetailsViewModel {

        public HomeDetailsViewModel() {

        }

        public string PageTitle { get; set; }
        public Employee Employee { get; set; }
    }
}
