using AutoMapper;
using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.DTOs.Reservation;
using SGBLApp.Core.Domain.Entities;

namespace SGBLApp.Core.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<AuthorDto, Author>().ReverseMap();
            CreateMap<GenreDto, Genre>().ReverseMap();
            CreateMap<Library, LibraryDto>().ReverseMap();
            CreateMap<Book, BookDto>().ReverseMap();

            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));



            CreateMap<Book, BookDetailDto>()
                .IncludeBase<Book, BookDto>()
                .ReverseMap();
        }

    }
}
