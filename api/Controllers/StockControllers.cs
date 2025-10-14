using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/stock/")]
    [ApiController]
    public class StockControllers : ControllerBase
    {
        private readonly ApplicationDBContext _context; //keeps unmutable the database context
        private readonly IStockRepository _stockRepo;
        public StockControllers(ApplicationDBContext context, IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
            _context = context;
        }
        [HttpGet]
        [Authorize]
        //public IActionResult GetAll()  normal Synchronous method
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query) //Asynchronous method
        {
            if (!ModelState.IsValid)  //Data validation -> ControllerBase
                return BadRequest(ModelState);

            //var stocks = _context.Stocks.ToList()  Synchronous method
            //             .Select(s => s.ToStockDto());
            //var stocks = await _context.Stocks.ToListAsync(); //Without repository&Interface
            //var stocks = await _stockRepo.GetAllAsync(); //With repository&Interface
            var stocks = await _stockRepo.GetAllAsync(query); // Querying
            var stockDTO = stocks.Select(s => s.ToStockDto());

            return Ok(stockDTO);
        }
        [HttpGet()]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)  //Data validation -> ControllerBase
                return BadRequest(ModelState);

            var stock = await _stockRepo.GetByIdAsync(id);

            if (stock == null)
                return NotFound();
            return Ok(stock.ToStockDto());
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            if (!ModelState.IsValid)  //Data validation -> ControllerBase
                return BadRequest(ModelState);

            var stockModel = stockDto.ToStockFromCreateDTO();
            await _stockRepo.CreateAsync(stockModel);
            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto()); //returns already created object using previous endpoint
        }
        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            if (!ModelState.IsValid)  //Data validation -> ControllerBase
                return BadRequest(ModelState);

            var stockModel = await _stockRepo.UpdateAsync(id, updateDto);

            if (stockModel == null)
            {
                return NotFound();
            }
            return Ok(stockModel.ToStockDto());
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)  //Data validation -> ControllerBase
                return BadRequest(ModelState);

            var stockModel = await _stockRepo.DeleteAsync(id);
            if (stockModel == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}