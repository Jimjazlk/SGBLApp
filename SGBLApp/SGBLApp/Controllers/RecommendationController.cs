using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.Interfaces;
using System.Security.Claims;

namespace SGBLApp.Controllers
{
    [Authorize(Policy = "UserOnly")]
    public class RecommendationController : Controller
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IBookFeedbackService _feedbackService; // ✅ Nuevo servicio
        private readonly IMapper _mapper;

        public RecommendationController(
            IRecommendationService recommendationService,
            IBookFeedbackService feedbackService, // ✅ Inyectado aquí
            IMapper mapper)
        {
            _recommendationService = recommendationService;
            _feedbackService = feedbackService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 📦 Recomendaciones personalizadas y populares
            var personalized = await _recommendationService.GetUserRecommendationsAsync(userId);
            var popular = await _recommendationService.GetPopularBooksAsync(30);

            // 📦 Feedbacks desde BookFeedbacks
            var likedBooks = await _feedbackService.GetLikedBooksAsync(userId);
            var dislikedBooks = await _feedbackService.GetDislikedBooksAsync(userId);

            // Construcción del DTO completo
            var dto = new RecommendationDto
            {
                PersonalizedRecommendations = _mapper.Map<IEnumerable<BookDto>>(personalized),
                PopularRecommendations = _mapper.Map<IEnumerable<BookDto>>(popular),
                LikedBooks = likedBooks,          // ✅ Aquí ya son BookDto
                DislikedBooks = dislikedBooks     // ✅ Aquí también
            };

            return View(dto);
        }
    }
}
