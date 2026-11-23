namespace ArthaNiti.Domain.Entities;

public sealed class Tenant
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
}
