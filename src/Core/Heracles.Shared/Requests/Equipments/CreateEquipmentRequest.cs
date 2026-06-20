using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.Equipments;

/// <summary>
///     Represents the groupRequest to create a new <see cref="Equipment" />.
/// </summary>
/// <param name="Type"> The type of the <see cref="Equipment" />.</param>
/// <param name="Weight"> The weight of the <see cref="Equipment" />.</param>
/// <param name="Resistance"> The resistance of the <see cref="Equipment" />.</param>
public record CreateEquipmentRequest(string Type, double Weight, double Resistance);
