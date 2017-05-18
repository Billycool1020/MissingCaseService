using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissingCaseService.Model
{
    class Threads
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ThreadId { get; set; }
        public DateTime PostDate { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Product { get; set; }
    }
}
