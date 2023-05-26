namespace BooksManagementSrv.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // Insert di un nuovo libro nel database, API accessibile solamente dall'ADMIN
        [Authorize(Roles = "Admin")]
        [HttpPost("InsertBook")]
        public async Task<ActionResult<ServiceResponse<ReadBookDTO>>> CreateBook(CreateBookDTO bookToCreate){
            string? bearerToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
            ServiceResponse<ReadBookDTO> response = await _bookService.InsertBook(bookToCreate, bearerToken!);
            return new ObjectResult(response) {StatusCode = (int) response.StatusCode};
        }
        

        // GetAll di tutti i libri presenti nel database
        [HttpGet("GetAllBooks")]
        public async Task<ActionResult<ServiceResponse<List<ReadBookDTO>>>> ReadAllBook(){
            ServiceResponse<List<ReadBookDTO>> response = await _bookService.ReadAllBook();
            return new ObjectResult(response){StatusCode = (int) response.StatusCode};
        }
    
    
        // Read Book by ID
        [HttpGet("ReadBookById")]
        public async Task<ActionResult<ServiceResponse<ReadBookDTO>>> ReadBookById(int idBook){
            ServiceResponse<ReadBookDTO> response = await _bookService.ReadBook(idBook);
            return new ObjectResult(response) {StatusCode = (int) response.StatusCode};
        }


        // GetAll di tutti i libri filtrati per autore
        [HttpGet("GetAllByAuthor")]
        public async Task<ActionResult<ServiceResponse<List<ReadBookDTO>>>> GetBooksByAuthor(int idAuthor){
            ServiceResponse<List<ReadBookDTO>> response = await _bookService.ReadBooksByAuthor(idAuthor);
            return new ObjectResult(response) {StatusCode = (int) response.StatusCode};
        } 


        // GetAll di tutti i libri filtrati per genere
        [HttpGet("GetAllByGenre")]
        public async Task<ActionResult<ServiceResponse<List<ReadBookDTO>>>> GetBooksByGenre(Genre genre){
            ServiceResponse<List<ReadBookDTO>> response = await _bookService.ReadBooksByGenre(genre);
            return new ObjectResult(response) {StatusCode = (int) response.StatusCode};
        } 
    

        // Delete di un libro, API accessibile solamente all'Autore del libro che si vuole cancellare
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteMyBook")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteMyBook(int idBook){
            string? bearerToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
            ServiceResponse<bool> response = await _bookService.DeleteMyBook(idBook, bearerToken!);
            return new ObjectResult(response){StatusCode = (int) response.StatusCode};
        }


        // Update di un libro, API accessibile solamente all'Autore del libro che si vuole modificare
        [Authorize(Roles = "Admin")]
        [HttpPatch("UpdateMyBook")]
        public async Task<ActionResult<ServiceResponse<ReadBookDTO>>> UpdateMyBook(UpdateBookDTO updateBookDTO){
            string? bearerToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
            ServiceResponse<ReadBookDTO> response = await _bookService.UpdateMyBook(updateBookDTO, bearerToken!);
            return new ObjectResult(response){StatusCode = (int) response.StatusCode};
        }

    }
}