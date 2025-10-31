using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Products:BaseEntity<Guid>
{
    public string ProductName { get; set; }
    public byte[] ProductImage { get; set; }
    public string ProductDesc { get; set; }
    public decimal ProductPrice { get; set; }
    public decimal ListPrice { get; set; }
    public int Count { get; set; }
    public Guid CreateUser_Id { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public Guid ProducerId { get; set; }
    public string ProductReferance { get; set; }
    public decimal PurchasePrice { get; set; }
    public virtual ProducerBrand ProducerBrand { get; set; } 
    public virtual User User { get; set; }  

}
