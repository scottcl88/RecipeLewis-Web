namespace RecipeLewis.Models;

public class CategoryModel : EntityDataUserModel
{
    public int CategoryId { get; set; }
    public string Name { get; set; }

    // Note: this is important so the MudSelect can compare pizzas
    public override bool Equals(object o)
    {
        var other = o as CategoryModel;
        return other?.Name == Name;
    }

    // Note: this is important too!
    public override int GetHashCode() => Name?.GetHashCode() ?? 0;

    // Implement this for the Pizza to display correctly in MudSelect
    public override string ToString() => Name;
}