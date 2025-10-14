using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;
        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentModel = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (commentModel == null)
            {
                return null;
            }
            _context.Comments.Remove(commentModel);
            await _context.SaveChangesAsync();    
            return commentModel;
        }

        public async Task<List<Comment>> GetAllAsync(CommentQueryObject queryObject)
        {//Include(LEFT JOIN) to load the related AppUser data along with each Comment, implemented after adding identity at the end
         //return await _context.Comments.Include(a => a.AppUser).ToListAsync();
            var comments = _context.Comments.Include(x => x.AppUser).AsQueryable();

            if (!string.IsNullOrEmpty(queryObject.Symbol))
            {
                comments = comments.Where(x => x.Stock!.Symbol == queryObject.Symbol);
            }
            if (queryObject.IsDescending == true)
            {
                comments = comments.OrderByDescending(x => x.CreatedOn);
            }
            return await comments.ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            //return await _context.Comments.FindAsync(id);    
            return await _context.Comments.Include(a => a.AppUser) //Include to load the related AppUser data along with each Comment,
                                          .FirstOrDefaultAsync(x => x.Id == id);// implemented after adding identity at the end
        }

        public async Task<Comment?> UpdateAsync(int id, Comment commentModel)
        {
            var existingComment = await _context.Comments.FindAsync(id);
            
            if (existingComment == null)
            {
                return null;
            }
            existingComment.Title = commentModel.Title;
            existingComment.Content = commentModel.Content;
            await _context.SaveChangesAsync();
            return existingComment;
        }
    }
}