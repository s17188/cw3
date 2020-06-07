using System;
using System.Collections.Generic;

namespace Cw3.ModelsGenerated
{
    public partial class Gosc
    {
        public Gosc()
        {
            Rezerwacja = new HashSet<Rezerwacja>();
        }

        public int IdGosc { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int? ProcentRabatu { get; set; }

        public virtual ICollection<Rezerwacja> Rezerwacja { get; set; }
    }
}
