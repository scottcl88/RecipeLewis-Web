namespace RecipeLewis.Models;

public class TagModel : EntityDataUserModel
{
    public TagModel()
    { }

    public TagModel(string name)
    {
        Name = name;
    }

    public int TagId { get; set; }
    public string Name { get; set; }
}