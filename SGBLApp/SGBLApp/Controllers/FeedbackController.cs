using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Infraestructure.Persistence.Context;

[Authorize]
public class FeedbackController : Controller
{
    private readonly IBookFeedbackService _feedbackService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationContext _context;

    public FeedbackController(
        IBookFeedbackService feedbackService,
        UserManager<ApplicationUser> userManager,
        ApplicationContext context)
    {
        _feedbackService = feedbackService;
        _userManager = userManager;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Vote([FromBody] FeedbackVoteModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var bookExists = await _context.Books.AnyAsync(b => b.BookId == model.BookId);
        if (!bookExists)
            return BadRequest("El libro no existe.");

        await _feedbackService.GiveFeedbackAsync(user.Id, model.BookId, model.IsLiked);
        return Ok();
    }
}

public class FeedbackVoteModel
{
    public int BookId { get; set; }
    public bool IsLiked { get; set; }
}
