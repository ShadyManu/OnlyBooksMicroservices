namespace BooksManagementSrv.Services.Interfaces
{
    public interface IBookService
    {
        Task<ServiceResponse<ReadBookDTO>> InsertBook(CreateBookDTO bookToCreate, string token);
        Task<ServiceResponse<List<ReadBookDTO>>> ReadAllBook();
        Task<ServiceResponse<ReadBookDTO>> ReadBook(int idBook);
        Task<ServiceResponse<List<ReadBookDTO>>> ReadBooksByAuthor(int idAuthor);
        Task<ServiceResponse<List<ReadBookDTO>>> ReadBooksByGenre(Genre genre);
        Task<ServiceResponse<bool>> DeleteMyBook(int idBook, string bearerToken);
        Task<ServiceResponse<ReadBookDTO>> UpdateMyBook(UpdateBookDTO updateBookDTO, string bearerToken);
    }
}