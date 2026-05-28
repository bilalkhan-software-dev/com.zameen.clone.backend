using System;
using com.zameen.Models.Enums;

namespace com.zameen.Models
{
    public class Property : AbstractEntity
    {
        public int Id { get; set; }
        public string AgentId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal AreaSize { get; set; }
        public AreaUnit AreaUnit { get; set; }
        public bool IsActive { get; set; } = true;
        public PropertyStatus Status { get; set; }
        public PropertyType PropertyType { get; set; }
    }
}
