using System;

namespace Books.Models
{
    public class BookModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}