    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    namespace Lester.Models
    {
        public class Embarques
        {
            public int id { get; set; }
            public string codebar { get; set; }
            public string acronimo { get; set; }
            public DateTime fechaLectura { get; set; }
            public string objReferencia { get; set; }
            public int tipo {  get; set; }
            public string Viaje { get; set; }

        }
    }