namespace BooksManagementSrv.Mapper
{
        public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            // Mapper per UserDetails
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Author, ReadAuthorDTO>().ReverseMap();
            CreateMap<AuthorDTO, ReadAuthorDTO>().ReverseMap();
            

            // Mapper per Book
            CreateMap<CreateBookDTO, Book>();
            CreateMap<Book, ReadBookDTO>()
            .ForMember(o => o.Genre, opt => opt.MapFrom(book => Enum.GetName(typeof(Genre), book.Genre)))
            .ForMember(o => o.Author, opt => opt.MapFrom(source => source.Author));
        }
    }
}