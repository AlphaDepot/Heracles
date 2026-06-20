using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.Equipments;

/// <summary>
///     Represents the groupRequest to update an <see cref="Equipment" />.
/// </summary>
/// <param name="Id">The Id of the <see cref="Equipment" /> to update.</param>
/// <param name="Type">The type of the <see cref="Equipment" />.</param>
/// <param name="Concurrency">The concurrency token of the <see cref="Equipment" />.</param>
/// <param name="Weight">The weight of the <see cref="Equipment" />.</param>
/// <param name="Resistance">The resistance of the <see cref="Equipment" />.</param>
public record UpdateEquipmentRequest(int Id, string Type, string? Concurrency, double Weight, double Resistance);
