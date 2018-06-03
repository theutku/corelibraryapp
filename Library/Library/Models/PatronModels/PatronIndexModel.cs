using System;
using System.Collections.Generic;

namespace Library.Models.PatronModels
{
    public class PatronIndexModel
    {
        public IEnumerable<PatronDetailModel> Patrons { get; set; }
    }
}
