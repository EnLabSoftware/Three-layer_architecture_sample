using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThreeLayerSample.Domain.Entities;
using ThreeLayerSample.Domain.Interfaces.Services;

namespace ThreeLayerSample.Web_Razor_.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWorkService _workService;

        public IList<Work> Works { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, IWorkService workService)
        {
            _logger = logger;
            _workService = workService;
        }

        public async Task OnGetAsync()
        {
            Works = await _workService.GetAll();
        }
    }
}
