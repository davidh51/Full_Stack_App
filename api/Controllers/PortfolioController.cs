using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio/")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;
        private readonly IFMPService _fmpService;
        public PortfolioController(UserManager<AppUser> userManager,
                                    IStockRepository stockRepo,
                                    IPortfolioRepository portfolioRepo,
                                    IFMPService fmpService)
        {
            _userManager = userManager;
            _stockRepo = stockRepo;
            _portfolioRepo = portfolioRepo;
            _fmpService = fmpService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUsername(); //Extension method retrieving the username from the token claims
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser!);
            return Ok(userPortfolio);
        }
        [HttpPost]
        [Route("{symbol:alpha}")]
        [Authorize]
        public async Task<IActionResult> AddPortafolio([FromRoute] string symbol)
        {
            var username = User.GetUsername(); //Extension method retrieving the username from the token claims
            var appUser = await _userManager.FindByNameAsync(username);
            var stock = await _stockRepo.GetBySymbolAsync(symbol);

            if (stock == null) // With FMP; auto creates populates stock if not in DB
            {//Check in FMP
                stock = await _fmpService.FindStockBySymbolAsync(symbol);
                if (stock == null)  return BadRequest("Stock does not exist");
                else                await _stockRepo.CreateAsync(stock);
            }
            /*if (stock == null)      return BadRequest("Stock not found");*/

            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser!);
            if (userPortfolio.Any(s => s.Symbol.ToLower() == symbol.ToLower()))
                return BadRequest("Stock already in portfolio");

            var portfolioModel = new Portfolio
            {
                StockId = stock.Id,
                AppUserId = appUser!.Id
            };
            await _portfolioRepo.AddToPortfolio(portfolioModel);
            if (portfolioModel == null)
                return StatusCode(500, "Could not add stock to portfolio");
            else
                return Created();
        }
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio([FromBody] string symbol)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser!);

            var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower());
            if (filteredStock.Count() == 1)
                await _portfolioRepo.DeletePortfolio(appUser!, symbol);
            else
                return BadRequest("Stock not found in portfolio");
            return Ok();
        }
    }
}