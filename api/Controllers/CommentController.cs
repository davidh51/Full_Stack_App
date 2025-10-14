using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment/")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFMPService _fmpService;
        public CommentController(ICommentRepository commentRepo,
                                    IStockRepository stockRepo,
                                    UserManager<AppUser> userManager,
                                    IFMPService fmpService)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
            _userManager = userManager;
            _fmpService = fmpService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] CommentQueryObject queryObject)
        {
            if (!ModelState.IsValid)  //Data validation -> ControllerBase
                return BadRequest(ModelState);

            var comments = await _commentRepo.GetAllAsync(queryObject);
            var commentDto = comments.Select(x => x.ToCommentDto());
            return Ok(commentDto);
        }
        [HttpGet()]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)  //Data validation -> ControllerBase
                return BadRequest(ModelState);

            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }
        [HttpPost]
        //[Route("{stockId:int}")] //then we will check by symbol if stock exists
        [Route("{symbol:alpha}")]
        [Authorize]
        public async Task<IActionResult> Create([FromRoute] string symbol, [FromBody] CreateCommentDto commentDto)
        {
            if (!ModelState.IsValid)  //Data validation -> ControllerBase
                return BadRequest(ModelState);

            var stock = await _stockRepo.GetBySymbolAsync(symbol);
            if (stock == null)//With FMP; auto creates populates stock if not in DB
            {//Check in FMP
                stock = await _fmpService.FindStockBySymbolAsync(symbol);
                if (stock == null)
                    return BadRequest("Stock does not exist");

                await _stockRepo.CreateAsync(stock);
            }
            /*if (!await _stockRepo.StockExists(stockId))
                return BadRequest("Stock not found");*/

            var username = User.GetUsername(); //Extension method retrieving the username from the token claims
            var appUser = await _userManager.FindByNameAsync(username); //added after implementing identity/token

            var commentModel = commentDto.ToCommentFromCreate(stock!.Id);
            commentModel.AppUserId = appUser!.Id;
            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }
        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommenRequestDto updateDto)
        {
            if (!ModelState.IsValid)  //Data validation -> ControllerBase
                return BadRequest(ModelState);

            var commentModel = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdate());
            if (commentModel == null)
            {
                return NotFound("Comment not found");
            }
            return Ok(commentModel.ToCommentDto());
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)  //Data validation -> ControllerBase
                return BadRequest(ModelState);

            var commentModel = await _commentRepo.DeleteAsync(id);
            if (commentModel == null)
            {
                return NotFound("Comment does not exist");
            }
            return NoContent();
        }
    }
}