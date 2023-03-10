//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PAEE_FINAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Plato
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Plato()
        {
            this.Comandas = new HashSet<Comanda>();
        }
    
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Ingredientes { get; set; }
        public Nullable<double> Precio { get; set; }
        public Nullable<int> Categoria { get; set; }
    
        public virtual Categoria Categoria1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comanda> Comandas { get; set; }
    }
}
