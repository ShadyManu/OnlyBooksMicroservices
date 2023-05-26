using System.Net.Http.Headers;

namespace BooksManagementSrv.Services
{
    public class BookService : IBookService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly HttpClient _http;
        private readonly IConfiguration _configuration;

        public BookService(IMapper mapper, DataContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _mapper = mapper;
            _context = context;
            _http = httpClient;
            _configuration = configuration;
        }


        // Metodo ASYNC che chiama l'altro Microservizio per ottenere l'ID che è nel CLAIM.
        public async Task<ServiceResponse<ReadBookDTO>> InsertBook(CreateBookDTO bookToCreate, string bearerToken)
        {
            ServiceResponse<ReadBookDTO> response = new ServiceResponse<ReadBookDTO>();
            if(bookToCreate.Price < 0) {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = "You can't put a negative numbere for price.";
                return response;
            }
            try{
                var request = new HttpRequestMessage(HttpMethod.Get, _configuration["AuthService"]);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                var result = await _http.SendAsync(request);

                if(result.IsSuccessStatusCode){
                    Book book = _mapper.Map<Book>(bookToCreate);
                    book.AuthorId = int.Parse(result.Content.ReadAsStringAsync().Result);

                    var insert = await _context.Books.AddAsync(book);
                    Console.WriteLine(insert);
                    await _context.SaveChangesAsync();
                    //ReadBookDTO readBookDTO = _mapper.Map<ReadBookDTO>(insert.Entity);
                    ReadBookDTO readBookDTO = _mapper.Map<ReadBookDTO>(await _context.Books
                    .Include(u => u.Author)
                    .FirstOrDefaultAsync(b => b.Id == insert.Entity.Id));

                    response.Data = readBookDTO;
                }
            } catch(Exception ex){
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
            }
            return response;
        }


        // Get All dei Libri disponibili nello store
        public async Task<ServiceResponse<List<ReadBookDTO>>> ReadAllBook(){
            ServiceResponse<List<ReadBookDTO>> response = new ServiceResponse<List<ReadBookDTO>>();
            try{
                var result = await _context.Books.Include(b => b.Author).ToListAsync();
                if(result.Count == 0) {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "There are no books available.";
                } else {
                    List<ReadBookDTO> bookDTOs = _mapper.Map<List<ReadBookDTO>>(result);
                    response.Data = bookDTOs;
                }
            } catch(Exception ex){
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
            }
            return response;
        }
    

        // Read di un libro filtrato per id
        public async Task<ServiceResponse<ReadBookDTO>> ReadBook(int idBook){
            ServiceResponse<ReadBookDTO> response = new ServiceResponse<ReadBookDTO>();
            try{
                var result = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == idBook);
                if(result is null){
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = $"Book with ID: {idBook} does not exist.";
                } else response.Data = _mapper.Map<ReadBookDTO>(result);
            } catch(Exception ex){
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
            }
            return response;
        }
    
    
        // Get All di tutti i libri filtrati per Autore
        public async Task<ServiceResponse<List<ReadBookDTO>>> ReadBooksByAuthor(int idAuthor){
            ServiceResponse<List<ReadBookDTO>> response = new ServiceResponse<List<ReadBookDTO>>();
            try{
                var result = await _context.Books.Include(b => b.Author).Where(b => b.AuthorId == idAuthor).ToListAsync();
                if(result.Count == 0) {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "No books found for the author requested.";
                } else response.Data = _mapper.Map<List<ReadBookDTO>>(result);
            } catch(Exception ex){
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
            }
            return response;
        }
    

        // Get All di tutti i libri filtrati per genere
        public async Task<ServiceResponse<List<ReadBookDTO>>> ReadBooksByGenre(Genre genre)
        {
            ServiceResponse<List<ReadBookDTO>> response = new ServiceResponse<List<ReadBookDTO>>();
            try{
                var result = await _context.Books.Include(b => b.Author).Where(b => b.Genre == genre).ToListAsync();
                if(result.Count == 0){
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "No books found with this genre.";
                } else response.Data = _mapper.Map<List<ReadBookDTO>>(result);
            } catch(Exception ex){
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
            }
            return response;
        }


        // Update di un libro, solo se si è Autore di quel libro, con chiamata al microservizio Auth
        public async Task<ServiceResponse<ReadBookDTO>> UpdateMyBook(UpdateBookDTO updateBookDTO, string bearerToken)
        {
            ServiceResponse<ReadBookDTO> response = new ServiceResponse<ReadBookDTO>();
            try{
                Book? book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == updateBookDTO.Id);
                if(book is null){
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = $"Book with ID {updateBookDTO.Id} was not found.";
                    return response;
                }

                var request = new HttpRequestMessage(HttpMethod.Get, _configuration["AuthService"]);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                var result = await _http.SendAsync(request);

                if(result.IsSuccessStatusCode){
                    int idLogged = int.Parse(result.Content.ReadAsStringAsync().Result);
                    if(idLogged != book.AuthorId){
                        response.StatusCode = HttpStatusCode.Forbidden;
                        response.Message = "You can't update a book of which you are not the Author.";
                        return response;
                    }
                    var properties = updateBookDTO.GetType().GetProperties();
                    foreach(var property in properties){
                        var value = property.GetValue(updateBookDTO);
                        if(value != null) typeof(Book).GetProperty(property.Name)?.SetValue(book, value);
                    }
                    await _context.SaveChangesAsync();
                    response.Data = _mapper.Map<ReadBookDTO>(book);
                    return response;
                } else{
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "There was a problem calling the Auth Microservice.";
                    return response;
                }
            } catch(Exception ex){
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                return response;
            }
        }


        // Cancella un libro solo se a cancellarlo è l'Autore, con chiamata asincrona all'altro Microservizio per riprendere ID 
        public async Task<ServiceResponse<bool>> DeleteMyBook(int idBook, string bearerToken)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();
            try{
                Book? book = await _context.Books.FindAsync(idBook);
                if(book is null){
                    response.Data = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = $"Book with ID {idBook} does not exists.";
                    return response;
                }

                var request = new HttpRequestMessage(HttpMethod.Get, _configuration["AuthService"]);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                var result = await _http.SendAsync(request);

                if(result.IsSuccessStatusCode){
                    int idLogged = int.Parse(result.Content.ReadAsStringAsync().Result);
                    if(idLogged != book.AuthorId){
                        response.StatusCode = HttpStatusCode.Forbidden;
                        response.Message = "You can't delete a book of which you are not the Author.";
                        return response;
                    } else {
                        var delete = _context.Books.Remove(book);
                        await _context.SaveChangesAsync();
                        response.Data = true;
                        response.Message = "Book successfully deleted.";
                        return response;
                    }
                } else {
                    response.Data = false;
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "There was a problem calling the Auth Microservice.";
                    return response;
                }
            } catch(Exception ex){
                response.Data = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                return response;
            }
        }

    }
}