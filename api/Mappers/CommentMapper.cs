using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Models;

namespace api.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToCommentDto(this Comment commentModel)
        {
            return new CommentDto
            {
                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                CreatedOn = commentModel.CreatedOn,
                CreatedBy = commentModel.AppUser!.UserName!, // Added after implementing identity at the end
                StockId = commentModel.StockId
            };
        }
        public static Comment ToCommentFromCreate(this CreateCommentDto commentDto, int stockId)
        {
            return new Comment
            {
                Title = commentDto.Tittle,
                Content = commentDto.Content,
                StockId = stockId
            };
        }
        public static Comment ToCommentFromUpdate(this UpdateCommenRequestDto commentDto)
        {
            return new Comment
            {
                Title = commentDto.Tittle,
                Content = commentDto.Content
            };
        }
    }
}