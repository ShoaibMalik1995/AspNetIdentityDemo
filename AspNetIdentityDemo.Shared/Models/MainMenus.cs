using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetIdentityDemo.Shared.Models
{
    public class MainMenus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Route { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
