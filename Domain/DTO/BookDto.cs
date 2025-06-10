using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public DateTime PublishedDate { get; set; }
    }
    public class CreateBookDto
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public DateTime PublishedDate { get; set; }
    }
    public class UpdateBookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public DateTime PublishedDate { get; set; }
    }

}
