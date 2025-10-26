using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Model : BaseEntity<Guid>
{
    public string ModelName { get; set; }
    public int? ModelYearStart { get; set; }
    public int? ModelYearEnd { get; set; }
    public int TransmissionId { get; set; }
    public int FuelTypeId { get; set; }
    public string? Description { get; set; }
    public virtual FuelType FuelType { get; set; }
    public virtual Transmission Transmission { get; set; }

    public Model(
        string modelName, 
        int? modelYearStart, 
        int? modelYearEnd, 
        int transmissionId, 
        int fuelTypeId, 
        string? description, 
        FuelType fuelType, 
        Transmission transmission)
    {
        ModelName = modelName;
        ModelYearStart = modelYearStart;
        ModelYearEnd = modelYearEnd;
        TransmissionId = transmissionId;
        FuelTypeId = fuelTypeId;
        Description = description;
        FuelType = fuelType;
        Transmission = transmission;
    }
}
