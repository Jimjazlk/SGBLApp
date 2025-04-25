    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGBLApp.Core.Domain.Entities
{
    public class BookFeedback
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public bool IsLiked { get; set; } // true = Me gusta, false = No me gusta
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
