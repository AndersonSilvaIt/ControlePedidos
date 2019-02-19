using System;

namespace DATA.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public  DateTime DateCadaster { get; set; }
    }
}
