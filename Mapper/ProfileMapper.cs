namespace AuthSrv.Mapper
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            // Mapper per User
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, ReadUserDTO>();
            CreateMap<UserDTO, ReadUserDTO>();

            // Mapper per UserDetails
            CreateMap<UserDetails, UserDetailsDTO>().ReverseMap();
            CreateMap<UserDetails, ReadUserDetailsDTO>()
            .ForMember(o => o.FavouriteGenre, opt => opt.MapFrom(model => Enum.GetName(typeof(Genre), model.FavouriteGenre)));
            //.ForMember(o => o.FavouriteGenre, opt => opt.MapFrom(g => Enum.GetName(typeof(Genre), g.FavouriteGenre)));
            CreateMap<CreateUserDetailsDTO, UserDetailsDTO>();
            CreateMap<UserDetailsDTO, ReadUserDetailsDTO>();
            
        }
    }
}